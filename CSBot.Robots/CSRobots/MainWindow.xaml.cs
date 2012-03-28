using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;
using CSBot.Robots;

namespace CSRobots
{
    public partial class MainWindow : Window
    {
        private readonly Timer _timer;
        private readonly Battlefield _battlefield;

        private readonly IList<Robot> _robots = new List<Robot>();
        private readonly IList<BitmapSource> _explosionImages = new List<BitmapSource>();
        private const int SpeedMultiplier = 1;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new Timer {Interval = 20};
            _timer.Elapsed += TimerElapsed;
            Height = Program.RunOptions.Y;
            Width = Program.RunOptions.X;
            _battlefield = Program.RunOptions.Battlefield;
            // @on_game_over_handlers = []
            // @radar_lines = []
            // @show_radar = false
            InitCanvas();
            DrawFrame();
            _timer.Enabled = true;
        }

        // def on_game_over(&block)
        //     @on_game_over_handlers << block
        // end

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                DrawFrame();
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

            foreach(var robot in _battlefield.Robots)
            {
                var displayRobot = new Robot(robot, robotImages[robot.Team]);
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
            /*@explosions.reject!{|e,tko| @canvas.delete(tko) if e.dead; e.dead }
    @bullets.reject!{|b,tko| @canvas.delete(tko) if b.dead; b.dead }
    @robots.reject! do |ai,tko|
      if ai.dead
        tko.status.configure(:text => "#{ai.name.ljust(20)} dead")
        tko.each{|part| @canvas.delete(part) if part != tko.status}
        true
      end
    end*/
            for (int index = 0; index < ticks; index++)
            {
                if (_battlefield.GameOver)
                {
                    /*@on_game_over_handlers.each{|h| h.call(@battlefield) }
        unless @game_over
          winner = @robots.keys.first
          whohaswon = if winner.nil?
            "Draw!"
          elsif @battlefield.teams.all?{|k,t|t.size<2}
            "#{winner.name} won!"
          else
            "Team #{winner.team} won!"
          end
          text_color = winner ? winner.team : 7
          @game_over = TkcText.new(canvas,
            :fill => @text_colors[text_color],
            :anchor => 'c', :coords => [400,400], :font=>'courier 36', :justify => 'center',
            :text => "GAME OVER\n#{whohaswon}")
        end*/
                }
                _battlefield.Tick();
            }
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
            foreach (var robot in _robots.Where(robot => !robot.Dead()))
            {
                robot.Draw();
        //        @robots[ai].status.configure(:text => "#{ai.name.ljust(20)} #{'%.1f' % ai.energy}")*/
                draw_radar(robot);
            }
        }

        private void draw_radar(Robot robot)
        {
            /*if (show_radar)
            {
                 angle = robot.radar_heading
                 x = robot.x + (Math.cos(angle * Math::PI/180) * 3200)
                 y = robot.y - (Math.sin(angle * Math::PI/180) * 3200)
                 radar_line = TkcLine.new(@canvas, robot.x/2, robot.y/2, x/2, y/2)
                 radar_line.fill @text_colors[robot.team]
                 @radar_lines << radar_line
            }*/
        }

        private void DrawBullets()
        {
            foreach (var bullet in _battlefield.Bullets)
            {
                /* @bullets[bullet] ||= TkcOval.new(
                   @canvas, [-2, -2], [3, 3],
                   :fill=>'#'+("%02x" % (128+bullet.energy*14).to_i)*3)
                 @bullets[bullet].coords(
                   bullet.x / 2 - 2, bullet.y / 2 - 2,
                   bullet.x / 2 + 3, bullet.y / 2 + 3)*/
            }
        }

        private void DrawExplosions()
        {
            foreach (var explosion in _battlefield.Explosions)
            {
                //@explosions[explosion] ||= TkcImage.new(@canvas, explosion.x / 2, explosion.y / 2)
                //@explosions[explosion].image(boom[explosion.t])
            }
        }
    }
}
