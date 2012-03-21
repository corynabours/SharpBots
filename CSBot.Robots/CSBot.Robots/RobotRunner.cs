using System;

namespace CSBot.Robots
{
    public class RobotRunner
    {
        private const decimal FireMin = 0.0m;
        private const decimal FireMax = 3.0m;
        private const double TurnMin = -10.0;
        private const double TurnMax = 10.0;
        private const double TurnGunMin = -30.0;
        private const double TurnGunMax = 30.0;
        private const double TurnRadarMin = -60.0;
        private const double TurnRadarMax = 60.0;
        private const int AccelerateMin = -1;
        private const int AccelerateMax = 1;
        private const int SayMax = 256;
        private const int BroadcastMax = 50;
        private readonly Battlefield _battlefield;
        private readonly Events _events = new Events();

        private readonly State _state = new State();

        private Actions _actions = new Actions();
        private double _newRadarHeading;
        private double _oldRadarHeading;

        private string _speech = string.Empty;

        private int _speechCounter;

        public RobotRunner(Robot robot, Battlefield battlefield, int team)
        {
            Robot = robot;
            _battlefield = battlefield;
            _state.Team = team;
            SetInitialState();
        }

        private Robot Robot { get; set; }
        public decimal DamageGiven { get; set; }
        public int Kills { get; set; }

        private Actions Actions
        {
            get { return _actions; }
        }

        private string Speech
        {
            get { return _speech; }
        }

        private string SkinPrefix
        {
            get { return Robot.SkinPrefix; }
            //set { .Robot.SkinPrefix = value; }
        }

        internal double X
        {
            get { return _state.X; }
        }

        internal double Y
        {
            get { return _state.Y; }
        }

        internal int Team
        {
            get { return _state.Team; }
        }

        public string Name
        {
            get { return Robot.Name; }
        }

        public decimal Energy
        {
            get { return _state.Energy; }
            set { _state.Energy = value; }
        }

        private void SetInitialState()
        {
            _state.X = Convert.ToDouble(_battlefield.Width)/2;
            _state.Y = Convert.ToDouble(_battlefield.Height)/2;
            _speechCounter = -1;
            _speech = null;
            _state.Time = 0;
            _state.Size = 60;
            _state.Speed = 0;
            _state.Energy = 100;
            DamageGiven = 0;
            Kills = 0;
            Teleport();
        }

        private void Teleport()
        {
            int distanceX = _battlefield.Width/2;
            int distanceY = _battlefield.Height/2;
            Random random = _battlefield.Random;
            _state.X += Convert.ToDouble((random.NextDouble() - 0.5))*2*distanceX;
            _state.Y += Convert.ToDouble((random.NextDouble() - 0.5)*2*distanceY);
            _state.GunHeat = 3;
            _state.Heading = (random.Next()*360);
            _state.GunHeading = _state.Heading;
            _state.RadarHeading = _state.Heading;
        }

        internal decimal Hit(Bullet bullet)
        {
            decimal damage = bullet.Energy;
            _state.Energy -= damage;
            _events.GotHit.Add(_state.Energy);
            return damage;
        }

        public bool Dead()
        {
            return _state.Energy < 0;
        }

        private double Clamp(double var, double min, double max)
        {
            return (var > max) ? max : (var < min ? min : var);
        }

        private decimal Clamp(decimal var, decimal min, decimal max)
        {
            return (var > max) ? max : (var < min ? min : var);
        }

        private int Clamp(int var, int min, int max)
        {
            return (var > max) ? max : (var < min ? min : var);
        }

        internal void InternalTick()
        {
            UpdateState();
            RobotTick();
            ParseActions();
            Fire();
            Turn();
            Move();
            Scan();
            Speak();
            Broadcast();
            _state.Time += 1;
        }

        private void ParseActions()
        {
            _actions = new Actions();
            if (Robot.Actions.Fire != 0) Actions.Fire = Clamp(Robot.Actions.Fire, FireMin, FireMax);
            if (Math.Abs(Robot.Actions.Turn) <= 0.01) Actions.Turn = Clamp(Robot.Actions.Turn, TurnMin, TurnMax);
            if (Math.Abs(Robot.Actions.TurnGun) <= 0.01)
                Actions.TurnGun = Clamp(Robot.Actions.TurnGun, TurnGunMin, TurnGunMax);
            if (Math.Abs(Robot.Actions.TurnRadar) <= 0.01)
                Actions.TurnRadar = Clamp(Robot.Actions.TurnRadar, TurnRadarMin, TurnRadarMax);
            if (Robot.Actions.Accelerate != 0)
                Actions.Accelerate = Clamp(Robot.Actions.Accelerate, AccelerateMin, AccelerateMax);
            if (!String.IsNullOrEmpty(Robot.Actions.Say)) Actions.Say = MaxLength(Robot.Actions.Say, SayMax);
            if (!String.IsNullOrEmpty(Robot.Actions.Broadcast))
                Actions.Say = MaxLength(Robot.Actions.Broadcast, BroadcastMax);
        }


