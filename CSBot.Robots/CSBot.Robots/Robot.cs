using System;
using System.Collections.Generic;

namespace CSBot.Robots
{
    public abstract class Robot
    {
        public Actions Actions { get; internal set; }
        public Events Events { get; internal set; }
        public State State { get; internal set; }
        public string SkinPrefix { get; internal set; }

        public Robot()
        {
            Events = new Events();
            Actions = new Actions();
            State = new State();
        }

        public void Accelerate(int acceleration)
        {
            Actions.Accelerate = acceleration;
        }

        public void Stop()
        {
            var acceleration = (State.Speed > 0) ? -1 : (State.Speed < 0) ? 1 : 0;
            Accelerate(acceleration);
        }

        public void Fire(decimal fire)
        {
            Actions.Fire = fire;
        }

        public void Turn(double turn)
        {
            Actions.Turn = turn;
        }

        public void TurnGun(double turn)
        {
            Actions.TurnGun = turn;
        }

        public void TurnRadar(double turn)
        {
            Actions.TurnRadar = turn;
        }

        public void Broadcast(string message)
        {
            Actions.Broadcast = message;
        }

        public void Say(string message)
        {
            Actions.Say = message;
        }

        public List<decimal> GotHit()
        {
            return Events.GotHit;
        }

        public List<double> RobotsScanned()
        {
            return Events.RobotsScanned;
        }

        public List<string> Broadcasts()
        {
            return Events.Broadcasts;
        }

        public object Trial
        {
            get { return State.Trial; }
        }

        public int Team
        {
            get { return State.Team; }
        }

        public int BattlefieldHeight
        {
            get { return State.BattlefieldHeight; }
        }

        public int BattlefieldWidth
        {
            get { return State.BattlefieldWidth; }
        }

        public int Size
        {
            get { return State.Size; }
        }

        public bool GameOver
        {
            get { return State.GameOver; }
        }

        public decimal Energy
        {
            get { return State.Energy; }
        }

        public int Time
        {
            get { return State.Time; }
        }

        public double Heading
        {
            get { return State.Heading; }
        }

        public double GunHeading
        {
            get { return State.GunHeading; }
        }

        public double RadarHeading
        {
            get { return State.RadarHeading; }
        }

        public decimal GunHeat
        {
            get { return State.GunHeat; }
        }

        public int Speed
        {
            get { return State.Speed; }
        }

        public double X
        {
            get { return State.X; }
        }

        public double Y
        {
            get { return State.Y; }
        }

        public Random Random { get { return State.Random; } }

        public abstract void Tick(Events events);

        public abstract string Name
        {
            get;
        }
    }
}
