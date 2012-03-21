
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

        public override void Tick(Events events)
        {
            if (Time == 0) TurnRadar(5)  ;
            //if (RobotsScanned().Count>0) Fire(3.0m);
            Fire(0.1m);
            TurnGun(1);
        }
    }
}
