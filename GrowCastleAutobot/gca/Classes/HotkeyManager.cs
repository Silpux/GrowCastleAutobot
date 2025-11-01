using gca.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace gca.Classes
{
    public class HotkeyManager
    {


        private IntPtr windowHandle;
        private HwndSource source = null!;
#if DEBUG
        public const int HOTKEY_START_ID = 9121;
        public const int HOTKEY_STOP_ID = 9122;
#else
        public const int HOTKEY_START_ID = 9123;
        public const int HOTKEY_STOP_ID = 9124;
#endif
        public const string DEFAULT_START_HOTKEY = "Alt+F1";
        public const string DEFAULT_STOP_HOTKEY = "Alt+F2";

        public const string UPDATE_HOTKEY_TEXT = "Press new shortcut...";

        public Hotkey ListeningFor { get; private set; } = Hotkey.None;

        private uint currentModifiers = 0;
        private uint currentKey = 0;

        public event Action? OnStartHotkeyPressed;
        public event Action? OnStopHotkeyPressed;

        public event Action<string, Hotkey>? OnHotkeyChanged;

        public event Action? OnHotkeyFailed;
        
        public HotkeyManager(Window window)
        {
            WindowInteropHelper helper = new(window);
            windowHandle = helper.Handle;

            source = HwndSource.FromHwnd(windowHandle)!;
            source.AddHook(HwndHook);
        }

        public void RegisterInitialHotkeys()
        {
            SaveShortcut(DEFAULT_START_HOTKEY, Hotkey.Start);
            SaveShortcut(DEFAULT_STOP_HOTKEY, Hotkey.Stop);
        }

        public void UnregisterAll()
        {
            WinAPI.UnregisterHotKey(windowHandle, HOTKEY_START_ID);
            WinAPI.UnregisterHotKey(windowHandle, HOTKEY_STOP_ID);
        }

        public void StartListeningFor(Hotkey hotkey, System.Windows.Controls.TextBox targetBox)
        {
            UnregisterHotkey(hotkey);
            ListeningFor = hotkey;
            targetBox.Focus();
            targetBox.Text = UPDATE_HOTKEY_TEXT;
        }
        private void UnregisterHotkey(Hotkey hotkey)
        {
            int id = GetHotkeyID(hotkey);
            WinAPI.UnregisterHotKey(windowHandle, id);
        }


        public void StopListening()
        {
            ListeningFor = Hotkey.None;
        }

        public void HandleKeyInput(System.Windows.Controls.TextBox targetBox, System.Windows.Input.KeyEventArgs e)
        {

            if (ListeningFor == Hotkey.None)
            {
                return;
            }

            e.Handled = true;

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (IsModifierKey(key))
            {
                return;
            }

            string shortcut = BuildShortcutString(key);
            SaveShortcut(shortcut, ListeningFor);

            ListeningFor = Hotkey.None;
        }

        private bool IsModifierKey(Key key) =>
            key is Key.LeftCtrl or Key.RightCtrl or
                 Key.LeftShift or Key.RightShift or
                 Key.LeftAlt or Key.RightAlt or
                 Key.LWin or Key.RWin;

        private string BuildShortcutString(Key key)
        {
            StringBuilder sb = new();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) sb.Append("Ctrl+");
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) sb.Append("Shift+");
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) sb.Append("Alt+");
            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) sb.Append("Win+");

            string keyName = key.ToString();
            if (keyName.StartsWith("Oem"))
                keyName = GetFriendlyOemKeyName(key);

            sb.Append(keyName);
            return sb.ToString();
        }

        public void Close()
        {
            source.RemoveHook(HwndHook);
            UnregisterAll();
        }

        private void ParseShortcut(string shortcutText, out uint modifiers, out uint key)
        {
            modifiers = 0;
            key = 0;

            string[] parts = shortcutText.Split('+', StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string p = part.Trim();

                switch (p.ToLower())
                {
                    case "ctrl":
                        modifiers |= WinAPI.MOD_CONTROL;
                        break;
                    case "shift":
                        modifiers |= WinAPI.MOD_SHIFT;
                        break;
                    case "alt":
                        modifiers |= WinAPI.MOD_ALT;
                        break;
                    case "win":
                        modifiers |= WinAPI.MOD_WIN;
                        break;
                    default:
                        key = (uint)KeyInterop.VirtualKeyFromKey(ParseKeyFromString(p));
                        break;
                }
            }
        }

        private Key ParseKeyFromString(string keyString)
        {
            if (keyString.Length == 1 && char.IsLetter(keyString[0]))
            {
                return (Key)Enum.Parse(typeof(Key), keyString, true);
            }

            return keyString.ToUpper() switch
            {
                "+" => Key.OemPlus,
                "-" => Key.OemMinus,
                "," => Key.OemComma,
                "." => Key.OemPeriod,
                "/" => Key.OemQuestion,
                ";" => Key.OemSemicolon,
                "'" => Key.OemQuotes,
                "[" => Key.OemOpenBrackets,
                "]" => Key.OemCloseBrackets,
                "\\" => Key.OemPipe,
                _ => (Key)Enum.Parse(typeof(Key), keyString, true)
            };
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WinAPI.WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == HOTKEY_START_ID)
                {
                    OnStartHotkeyPressed?.Invoke();
                    handled = true;
                }
                if (id == HOTKEY_STOP_ID)
                {
                    OnStopHotkeyPressed?.Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private string GetFriendlyOemKeyName(Key key)
        {
            return key switch
            {
                Key.OemPlus => "+",
                Key.OemMinus => "-",
                Key.OemComma => ",",
                Key.OemPeriod => ".",
                Key.OemQuestion => "/",
                Key.OemSemicolon => ";",
                Key.OemQuotes => "'",
                Key.OemOpenBrackets => "[",
                Key.OemCloseBrackets => "]",
                Key.OemPipe => "\\",
                _ => key.ToString(),
            };
        }

        public int GetHotkeyID(Hotkey hotkey) => hotkey switch
        {
            Hotkey.Start => HOTKEY_START_ID,
            Hotkey.Stop => HOTKEY_STOP_ID,
            _ => throw new ArgumentException($"Wrong hotkey: {hotkey}")
        };

        public void SaveShortcut(string shortcut, Hotkey hotkey)
        {
            int hotkeyId = GetHotkeyID(hotkey);

            ParseShortcut(shortcut, out uint modifiers, out uint key);

            WinAPI.UnregisterHotKey(windowHandle, hotkeyId);

            Log.T($"Try register {(hotkeyId == HOTKEY_START_ID ? "start" : "stop")} hotkey: {shortcut}");
            bool success = WinAPI.RegisterHotKey(windowHandle, hotkeyId, modifiers, key);
            if (!success)
            {
#if !DEBUG
                OnHotkeyFailed?.Invoke();
#endif
                return;
            }
            Log.I($"Register {(hotkeyId == HOTKEY_START_ID ? "start" : "stop")} hotkey: {shortcut}");
            OnHotkeyChanged?.Invoke(shortcut, ListeningFor);

            currentModifiers = modifiers;
            currentKey = key;

        }
    }
}
