using System;
using System.Globalization;
using CSBot.Robots;

namespace Pair
{
    public class Pair : Robot
    {
        private const double MaxRobotTurn = 10; 
        private const double MaxDistanceFromCenter = 725;
        private const double MinDistanceFromPartner = 410;
        private const double MaxDistanceFromPartner = 590;
        private const double MinPartnerSafetyAngle = 14;
        private const double MinGunTurn = 16;
        private const decimal FirePower = 2.8m;
        private bool _partnerDead;

        public Pair()
        {
            _partnerDead = false;
        }

        private PairVector Partner { get; set; }

        public override string Name
        {
            get { return "Pair"; }
        }

        public override void Tick(Events events)
        {
            if (Time==0) Partner = Center();
            CheckPartnerMsgs();
            if (ScannedEnemy()) Fire(FirePower);
            AdjustGunAccordingTo(TurnRobot());
            Broadcast("_PP" + X.ToString(CultureInfo.InvariantCulture) + "|" + Y.ToString(CultureInfo.InvariantCulture));
        }

        private bool ScannedEnemy()
        {
            return RobotScanned() && NotPointingTowardPartner();
        }

        private void AdjustGunAccordingTo(double turnedBot)
        {
            var magnitude = Math.Min(Math.Abs(turnedBot), MaxRobotTurn);
            var gunAdjustment = MinGunTurn;
            if (turnedBot > 0)
                gunAdjustment -= magnitude;
            else
                gunAdjustment += magnitude;

            TurnGun(gunAdjustment);
        }

        private bool NotPointingTowardPartner()
        {
            return ((Math.Abs(TowardPoint(Partner, GunHeading)) > MinPartnerSafetyAngle) || _partnerDead);
        }

        private bool RobotScanned()
        {
            return Events.RobotsScanned.Count > 0;
        }

        private double TurnRobot()
        {
            var desiredTurn = 0.0;
            if (DistanceFromPoint(Partner) > MaxDistanceFromPartner)
                desiredTurn = TowardPoint(Partner, Heading);
            if (DistanceFromPoint(Partner) < MinDistanceFromPartner)
                desiredTurn = AwayFromPoint(Partner, Heading);
            if (DistanceFromPoint(Center()) > MaxDistanceFromCenter)
                desiredTurn = TowardPoint(Center(), Heading);
            Turn(desiredTurn);
            Accelerate(1);
            return desiredTurn;
        }


        private void CheckPartnerMsgs()
        {
            var partnerSaid = Events.Broadcasts;
            if (partnerSaid.Count == 0)
                _partnerDead = true;
            else
            {
                _partnerDead = false;
                if (partnerSaid[0] != null)
                {
                    var locationParts = partnerSaid[0].Substring(3).Split('|');
                    Partner = new PairVector(Convert.ToDouble(locationParts[0]), Convert.ToDouble(locationParts[1]));
                }
            }
        }


        private double AwayFromPoint(PairVector point, double fromHeading)
        {
            return -1*(TowardPoint(point, fromHeading));
        }

        private PairVector Center()
        {
            return new PairVector((double) BattlefieldWidth/2, (double) BattlefieldHeight/2);
        }


        private static double TowardHeading(double toHeading, double fromHeading)
        {
            double differenceBetween = toHeading - fromHeading;
            double desiredTurn;
            if (differenceBetween > 0)
                if (differenceBetween < 180)
                    desiredTurn = differenceBetween;
                else
                    desiredTurn = -1*(360 - Math.Abs(differenceBetween));
            else if (differenceBetween > -180)
                desiredTurn = differenceBetween;
            else
                desiredTurn = 1*(360 - Math.Abs(differenceBetween));
            return desiredTurn;
        }

        private double TowardPoint(PairVector point, double fromHeading)
        {
            return TowardHeading(DegreeFromPoint(point), fromHeading);
        }

        private double DistanceFromPoint(PairVector point)
        {
            double a = point.X - X;
            double b = point.Y - Y;
            return Math.Sqrt(a*a + b*b);
        }

        private double DegreeFromPoint(PairVector point)
        {
            return Math.Atan2(Y - point.Y, point.X - X)/Math.PI*180%360;
        }
    }
}