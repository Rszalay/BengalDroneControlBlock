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

namespace BengalDroneControlBlock.Drivers
{
    class ThrustDriver
    {
        Vector3D _pv, _sp, currentPosition, lastPosition, velocitySp;
        public Vector3D velocityPv { get; private set; }
        Ideal ConX, ConY, ConZ;
        DroneBlock ThisController;
        public Dictionary<Base6Directions.Direction, List<IMyThrust>> thrusterSet;
        double xThrottle;
        double yThrottle;
        double zThrottle;
        double speedSp;
        int ticksSinceLastRun = 0;

        public ThrustDriver(List<IMySlimBlock> blocks, DroneBlock thisController, DroneSettings settings)
        {
            ThisController = thisController;
            ConX = new Ideal(settings.Thrust, ThisController);
            ConY = new Ideal(settings.Thrust, ThisController);
            ConZ = new Ideal(settings.Thrust, ThisController);
            thrusterSet = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
            Purge(blocks);
        }

        public void Purge(List<IMySlimBlock> blocks)
        {
            foreach (IMySlimBlock block in blocks)
            {
                if (block.FatBlock is IMyThrust)
                {
                    if (!thrusterSet.ContainsKey(block.Orientation.Forward))
                    {
                        thrusterSet.Add(block.Orientation.Forward, new List<IMyThrust>());
                        thrusterSet[block.Orientation.Forward].Add(block.FatBlock as IMyThrust);
                    }
                    else { thrusterSet[block.Orientation.Forward].Add(block.FatBlock as IMyThrust); }
                }
            }
        }

        public void Load(Vector3D sp, double speed)
        {
            //ThisController.Echo("ticksSinceLastRun" + ticksSinceLastRun);
            currentPosition = ThisController.Entity.WorldMatrix.Translation;
            speedSp = speed;
            //_pv = (currentPosition - lastPosition) * (1 / ticksSinceLastRun);
            _pv = ThisController.Block.GetShipVelocities().LinearVelocity;
            velocityPv = _pv;
            _sp = sp;
            Vector3D error = _sp - velocityPv;
            ConX.Load(error.X);
            ConY.Load(error.Y);
            ConZ.Load(error.Z);
            lastPosition = currentPosition;
            ticksSinceLastRun = 0;
        }

        public void Run()
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
        }

        public void Tick()
        {
            //ThisController.Echo("Driver Ticked");
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