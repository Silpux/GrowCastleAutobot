using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gca_clicker.Classes.Tooltips
{
    public static class TooltipHelper
    {
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
    }
}
