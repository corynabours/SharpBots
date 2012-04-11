using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CSBot.Robots;

namespace CSRobots.ViewModels
{
    public class RobotView : BaseViewModel
    {
        private readonly RobotRunner _robot;
        private readonly RobotColorImages _images;
        private readonly int _index;

        internal RobotView(RobotRunner robot, RobotColorImages images, int index)
        {
            _robot = robot;
            _images = images;
            _index = index;
            RobotName = robot.Name;
            BodyImage = images.Bodies[0];
            RadarImage = images.Radars[0];
            TurretImage = images.Turrets[0];
        }

        private int _left;
        public int Left
        {
            get { return _left; }
            set
            {
                SetPropertyValue(out _left, value);
            }
        }

        private int _top;
        public int Top
        {
            get { return _top; }
            set
            {
                SetPropertyValue(out _top, value);
            }
        }

        private string _robotName;
        public string RobotName
        {
            get { return _robotName; }
            set { SetPropertyValue(out _robotName, value); }
        }

        public decimal Energy
        {
            get { return _robot.Energy; }
        }

        public Brush Color
        {
            get { return _images.TextColor; }
        }

        public int Index
        {
            get { return _index; }
        }

        private int _radarHeading;
        public int RadarHeading
        {
            get { return _radarHeading; }
            set { SetPropertyValue(out _radarHeading, value); }
        }

        public double X
        {
            get { return _robot.X; }
        }

        public double Y
        {
            get { return _robot.Y; }
        }

        private BitmapSource _bodyImage;
        public BitmapSource BodyImage
        {
            get { return _bodyImage; }
            set { SetPropertyValue(out _bodyImage, value); }
        }

        private BitmapSource _turretImage;
        public BitmapSource TurretImage
        {
            get { return _turretImage; }
            set { SetPropertyValue(out _turretImage, value); }
        }

        private BitmapSource _radarImage;
        public BitmapSource RadarImage
        {
            get { return _radarImage; }
            set { SetPropertyValue(out _radarImage, value); }
        }


        private int _bodyHeading;
        public int BodyHeading
        {
            get { return _bodyHeading; }
            set { SetPropertyValue(out _bodyHeading, value); }
        }

        private int _turretHeading;
        public int TurretHeading
        {
            get { return _turretHeading; }
            set { SetPropertyValue(out _turretHeading, value); }
        }

        internal void Draw()
        {
            Top = Convert.ToInt32((_robot.Y - _robot.Size)/2);
            Left = Convert.ToInt32((_robot.X - _robot.Size) / 2);
            BodyHeading = 0 - Convert.ToInt32(_robot.Heading);
            TurretHeading = 0 - Convert.ToInt32(_robot.GunHeading);
            RadarHeading = 0 - Convert.ToInt32(_robot.RadarHeading);
            Speech = _robot.Speech;
            Speech = _robot.Speech;
            Health = "".PadRight(Convert.ToInt32(_robot.Energy) / 5, '|');
        }

        private string _speech;
        public string Speech
        {
            get { return _speech; }
            set { SetPropertyValue(out _speech, value); }
        }

        private string _health;
        public string Health { get { return _health; }
            set { SetPropertyValue(out _health, value); }
        }

        public bool Dead()
        {
            return _robot.Dead();
        }
    }
}
