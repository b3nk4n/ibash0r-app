using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Media;

namespace Bash.App.Pages
{
    public partial class ColorPickerPage : PhoneApplicationPage
    {
        static string[] colorNames =
            {
	            "Yellow","BananaYellow","LaserLemon","Jasmine","Green","Emerald",
	            "GreenYellow","Lime","Chartreuse","LimeGreen","SpringGreen","LightGreen",
	            "MediumSeaGreen","MediumSpringGreen","Olive","SeaGreen","Red","OrangeRed",
	            "DarkOrange","Orange","ImperialRed","Maroon","Brown","Chocolate",
	            "Coral","Crimson","DarkSalmon","DeepPink","Firebrick","HotPink",
	            "IndianRed","LightCoral","LightPink","LightSalmon","Magenta","MediumVioletRed",
	            "Orchid","PaleVioletRed","Salmon","SandyBrown","Navy","Indigo",
	            "MidnightBlue","Blue","Purple","BlueViolet","CornflowerBlue","Cyan",
	            "DarkCyan","DarkSlateBlue","DeepSkyBlue","DodgerBlue","LightBlue","LightSeaGreen",
	            "LightSkyBlue","LightSteelBlue","Mauve","MediumSlateBlue","RoyalBlue","SlateBlue",
	            "SlateGray","SteelBlue","Teal","Turquoise","Black","Gray",
                "DarkGrey","LightGray"
            };

        static uint[] uintColors =
            { 
	            0xFFFFFF00,0xFFFFE135,0xFFFFFF66,0xFFF8DE7E,0xFF008000,0xFF008A00,
	            0xFFADFF2F,0xFF00FF00,0xFF7FFF00,0xFF32CD32,0xFF00FF7F,0xFF90EE90,
	            0xFF3CB371,0xFF00FA9A,0xFF808000,0xFF2E8B57,0xFFFF0000,0xFFFF4500,
	            0xFFFF8C00,0xFFFFA500,0xFFED2939,0xFF800000,0xFFA52A2A,0xFFD2691E,
	            0xFFFF7F50,0xFFDC143C,0xFFE9967A,0xFFFF1493,0xFFB22222,0xFFFF69B4,
	            0xFFCD5C5C,0xFFF08080,0xFFFFB6C1,0xFFFFA07A,0xFFFF00FF,0xFFC71585,
	            0xFFDA70D6,0xFFDB7093,0xFFFA8072,0xFFF4A460,0xFF000080,0xFF4B0082,
	            0xFF191970,0xFF0000FF,0xFF800080,0xFF8A2BE2,0xFF6495ED,0xFF00FFFF,
	            0xFF008B8B,0xFF483D8B,0xFF00BFFF,0xFF1E90FF,0xFFADD8E6,0xFF20B2AA,
	            0xFF87CEFA,0xFFB0C4DE,0xFF76608A,0xFF7B68EE,0xFF4169E1,0xFF6A5ACD,
	            0xFF708090,0xFF4682B4,0xFF008080,0xFF40E0D0,0xFF000000,0xFF808080,
                0xFFA9A9A9,0xFFD3D3D3
            };

        public ColorPickerPage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                List<ColorItem> item = new List<ColorItem>();
                for (int i = 0; i < colorNames.Length; i++)
                {
                    item.Add(new ColorItem() { Text = colorNames[i], Color = ConvertColor(uintColors[i]) });
                };

                listBox.ItemsSource = item; //Fill ItemSource with all colors
            };
        }

        private Color ConvertColor(uint uintCol)
        {
            byte A = (byte)((uintCol & 0xFF000000) >> 24);
            byte R = (byte)((uintCol & 0x00FF0000) >> 16);
            byte G = (byte)((uintCol & 0x0000FF00) >> 8);
            byte B = (byte)((uintCol & 0x000000FF) >> 0);

            return Color.FromArgb(A, R, G, B); ;
        }

        private void lstColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Settings.LockScreenBackgroundColor.Value = ((ColorItem)e.AddedItems[0]).Color;
                if (this.NavigationService.CanGoBack)
                    this.NavigationService.GoBack();
            }
        }
    }

    public class ColorItem
    {
        public string Text { get; set; }
        public Color Color { get; set; }
    }
}