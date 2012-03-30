using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CSRobots
{
    public partial class Explosion
    {
        private readonly CSBot.Robots.Explosion _explosion;
        private readonly IList<BitmapSource> _explosionImages;

        public Explosion(CSBot.Robots.Explosion explosion, IList<BitmapSource> explosionImages)
        {
            InitializeComponent();
            _explosion = explosion;
            _explosionImages = explosionImages;
        }

        public bool Dead    
        {
            get { return _explosion.Dead; }
        }

        internal void Draw()
        {
            Dispatcher.Invoke(DispatcherPriority.Render,
                              new Action(delegate
                                             {
                                                 if ((Boom != null)&&(_explosion.T<15))
                                                     Boom.Source = _explosionImages[_explosion.T];
                                                 Canvas.SetTop(this, Convert.ToDouble(_explosion.Y - _explosion.Size)/2);
                                                 Canvas.SetLeft(this, Convert.ToDouble(_explosion.X - _explosion.Size)/2);
                                             }));
        }
    }
}
