using System;

namespace CSBot.Robots
{
    public class State
    {
        public object Trial { get; set; }

        public int Team { get; set; }
        public int BattlefieldHeight { get; set; }
        public int BattlefieldWidth { get; set; }
        public int Size { get; set; }

        public bool GameOver { get; set; }
        public decimal Energy { get; set; }
        public int Time { get; set; }

        public double Heading { get; set; }
        public double GunHeading { get; set; }
        public double RadarHeading { get; set; }
        public decimal GunHeat { get; set; }

        public int Speed { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public Random Random { get; set; }

        public State Copy()
        {
            return (State)MemberwiseClone();
        }
    }
}
