using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using CSBot.Robots;

namespace CSRobots.ViewModels
{
    public class ExplosionView:BaseViewModel
    {
        private readonly Explosion _explosion;
        private readonly IList<BitmapSource> _explosionImages;

        public ExplosionView(Explosion explosion, IList<BitmapSource> explosionImages)
        {
            _explosion = explosion;
            _explosionImages = explosionImages;
        }

        public bool Dead    
        {
            get { return _explosion.Dead; }
        }

        internal void Draw()
        {
            if (_explosion.T < 15)
                BoomImage = _explosionImages[_explosion.T];
            Top = Convert.ToInt32(Convert.ToDouble(_explosion.Y - _explosion.Size)/2);
            Left = Convert.ToInt32(Convert.ToDouble(_explosion.X - _explosion.Size) / 2);
        }

        private BitmapSource _boomImage;
        public BitmapSource BoomImage
        {
            get { return _boomImage; }
            set {SetStructPropertyValue(ref _boomImage, value);}
        }

        private int _left;
        public int Left
        {
            get { return _left; }
            set
            {
                SetStructPropertyValue(ref _left, value);
            }
        }

        private int _top;
        public int Top
        {
            get { return _top; }
            set
            {
                SetStructPropertyValue(ref _top, value);
            }
        }

        protected override void RegisterForMessages(){}
        protected override void SetDesignTimeInfo(){}
    }
}
