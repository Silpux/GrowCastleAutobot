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

        public static readonly DependencyProperty EnabledTooltipProperty =
            DependencyProperty.RegisterAttached(
                "EnabledTooltip",
                typeof(string),
                typeof(TooltipHelper),
                new FrameworkPropertyMetadata(null, OnTooltipChanged));

        public static void SetEnabledTooltip(UIElement element, string value)
        {
            element.SetValue(EnabledTooltipProperty, value);
        }

        public static string GetEnabledTooltip(UIElement element)
        {
            return (string)element.GetValue(EnabledTooltipProperty);
        }

        private static void OnTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {

                element.MouseEnter -= Element_MouseEnter;
                element.MouseEnter += Element_MouseEnter;

                element.MouseLeave -= Element_MouseLeave;
                element.MouseLeave += Element_MouseLeave;
            }
        }

        private static void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is UIElement element)
            {
                bool isEnabled = (element as Control)?.IsEnabled ?? true;
                string tooltip = isEnabled ? GetEnabledTooltip(element) : GetDisabledTooltip(element);

                if (!string.IsNullOrWhiteSpace(tooltip))
                {
                    Mouse.OverrideCursor = Cursors.Help;
                }
            }
        }

        private static void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }


        public static readonly DependencyProperty DisabledTooltipProperty =
            DependencyProperty.RegisterAttached(
                "DisabledTooltip",
                typeof(string),
                typeof(TooltipHelper),
                new FrameworkPropertyMetadata(null, OnTooltipChanged));

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
                            bool isEnabled = (uiElement as Control)?.IsEnabled ?? true;
                            if (!isEnabled)
                            {
                                string tooltip = TooltipHelper.GetDisabledTooltip(uiElement);

                                if (!string.IsNullOrWhiteSpace(tooltip))
                                {
                                    Mouse.OverrideCursor = Cursors.Hand;
                                    return;
                                }
                            }
                            else
                            {
                                string tooltip = TooltipHelper.GetEnabledTooltip(uiElement);

                                if (!string.IsNullOrWhiteSpace(tooltip))
                                {
                                    return;
                                }
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
