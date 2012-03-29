using System;
using System.Windows.Controls;
using System.Windows.Media;
using CSBot.Robots;

namespace CSRobots
{
    public partial class Robot
    {
        private readonly RobotRunner _robot;
        private readonly RobotColorImages _images;
        private readonly int _index;

        internal Robot(RobotRunner robot, RobotColorImages images, int index)
        {
            InitializeComponent();
            _robot = robot;
            _images = images;
            _index = index;
        }

        public string RobotName
        {
            get { return _robot.Name; }
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

        internal void Draw()
        {
            var heading = _robot.Heading;
            var headingIndex = Convert.ToInt32(Math.Floor(heading/10));
            heading = _robot.GunHeading;
            var turretIndex = Convert.ToInt32(Math.Floor(heading/10));
            heading = _robot.RadarHeading;
            var radarIndex = Convert.ToInt32(Math.Floor(heading/10));
            Body.Source = _images.Bodies[headingIndex];
            Turret.Source = _images.Turrets[turretIndex];
            Radar.Source = _images.Radars[radarIndex];
            Canvas.SetTop(this, (_robot.Y - _robot.Size)/2);
            Canvas.SetLeft(this, (_robot.X - _robot.Size)/2);
            Name.Foreground = _images.TextColor;
            Name.Text = _robot.Name;
            Speech.Text = _robot.Speech;
            Speech.Foreground = _images.TextColor;
            Health.Text = "".PadRight(Convert.ToInt32(_robot.Energy)/5, '|');
            Health.Foreground = _images.TextColor;
        }

        public bool Dead()
        {
            return _robot.Dead();
        }
    }
}
