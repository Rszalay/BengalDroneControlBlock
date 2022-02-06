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
using VRage.Game.ObjectBuilders.Definitions;
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
using BengalDroneControlBlock.DroneBlocks;
using Sandbox.ModAPI;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI;
using VRage.Game.Entity;
using VRage.Utils;
using Sandbox.Definitions;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems.Conveyors;
using VRage.Import;
using Sandbox.Common.ObjectBuilders.Definitions;
using VRageRender;
using System.Diagnostics;
using VRageRender.Import;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections;
using VRage.Collections;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;



namespace BengalDroneControlBlock.DroneBlocks
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RemoteControl), true, "BengalDroneControlBlock")]
    partial class DroneBlock : MyGameLogicComponent
    {
        public IMyRemoteControl Block;
        private DroneSettings controlSettings;
        List<string> EchoStrings = new List<string>();
        int count = 0;
        bool inited = false;
        MyIni _ini = new MyIni();

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Block = (IMyRemoteControl)Entity;
            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            if (!inited)
                return;
            count++;
            Echo(count.ToString());
            Echo(myThrustDriver.ThrustData());
            Block.RefreshCustomInfo();
        }

        public override void Close() // called when block is removed for whatever reason (including ship despawn)
        {
            IMyCubeGrid grid = Block.CubeGrid;
            Session.Session.Instance.terminalProperties.CloseBlock(Block);
            Session.Session.Instance?.DroneBlocks.Remove(Block.EntityId);
            Block.AppendingCustomInfo -= AppendCustomInfo;
            grid.OnBlockRemoved -= RegisterForPurge;
            Block.CustomDataChanged -= CustomDataChanged;
        }

        public override void UpdateOnceBeforeFrame() // first update of the block
        {
            var block = (IMyRemoteControl)Entity;

            if (block.CubeGrid?.Physics == null) // ignore projected and other non-physical grids
                return;
            IMyCubeGrid grid = Block.CubeGrid;
            Session.Session.Instance?.DroneBlocks.Add(Block.EntityId, this);
            Session.Session.Instance.terminalProperties.OpenBlock(Block);
            block.AppendingCustomInfo += AppendCustomInfo;
            grid.OnBlockRemoved += RegisterForPurge;
            grid.OnBlockAdded += RegisterForPurge;
            block.CustomDataChanged += CustomDataChanged;
            InitializeDrone();
            inited = true;
        }

        private void AppendCustomInfo(IMyTerminalBlock block, StringBuilder text)
        {
            if (block == null) return;
            text.Clear();
            text.AppendLine("* * * * * * * * * * * * *");
            foreach (var line in EchoStrings) text.AppendLine(line);
            EchoStrings.Clear();
            EchoStrings = new List<string>();
        }

        public void CustomDataChanged(IMyTerminalBlock thisBlock)
        {
            MyIniParseResult result;
            if (!_ini.TryParse(thisBlock.CustomData, out result))
                throw new Exception(result.ToString());
            DroneSettings droneSettings = new DroneSettings();
            droneSettings.SetThrust(_ini.Get("Thrust", "kp").ToSingle(), _ini.Get("Thrust", "ki").ToSingle(), _ini.Get("Thrust", "kd").ToSingle(), 1);
            droneSettings.SetYaw(_ini.Get("Yaw", "kp").ToSingle(), _ini.Get("Yaw", "ki").ToSingle(), _ini.Get("Yaw", "kd").ToSingle(), 1);
            droneSettings.SetPitch(_ini.Get("Pitch", "kp").ToSingle(), _ini.Get("Pitch", "ki").ToSingle(), _ini.Get("Pitch", "kd").ToSingle(), 1);
            droneSettings.SetRoll(_ini.Get("Roll", "kp").ToSingle(), _ini.Get("Roll", "ki").ToSingle(), _ini.Get("Roll", "kd").ToSingle(), 1);
            _ini.Clear();
            _ini = new MyIni();
        }

        public void Echo(string echoString)
        {
            EchoStrings.Add(echoString);
        }
    }
}
