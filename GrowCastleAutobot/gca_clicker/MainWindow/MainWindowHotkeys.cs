using gca_clicker.Classes;
using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private bool isListeningForStartShortcut = false;
        private bool isListeningForStopShortcut = false;

        private IntPtr windowHandle;
        private HwndSource source;
#if DEBUG
        private const int HOTKEY_START_ID = 9121;
        private const int HOTKEY_STOP_ID = 9122;
#else
        private const int HOTKEY_START_ID = 9123;
        private const int HOTKEY_STOP_ID = 9124;
#endif
        private const string DEFAULT_START_HOTKEY = "Alt+F1";
        private const string DEFAULT_STOP_HOTKEY = "Alt+F2";
        private const string UPDATE_HOTKEY_TEXT = "Press new shortcut...";

        private uint currentModifiers = 0;
        private uint currentKey = 0;
        private void StartClickerShortcutBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            WinAPI.UnregisterHotKey(windowHandle, HOTKEY_START_ID);
            StartClickerShortcutBox.Focus();
            isListeningForStartShortcut = true;
            StartClickerShortcutBox.Text = UPDATE_HOTKEY_TEXT;
            e.Handled = true;
        }

        private void StartClickerShortcutBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isListeningForStartShortcut)
            {
                isListeningForStartShortcut = false;
                if (string.IsNullOrEmpty(StartClickerShortcutBox.Text) || StartClickerShortcutBox.Text == UPDATE_HOTKEY_TEXT)
                    StartClickerShortcutBox.Text = DEFAULT_START_HOTKEY;

                SaveShortcut(StartClickerShortcutBox.Text, HOTKEY_START_ID);
            }
        }

        private void StartClickerShortcutBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!isListeningForStartShortcut)
                return;

            e.Handled = true;

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LWin || key == Key.RWin)
                return;

            StringBuilder sb = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) sb.Append("Ctrl+");
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) sb.Append("Shift+");
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) sb.Append("Alt+");
            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) sb.Append("Win+");

            string keyName = key.ToString();
            if (keyName.StartsWith("Oem"))
            {
                keyName = GetFriendlyOemKeyName(key);
            }

            sb.Append(keyName);

            StartClickerShortcutBox.Text = sb.ToString();
            isListeningForStartShortcut = false;

            SaveShortcut(StartClickerShortcutBox.Text, HOTKEY_START_ID);
            RewriteCurrentSettings();
        }












        private void StopClickerShortcutBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            WinAPI.UnregisterHotKey(windowHandle, HOTKEY_STOP_ID);
            StopClickerShortcutBox.Focus();
            isListeningForStopShortcut = true;
            StopClickerShortcutBox.Text = UPDATE_HOTKEY_TEXT;
            e.Handled = true;
        }

        private void StopClickerShortcutBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isListeningForStopShortcut)
            {
                isListeningForStopShortcut = false;
                if (string.IsNullOrEmpty(StopClickerShortcutBox.Text) || StopClickerShortcutBox.Text == UPDATE_HOTKEY_TEXT)
                    StopClickerShortcutBox.Text = DEFAULT_STOP_HOTKEY;

                SaveShortcut(StopClickerShortcutBox.Text, HOTKEY_STOP_ID);
            }
        }

        private void StopClickerShortcutBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!isListeningForStopShortcut)
                return;

            e.Handled = true;

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LWin || key == Key.RWin)
                return;

            StringBuilder sb = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) sb.Append("Ctrl+");
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) sb.Append("Shift+");
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) sb.Append("Alt+");
            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) sb.Append("Win+");

            string keyName = key.ToString();
            if (keyName.StartsWith("Oem"))
            {
                keyName = GetFriendlyOemKeyName(key);
            }

            sb.Append(keyName);

            StopClickerShortcutBox.Text = sb.ToString();
            isListeningForStopShortcut = false;

            SaveShortcut(StopClickerShortcutBox.Text, HOTKEY_STOP_ID);
            RewriteCurrentSettings();
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

        private void SaveShortcut(string shortcut, int hotkeyId)
        {

            ParseShortcut(shortcut, out uint modifiers, out uint key);

            WinAPI.UnregisterHotKey(windowHandle, hotkeyId);

            Log.T($"Try register {(hotkeyId == HOTKEY_START_ID ? "start" : "stop")} hotkey: {shortcut}");
            bool success = WinAPI.RegisterHotKey(windowHandle, hotkeyId, modifiers, key);
            if (!success)
            {
                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show("Failed to register hotkey. Choose another, this may be in use already", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                WinAPI.RegisterHotKey(windowHandle, hotkeyId, currentModifiers, currentKey);
                return;
            }
            Log.I($"Register {(hotkeyId == HOTKEY_START_ID ? "start" : "stop")} hotkey: {shortcut}");
            UpdateThreadStatusShortcutLabel();

            currentModifiers = modifiers;
            currentKey = key;

        }





        private void ParseShortcut(string shortcutText, out uint modifiers, out uint key)
        {
            modifiers = 0;
            key = 0;

            var parts = shortcutText.Split('+', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var p = part.Trim();

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
                    OnStartHotkey();
                    handled = true;
                }
                if (id == HOTKEY_STOP_ID)
                {
                    OnStopHotkey();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }


    }
}
