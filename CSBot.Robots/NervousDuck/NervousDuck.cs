
using CSBot.Robots;

namespace NervousDuck
{
    public class NervousDuck : Robot
    {
        public override void Tick(Events events)
        {
            if(Time == 0) TurnRadar(1);
            if (Time < 3) TurnGun(30);
            Accelerate(1);
            Turn(2);
            if(RobotsScanned().Count > 0) Fire(3.0m);
        }

        public override string Name
        {
            get
            {
                return "NervousDuck";
            }
        }
    }
}
