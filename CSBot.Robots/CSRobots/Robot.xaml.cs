using System;
using System.Windows.Controls;
using System.Windows.Threading;
using CSBot.Robots;

namespace CSRobots
{
    public partial class Robot
    {
        private readonly RobotRunner _robot;
        private readonly RobotColorImages _images;
        internal Robot(RobotRunner robot, RobotColorImages images)
        {
            InitializeComponent();
            _robot = robot;
            _images = images;
        }

        internal void Draw()
        {
            var heading = _robot.Heading;
            var headingIndex = Convert.ToInt32(Math.Floor(heading / 10));
            heading = _robot.GunHeading;
            var turretIndex = Convert.ToInt32(Math.Floor(heading / 10));
            heading = _robot.RadarHeading;
            var radarIndex = Convert.ToInt32(Math.Floor(heading / 10));
            Dispatcher.Invoke(DispatcherPriority.Render,
                              new Action(delegate
                                             {
                                                 Body.Source = _images.Bodies[headingIndex];
                                                 Turret.Source = _images.Turrets[turretIndex];
                                                 Radar.Source = _images.Radars[radarIndex];
                                                 Canvas.SetTop(this, (_robot.Y - _robot.Size)/2);
                                                 Canvas.SetLeft(this, (_robot.X - _robot.Size)/2);
                                                 Name.Foreground = _images.TextColor;
                                                 Name.Text = _robot.Name;
                                                 Speech.Text = _robot.Speech;
                                                 Speech.Foreground = _images.TextColor;
                                                 Health.Text = "".PadRight(Convert.ToInt32(_robot.Energy) / 5, '|');
                                                 Health.Foreground = _images.TextColor;
                                             }));
        }
        /*@robots[ai] ||= TkRobot.new(
          TkcImage.new(@canvas, 0, 0),
          TkcImage.new(@canvas, 0, 0),
          TkcImage.new(@canvas, 0, 0),
          TkcText.new(@canvas,
          :fill => @text_colors[ai.team],
          :anchor => 's', :justify => 'center', :coords => [ai.x / 2, ai.y / 2 - ai.size / 2]),
          TkcText.new(@canvas,
          :fill => @text_colors[ai.team],
          :anchor => 'n', :justify => 'center', :coords => [ai.x / 2, ai.y / 2 + ai.size / 2]),
          TkcText.new(@canvas,
          :fill => @text_colors[ai.team],
          :anchor => 'nw', :coords => [10, 15 * i + 10], :font => TkFont.new("courier 9")))
        @robots[ai].body.configure( :image => @colors[ai.team].body[(ai.heading+5) / 10],
                                    :coords => [ai.x / 2, ai.y / 2])
        @robots[ai].gun.configure(  :image => @colors[ai.team].gun[(ai.gun_heading+5) / 10],
                                    :coords => [ai.x / 2, ai.y / 2])
        @robots[ai].radar.configure(:image => @colors[ai.team].radar[(ai.radar_heading+5) / 10],
                                    :coords => [ai.x / 2, ai.y / 2])*/
        /*
        @robots[ai].speech.configure(:text => "#{ai.speech}",
                                     :coords => [ai.x / 2, ai.y / 2 - ai.size / 2])
        @robots[ai].info.configure(:text => "#{ai.name}\n#{'|' * (ai.energy / 5)}",
                                   :coords => [ai.x / 2, ai.y / 2 + ai.size / 2])
        @robots[ai].status.configure(:text => "#{ai.name.ljust(20)} #{'%.1f' % ai.energy}")*/

        public bool Dead()
        {
            return _robot.Dead();
        }
    }
}
