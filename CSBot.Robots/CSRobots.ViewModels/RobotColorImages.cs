using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CSRobots.ViewModels
{
    public class RobotColorImages
    {
        public IList<BitmapImage> Bodies { get; set; }
        public IList<BitmapImage> Turrets { get; set; }
        public IList<BitmapImage> Radars { get; set; }
        public RobotColorImages(IList<BitmapImage> bodies, IList<BitmapImage> turrets, IList<BitmapImage> radars)
        {
            Bodies = bodies;
            Turrets = turrets;
            Radars = radars;
        }
    }
}
