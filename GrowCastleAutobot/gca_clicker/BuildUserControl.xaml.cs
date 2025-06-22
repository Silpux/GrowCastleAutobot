using gca_clicker.Classes;
using gca_clicker.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gca_clicker
{
    /// <summary>
    /// Interaction logic for BuildUserControl.xaml
    /// </summary>
    public partial class BuildUserControl : UserControl
    {
        private const string PW_CAPTION = "Pw";
        private const string SMITH_CAPTION = "Smith";
        private const string ORCBAND_CAPTION = "Orc\nband";
        private const string MILITARY_F_CAPTION = "Milit";
        private const string CHRONO_CAPTION = "Chrono";
        private const string CLICKABLE_CAPTION = "X";
        private const string NO_PRESS_CAPTION = "";

        public event Action OnUpdate;

        private List<Button> slots;

        public BuildUserControl()
        {
            InitializeComponent();

            slots = new List<Button>();

            Style buttonStyle = (Style)FindResource("WhiteButton");

            BuildButtons(124, 201, 45, 45, 3, 4, buttonStyle, OnSlotClick);
            BuildButtons(70, 246, 45, 45, 1, 1, buttonStyle, OnSlotClick);
            BuildButtons(45, 360, 45, 45, 1, 1, buttonStyle, OnSlotClick);
            BuildButtons(45, 420, 45, 45, 1, 1, buttonStyle, OnSlotClick);
        }

        private void OnSlotClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                Hero newTag = GetSelectedHero();

                if((Hero)b.Tag == newTag)
                {
                    newTag = Hero.None;
                }

                if(newTag != Hero.Clickable)
                {
                    RemoveTag(newTag);
                }

                ((TextBlock)b.Content).Text = GetHeroCaption(newTag);
                b.Tag = newTag;

                OnUpdate?.Invoke();
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
            return hero;
        }

        private string GetHeroCaption(Hero hero) => hero switch
        {
            Hero.Pw => PW_CAPTION,
            Hero.Smith => SMITH_CAPTION,
            Hero.OrcBand => ORCBAND_CAPTION,
            Hero.MilitaryF => MILITARY_F_CAPTION,
            Hero.Chrono => CHRONO_CAPTION,
            Hero.Clickable => CLICKABLE_CAPTION,
            _ => NO_PRESS_CAPTION
        };

        private void RemoveTag(Hero tag)
        {
            foreach (Button b in slots)
            {
                if((Hero)b.Tag! == tag)
                {
                    b.Tag = Hero.None;
                    ((TextBlock)b.Content).Text = NO_PRESS_CAPTION;
                }
            }
        }

        public BuildSettings GetBuildSettings()
        {
            BuildSettings settings = new BuildSettings();

            for(int i = 0; i < 15; i++)
            {
                Hero hero = (Hero)slots[i].Tag!;
                settings[i] = hero == Hero.Clickable;

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
                    default:
                        break;
                }
            }

            return settings;

        }

        public void SetBuildSettings(BuildSettings settings)
        {

            for(int i = 0; i < 15; i++)
            {
                if (settings.SlotsToPress[i])
                {
                    slots[i].Tag = Hero.Clickable;
                    slots[i].Content = CreateTextBlock(CLICKABLE_CAPTION);
                }
                else if(i == settings.PwSlot)
                {
                    slots[i].Tag = Hero.Pw;
                    slots[i].Content = CreateTextBlock(PW_CAPTION);
                }
                else if (i == settings.SmithSlot)
                {
                    slots[i].Tag = Hero.Smith;
                    slots[i].Content = CreateTextBlock(SMITH_CAPTION);
                }
                else if (i == settings.OrcBandSlot)
                {
                    slots[i].Tag = Hero.OrcBand;
                    slots[i].Content = CreateTextBlock(ORCBAND_CAPTION);
                }
                else if (i == settings.MiliitaryFSlot)
                {
                    slots[i].Tag = Hero.MilitaryF;
                    slots[i].Content = CreateTextBlock(MILITARY_F_CAPTION);
                }
                else if (i == settings.ChronoSlot)
                {
                    slots[i].Tag = Hero.Chrono;
                    slots[i].Content = CreateTextBlock(CHRONO_CAPTION);
                }
                else
                {
                    slots[i].Tag = Hero.None;
                    slots[i].Content = CreateTextBlock(NO_PRESS_CAPTION);
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
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        public void BuildButtons(
            double x, double y,
            double cellWidth, double cellHeight,
            int cols, int rows,
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
                    Background = Brushes.Gray,
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
                    Background = Brushes.Gray,
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
                    var button = new Button
                    {
                        Width = cellWidth,
                        Height = cellHeight,
                        Style = transparentButtonStyle,
                        FontSize = 12,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Tag = Hero.None,
                        Content = CreateTextBlock(NO_PRESS_CAPTION)
                    };
                    button.Click += clickHandler;

                    Canvas.SetLeft(button, c * cellWidth);
                    Canvas.SetTop(button, r * cellHeight);
                    container.Children.Add(button);
                    slots.Add(button);
                }
            }

            BuildCanvas.Children.Add(container);
        }
    }
}
