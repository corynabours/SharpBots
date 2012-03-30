using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CSRobots.ViewModels
{
    public class RobotColorImages
    {
        private readonly byte[] _textColors = new byte[]
                                           {
                                               0xFF, 0x00, 0x00,
                                               0x00, 0xFF, 0x00,
                                               0x00, 0x00, 0xFF,
                                               0xFF, 0xFF, 0x00,
                                               0x00, 0xFF, 0xFF,
                                               0xFF, 0x00, 0xFF,
                                               0xFF, 0xFF, 0xFF,
                                               0x77, 0x77, 0x77
                                           };
        private readonly int[] _colorValues = new[] { 6, 5, 3, 4, 1, 2, 0, 7 };
        private const string DefaultSkinPrefix = "images.red_";
        public IList<BitmapSource> Bodies { get; set; }
        public IList<BitmapSource> Turrets { get; set; }
        public IList<BitmapSource> Radars { get; set; }

        public Brush TextColor { get; set; }

        public RobotColorImages(int team)
        {
            TextColor = new SolidColorBrush(Color.FromRgb(_textColors[team * 3], _textColors[team * 3 + 1], _textColors[team * 3 + 2]));
            Bodies = new List<BitmapSource>();
            Turrets = new List<BitmapSource>();
            Radars = new List<BitmapSource>();
            var c1 = _colorValues[team] % 2;
            var c2 = Convert.ToInt32(Math.Floor(((decimal)_colorValues[team] % 4) / 2));
            var c3 = Convert.ToInt32(Math.Floor((decimal)_colorValues[team] / 4));
            for (var j = 0; j < 1; j++)
            {
                Bodies.Add(EmbeddedImage.ModifyImageResource("CSRobots.ViewModels." + DefaultSkinPrefix + "body" + j.ToString("D2") + "0.gif", c1, c2, c3));
                Turrets.Add(EmbeddedImage.ModifyImageResource("CSRobots.ViewModels." + DefaultSkinPrefix + "turret" + j.ToString("D2") + "0.gif", c1, c2, c3));
                Radars.Add(EmbeddedImage.ModifyImageResource("CSRobots.ViewModels." + DefaultSkinPrefix + "radar" + j.ToString("D2") + "0.gif", c1, c2, c3));
            }
        }

    }
}
