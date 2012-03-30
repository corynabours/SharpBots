using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CSBot.Robots;
using Timer = System.Timers.Timer;

namespace CSRobots.ViewModels
{
    public class MainView : BaseViewModel
    {
        private readonly Timer _timer;
        private readonly Battlefield _battlefield;

        //private readonly IList<Line> _radarLines = new List<Line>(); 
        private readonly IList<BitmapSource> _explosionImages = new List<BitmapSource>();
        private readonly bool _showRadar; 
        private const int SpeedMultiplier = 1;

        public static RunOptions RunOptions { get; set; }
        private readonly Thread _uiThread;

        public MainView()
        {
            _uiThread = Thread.CurrentThread;
            _timer = new Timer {Interval = 5};
            _timer.Elapsed += TimerElapsed;
            Height = RunOptions.Y;
            Width = RunOptions.X;
            _battlefield = RunOptions.Battlefield;
            _showRadar = false;
            InitCanvas();
            DrawFrame();
            _timer.Enabled = true;
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set {SetStructPropertyValue(ref _height, value);}
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set {SetStructPropertyValue(ref _width, value);}
        }

        private static bool _inTimer;
        private int _gameOverTicks;

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_inTimer) return;
                _inTimer = true;
                DrawFrame();
                if (_gameOverTicks == 50)
                {
                    Close();
                }
                _inTimer = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private void Close()
        {
            
        }

        public void InitCanvas()
        {
            var robotImages = new List<RobotColorImages>();
            for (var i = 0; i < 8; i++)
            {
                robotImages.Add(new RobotColorImages(i));
            }

            var index = 0;
            Bots = new ObservableCollection<RobotView>();
            foreach(var robot in _battlefield.Robots)
            {
                var displayRobot = new RobotView(robot, robotImages[robot.Team], index++);
                _bots.Add(displayRobot);
                var displayStatus = new RobotStatus {Color = robotImages[robot.Team].TextColor};
                _status.Add(displayStatus);
            }

            for (var i = 0; i < 15; i++)
            {
                try
                {
                    var boom = EmbeddedImage.GetEmbeddedImageResource("CSRobots.ViewModels.images.explosion" + i.ToString("D2") + ".gif");
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
            var explosionRemovals = _explosions.Where(explosion => explosion.Dead).ToList();
            foreach(var explosion in explosionRemovals)
            {
                var explosion1 = explosion;
                Dispatcher.FromThread(_uiThread).Invoke(DispatcherPriority.Render, new Action(() => RemoveExplosion(explosion1)));
            }

            var bulletRemovals = _bullets.Where(bullet => bullet.Dead).ToList();
            foreach (var bullet in bulletRemovals)
            {
                var bullet1 = bullet;
                Dispatcher.FromThread(_uiThread).Invoke(DispatcherPriority.Render, new Action(() => RemoveBullet(bullet1)));
            }

            //foreach (var line in _radarLines)
            //{
                //Canvas.Children.Remove(line);
            //}
            //_radarLines.Clear();

            var botRemovals = Bots.Where(bot => bot.Dead()).ToList();
            foreach (var bot in botRemovals)
            {
                var bot1 = bot;
                Dispatcher.FromThread(_uiThread).Invoke(DispatcherPriority.Render, new Action(() => RemoveRobot(bot1)));
                Dispatcher.FromThread(_uiThread).Invoke(DispatcherPriority.Render, new Action(() => SetStatusText(bot1.Index, bot1.RobotName.PadRight(20, ' ') + "dead")));
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

                    GameOver = "Team #" + team.ToString(CultureInfo.InvariantCulture) + " won!";
                }
                _battlefield.Tick();
            }

        }

        private string _gameOver;
        public string GameOver
        {
            get { return _gameOver; }
            set {SetStructPropertyValue(ref _gameOver, value);}
        }

        private void DisplayDraw()
        {
            GameOver ="Draw!";
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
            foreach (var robot in Bots.Where(robot => !robot.Dead()))
            {
                robot.Draw();
                var robot1 = robot;
                Dispatcher.FromThread(_uiThread).Invoke(DispatcherPriority.Render, new Action(() => SetStatusText(robot1.Index, robot1.RobotName.PadRight(20, ' ') + robot1.Energy.ToString(CultureInfo.InvariantCulture))));
                DrawRadar(robot);
            }
        }

        private void SetStatusText(int index, string value)
        {
            Status[index].Text = value;
        }

        private void DrawRadar(RobotView robot)
        {
            if (!_showRadar) return;
            var angle = robot.RadarHeading;
            var x = robot.X + (Math.Cos(angle*Math.PI/180)*3200);
            var y = robot.Y - (Math.Sin(angle*Math.PI/180)*3200);
            //var radarLine = new Line {X1 = robot.X/2, Y1 = robot.Y/2, X2 = x/2, Y2 = y/2};
            //_radarLines.Add(radarLine);
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

            var bullets = new List<Bullet>(_battlefield.NewBullets);
            foreach (var bullet in bullets)
            {
                var newBullet = new BulletView(bullet);
                    //Dispatcher.Invoke(DispatcherPriority.Render,new Action(Close));

                Dispatcher.FromThread(_uiThread).Invoke(DispatcherPriority.Render, new Action(() => AddBullet(newBullet)));
                _battlefield.Process(bullet);
                newBullet.Draw();
            }
        }

        private void AddBullet(BulletView newBullet)
        {
            _bullets.Add(newBullet);
        }

        private void RemoveBullet(BulletView bullet)
        {
            _bullets.Remove(bullet);
        }

        private void AddExplosion(ExplosionView newExplosion)
        {
            _explosions.Add(newExplosion);
        }

        private void RemoveExplosion(ExplosionView explosion)
        {
            _explosions.Remove(explosion);
        }

        private void RemoveRobot(RobotView robot)
        {
            _bots.Remove(robot);
        }

        private void DrawExplosions()
        {
            
            foreach (var explosion in _explosions)
            {
                explosion.Draw();
            }

            var explosions = new List<Explosion>(_battlefield.NewExplosions);
            foreach (var explosion in explosions)
            {
                var newExplosion = new ExplosionView(explosion, _explosionImages);
                Dispatcher.FromThread(_uiThread).Invoke(DispatcherPriority.Render, new Action(() => AddExplosion(newExplosion)));
                _battlefield.Process(explosion);
                newExplosion.Draw();
            }
        }

        private ObservableCollection<RobotView> _bots = new ObservableCollection<RobotView>();
        public ObservableCollection<RobotView> Bots
        {
            get { return _bots; }
            set {SetPropertyValue(ref _bots, value);}
        }

        private ObservableCollection<BulletView> _bullets = new ObservableCollection<BulletView>();
        public ObservableCollection<BulletView> Bullets
        {
            get { return _bullets; }
            set {SetStructPropertyValue(ref _bullets, value);}
        }

        private ObservableCollection<ExplosionView> _explosions = new ObservableCollection<ExplosionView>();

        public ObservableCollection<ExplosionView> Explosions
        {
            get { return _explosions; }
            set { SetStructPropertyValue(ref _explosions, value); }
        }

        private ObservableCollection<RobotStatus> _status = new ObservableCollection<RobotStatus>();
        public ObservableCollection<RobotStatus> Status
        {
            get { return _status; }
            set {SetStructPropertyValue(ref _status, value);}
        }

        protected override void RegisterForMessages() {}
        protected override void SetDesignTimeInfo() {}
    }
}
