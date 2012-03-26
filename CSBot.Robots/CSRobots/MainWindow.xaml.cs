using System;
using System.Collections.Generic;
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

        private string _defaultSkinPrefix = "images/red_";
        private IList<RobotAnimation> _robots = new List<RobotAnimation>();
        private IList<BulletAnimation> _bullets = new List<BulletAnimation>();
        private IList<ExplosionAnimation> _explosions = new List<ExplosionAnimation>();
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
            foreach (var robot in _battlefield.Robots)
            {
                var imagePath = robot.SkinPrefix ?? _defaultSkinPrefix;
            }
            var defaultColors = new int[8][];
            for (var i = 0; i < 8; i++)
            {
                defaultColors[i] = new int[3];
                defaultColors[i][0] = i%2;
                defaultColors[i][1] = Convert.ToInt32(Math.Floor(((decimal) i%4)/2));
                defaultColors[i][2] = Convert.ToInt32(Math.Floor((decimal) i/4));
            }
            /*
            [[0,1,1],[1,0,1],[1,1,0],[0,0,1],[1,0,0],[0,1,0],[0,0,0],[1,1,1]][0...@battlefield.robots.length].zip(@battlefield.robots) do |color, robot|
              bodies, guns, radars = [], [], []
              image_path = robot.skin_prefix || @default_skin_prefix
              reader = robot.skin_prefix ? lambda{|fn| TkPhotoImage.new(:file => fn) } : lambda{|fn| read_gif(fn, *color)}
              36.times do |i|
                bodies << reader["#{image_path}body#{(i*10).to_s.rjust(3, '0')}.gif"]
                guns << reader["#{image_path}turret#{(i*10).to_s.rjust(3, '0')}.gif"]
                radars << reader["#{image_path}radar#{(i*10).to_s.rjust(3, '0')}.gif"]
              end
              @colors << TkRobot.new(bodies << bodies[0], guns << guns[0], radars << radars[0])
            end

             */
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
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null) throw new Exception("Cannot get existing assembly.");
            var stream = assembly.GetManifestResourceStream(fileName);
            if (stream == null) throw new Exception("Cannot find requested resource: " + fileName);
            var image = new BitmapImage {StreamSource = stream};
            return image;
        }

        /*def read_gif name, c1, c2, c3
          data = nil
          open(name, 'rb') do |f|
            data = f.read()
            ncolors = 2**(1 + data[10][0].to_i + data[10][1].to_i * 2 + data[10][2].to_i * 4)
            ncolors.times do |j|
              data[13 + j.to_i * 3 + 0], data[13 + j.to_i * 3 + 1], data[13 + j.to_i * 3 + 2] =
                data[13 + j.to_i * 3 + c1.to_i], data[13 + j.to_i * 3 + c2.to_i], data[13 + j.to_i * 3 + c3.to_i]
            end
          end
          TkPhotoImage.new(:data => Base64.encode64(data))
        end*/

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
