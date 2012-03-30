using System;
using System.Drawing;

namespace CSRobots.ViewModels
{
    public class BulletView : BaseViewModel
    {
        private readonly CSBot.Robots.Bullet _bullet;

        public BulletView(CSBot.Robots.Bullet bullet)
        {
            _bullet = bullet;
//           :fill=>'#'+("%02x" % (128+bullet.energy*14).to_i)*3)
            _color = new SolidBrush(new Color());
        }

        public bool Dead
        {
            get { return _bullet.Dead; }
        }

        private int _top;
        public int Top
        {
            get { return _top; }
            set { SetPropertyValue(out _top, value); }
        }

        private int _left;
        public int Left
        {
            get { return _left; }
            set { SetPropertyValue(out _left, value); }
        }

        public void Draw()
        {
            Top = Convert.ToInt32((_bullet.Y) / 2);
            Left = Convert.ToInt32((_bullet.X) / 2);
        }

        private Brush _color;
        public Brush Color
        {
            get { return _color; }
            set { SetPropertyValue(out _color, value); }
        }
    }
}
