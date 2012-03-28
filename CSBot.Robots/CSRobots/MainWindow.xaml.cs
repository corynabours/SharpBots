using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
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

        private string[] _textColors = new string[]
                                           {
                                               "#ff0000", "#00ff00", "#0000ff", "#ffff00", "#00ffff", "#ff00ff",
                                               "#ffffff",
                                               "#777777"
                                           };

        private const string DefaultSkinPrefix = "images.red_";
        private readonly IList<RobotAnimation> _robotImages = new List<RobotAnimation>();
        private readonly IList<BitmapImage> _explosionImages = new List<BitmapImage>();
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
            /*foreach (var robot in _battlefield.Robots)
            {
                var imagePath = robot.SkinPrefix ?? _defaultSkinPrefix;
            }*/
            var colorValues = new[] {6, 5, 3, 4, 1, 2, 0, 7};
            for (var i = 0; i < 8; i++)
            {
                int c1 = colorValues[i] % 2;
                int c2 = Convert.ToInt32(Math.Floor(((decimal)colorValues[i] % 4) / 2));
                int c3 = Convert.ToInt32(Math.Floor((decimal)colorValues[i] / 4));
                var bodies = new List<BitmapImage>();
                var turrets = new List<BitmapImage>();
                var radar = new List<BitmapImage>();
                for (int j=0;j<36;j++)
                {
                    bodies.Add(ModifyImageResource("CSRobots." + DefaultSkinPrefix + "body" + j.ToString("D3") + ".gif", c1, c2, c3));
                    turrets.Add(ModifyImageResource("CSRobots." + DefaultSkinPrefix + "turret" + j.ToString("D3") + ".gif", c1, c2, c3));
                    radar.Add(ModifyImageResource("CSRobots." + DefaultSkinPrefix + "radar" + j.ToString("D3") + ".gif", c1, c2, c3));
                }
                _robotImages.Add(new RobotAnimation(bodies, turrets, radar));
            }

            for (var i = 0; i < 15; i++)
            {
                try
                {
                    var boom = GetEmbeddedImageResource("CSRobots.images.explosion" + i.ToString("D2") + ".gif");
                    _explosionImages.Add(boom);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        private static BitmapImage GetEmbeddedImageResource(string fileName)
        {
            var stream = GetDataStreamFromEmbeddResource(fileName);
            var image = Image.FromStream(stream);
            return CreateBitmapImageFromImage(image);
        }

        private static BitmapImage ModifyImageResource(string filename, int c1, int c2, int c3)
        {
            var stream = GetDataStreamFromEmbeddResource(filename);

            var buffer = new byte[1662];
            var read = stream.Read(buffer, 0, 1662);
            var ncolors = Math.Pow(2, 1 + Convert.ToInt32(buffer[10] & 7));
            for (var j = 0;j<ncolors;j++)
            {
                var off = buffer[13 + j * 3];
                var on = buffer[14 + j * 3];
                buffer[13 + j * 3] = (c1 == 1) ? on : off;
                buffer[14 + j * 3] = (c2 == 1) ? on : off;
                buffer[15 + j * 3] = (c3 == 1) ? on : off;
            }
            var ms = new MemoryStream();
            ms.Write(buffer,0,read);

            var image = Image.FromStream(ms);
            return CreateBitmapImageFromImage(image);
        }

        private static BitmapImage CreateBitmapImageFromImage(Image image)
        {
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            var bImg = new BitmapImage();
            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(ms.ToArray());
            bImg.EndInit();
            return new BitmapImage();
        }

        private static Stream GetDataStreamFromEmbeddResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null) throw new Exception("Cannot get existing assembly.");
            var stream = assembly.GetManifestResourceStream(fileName);
            if (stream == null) throw new Exception("Cannot find requested resource: " + fileName);
            return stream;
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
            var index = -1;
            foreach (var robot in _battlefield.Robots)
            {
                index++;
                if (robot.Dead()) continue;
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
                draw_radar(robot);
                /*
                @robots[ai].speech.configure(:text => "#{ai.speech}",
                                             :coords => [ai.x / 2, ai.y / 2 - ai.size / 2])
                @robots[ai].info.configure(:text => "#{ai.name}\n#{'|' * (ai.energy / 5)}",
                                           :coords => [ai.x / 2, ai.y / 2 + ai.size / 2])
                @robots[ai].status.configure(:text => "#{ai.name.ljust(20)} #{'%.1f' % ai.energy}")*/
            }
        }

        private void draw_radar(RobotRunner robot)
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
