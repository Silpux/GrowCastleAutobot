using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace gca_clicker.Classes.Tooltips
{
    public static class TooltipHelper
    {

        public static readonly DependencyProperty TooltipCursorProperty =
        DependencyProperty.RegisterAttached(
            "TooltipCursor",
            typeof(Cursor),
            typeof(TooltipHelper),
            new FrameworkPropertyMetadata(Cursors.Help));

        public static void SetTooltipCursor(UIElement element, Cursor value)
        {
            element.SetValue(TooltipCursorProperty, value);
        }

        public static Cursor GetTooltipCursor(UIElement element)
        {
            return (Cursor)element.GetValue(TooltipCursorProperty);
        }

        public static readonly DependencyProperty EnabledTooltipProperty =
            DependencyProperty.RegisterAttached(
                "EnabledTooltip",
                typeof(string),
                typeof(TooltipHelper),
                new FrameworkPropertyMetadata(null));

        public static void SetEnabledTooltip(UIElement element, string value)
        {
            element.SetValue(EnabledTooltipProperty, value);
        }

        public static string GetEnabledTooltip(UIElement element)
        {
            return (string)element.GetValue(EnabledTooltipProperty);
        }


        public static readonly DependencyProperty DisabledTooltipProperty =
            DependencyProperty.RegisterAttached(
                "DisabledTooltip",
                typeof(string),
                typeof(TooltipHelper),
                new FrameworkPropertyMetadata(null));

        public static void SetDisabledTooltip(UIElement element, string value)
        {
            element.SetValue(DisabledTooltipProperty, value);
        }

        public static string GetDisabledTooltip(UIElement element)
        {
            return (string)element.GetValue(DisabledTooltipProperty);
        }

        public static void AttachTooltipCursorWatcherTo(FrameworkElement root)
        {
            root.PreviewMouseMove -= Root_PreviewMouseMove;
            root.PreviewMouseMove += Root_PreviewMouseMove;
        }

        private static void Root_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement root)
            {
                Point pos = e.GetPosition(root);
                HitTestResult result = VisualTreeHelper.HitTest(root, pos);
                if (result != null)
                {

                    DependencyObject element = result.VisualHit;

                    while (element != null)
                    {
                        if (element is UIElement uiElement)
                        {
                            bool isEnabled = (uiElement as Control)?.IsEnabled ?? false;
                            string tooltip = isEnabled ? TooltipHelper.GetEnabledTooltip(uiElement) : TooltipHelper.GetDisabledTooltip(uiElement);

                            if (!string.IsNullOrWhiteSpace(tooltip))
                            {
                                Mouse.OverrideCursor = TooltipHelper.GetTooltipCursor(uiElement);
                                return;
                            }
                        }

                        element = VisualTreeHelper.GetParent(element);
                    }
                    Mouse.OverrideCursor = null;

                }
            }
        }
    }
}
