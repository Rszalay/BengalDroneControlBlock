using DroneBlockSystem.ControlBlock.DroneBlocks;
using DroneBlockSystem.ControlBlock.Settings;

namespace DroneBlockSystem.Controls
{
    class Ideal
    {
        readonly DroneBlock thisDroneController;//keep incase echo() is needed
        public double _kp, _ki, _kd;
        double last, acc;
        readonly double saturation;
        int ticksSinceLastRun = 0;
        double _error;
        bool oneShot = true;
        double offset = 1;

        internal Ideal(IdealSettings settings, DroneBlock thisController)
        {
            thisDroneController = thisController;
            _kp = settings.Kp;
            _ki = settings.Ki;
            _kd = settings.Kd;
            saturation = settings.Saturation;
            last = 0;
            acc = 0;
        }

        internal void UpdateOffset(double newOffset)
        {
            offset = newOffset;
        }

        internal void UpdateGains(IdealSettings newSettings)
        {
            _kp = newSettings.Kp;
            _ki = newSettings.Ki;
            _kd = newSettings.Kd;
        }

        internal void Load(double error)
        {
            _error = error;
        }

        internal double Run()
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

        internal double Integral(double error, double pd)
        {
            if (oneShot || saturation == 0)
                return 0;
            if(saturation == 0)
            {
                acc += error;
                return acc * _ki * (ticksSinceLastRun / 60.0);
            }
            float precheck = (float)(pd + (acc + error) * _ki * (ticksSinceLastRun / 60.0));
            if (precheck > 0 && precheck < saturation)
                acc += error;
            else if (precheck < 0 && precheck > -saturation)
                acc += error;
            else
                acc *= .7;
            return acc * _ki * (ticksSinceLastRun / 60.0);
        }

        internal void Tick()
        {
            //thisDroneController.Echo("Ideal Ticked");
            ticksSinceLastRun += 1;
        }
    }
}
