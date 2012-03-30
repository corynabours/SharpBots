using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CSBot.Robots;

namespace CSRobots
{
    public partial class MainWindow
    {
        private readonly Timer _timer;
        private readonly Battlefield _battlefield;

        private readonly IList<Robot> _robots = new List<Robot>();
        private readonly IList<Bullet>  _bullets = new List<Bullet>();
        private readonly IList<Explosion> _explosions = new List<Explosion>();
        private readonly IList<Line> _radarLines = new List<Line>(); 
        private readonly IList<BitmapSource> _explosionImages = new List<BitmapSource>();
        private readonly bool _showRadar; 
        private const int SpeedMultiplier = 1;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new Timer {Interval = 5};
            _timer.Elapsed += TimerElapsed;
            Height = Program.RunOptions.Y;
            Width = Program.RunOptions.X;
            _battlefield = Program.RunOptions.Battlefield;
            _showRadar = false;
            InitCanvas();
            DrawFrame();
            _timer.Enabled = true;
        }

        private static bool _inTimer;
        private int _gameOverTicks;

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_inTimer) return;
                _inTimer = true;
                Dispatcher.Invoke(DispatcherPriority.Render,new Action(DrawFrame));
                if (_gameOverTicks == 50)
                {
                    Dispatcher.Invoke(DispatcherPriority.Render,new Action(Close));
                }
                _inTimer = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public void InitCanvas()
        {
            var robotImages = new List<RobotColorImages>();
            for (var i = 0; i < 8; i++)
            {
                robotImages.Add(new RobotColorImages(i));
            }

            var index = 0;
            foreach(var robot in _battlefield.Robots)
            {
                var displayRobot = new Robot(robot, robotImages[robot.Team], index++);
                _robots.Add(displayRobot);
                Canvas.Children.Add(displayRobot);
            }

            for (var i = 0; i < 15; i++)
            {
                try
                {
                    var boom = EmbeddedImage.GetEmbeddedImageResource("CSRobots.images.explosion" + i.ToString("D2") + ".gif");
                    _explosionImages.Add(boom);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        private void DrawFrame()
        {
            Simulate(SpeedMultiplier);
            DrawBattlefield();
        }

        private void Simulate(int ticks)
        {
            var explosionRemovals = new List<Explosion>();
            foreach (var explosion in _explosions.Where(explosion => explosion.Dead))
            {
                Canvas.Children.Remove(explosion);
                explosionRemovals.Add(explosion);
            }
            foreach(var explosion in explosionRemovals) _explosions.Remove(explosion);

            var bulletRemovals = new List<Bullet>();
            foreach (var bullet in _bullets.Where(bullet => bullet.Dead))
            {
                Canvas.Children.Remove(bullet);
                bulletRemovals.Add(bullet);
            }
            foreach (var bullet in bulletRemovals) _bullets.Remove(bullet);
            var statusText = new[] {Robot0, Robot1, Robot2, Robot3, Robot4, Robot5, Robot6, Robot7};

            foreach (var line in _radarLines)
            {
                Canvas.Children.Remove(line);
            }
            _radarLines.Clear();

            for (var index = 0; index < _robots.Count; index++)
            {
                if (!_robots[index].Dead()) continue;
                var robot = _robots[index];
                _robots.RemoveAt(index);
                Canvas.Children.Remove(robot);
                statusText[robot.Index].Text= robot.RobotName.PadRight(20,' ') + "dead";
            }

            for (var index = 0; index < ticks; index++)
            {
                if (_battlefield.GameOver)
                {
                    _gameOverTicks++;
                    var winners = _battlefield.Robots.Where(robot => !robot.Dead()).ToList();
                    if (winners.Count == 0)
                    {
                        DisplayDraw();
                        return;
                    }

                    var team = winners[0].Team;
                    var draw = _battlefield.Robots.Any(robot => !robot.Dead() && (robot.Team != team));

                    if (draw)
                    {
                        DisplayDraw();
                        return;
                    }

                    GameOver.Text = "Team #" + team.ToString(CultureInfo.InvariantCulture) + " won!";
                }
                _battlefield.Tick();
            }

        }

        private void DisplayDraw()
        {
            GameOver.Text="Draw!";
        }

        private void DrawBattlefield()
        {
            /*while (!_radar_lines.empty ? )
            {
                old_line = @radar_lines.pop
                @canvas.delete(old_line)
            }*/
            DrawRobots();
            DrawBullets();
            DrawExplosions();
        }

        private void DrawRobots()
        {
            var statusText = new[] { Robot0, Robot1, Robot2, Robot3, Robot4, Robot5, Robot6, Robot7 };
            foreach (var robot in _robots.Where(robot => !robot.Dead()))
            {
                robot.Draw();
                statusText[robot.Index].Text = robot.RobotName.PadRight(20, ' ') + robot.Energy.ToString(CultureInfo.InvariantCulture);
                statusText[robot.Index].Foreground = robot.Color;
                DrawRadar(robot);
            }
        }

        private void DrawRadar(Robot robot)
        {
            if (!_showRadar) return;
            var angle = robot.RadarHeading;
            var x = robot.X + (Math.Cos(angle*Math.PI/180)*3200);
            var y = robot.Y - (Math.Sin(angle*Math.PI/180)*3200);
            var radarLine = new Line {X1 = robot.X/2, Y1 = robot.Y/2, X2 = x/2, Y2 = y/2};
            _radarLines.Add(radarLine);
            //radar_line = TkcLine.new(@canvas, robot.x/2, robot.y/2, x/2, y/2)
            //radar_line.fill @text_colors[robot.team]
            //@radar_lines << radar_line
        }

        private void DrawBullets()
        {
            foreach (var bullet in _bullets)
            {
                bullet.Draw();
            }

            var bullets = new List<CSBot.Robots.Bullet>(_battlefield.NewBullets);
            foreach (var bullet in bullets)
            {
                var newBullet = new Bullet(bullet);
                Canvas.Children.Add(newBullet);
                _bullets.Add(newBullet);
                _battlefield.Process(bullet);
                newBullet.Draw();
            }
        }

        private void DrawExplosions()
        {
            foreach (var explosion in _explosions)
            {
                explosion.Draw();
            }

            var explosions = new List<CSBot.Robots.Explosion>(_battlefield.NewExplosions);
            foreach (var explosion in explosions)
            {
                var newExplosion = new Explosion(explosion, _explosionImages);
                Canvas.Children.Add(newExplosion);
                _explosions.Add(newExplosion);
                _battlefield.Process(explosion);
                newExplosion.Draw();
            }
        }
    }
}
