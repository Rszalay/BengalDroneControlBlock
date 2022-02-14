using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using Sandbox.Game.Entities;
using VRageMath;
using VRage.Game.ModAPI;
using DroneBlockSystem.ControlBlock.DroneBlocks;
using DroneBlockSystem.Controls;
using DroneBlockSystem.ControlBlock.Settings;

namespace DroneBlockSystem.ControlBlock.Drivers
{
    class ThrustDriver
    {
        Vector3D _pv, _sp;
        public Vector3D PelocityPv { get; private set; }
        readonly Ideal ConX, ConY, ConZ;
        readonly DroneBlock ThisController;
        public Dictionary<Base6Directions.Direction, List<IMyThrust>> thrusterSet;
        double xThrottle;
        double yThrottle;
        double zThrottle;
        public int ticksSinceLastRun = 0;

        float gridForwardThrust = 0;

        internal ThrustDriver(List<IMySlimBlock> blocks, DroneBlock thisController, DroneSettings settings)
        {
            ThisController = thisController;
            ConX = new Ideal(settings.Thrust, ThisController);
            ConY = new Ideal(settings.Thrust, ThisController);
            ConZ = new Ideal(settings.Thrust, ThisController);
            thrusterSet = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
            Purge(blocks);
        }

        internal void UpdateOffsets(float thrust)
        {
            ConX.UpdateOffset(thrust);
            ConY.UpdateOffset(thrust);
            ConZ.UpdateOffset(thrust);
        }

        internal void Purge(List<IMySlimBlock> blocks)
        {
            foreach (var orientation in thrusterSet)
                orientation.Value?.Clear();
            thrusterSet?.Clear();
            gridForwardThrust = 0;
            thrusterSet = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
            foreach (IMySlimBlock block in blocks)
            {
                if (block.FatBlock is IMyThrust)
                {
                    if(block.FatBlock.WorldMatrix.Forward == ThisController.Block.WorldMatrix.Backward)
                    {
                        gridForwardThrust += (block.FatBlock as IMyThrust).MaxThrust;
                    }
                    if (!thrusterSet.ContainsKey(block.Orientation.Forward))
                    {
                        thrusterSet.Add(block.Orientation.Forward, new List<IMyThrust>());
                        thrusterSet[block.Orientation.Forward].Add(block.FatBlock as IMyThrust);
                    }
                    else { thrusterSet[block.Orientation.Forward].Add(block.FatBlock as IMyThrust); }
                }
            }
            float mass = (ThisController.Block.CubeGrid as MyCubeGrid).GetCurrentMass();
            float thrust2Mass = gridForwardThrust / mass;
            IdealSettings newSettings = new IdealSettings(thrust2Mass * .01f, (float)Math.Sqrt(thrust2Mass) * .005f, (float)Math.Pow(thrust2Mass, 2) * 0f, 1);
            ConX.UpdateGains(newSettings);
            ConY.UpdateGains(newSettings);
            ConZ.UpdateGains(newSettings);
        }

        internal void Load(Vector3D sp)
        {
            _pv = ThisController.Block.GetShipVelocities().LinearVelocity;
            PelocityPv = _pv;
            _sp = sp;
            Vector3D error = _sp - PelocityPv;
            ConX.Load(error.X);
            ConY.Load(error.Y);
            ConZ.Load(error.Z);
            ticksSinceLastRun = 0;
        }

        internal void Run()
        {
            xThrottle = ConX.Run();
            yThrottle = ConY.Run();
            zThrottle = ConZ.Run();
            Vector3D thrustVector = new Vector3D(xThrottle, yThrottle, zThrottle);
            foreach (var orientation in thrusterSet)
            {
                var first = orientation.Value.First();
                if (orientation.Value.Count() > 0)//should probably sort by throttle settings first so you don't have to check orientations on thruster that should be off
                {
                    Vector3D thrusterWF = first.WorldMatrix.Forward;

                    double thrusterDot = Vector3D.Dot(first.WorldMatrix.Backward, thrustVector);
                    if (thrusterDot > 0)
                    {
                        foreach (var thruster in orientation.Value) { thruster.ThrustOverridePercentage = (float)thrusterDot; }
                    }
                    else
                    {
                        foreach (var thruster in orientation.Value) { thruster.ThrustOverridePercentage = 0; }
                    }
                }
            }
            ticksSinceLastRun = 0;
        }

        internal void UpdateGains(DroneSettings droneSettings)
        {
            ConX.UpdateGains(droneSettings.Thrust);
            ConY.UpdateGains(droneSettings.Thrust);
            ConZ.UpdateGains(droneSettings.Thrust);
        }

        internal void Tick()
        {
            ticksSinceLastRun += 1;
            ConX.Tick();
            ConY.Tick();
            ConZ.Tick();
        }

        public string ThrustData()
        {
            string data = "Thrust Data: \n";
            foreach (var orientation in thrusterSet)
            {
                data += orientation.Key.ToString() + ":  " + orientation.Value.Count + "\n";
            }
            return data;
        }
    }
}