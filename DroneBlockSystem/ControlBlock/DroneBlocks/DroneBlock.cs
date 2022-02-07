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
using DroneBlockSystem.TargetingBlock.TargetingBlocks;



namespace DroneBlockSystem.ControlBlock.DroneBlocks
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RemoteControl), true, "BengalDroneControlBlock")]
    partial class DroneBlock : MyGameLogicComponent
    {
        internal IMyRemoteControl Block;
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

        internal void CustomDataChanged(IMyTerminalBlock thisBlock)
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
