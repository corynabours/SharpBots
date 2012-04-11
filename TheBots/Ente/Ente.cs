using System;
using System.Collections.Generic;
using System.Linq;
using CSBot.Robots;

namespace Ente
{
    public class Ente : Pointanizer
    {
        private const double BulletSpeed = 30.0;
        private const double TrackTimeout = 5;
        private const double HistorySize = 7;
        private const double MinPoints = 4;
        private const double HistoryTimeout = 20;
        private const double ScanSwitch = 2.8;
        private const double ScanSwitch2 = 1.2;
        private const double TrackRange = 1150.0;

        //movement
        private const double Infactor = 9;
        private const double Outfactor = 10;
        private const int Randomize = 7;
        private const double Timeout = 20;
        private const double Diff = 5.3;
        private const double Hitaway = 5;
        private const double Sdiff = 60*Diff;
        private const double Randturn = 0.11;
        internal double? Predx { get; set; }
        internal double? Predy { get; set; }
        private readonly List<PointTime> _points = new List<PointTime>();
        private double _lastSeenTime = -TrackTimeout;
        private double _radarSpeed = 1;
        private double _trackMul = 1;

        //movement
        private double _moveDirection = 1;
        private double _lasthitcount;
        private bool _lasthitcount2;
        private double _lastchange = -Timeout;
        private int _direction;

        private Data OldPredict(double ptime)
        {
            if (_points.Count < MinPoints)
                return new Data(Rand(BattlefieldWidth), Rand(BattlefieldHeight));

            var ltime = _points[_points.Count - 1].ktime;
            var ftime = ltime - HistoryTimeout;
            var xint = new List<WeightedData>();
            var yint = new List<WeightedData>();
            foreach (var point in _points)
            {
                var r = ((point.ktime - ftime)/HistoryTimeout);
                r = Math.Pow(Math.Pow(10, r), 2);
                xint.Add(new WeightedData(point.ktime, point.X, r));
                yint.Add(new WeightedData(point.ktime, point.Y, r));
            }

            var x = RobotMath.WeightedLinearRegression(xint);
            var y = RobotMath.WeightedLinearRegression(yint);

            return new Data(x.X*ptime + x.Y, y.X*ptime + y.Y);
        }

        private Data Predict(double ptime)
        {
            if (_points.Count < MinPoints)
                return new Data(Rand(BattlefieldWidth), Rand(BattlefieldHeight));
            var ltime = (_points[_points.Count - 1].ktime + _points[_points.Count - 3].ktime) / 2.0;
            var lx = (_points[_points.Count - 1].X + _points[_points.Count - 3].X) / 2.0;
            var ly = (_points[_points.Count - 1].Y + _points[_points.Count - 3].Y) / 2.0;
            var xint = _points.Select(point => new Data(point.ktime - ltime, point.X - lx)).ToList();
            var yint = _points.Select(point => new Data(point.ktime - ltime, point.Y - ly)).ToList();
            var xa = RobotMath.ZeroFixedLinearRegression(xint);
            var ya = RobotMath.ZeroFixedLinearRegression(yint);
            var q = (ptime - ltime);
            var x = q*xa + lx;
            var y = q*ya + ly;
            var data = OldPredict(ptime);
            return new Data((x + data.X)/2, (y + data.Y)/2);
        }

        private void Predcurrent()
        {
            if (Predx.HasValue) return;
            var pred = Predict(Time);
            Predx = pred.X;
            Predy = pred.Y;
        }

        public override void Tick(Events events)
        {
            Fire(0.1m);

            //event processing

            if (events.RobotsScanned.Count > 0)
            {
                var dist = events.RobotsScanned[0];
                var data = RadToXy(MidRadarHeading(), dist);
                _points.Add(new PointTime(data.X, data.Y, Time));

                if (_points.Count > HistorySize || Time - _points[0].ktime >= HistoryTimeout)
                    ShiftPoints();
                _lastSeenTime = Time;
                _radarSpeed = 1;

                _trackMul = 1;

                HeadRadarTo(MidRadarHeading());
                _direction = 1;
            }

            //moving

            Predx = null;
            Predy = null;

            if (events.GotHit.Count > 0)
            {
                _lasthitcount += 1;
            }
            else if (_lasthitcount2)
            {
                _lasthitcount2 = false;
                _lasthitcount = 0;
            }
            else
            {
                _lasthitcount2 = true;
            }

            if (((OnWall() || (_lasthitcount >= Hitaway) || Rand(1) < Randturn) && Time - _lastchange > Timeout) ||
                _lasthitcount >= Hitaway*4)
            {
                _lasthitcount2 = false;
                _lasthitcount = 0;
                _lastchange = Time;
                _moveDirection *= -1;
            }
            Halt();
            Accelerate(8);
            Predcurrent();
            var yc = Predy.HasValue? Y - Predy.Value : 0.0;
            var xc = Predx.HasValue ? Predx.Value - X : 0.0;
            var deg = RobotMath.ToDeg(Math.Atan2(yc, xc)) + 90*_moveDirection;
            var hyp = Math.Sqrt(yc*yc + xc*xc);

            if (hyp < Sdiff)
                deg += Outfactor*_moveDirection;
            else if (hyp > Sdiff + Size)
                deg -= Infactor*_moveDirection;


            deg += Rand(Randomize);
            deg -= Rand(Randomize);

            HeadTo(deg);

            //aiming

            if (_points.Count >= MinPoints)
            {
                Predcurrent();
                hyp = hypot(Predx.HasValue?Predx.Value:0 - X,Predy.HasValue?Predy.Value:0 - Y);

                var steps = (hyp - 20)/BulletSpeed;
                var f = Predict(Time + steps + 1);

                var gh = RobotMath.ToDeg(Math.Atan2(Y - f.Y, f.X - X)) + Rand(7) - 3;

                HeadGunTo(gh);
            }

            //#scanning

            if (Time - _lastSeenTime >= TrackTimeout || _points.Count < MinPoints)
            {
                Say("Searching,");

                if (RadarReady())
                {
                    turn_radar(_radarSpeed);
                    _radarSpeed *= -ScanSwitch;
                }
            }
            else
            {
                Say("Seek and Destroy!");

                if (RadarReady())
                {
                    Predcurrent();
                    yc = Predy.HasValue ? Y - Predy.Value : 0;
                    xc = Predx.HasValue ? Predx.Value - X : 0;

                    deg = RobotMath.ToDeg(Math.Atan2(yc, xc));
                    var dist = hypot(yc, xc);
                    var signs = new[] {-0.5, -1.5, -0.5, 0.5, 1.5, 0.5};
                    var sign = signs[_direction%6];
                    _direction += 1;
                    deg += (TrackRange*_trackMul*sign)/dist;
                    _trackMul *= ScanSwitch2;
                    HeadRadarTo(deg);
                }
            }
            FinalPoint();
            FinalTurn();
        }

        private void ShiftPoints()
        {
            _points.RemoveAt(0);
        }

        private double Rand(int p0)
        {
            return this.Random.NextDouble() * p0;
        }

        public override string Name
        {
            get { return "Ente"; }
        }

        private double hypot(double x, double y)
        {
            return Math.Sqrt(x*x + y*y);
        }
    }
}
