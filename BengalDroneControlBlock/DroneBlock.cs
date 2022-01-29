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


namespace BengalDroneControlBlock
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RemoteControl), true, "BengalDroneControlBlock")]
    partial class DroneBlock : MyGameLogicComponent
    {
        public IMyRemoteControl Block;
        private DroneSettings controlSettings;
        List<string> EchoStrings = new List<string>();
        int count = 0;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Block = (IMyRemoteControl)Entity;
            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            count++;
            Echo(count.ToString());
            Purge();
            Echo(myThrustDriver.ThrustData());
            Block.RefreshCustomInfo();
        }


        public override void Close() // called when block is removed for whatever reason (including ship despawn)
        {
            Session.Session.Instance.terminalProperties.CloseBlock(Block);
            Session.Session.Instance?.DroneBlocks.Remove(Block.EntityId);
            Block.AppendingCustomInfo -= AppendCustomInfo;
        }

        public override void UpdateOnceBeforeFrame() // first update of the block
        {
            var block = (IMyRemoteControl)Entity;

            if (block.CubeGrid?.Physics == null) // ignore projected and other non-physical grids
                return;

            Session.Session.Instance?.DroneBlocks.Add(Block.EntityId, this);
            Session.Session.Instance.terminalProperties.OpenBlock(Block);
            block.AppendingCustomInfo += AppendCustomInfo;
            InitializeDrone();
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

        public void Echo(string echoString)
        {
            EchoStrings.Add(echoString);
        }
    }
}
