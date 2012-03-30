
using CSBot.Robots;

namespace SittingDuck
{
    public class SittingDuck : Robot
    {
        public override string Name
        {
            get
            {
                return "SittingDuck";
            }
        }

        private int _lastHit;
        public override void Tick(Events events)
        {
            if (Time == 0) TurnRadar(5)  ;
            if (RobotsScanned().Count>0) Fire(3.0m);
            TurnGun(1);
            Turn(2);
            if (GotHit().Count>0) _lastHit = Time;
            if (_lastHit > 0 && Time - _lastHit < 20)
                Accelerate(1);
            else
                Stop();
        }
    }
}
