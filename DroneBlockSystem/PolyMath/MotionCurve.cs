using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.Common.ObjectBuilders;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.ModAPI;
using VRage.Game.ModAPI.Ingame.Utilities;
using DroneBlockSystem.ControlBlock.Settings;
using DroneBlockSystem.TargetingBlock.CoreSystems.Api;
using Sandbox.Common;
using Sandbox.Game;
using SpaceEngineers.Game;
using SpaceEngineers.ObjectBuilders;
using VRage.Game;
using Sandbox.Game.Entities;
using VRage.Utils;
using DroneBlockSystem.ControlBlock.DroneBlocks;
using DroneBlockSystem.Interface;
using DroneBlockSystem.NetworkProtobuf;
using DroneBlockSystem.TargetingBlock.TargetingBlocks;
using ProtoBuf;
using VRage;
using VRage.Game.Entity;
using VRageMath;
using System.Linq;
using DroneBlockSystem.Controls;

namespace DroneBlockSystem.PolyMath
{
    class MotionCurve
    {
        public Vector3D CurrentPos { get; private set; } = new Vector3D();
        Vector3D lastPos = new Vector3D();
        public Vector3D CurrentVel { get; private set; } = new Vector3D();
        Vector3D lastVel = new Vector3D();
        public Vector3D CurrentAcc { get; private set; } = new Vector3D();
        Vector3D lastAcc = new Vector3D();
        public Vector3D CurrentJrk { get; private set; } = new Vector3D();

        public MotionCurve(Vector3D vector)
        {
            CurrentPos = vector;
            CurrentVel = Vector3D.Zero;
            CurrentAcc = Vector3D.Zero;
            CurrentJrk = Vector3D.Zero;
        }

        public void Update(Vector3D vector ,int t)
        {
            if (t == 0)
                return;
            lastPos = CurrentPos;
            lastVel = CurrentVel;
            lastAcc = CurrentAcc;

            CurrentPos = vector;
            CurrentVel = (CurrentPos - lastPos) / t;
            CurrentAcc = (CurrentVel - lastVel) / t;
            CurrentJrk = (CurrentAcc - lastAcc) / t;
        }

        public Vector3D Solve(int t)
        {
            return CurrentPos + CurrentVel * t + CurrentAcc * .5 * Math.Pow(t, 2) + CurrentJrk * .1666 * Math.Pow(t, 3);
        }

        public Vector3D SpeedLimitSolve(int t, double limit)
        {
            if (t == 0)
                return CurrentPos;
            int i = 1;
            Vector3D simJrk = CurrentJrk;
            Vector3D simAcc = CurrentAcc;
            Vector3D simVel = CurrentVel;
            Vector3D simPos = CurrentPos;
            while (i < t)
            {
                simAcc += simJrk;
                simVel += simAcc;
                if (simVel.Length() > limit / 60.0)
                    simVel = Vector3D.Normalize(simVel) * limit / 60.0;
                simPos += simVel;
            }
            return simPos;
        }
    }

    class Polynomial
    {
        public double[] Terms { get; private set; }

        public Polynomial(double[] terms)
        {
            Terms = terms;
        }

        public Polynomial(List<double> terms)
        {
            Terms = terms.ToArray();
        }

        public Polynomial(int degree)
        {
            Terms = new double[degree + 1];
        }

        public int Degree()
        {
            return Terms.Length;
        }

        public double Solve(double t)
        {
            int i = Terms.Length;
            double f_x = 0;
            if (i == 0)
                return f_x;
            while(t > 0)
            {
                f_x += Terms[i] * Math.Pow(t, i);
                t--;
            }
            return f_x;
        }

        public static Polynomial operator+(Polynomial a, Polynomial b)
        {
            Polynomial c;
            if (a.Terms.Length >= b.Terms.Length)
            {
                c = new Polynomial(a.Terms.Length);
                int i = 0;
                while (i != b.Terms.Length)
                {
                    c.Terms[i] = a.Terms[i] + b.Terms[i];
                    i++;
                }
            }
            else
            {
                c = new Polynomial(b.Terms.Length);
                int i = 0;
                while (i != a.Terms.Length)
                {
                    c.Terms[i] = a.Terms[i] + b.Terms[i];
                    i++;
                }
            }
            return c;
        }

        public static Polynomial operator -(Polynomial a, Polynomial b)
        {
            Polynomial c;
            if (a.Terms.Length >= b.Terms.Length)
            {
                c = new Polynomial(a.Terms.Length);
                int i = 0;
                while (i != b.Terms.Length)
                {
                    c.Terms[i] = a.Terms[i] - b.Terms[i];
                    i++;
                }
            }
            else
            {
                c = new Polynomial(b.Terms.Length);
                int i = 0;
                while (i != a.Terms.Length)
                {
                    c.Terms[i] = a.Terms[i] - b.Terms[i];
                    i++;
                }
            }
            return c;
        }
    }
}
