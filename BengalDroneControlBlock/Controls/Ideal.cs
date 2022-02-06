using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using VRage;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Utils;
using VRageMath;
using ProtoBuf;
using BengalDroneControlBlock.Controls;
using BengalDroneControlBlock.Settings;
using BengalDroneControlBlock.DroneBlocks;
using BengalDroneControlBlock;

namespace BengalDroneControlBlock.Controls
{
    class Ideal
    {
        DroneBlock thisDroneController;
        public double _kp, _ki, _kd;
        double last, acc;
        double saturation;
        int ticksSinceLastRun = 0;
        double _error;
        bool oneShot = true;
        double offset = 1;

        public Ideal(IdealSettings settings, DroneBlock thisController)
        {
            thisDroneController = thisController;
            _kp = settings.Kp;
            _ki = settings.Ki;
            _kd = settings.Kd;
            saturation = settings.Saturation;
            last = 0;
            acc = 0;
        }

        public void UpdateOffset(double newOffset)
        {
            offset = newOffset;
        }

        public void UpdateGains(IdealSettings newSettings)
        {
            _kp = newSettings.Kp;
            _ki = newSettings.Ki;
            _kd = newSettings.Kd;
        }

        public void Load(double error)
        {
            _error = error;
        }

        public double Run()
        {
            double p = _error * _kp * offset;
            double d = (p - last) / (ticksSinceLastRun / 60.0) * _kd;
            //thisDroneController.Echo(ticksSinceLastRun.ToString());
            last = p;
            double i = Integral(p, p + d);
            ticksSinceLastRun = 0;
            oneShot = false;
            return p + i + d;
        }

        public double Integral(double error, double pd)
        {
            if (oneShot || saturation == 0)
                return 0;
            float precheck = (float)(pd + (acc + error) * _ki * (ticksSinceLastRun / 60.0));
            if (precheck > 0 && precheck < saturation)
                acc += error;
            else if (precheck < 0 && precheck > -saturation)
                acc += error;
            else
                acc = acc * .7;
            return acc * _ki * (ticksSinceLastRun / 60.0);
        }

        public void Tick()
        {
            //thisDroneController.Echo("Ideal Ticked");
            ticksSinceLastRun += 1;
        }
    }
}
