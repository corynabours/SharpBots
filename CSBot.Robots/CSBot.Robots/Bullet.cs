using System;
namespace CSBot.Robots
{
    public class Bullet
    {
        private readonly Battlefield _battlefield;
        private double _x;
        private double _y;
        private readonly double _heading;
        private readonly int _speed;
        private readonly decimal _strength;
        private readonly RobotRunner _origin;
        private bool _dead;

        public bool AddedToScreen { get; set; }

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }

        public decimal Energy
        {
            get { return _strength; }
        }

        public Bullet(Battlefield battlefield, double x, double y, double heading, int speed, decimal strength, RobotRunner robotRunner)
        {
            _battlefield = battlefield;
            _x = x;
            _y = y;
            _heading = heading;
            _speed = speed;
            _strength = strength;
            _origin = robotRunner;
            _dead = false;
        }

        public void Tick()
        {
            if (_dead) return;
            _x += Math.Cos(ToRadians(_heading))*_speed;
            _y -= Math.Sin(ToRadians(_heading))*_speed;
            _dead = (_x < 0) || (_x >= _battlefield.Width);
            _dead = _dead || (_y < 0) || (_y >= _battlefield.Height);
            foreach (var other in _battlefield.Robots)
                if ((other != _origin) && (Hypotenuse(_y - other.Y, other.X - _x) < 40) && (!other.Dead()))
                {
                    var explosion = new Explosion(Convert.ToInt32(other.X), Convert.ToInt32(other.Y));
                    _battlefield.Add(explosion);
                    var damage = other.Hit(this);
                    _origin.DamageGiven += damage;
                    if (other.Dead()) _origin.Kills += 1;
                    _dead = true;
                }
        }

        public bool Dead
        {
            get { return _dead; }
            set { _dead = value; }
        }

        private double ToRadians(double heading)
        {
            return heading * Math.PI / 180;
        }

        private double Hypotenuse(double a, double b)
        {
            return Math.Sqrt(a * a + b * b);
        }
    }
}
