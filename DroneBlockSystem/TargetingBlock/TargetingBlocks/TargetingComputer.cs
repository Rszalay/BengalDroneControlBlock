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



namespace DroneBlockSystem.TargetingBlock.TargetingBlocks
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_ConveyorSorter), true, "TargetingComputer")]
    partial class TargetingComputer : MyGameLogicComponent
    {
        public IMyConveyorSorter Block;
        List<string> EchoStrings = new List<string>();
        bool inited = false;
        MyIni _ini = new MyIni();
        List<ObjectiveTracker> objectiveTrackers;


        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Block = (IMyConveyorSorter)Entity;
            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            if (!inited)
                return;
            Block.RefreshCustomInfo();
            GetLocalTrackers();
        }

        public void GetLocalTrackers()
        {
            objectiveTrackers?.Clear();
            objectiveTrackers = new List<ObjectiveTracker>();
            Session.Session.Instance.TargetTracker.GetThreatTrackers(Block.EntityId, ref objectiveTrackers);

        }

        public override void Close() // called when block is removed for whatever reason (including ship despawn)
        {
            IMyCubeGrid grid = Block.CubeGrid;
            Session.Session.Instance.terminalProperties.CloseBlock(Block);
            Session.Session.Instance?.TargetingBlocks.Remove(Block.EntityId);
            Block.AppendingCustomInfo -= AppendCustomInfo;
            Block.CustomDataChanged -= CustomDataChanged;
        }

        public override void UpdateOnceBeforeFrame() // first update of the block
        {
            var block = (IMyRemoteControl)Entity;

            if (block.CubeGrid?.Physics == null) // ignore projected and other non-physical grids
                return;
            IMyCubeGrid grid = Block.CubeGrid;
            Session.Session.Instance?.TargetingBlocks.Add(Block.EntityId, this);
            Session.Session.Instance.terminalProperties.OpenBlock(Block);
            block.AppendingCustomInfo += AppendCustomInfo;
            block.CustomDataChanged += CustomDataChanged;
            //inited = true; Inited should be set true when the wc api is loaded, use callback
        }

        private void AppendCustomInfo(IMyTerminalBlock block, StringBuilder text)
        {
            if (block == null) return;
            text.Clear();
            text = new StringBuilder();
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
            _ini.Clear();
            _ini = new MyIni();
        }

        public void Echo(string echoString)
        {
            EchoStrings.Add(echoString);
        }
    }
}
