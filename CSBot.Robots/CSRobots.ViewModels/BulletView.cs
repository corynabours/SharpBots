using System;

namespace CSRobots.ViewModels
{
    public class BulletView : BaseViewModel
    {
        private readonly CSBot.Robots.Bullet _bullet;

        public BulletView(CSBot.Robots.Bullet bullet)
        {
            _bullet = bullet;
        }

        public bool Dead
        {
            get { return _bullet.Dead; }
        }

        private int _top;
        public int Top
        {
            get { return _top; }
            set { SetStructPropertyValue(ref _top, value);}
        }

        private int _left;
        public int Left
        {
            get { return _left; }
            set { SetStructPropertyValue(ref _left, value);}
        }

        public void Draw()
        {
            Top = Convert.ToInt32((_bullet.Y) / 2);
            Left = Convert.ToInt32((_bullet.X) / 2);
        }

        /* @bullets[bullet] ||= TkcOval.new(
           @canvas, [-2, -2], [3, 3],
           :fill=>'#'+("%02x" % (128+bullet.energy*14).to_i)*3)
         @bullets[bullet].coords(
           bullet.x / 2 - 2, bullet.y / 2 - 2,
           bullet.x / 2 + 3, bullet.y / 2 + 3)*/

        protected override void RegisterForMessages(){}
        protected override void SetDesignTimeInfo(){}
    }
}
