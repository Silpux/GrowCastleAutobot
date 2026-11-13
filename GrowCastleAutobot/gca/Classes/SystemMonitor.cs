using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gca.Classes
{
    public static class SystemMonitor
    {

        [DllImport("kernel32.dll")]
        private static extern bool GetSystemTimes(out FILETIME idleTime, out FILETIME kernelTime, out FILETIME userTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetProcessTimes(IntPtr hProcess, out FILETIME creationTime, out FILETIME exitTime, out FILETIME kernelTime, out FILETIME userTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint Low;
            public uint High;

            public long ToLong()
            {
                return ((long)High << 32) | Low;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        private static long previousIdle;
        private static long previousKernel;
        private static long previousUser;
        private static bool systemInitialized;

        private class ProcessCpuSample
        {
            public long Kernel;
            public long User;
            public long Timestamp;
        }

        private static readonly object cpuLock = new object();

        private static readonly System.Collections.Generic.Dictionary<int, ProcessCpuSample> processSamples = new();

        public static double GetOverallCpuUsage()
        {
            lock (cpuLock)
            {
                if (!GetSystemTimes(out FILETIME idle, out FILETIME kernel, out FILETIME user))
                {
                    return -1;
                }

                long idleNow = idle.ToLong();
                long kernelNow = kernel.ToLong();
                long userNow = user.ToLong();

                if (!systemInitialized)
                {
                    previousIdle = idleNow;
                    previousKernel = kernelNow;
                    previousUser = userNow;
                    systemInitialized = true;
                    return -1;
                }

                long idleDelta = idleNow - previousIdle;
                long kernelDelta = kernelNow - previousKernel;
                long userDelta = userNow - previousUser;

                previousIdle = idleNow;
                previousKernel = kernelNow;
                previousUser = userNow;

                long totalDelta = kernelDelta + userDelta;

                if (totalDelta <= 0)
                {
                    return -1;
                }

                double usage = (1.0 - (double)idleDelta / totalDelta);

                return Math.Clamp(usage, 0, 100);
            }
        }

        public static double GetProcessCpuUsage(int processId)
        {
            const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

            IntPtr handle = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, processId);

            if (handle == IntPtr.Zero)
            {
                return -1;
            }

            try
            {
                if (!GetProcessTimes(handle, out _, out _, out FILETIME kernel, out FILETIME user))
                {
                    return -1;
                }

                long kernelNow = kernel.ToLong();
                long userNow = user.ToLong();
                long timestamp = Stopwatch.GetTimestamp();

                lock (cpuLock)
                {
                    if (!processSamples.TryGetValue(processId, out var previous))
                    {
                        processSamples[processId] = new ProcessCpuSample
                        {
                            Kernel = kernelNow,
                            User = userNow,
                            Timestamp = timestamp
                        };

                        return -1;
                    }

                    long cpuDelta = (kernelNow - previous.Kernel) + (userNow - previous.User);

                    long timeDelta = timestamp - previous.Timestamp;

                    previous.Kernel = kernelNow;
                    previous.User = userNow;
                    previous.Timestamp = timestamp;

                    if (timeDelta <= 0 || cpuDelta < 0)
                    {
                        return -1;
                    }

                    double elapsedCpuSeconds = cpuDelta / 10_000_000.0;

                    double elapsedSeconds = (double)timeDelta / Stopwatch.Frequency;

                    double usage = elapsedCpuSeconds / elapsedSeconds;

                    return Math.Max(0, usage) / 8;
                }
            }
            finally
            {
                CloseHandle(handle);
            }
        }
    }
}
