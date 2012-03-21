using System.Collections.Generic;

namespace CSBot.Robots
{
    public class Events
    {
        private List<double> _robotsScanned = new List<double>();
        private List<decimal> _gotHit = new List<decimal>();
        private List<string> _broadcasts = new List<string>();

        public List<double> RobotsScanned { get { return _robotsScanned; } }
        public List<decimal> GotHit { get { return _gotHit; } }
        public List<string> Broadcasts { get { return _broadcasts; } }

        public void Clear()
        {
            _robotsScanned = new List<double>();
            _gotHit = new List<decimal>();
            _broadcasts = new List<string>();
        }
    }
}