        private static string MaxLength(string value, int maxLength)
        {
            return value.Substring(0, Math.Min(maxLength, value.Length));
        }

        private void UpdateState()
        {
            Robot.State = _state.Copy();
            Robot.Events = _events;
            Robot.Actions = new Actions();
        }

        private void RobotTick()
        {
            Robot.Tick(Robot.Events);
            _events.Clear();
        }

        private void Fire()
        {
            decimal fireStrength = _actions.Fire;
            if ((fireStrength > 0) && (_state.GunHeat == 0.0m))
            {
                var bullet = new Bullet(_battlefield, _state.X, _state.Y, _state.GunHeading, 30, fireStrength*3.0m, this);
                for (int i = 0; i < 3; i++) bullet.Tick();
                _battlefield.Add(bullet);
                _state.GunHeat = fireStrength;
            }
            _state.GunHeat -= 0.1m;
            if (_state.GunHeat < 0) _state.GunHeat = 0;
        }

        private void Turn()
        {
            _oldRadarHeading = _state.RadarHeading;
            _state.Heading += _actions.Turn;

            _state.GunHeading += _actions.Turn + _actions.TurnGun;
            _state.RadarHeading += _actions.Turn + _actions.TurnGun + _actions.TurnRadar;
            _newRadarHeading = _state.RadarHeading;

            _state.Heading %= 360;
            _state.GunHeading %= 360;
            _state.RadarHeading %= 360;
        }

        private void Move()
        {
            _state.Speed += _actions.Accelerate;
            if (_state.Speed > 8) _state.Speed = 8;
            if (_state.Speed < -8) _state.Speed = -8;
            _state.X += Math.Cos(ToRadians(_state.Heading))*_state.Speed;
            _state.Y -= Math.Sin(ToRadians(_state.Heading))*_state.Speed;
            if (_state.X - _state.Size < 0) _state.X = _state.Size;
            if (_state.Y - _state.Size < 0) _state.Y = _state.Size;
            if (_state.X + _state.Size >= _battlefield.Width) _state.X = _battlefield.Width - _state.Size;
            if (_state.Y + _state.Size >= _battlefield.Height) _state.Y = _battlefield.Height - _state.Size;
        }

        private double ToRadians(double heading)
        {
            return heading*Math.PI/360;
        }

        private void Scan()
        {
            foreach (RobotRunner other in _battlefield.Robots)
            {
                if ((other != this) && (!other.Dead()))
                {
                    double a = GetAngleToOther(other);
                    if (Math.Abs(a) <= 0.01)
                    {
                        if ((_oldRadarHeading <= a && a <= _newRadarHeading) ||
                            (_oldRadarHeading >= a && a >= _newRadarHeading) ||
                            (_oldRadarHeading <= a + 360 && a + 360 <= _newRadarHeading) ||
                            (_oldRadarHeading >= a + 360 && a + 360 >= _newRadarHeading) ||
                            (_oldRadarHeading <= a - 360 && a - 360 <= _newRadarHeading) ||
                            (_oldRadarHeading >= a - 360 && a - 360 >= _newRadarHeading))
                        {
                            _events.RobotsScanned.Add(Hypotenuse(_state.Y - other.Y, other.X - _state.X));
                        }
                    }
                }
            }
        }

        private double Hypotenuse(double a, double b)
        {
            return Math.Sqrt(a*a + b*b);
        }

        private double GetAngleToOther(RobotRunner other)
        {
            if ((Math.Abs(Y - other.Y) <= 0.01) && (Math.Abs(other.X - X) <= 0.01)) return 0;
            return (Math.Atan2(Y - other.Y, other.X - X)/Math.PI*180%360);
        }

        private void Speak()
        {
            if (string.IsNullOrEmpty(_actions.Say))
            {
                _speech = _actions.Say;
                _speechCounter = 50;
            }
            else if ((_speech != null) && ((_speechCounter -= 1) < 0))
            {
                _speech = null;
            }
        }

        private void Broadcast()
        {
            foreach (RobotRunner other in _battlefield.Robots)
            {
                if ((other == this) || (other.Dead()) || (other.Team != Team)) continue;
                string msg = other.Actions.Broadcast;
                if (string.IsNullOrEmpty(msg))
                {
                    _events.Broadcasts.Add(msg);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}