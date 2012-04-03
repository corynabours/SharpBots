using System;

namespace Ente
{
    internal enum MoveMode
    {
        False,
        To,
        Away,
        SideA,
        SideB
    }

    public abstract class Pointanizer : Turnarounder
    {
        private double _moveX;
        private double _moveY;
        private MoveMode _moveMode;

        protected void Halt()
        {
            _moveMode = MoveMode.False;
        }

        protected bool OnWall()
        {
            return X <= Size*3 || Y <= Size*3 || BattlefieldWidth - X <= Size*3 ||
                   BattlefieldHeight - Y <= Size*3;
        }

        protected void FinalPoint()
    {
        var yc = 0.0;
        try
        {
            yc = Y - _moveY;
        }
        catch (Exception)
        {
        }

        var xc = 0.0;
        try
        {
            xc = _moveX - X;
        }
        catch (Exception)
        {
        }

        if (Math.Sqrt(yc*yc + xc*xc) < Size/3)
            _moveMode = MoveMode.False;

        var acc = true;

        switch (_moveMode)
        {
            case MoveMode.To:
                HeadTo(RobotMath.ToDeg(Math.Atan2(yc, xc)));
                break;
            case MoveMode.Away:
                HeadTo(RobotMath.ToDeg(Math.Atan2(yc, xc)) + 180);
                break;
            case MoveMode.SideA:
                HeadTo(RobotMath.ToDeg(Math.Atan2(yc, xc)) + 60);
                break;
            case MoveMode.SideB:
                HeadTo(RobotMath.ToDeg(Math.Atan2(yc, xc)) - 60);
                break;
            case MoveMode.False:
                acc = false;
                break;
            default:
                throw new Exception("Unknown move mode!");
        }

        if (acc) Accelerate(8);
    }

        internal Data RadToXy(double r,double d)
        {
            return new Data(X + Math.Cos(RobotMath.ToRad(r)) * d, Y - Math.Sin(RobotMath.ToRad(r)) * d);
        }
    }
}
