using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Windows.Media.Imaging;

namespace CSRobots
{
    /// <summary>
    /// Interaction logic for Explosion.xaml
    /// </summary>
    public partial class Explosion : UserControl
    {
        private readonly CSBot.Robots.Explosion _explosion;
        private readonly IList<BitmapSource> _explosionImages;

        public Explosion(CSBot.Robots.Explosion explosion)
        {
            InitializeComponent();
            _explosion = explosion;
        }

        public Explosion(CSBot.Robots.Explosion explosion, IList<BitmapSource> explosionImages)
        {
            _explosion = explosion;
            _explosionImages = explosionImages;
            Loaded += ExplosionLoaded;
        }

        void ExplosionLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Boom != null)
                Boom.Source = _explosionImages[_explosion.T];      
        }

        public bool Dead    
        {
            get { return _explosion.Dead; }
        }

        internal void Draw()
        {
            if (Boom != null)
                Boom.Source = _explosionImages[_explosion.T];
            Canvas.SetTop(this, Convert.ToDouble(_explosion.Y - _explosion.Size) / 2);
            Canvas.SetLeft(this, Convert.ToDouble(_explosion.X - _explosion.Size) / 2);
        }

        
    }
}
