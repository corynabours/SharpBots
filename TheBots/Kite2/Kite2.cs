using System.Collections.Generic;
using System.Linq;
using CSBot.Robots;
using System;

namespace Kite2
{
    public class Kite2 : Robot
    {
        private double _rt = 15;
        private double _radar_scan = 15;
        private double _min_radar_scan = 1.5;
        private double _max_radar_scan = 60;
        private bool _lock = false;
        private double _firing_threshold = 20.0;
        private double _wanted_turn = 0;
        private double _wanted_gun_turn = 0;
        private double _wanted_radar_turn = 0;
        private int _rturn_dir = 1;
        private double _racc_dir = 1;
        private readonly List<Vector> _target_positions = new List<Vector>();
        private int _sd_limit;
        private decimal _prev_health;
        private int _hit;
        private double? _min_distance;
        private double? _target_heading;
        private bool _on_target;
        private decimal _firepower;
        private double? _vsd;

        public override void Tick(Events events)
        {
            if (Time == 0) _prev_health = Energy;
            if (events.RobotsScanned.Count == 0)
                _radar_scan = Math.Min(_radar_scan*1.5, _max_radar_scan);
            else
                _radar_scan = Math.Max(_radar_scan*0.5, _min_radar_scan);
            if (Math.Abs(_radar_scan) < _max_radar_scan - 0.1)
                _rt = (Time/2%2 < 1 ? -_radar_scan/2.0 : _radar_scan/2.0);
            _wanted_radar_turn += _rt;
            firing_solution(events);


            if (events.GotHit.Count > 0)
            {
                _racc_dir = _racc_dir/Math.Abs(_racc_dir);
                _hit = 20;
            }
            else
            {
                _hit--;
            }

            _hit = -1;
            if (_hit < 0)
                _racc_dir = (int) ((_min_distance.HasValue && _min_distance.Value < 450)
                                       ? DoubleMath.sign(_racc_dir)
                                       : Math.Sin(Time*0.1 + rand()*0.2));

            Accelerate((int) _racc_dir);
            if (approaching_wall())
                _wanted_turn = 60*_rturn_dir;
            else if (_target_heading.HasValue && _wanted_turn <= 1)
                _wanted_turn =
                    heading_distance(
                        ((_min_distance.HasValue && _min_distance.Value < 450)
                             ? (90 - DoubleMath.sign(_rturn_dir)*45)*-DoubleMath.sign(_racc_dir)
                             : 0) +
                        Heading, (double) (90 + _target_heading));
            else if (rand() < 0.3)
                _wanted_turn += rand()*10*DoubleMath.sign(_racc_dir)*_rturn_dir;
            else if (rand() < 0.01)
                _rturn_dir *= -1;
            else if (rand() < 0.01)
                _racc_dir *= -1;

            turn_hull();
            turn_turret();
            turn_radar_dish();
            _prev_health = Energy;

        }

        private void firing_solution(Events events)
        {
            if (events.RobotsScanned.Count > 0)
            {
                var last = (_target_positions.Count > 0)
                               ? _target_positions[_target_positions.Count - 1]
                               : new Vector(0, 0);
                var robotsScanned = new List<Vector>();
                foreach (var d in events.RobotsScanned)
                {
                    var tx = X + Math.Cos(DoubleMath.deg2rad((RadarHeading - Math.Abs(_radar_scan)/2.0)))*d;
                    var ty = Y - Math.Sin(DoubleMath.deg2rad(RadarHeading - Math.Abs(_radar_scan)/2.0))*d;
                    robotsScanned.Add(new Vector(tx, ty));
                }
                var minRadius = double.MaxValue;
                Vector position = null;
                foreach (var pos in robotsScanned)
                {
                    var radius = (pos - last).r();
                    if (radius < minRadius)
                    {
                        minRadius = radius;
                        position = pos;
                    }
                }
                _target_positions.Add(position);
                _min_distance = null;
                if (robotsScanned.Count > 0)
                {
                    _min_distance = int.MaxValue;
                    foreach (var distance in events.RobotsScanned)
                        if (distance < _min_distance) _min_distance = distance;
                }
            }
            if (_target_positions.Count > 10) Shift(_target_positions);
            if (_target_positions.Count == 0) _min_distance = null;
            _target_heading = target_heading();
            double? gtd = null;
            if (_target_heading.HasValue) gtd = heading_distance(GunHeading, (double) _target_heading);
            _firepower = 100.0m/(_vsd.HasValue ? (decimal) _vsd.Value : 1500m)/7.0m;
            Fire(_firepower*0.2m);
            if (_on_target) Fire(_firepower);
            if (gtd.HasValue && Math.Abs(gtd.Value) < _firing_threshold)
            {
                _wanted_gun_turn = gtd.Value;
                _on_target = true;
            }
            else
            {

                _wanted_gun_turn = gtd.HasValue ? gtd.Value : (gun_radar_distance()/3.0);
                _on_target = false;
            }
        }

