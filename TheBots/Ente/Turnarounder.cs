using System;
using CSBot.Robots;

namespace Ente
{
    public abstract class Turnarounder : Robot
    {
        private double _lastRadarHeading;
        private double _wantedGunHeading;
        private double _wantedHeading;
        private double _wantedRadarHeading;

        internal void HeadTo(double deg)
        {
            _wantedHeading = deg;
        }

        protected void HeadGunTo(double deg)
        {
            _wantedGunHeading = deg;
        }

        protected void HeadRadarTo(double deg)
        {
            _wantedRadarHeading = deg;
        }

        private double NextDeltaHeading()
        {
            return Math.Max(Math.Min(RobotMath.Offset(_wantedHeading, Heading), 10), -10);
        }

        private double TurnAmount()
        {
            return NextDeltaHeading();
        }

        private double TurnGunAmount()
        {
            return Math.Max(-30, Math.Min(RobotMath.Offset(_wantedGunHeading, GunHeading + NextDeltaHeading()), 30));
        }

        private double NextDeltaGunHeading()
        {
            return NextDeltaHeading() + TurnGunAmount();
        }

        private double TurnRadarAmount()
        {
            return Math.Max(-60,
                            Math.Min(RobotMath.Offset(_wantedRadarHeading, RadarHeading + NextDeltaGunHeading()),
                                     60));
        }

        protected bool RadarReady()
        {
            return Math.Abs(RobotMath.Offset(NextRadarHeading() - _wantedRadarHeading)) < 2;
        }


        private double NextDeltaRadarHeading()
        {
            return NextDeltaGunHeading() + TurnRadarAmount();
        }

        private double NextRadarHeading()
        {
            return RadarHeading + NextDeltaRadarHeading();
        }

        protected void turn_radar(double x)
        {
            _wantedRadarHeading += x;
        }

        protected double MidRadarHeading()
        {
            double turnc = RobotMath.Offset(RadarHeading, _lastRadarHeading);
            return RadarHeading - (turnc/2.0);
        }

        protected void FinalTurn()
        {
            Turn(TurnAmount());
            TurnGun(TurnGunAmount());
            TurnRadar(TurnRadarAmount());
            _lastRadarHeading = RadarHeading;
        }
    }
}