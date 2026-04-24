using gca.Classes.SettingsScripts;
using gca.Classes.Tooltips;
using gca.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace gca
{
    /// <summary>
    /// Interaction logic for BuildUserControl.xaml
    /// </summary>
    public partial class BuildUserControl : System.Windows.Controls.UserControl
    {
        private const string PW_CAPTION = "Pw";
        private const string SMITH_CAPTION = "Smith";
        private const string ORCBAND_CAPTION = "Orc\nband";
        private const string MILITARY_F_CAPTION = "Milit";
        private const string CHRONO_CAPTION = "Chrono";
        private const string SINGLE_CLICK_CAPTION = "1";
        private const string CLICKABLE_CAPTION = "X";
        private const string NO_PRESS_CAPTION = "";

        public event Action<object> OnUpdate = null!;

        private List<System.Windows.Controls.Button> heroSlots;
        private List<System.Windows.Controls.ComboBox> heroPressIndexComboBoxes;

        public BuildUserControl()
        {
            InitializeComponent();

            heroSlots = new List<System.Windows.Controls.Button>();
            heroPressIndexComboBoxes = new List<System.Windows.Controls.ComboBox>();

            Style buttonStyle = (Style)FindResource("WhiteButton");

            BuildButtons(100, 200, 70, 70, 3, 4, 0, buttonStyle, OnSlotClick);
            BuildButtons(20, 270, 70, 70, 1, 1, 12, buttonStyle, OnSlotClick);
            BuildButtons(10, 455, 70, 70, 1, 1, 13, buttonStyle, OnSlotClick);
            BuildButtons(10, 535, 70, 70, 1, 1, 14, buttonStyle, OnSlotClick);
        }

        private void OnSlotClick(object sender, RoutedEventArgs e)
        {

            if (e.OriginalSource is DependencyObject source)
            {
                while (source != null)
                {
                    if (source is System.Windows.Controls.ComboBox)
                    {
                        return;
                    }

                    source = VisualTreeHelper.GetParent(source);
                }
            }

            if (sender is System.Windows.Controls.Button b)
            {
                Hero newHero = GetSelectedHero();

                if (GetButtonTag(b).Hero == newHero)
                {
                    newHero = Hero.None;
                }

                if (newHero != Hero.Clickable && newHero != Hero.SingleClick)
                {
                    RemoveHeroFromTag(newHero);
                }

                SetButtonHero(b, newHero);
                OnUpdate?.Invoke(sender);
            }
        }

        private Hero GetSelectedHero()
        {
            Hero hero = Hero.Clickable;
            if (BuildPw.IsChecked ?? false)
            {
                hero = Hero.Pw;
            }
            else if (BuildSmith.IsChecked ?? false)
            {
                hero = Hero.Smith;
            }
            else if (BuildOrcBand.IsChecked ?? false)
            {
                hero = Hero.OrcBand;
            }
            else if (BuildMilitaryF.IsChecked ?? false)
            {
                hero = Hero.MilitaryF;
            }
            else if (BuildChrono.IsChecked ?? false)
            {
                hero = Hero.Chrono;
            }
            else if (SingleClick.IsChecked ?? false)
            {
                hero = Hero.SingleClick;
            }
            return hero;
        }

        private string GetHeroCaption(Hero hero) => hero switch
        {
            Hero.Pw => PW_CAPTION,
            Hero.Smith => SMITH_CAPTION,
            Hero.OrcBand => ORCBAND_CAPTION,
            Hero.MilitaryF => MILITARY_F_CAPTION,
            Hero.Chrono => CHRONO_CAPTION,
            Hero.SingleClick => SINGLE_CLICK_CAPTION,
            Hero.Clickable => CLICKABLE_CAPTION,
            _ => NO_PRESS_CAPTION
        };

        private void RemoveHeroFromTag(Hero hero)
        {
            foreach (System.Windows.Controls.Button b in heroSlots)
            {
                if (GetButtonTag(b).Hero == hero)
                {
                    SetButtonHero(b, Hero.None);
                }
            }
        }

        public BuildSettings GetBuildSettings()
        {
            BuildSettings settings = new BuildSettings();

            for (int i = 0; i < 15; i++)
            {
                Hero hero = GetButtonTag(heroSlots[i]).Hero;
                settings[i] = hero == Hero.Clickable;
                settings.PressOrder[i] = heroPressIndexComboBoxes[i].SelectedIndex;

                switch (hero)
                {
                    case Hero.Pw:
                        settings.PwSlot = i;
                        break;
                    case Hero.Smith:
                        settings.SmithSlot = i;
                        break;
                    case Hero.OrcBand:
                        settings.OrcBandSlot = i;
                        break;
                    case Hero.MilitaryF:
                        settings.MiliitaryFSlot = i;
                        break;
                    case Hero.Chrono:
                        settings.ChronoSlot = i;
                        break;
                    case Hero.SingleClick:
                        settings.SingleClickSlots.Add(i);
                        break;
                    default:
                        break;
                }
            }

            return settings;

        }

        public void SetBuildSettings(BuildSettings settings)
        {

            for (int i = 0; i < 15; i++)
            {
                heroPressIndexComboBoxes[i].SelectedIndex = settings.PressOrder[i] % 15;
                if (settings.SlotsToPress[i])
                {
                    SetButtonHero(heroSlots[i], Hero.Clickable);
                }
                else if (i == settings.PwSlot)
                {
                    SetButtonHero(heroSlots[i], Hero.Pw);
                }
                else if (i == settings.SmithSlot)
                {
                    SetButtonHero(heroSlots[i], Hero.Smith);
                }
                else if (i == settings.OrcBandSlot)
                {
                    SetButtonHero(heroSlots[i], Hero.OrcBand);
                }
                else if (i == settings.MiliitaryFSlot)
                {
                    SetButtonHero(heroSlots[i], Hero.MilitaryF);
                }
                else if (i == settings.ChronoSlot)
                {
                    SetButtonHero(heroSlots[i], Hero.Chrono);
                }
                else if (settings.SingleClickSlots.Contains(i))
                {
                    SetButtonHero(heroSlots[i], Hero.SingleClick);
                }
                else
                {
                    SetButtonHero(heroSlots[i], Hero.None);
                }
            }

        }

        private TextBlock CreateTextBlock(string text)
        {
            return new TextBlock
            {
                Text = text,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.None,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        public void ResetColors()
        {
            foreach (var slot in heroSlots)
            {
                slot.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        public void BuildButtons(
            double x, double y,
            double cellWidth, double cellHeight,
            int cols, int rows,
            int startOrderIndex,
            Style transparentButtonStyle,
            RoutedEventHandler clickHandler)
        {
            double totalWidth = cols * cellWidth;
            double totalHeight = rows * cellHeight;

            var container = new Canvas
            {
                Width = totalWidth,
                Height = totalHeight
            };
            Canvas.SetLeft(container, x);
            Canvas.SetTop(container, y);

            for (int c = 0; c <= cols; c++)
            {
                var line = new Border
                {
                    Background = System.Windows.Media.Brushes.Gray,
                    Width = 1,
                    Height = totalHeight
                };
                Canvas.SetLeft(line, c * cellWidth);
                Canvas.SetTop(line, 0);
                container.Children.Add(line);
            }

            for (int r = 0; r <= rows; r++)
            {
                var line = new Border
                {
                    Background = System.Windows.Media.Brushes.Gray,
                    Width = totalWidth,
                    Height = 1
                };
                Canvas.SetLeft(line, 0);
                Canvas.SetTop(line, r * cellHeight);
                container.Children.Add(line);
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var grid = new Grid();

                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellHeight * 0.6) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellHeight * 0.4) });

                    var text = CreateTextBlock(NO_PRESS_CAPTION);
                    text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;

                    var combo = CreateComboBox();
                    combo.SelectedIndex = startOrderIndex + r * cols + c;

                    grid.Children.Add(text);
                    grid.Children.Add(combo);

                    Grid.SetRow(text, 0);
                    Grid.SetRow(combo, 1);

                    var button = new System.Windows.Controls.Button
                    {
                        Width = cellWidth,
                        Height = cellHeight,
                        Style = transparentButtonStyle,
                        FontSize = 12,
                        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                        Content = grid,
                        Tag = new HeroButtonData
                        {
                            Hero = Hero.None,
                            Text = text,
                            Combo = combo
                        }
                    };

                    button.Content = grid;

                    button.Click += clickHandler;

                    Canvas.SetLeft(button, c * cellWidth);
                    Canvas.SetTop(button, r * cellHeight);
                    container.Children.Add(button);
                    heroSlots.Add(button);
                    heroPressIndexComboBoxes.Add(combo);
                }
            }

            BuildCanvas.Children.Add(container);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnUpdate?.Invoke(sender);
        }

        private System.Windows.Controls.ComboBox CreateComboBox()
        {
            var combo = new System.Windows.Controls.ComboBox
            {
                Width = 50,
                Height = 22,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 5)
            };

            for (int i = 1; i <= 15; i++)
            {
                combo.Items.Add(i.ToString());
            }

            combo.PreviewMouseWheel += ComboBox_PreviewMouseWheel;
            combo.SelectionChanged += ComboBox_SelectionChanged;

            combo.SelectedIndex = 0;

            return combo;
        }

        private HeroButtonData GetButtonTag(System.Windows.Controls.Button button)
        {
            return (HeroButtonData)button.Tag!;
        }

        private void SetButtonHero(System.Windows.Controls.Button button, Hero hero)
        {
            HeroButtonData data = GetButtonTag(button);
            data.Hero = hero;
            data.Text.Text = GetHeroCaption(hero);
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox combo && combo.Items.Count > 0)
            {
                int direction = e.Delta > 0 ? -1 : 1;
                int newIndex = combo.SelectedIndex + direction;

                if (newIndex < 0)
                    newIndex = 0;
                else if (newIndex >= combo.Items.Count)
                    newIndex = combo.Items.Count - 1;

                combo.SelectedIndex = newIndex;
                e.Handled = true;
            }
        }
    }

    public class HeroButtonData
    {
        public Hero Hero { get; set; } = Hero.None;
        public System.Windows.Controls.TextBlock Text { get; set; } = null!;
        public System.Windows.Controls.ComboBox Combo { get; set; } = null!;
    }
}
