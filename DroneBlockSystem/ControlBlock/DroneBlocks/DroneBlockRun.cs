using System.Collections.Generic;
using VRageMath;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using DroneBlockSystem.ControlBlock.Drivers;
using DroneBlockSystem.ControlBlock.Settings;
using System;
using System.Text;
using Sandbox.Common.ObjectBuilders;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.ModAPI;
using VRage.Game.ModAPI.Ingame.Utilities;
using DroneBlockSystem.TargetingBlock.TargetingBlocks;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Linq;
using System.Threading.Tasks;
using VRage.Game.ModAPI.Network;
using VRage.Network;
using VRage.Utils;
using DroneBlockSystem.NetworkProtobuf;
using DroneBlockSystem.Utility;
using Sandbox.Game.EntityComponents;

namespace DroneBlockSystem.ControlBlock.DroneBlocks
{
    partial class DroneBlock : MyGameLogicComponent
    {
        public GyroDriver myGyroDriver;
        public ThrustDriver myThrustDriver;
        public DroneSettings myDroneSettings;

        List<IMySlimBlock> _tempSlim;
        List<IMySlimBlock> _tempFat;

        public void Tick()
        {
            myGyroDriver.Tick();
            myThrustDriver.Tick();
        }

        public void Run()
        {
            UpdateOffsets();
            //Purge();
            GetPropertyInputs();
            GetAttachedComps(Block.CubeGrid.EntityId);
            TargetingBlock?.GetLocalTrackers();

            myGyroDriver.Run();
            myThrustDriver.Run();
        }

        internal void UpdateOffsets()
        {
            float offsetYaw = Session.Session.Instance.terminalProperties.YawSensitivity.Get(Block);
            float offsetPitch = Session.Session.Instance.terminalProperties.PitchSensitivity.Get(Block);
            float offsetRoll = Session.Session.Instance.terminalProperties.RollSensitivity.Get(Block);
            float offsetThrust = Session.Session.Instance.terminalProperties.ThrustSensitivity.Get(Block);
            myGyroDriver.UpdateOffsets(offsetYaw, offsetPitch, offsetRoll);
            myThrustDriver.UpdateOffsets(offsetThrust);
            Echo(myThrustDriver.ticksSinceLastRun.ToString());
        }

        internal void GetPropertyInputs()
        {
            Vector3D tangent = Session.Session.Instance.terminalProperties.SP_Tangent.Get(Block);
            Vector3D nearNormal = Session.Session.Instance.terminalProperties.SP_Normal.Get(Block);
            Vector3D motion = Session.Session.Instance.terminalProperties.SP_Motion.Get(Block);
            myGyroDriver.Load(tangent, nearNormal);
            myThrustDriver.Load(motion);
        }

        internal void UpdatePV()
        {
            Session.Session.Instance.terminalProperties.PV_Normal.Set(Block, Block.WorldMatrix.Forward);
            Session.Session.Instance.terminalProperties.PV_Tangent.Set(Block, Block.WorldMatrix.Up);
            Session.Session.Instance.terminalProperties.PV_Motion.Set(Block, myThrustDriver.PelocityPv);
        }

        internal void InitializeDrone()
        {
            //drone setting set to default here, change to autotune later
            myDroneSettings = new DroneSettings();
            myDroneSettings.SetPitch(2, .01f, 0, 30);
            myDroneSettings.SetYaw(2, .01f, 0, 30);
            myDroneSettings.SetRoll(2, .01f, 0, 30);
            myDroneSettings.SetThrust(1, .01f, 0, 1);

            _tempFat = new List<IMySlimBlock>();
            _tempSlim = new List<IMySlimBlock>();
            Block.CubeGrid.GetBlocks(_tempSlim);
            foreach (var slimBlock in _tempSlim)
                if (slimBlock.FatBlock != null)
                    _tempFat.Add(slimBlock as IMySlimBlock);
            myGyroDriver = new GyroDriver(_tempFat, this, myDroneSettings);
            myThrustDriver = new ThrustDriver(_tempFat, this, myDroneSettings);
            Purge();
            _tempFat.Clear();
            _tempSlim.Clear();
        }

        internal void Purge()
        {
            _tempFat?.Clear();
            _tempSlim?.Clear();
            _tempFat = new List<IMySlimBlock>();
            _tempSlim = new List<IMySlimBlock>();

            myDroneSettings = new DroneSettings();
            myDroneSettings.SetPitch(2, .01f, 0, 30);
            myDroneSettings.SetYaw(2, .01f, 0, 30);
            myDroneSettings.SetRoll(2, .01f, 0, 30);
            myDroneSettings.SetThrust(1, .01f, 0, 1);

            Block.CubeGrid.GetBlocks(_tempSlim);
            foreach (var slimBlock in _tempSlim)
                if (slimBlock.FatBlock != null)
                    _tempFat.Add(slimBlock as IMySlimBlock);
            myGyroDriver.Purge(_tempFat);
            myThrustDriver.Purge(_tempFat);
        }

        internal void RegisterForPurge(IMySlimBlock damagedBlock)
        {
            Session.Session.Instance.RegisterForPurge(Block.EntityId);
        }
    }
}
