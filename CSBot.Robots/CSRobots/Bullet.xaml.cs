using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CSRobots
{
    public partial class Bullet
    {
        private readonly CSBot.Robots.Bullet _bullet;
       
        public Bullet(CSBot.Robots.Bullet bullet)
        {
            InitializeComponent();
            _bullet = bullet;
        }

        public bool Dead
        {
            get { return _bullet.Dead; }
        }

        public void Draw()
        {
            Dispatcher.Invoke(DispatcherPriority.Render,
                              new Action(delegate
                                             {
                                                 Canvas.SetTop(this, (_bullet.Y - 2) / 2);
                                                 Canvas.SetLeft(this, (_bullet.X - 2) / 2);
                                             }));
        }

        /* @bullets[bullet] ||= TkcOval.new(
           @canvas, [-2, -2], [3, 3],
           :fill=>'#'+("%02x" % (128+bullet.energy*14).to_i)*3)
         @bullets[bullet].coords(
           bullet.x / 2 - 2, bullet.y / 2 - 2,
           bullet.x / 2 + 3, bullet.y / 2 + 3)*/
    }
}