        private void Shift(List<Vector> targetPositions)
        {
            var vectors = targetPositions.ToArray();
            _target_positions.Clear();
            var skippedOne = false;
            foreach (var vector in vectors)
            {
                if (skippedOne)
                    _target_positions.Add(vector);
                else
                    skippedOne = true;
            }
        }

        private double rand()
        {
            return new Random().NextDouble();
        }

        private double? target_heading()
        {
            if (_target_positions.Count < 10)
                return null;
            if (_min_distance.HasValue && _min_distance.Value < 200)
                return RadarHeading;
            var lps = new List<double>();

            for (int index = 0; index < 5; index++)
            {
                lps.Add(average(new List<double> {_target_positions[index].X, _target_positions[index].Y}));
            }

            var p4 = lps[4];
            return null;
            /*
             * I have no idea what this is supposed to be doing.
             * 
            var vs = lps[0.. - 2].zip(lps[1.. - 1]).map{|a,b | (b - a)*0.5}
            _vsd = Math.Sqrt(vs.map{|v | v[0]}.sd**2 + vs.map{|v | v[1]}.sd**2)
            if (_vsd > _sd_limit) return null;
            var accs = vs[0.. - 2].zip(vs[1.. - 1]).map{|a,b | (b - a)}
            _vsd += Math.Sqrt(accs.map{|v | v[0]}.sd**2 + vs.map{|v | v[1]}.sd**2)
            _vsd *= 0.5;
            var v = average(vs);
            if (v.r() < 4.0) return heading_for(average(lps));
            var acc = average(accs);
            p4 = p4 + (v*0.5);
            var distance = p4 - new Vector(X, Y);
            var shot_speed = 30.0;
            var a = distance[0]**2 + distance[1]**2;
            var b = 2*distance[0]*v[0] + 2*distance[1]*v[1];
            var c = v[0]**2 + v[1]**2 - shot_speed**2;
            var d = b**2 - 4*a*c;
            if (d < 0) return heading_for(average(lps));
            var t = 2*a/(-b + Math.Sqrt(d));
            var r = v.r();
            if (acc.r() > 0.5)
                if ((v + acc*t).r > 8.0)
                    if ((v + acc).r > v.r())
                        v = (v + acc*0.5*t).normalize()*6.0;
                    else
                        v = v.normalize();
                else
                    v = v + acc*0.5*t;
            var ep = p4 + v*t;
            var estimated_position = new Vector(
                Math.Max(Size, Math.Min(BattlefieldWidth - Size, ep[0])),
                Math.Max(Size, Math.Min(BattlefieldHeight - Size, ep[1])));
            return heading_for(estimated_position) + (rand() - 0.5)*_vsd*0.2;*/
        }

        private double average(ICollection<double> arr)
        {
            var sum = arr.Sum();
            return sum*(1.0/arr.Count);
        }


        private double heading_for(Vector position)
        {
            var distance = position - new Vector(X, Y);
            var heading = DoubleMath.rad2deg(Math.Atan2(-distance.Y, distance.X));
            if (heading < 0) heading += 360;
            return heading;
        }

        private double heading_distance(double h1, double h2)
        {
            return limit(h2 - h1, 180);
        }

        private double limit(double value, double m)
        {
            if (value > 180) value -= 360;
            if (value < -180) value += 360;
            if (value < -m) value = -m;
            if (value > m) value = m;
            return value;
        }

        private double gun_radar_distance()
        {
            return heading_distance(GunHeading, RadarHeading);
        }

        private void turn_hull()
        {
            var turn_amt = Math.Max(-10.0, Math.Min(_wanted_turn, 10.0));
            Turn(turn_amt);
            _wanted_turn -= turn_amt;
            _wanted_gun_turn -= turn_amt;
            _wanted_radar_turn -= turn_amt;
        }

        private void turn_turret()
        {
            var turn_amt = Math.Max(-30.0, Math.Min(_wanted_gun_turn, 30.0));
            TurnGun(turn_amt);
            _wanted_gun_turn -= turn_amt;
            _wanted_radar_turn -= turn_amt;
        }

        private void turn_radar_dish()
        {
            var turn_amt = Math.Max(-60.0, Math.Min(_wanted_radar_turn, 60.0));
            TurnRadar(turn_amt);
            _wanted_radar_turn -= turn_amt;
        }

        private bool approaching_wall()
        {
            return approaching_vertical_wall() || approaching_horizontal_wall();

        }

        private bool approaching_horizontal_wall()
        {
            if (!((Speed > 0) ^ (Heading > 0.0 && Heading < 180.0)))
                return (Y < 100);
            return (Y > BattlefieldHeight - 100);
        }

        private bool approaching_vertical_wall()
        {
            if (!((Speed > 0) ^ (Heading > 90.0 && Heading < 270.0)))
                return X < 100;
            return (X > BattlefieldWidth - 100);
        }

        public override string Name
        {
            get { return "Kite2"; }
        }
    }
}
