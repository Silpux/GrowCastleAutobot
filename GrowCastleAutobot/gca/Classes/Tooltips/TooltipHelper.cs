using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace gca.Classes.Tooltips
{
    public static class TooltipHelper
    {

        public static readonly DependencyProperty TooltipCursorProperty =
        DependencyProperty.RegisterAttached(
            "TooltipCursor",
            typeof(System.Windows.Input.Cursor),
            typeof(TooltipHelper),
            new FrameworkPropertyMetadata(System.Windows.Input.Cursors.Help));

        public static void SetTooltipCursor(UIElement element, System.Windows.Input.Cursor value)
        {
            element.SetValue(TooltipCursorProperty, value);
        }

        public static System.Windows.Input.Cursor GetTooltipCursor(UIElement element)
        {
            return (System.Windows.Input.Cursor)element.GetValue(TooltipCursorProperty);
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

        private static void Root_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is FrameworkElement root)
            {
                System.Windows.Point pos = e.GetPosition(root);
                HitTestResult result = VisualTreeHelper.HitTest(root, pos);
                if (result != null)
                {

                    DependencyObject element = result.VisualHit;

                    while (element != null)
                    {
                        if (element is UIElement uiElement)
                        {
                            bool isEnabled = uiElement.IsEnabled;
                            string tooltip = isEnabled ? TooltipHelper.GetEnabledTooltip(uiElement) : TooltipHelper.GetDisabledTooltip(uiElement);

                            if (tooltip != null)
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
