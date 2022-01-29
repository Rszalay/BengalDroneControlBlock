using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BengalDroneControlBlock.Settings
{
    class DroneSettings
    {
        public IdealSettings Thrust { get; private set; }
        public IdealSettings Yaw { get; private set; }
        public IdealSettings Pitch { get; private set; }
        public IdealSettings Roll { get; private set; }

        public DroneSettings()
        {
            Thrust = new IdealSettings(0, 0, 0, 0);
            Yaw = new IdealSettings(0, 0, 0, 0);
            Pitch = new IdealSettings(0, 0, 0, 0);
            Roll = new IdealSettings(0, 0, 0, 0);
        }

        public void SetThrust(float kp, float ki, float kd, double saturation)
        {
            Thrust = new IdealSettings(kp, ki, kd, saturation);
        }
        public void SetYaw(float kp, float ki, float kd, double saturation)
        {
            Yaw = new IdealSettings(kp, ki, kd, saturation);
        }
        public void SetPitch(float kp, float ki, float kd, double saturation)
        {
            Pitch = new IdealSettings(kp, ki, kd, saturation);
        }
        public void SetRoll(float kp, float ki, float kd, double saturation)
        {
            Roll = new IdealSettings(kp, ki, kd, saturation);
        }
    }

    public class IdealSettings
    {
        public float Kp { get; set; }
        public float Ki { get; set; }
        public float Kd { get; set; }
        public double Saturation;

        public IdealSettings(float kp, float ki, float kd, double saturation)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
            Saturation = saturation;
        }
    }
}