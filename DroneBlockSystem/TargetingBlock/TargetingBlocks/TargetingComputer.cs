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
using DroneBlockSystem.CoreSystems.Api;
using Sandbox.Common;
using Sandbox.Game;
using SpaceEngineers.Game;
using SpaceEngineers.ObjectBuilders;
using SpaceEngineers.Game.Entities.Blocks;
using System.Linq;
using Sandbox.Definitions;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems.Conveyors;
using VRage.Import;
using VRageMath;
using Sandbox.Common.ObjectBuilders.Definitions;
using VRageRender;
using System.Diagnostics;
using Sandbox.Game.EntityComponents;
using VRage.Game;
using Sandbox.Game.Entities;
using VRageRender.Import;
using DroneBlockSystem.Utility;
using System.IO;
using DroneBlockSystem.Targeting;


namespace DroneBlockSystem.TargetingBlock.TargetingBlocks
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RemoteControl), true, "RagdollTargetingComputer")]
    partial class TargetingComputer : MyGameLogicComponent, IMyDBSBlock
    {
        public IMyRemoteControl Block;
        List<string> EchoStrings = new List<string>();
        bool inited = false;
        MyIni _ini = new MyIni();
        public List<IMyEntity> objectiveTrackers;
        int count = 0;
        bool IsServerOrHost = false;


        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Block = (IMyRemoteControl)Entity;
            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
            TextWriter textWriter = MyAPIGateway.Utilities.WriteFileInLocalStorage("asdf.txt", typeof(Session.Session));
            textWriter.WriteLine("I worked");
            textWriter.Flush();
            textWriter.Close();
            textWriter.Dispose();
        }

        public MyBlockType GetBlockType() { return MyBlockType.Targeting; }
        public long GetGridId() { return Block.CubeGrid.EntityId; }

        public override void UpdateBeforeSimulation()
        {
            if (!Session.Session.Instance.WcInited)
                return;
            count++;
        }

        public void GetLocalTrackers()
        {
            if (IsServerOrHost)
            {
                objectiveTrackers?.Clear();
                Session.Session.Instance.TargetTracker?.GetThreatTrackers(Block.CubeGrid.EntityId, out objectiveTrackers);
                //if (count % 60 == 0)
                    //MyAPIGateway.Utilities.SendMessage(Block.EntityId.ToString());
            }
            count++;
        }

        public override void Close() // called when block is removed for whatever reason (including ship despawn)
        {
            Session.Session.Instance.terminalProperties.CloseBlock(Block);
            Session.Session.Instance?.AllDroneBlocks.Remove(Block.EntityId);
            Session.Session.Instance?.AllBlocks.Remove(this);
            if (IsServerOrHost)
            {
                IMyCubeGrid grid = Block.CubeGrid;
                Block.AppendingCustomInfo -= AppendCustomInfo;
            }
        }

        public override void UpdateOnceBeforeFrame() // first update of the block
        {
            Block = (IMyRemoteControl)Entity;
            Session.Session.Instance.terminalProperties.OpenBlock(Block);
            Session.Session.Instance?.TargetingBlocks.Add(Block.EntityId, this);
            Session.Session.Instance?.AllBlocks.Add(this);
            if ((MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Multiplayer.MultiplayerActive) || !MyAPIGateway.Multiplayer.MultiplayerActive)
                IsServerOrHost = true;
            if (Block.CubeGrid?.Physics == null) // ignore projected and other non-physical grids
                return;
            if (IsServerOrHost)
            {
                IMyCubeGrid grid = Block.CubeGrid;
                Block.AppendingCustomInfo += AppendCustomInfo;
                inited = true;
            }
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
