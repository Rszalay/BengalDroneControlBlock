using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Common;
using Sandbox.Game;
using Sandbox.Graphics;
using SpaceEngineers.Game;
using SpaceEngineers.ObjectBuilders;
using VRage;
using VRage.Game;
using VRage.Input;
using VRage.Library;
using VRageMath;
using Sandbox.Common.ObjectBuilders;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using BengalDroneControlBlock.Settings;
using BengalDroneControlBlock.Session;
using BengalDroneControlBlock.Drivers;
using BengalDroneControlBlock.Interface;
using Sandbox.ModAPI;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI;
using VRage.Game.Entity;
using VRage.Utils;

namespace BengalDroneControlBlock.DroneBlocks
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
            //Echo("Controller Ticked");
            myGyroDriver.Tick();
            myThrustDriver.Tick();
        }

        public void Run()
        {
            float offsetYaw = Session.Session.Instance.terminalProperties.YawSensitivity.Get(Block);
            float offsetPitch = Session.Session.Instance.terminalProperties.PitchSensitivity.Get(Block);
            float offsetRoll = Session.Session.Instance.terminalProperties.RollSensitivity.Get(Block);
            //float offsetThrust = Session.Session.Instance.terminalProperties.ThrustSensitivity.Get(Block);
            myGyroDriver.UpdateOffsets(offsetYaw, offsetPitch, offsetRoll);

            Vector3D tangent = Session.Session.Instance.terminalProperties.SP_Tangent.Get(Block);
            Vector3D nearNormal = Session.Session.Instance.terminalProperties.SP_Normal.Get(Block);
            Vector3D motion = Session.Session.Instance.terminalProperties.SP_Motion.Get(Block);
            myGyroDriver.Load(tangent, nearNormal);
            myGyroDriver.Run();
            myThrustDriver.Load(motion, motion.Length());
            myThrustDriver.Run();
        }

        public void UpdatePV()
        {
            Session.Session.Instance.terminalProperties.PV_Normal.Set(Block, Block.WorldMatrix.Forward);
            Session.Session.Instance.terminalProperties.PV_Tangent.Set(Block, Block.WorldMatrix.Up);
            Session.Session.Instance.terminalProperties.PV_Motion.Set(Block, myThrustDriver.velocityPv);
        }

        public void InitializeDrone()
        {
            //drone setting set to default here, change to autotune later
            myDroneSettings = new DroneSettings();
            myDroneSettings.SetPitch(2, 5, 10, 30);
            myDroneSettings.SetYaw(2, 5, 10, 30);
            myDroneSettings.SetRoll(2, 5, 10, 30);
            myDroneSettings.SetThrust(1, .5f, 50, 1);

            _tempFat = new List<IMySlimBlock>();
            _tempSlim = new List<IMySlimBlock>();
            Block.CubeGrid.GetBlocks(_tempSlim);
            foreach (var slimBlock in _tempSlim)
                if (slimBlock.FatBlock != null)
                    _tempFat.Add(slimBlock as IMySlimBlock);
            myGyroDriver = new GyroDriver(_tempFat, this, myDroneSettings);
            myThrustDriver = new ThrustDriver(_tempFat, this, myDroneSettings);
            _tempFat.Clear();
            _tempSlim.Clear();
        }

        public void Purge()
        {
            _tempFat = new List<IMySlimBlock>();
            _tempSlim = new List<IMySlimBlock>();
            Block.CubeGrid.GetBlocks(_tempSlim);
            foreach (var slimBlock in _tempSlim)
                if (slimBlock.FatBlock != null)
                    _tempFat.Add(slimBlock as IMySlimBlock);
            myGyroDriver = new GyroDriver(_tempFat, this, myDroneSettings);
            myThrustDriver = new ThrustDriver(_tempFat, this, myDroneSettings);
            _tempFat.Clear();
            _tempSlim.Clear();
        }
    }
}
