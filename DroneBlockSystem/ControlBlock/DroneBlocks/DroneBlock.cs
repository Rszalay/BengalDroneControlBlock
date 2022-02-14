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
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RemoteControl), true, "BengalDroneControlBlock")]
    partial class DroneBlock : MyGameLogicComponent, IMyDBSBlock
    {
        internal IMyRemoteControl Block;
        public List<string> EchoStrings = new List<string>();
        int count = 0;
        bool inited = false;
        MyIni _ini = new MyIni();
        bool IsServerOrHost = false;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Block = (IMyRemoteControl)Entity;
            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
        }

        public MyBlockType GetBlockType() { return MyBlockType.Controller; }
        public long GetGridId() { return Block.CubeGrid.EntityId; }

        public override void UpdateBeforeSimulation()
        {
            if (!inited)
                return;
            Block.RefreshCustomInfo();
            if (IsServerOrHost)
            {
                count++;
                Echo(count.ToString());
                Echo(myThrustDriver.ThrustData());
            }
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
                grid.OnBlockRemoved -= RegisterForPurge;
            }
        }

        public override void UpdateOnceBeforeFrame() // first update of the block
        {
            Block = (IMyRemoteControl)Entity;
            Session.Session.Instance.terminalProperties.OpenBlock(Block);
            Session.Session.Instance?.AllDroneBlocks.Add(Block.EntityId, this);
            Session.Session.Instance?.AllBlocks.Add(this);
            if ((MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Multiplayer.MultiplayerActive) || !MyAPIGateway.Multiplayer.MultiplayerActive)
                IsServerOrHost = true;
            if (Block.CubeGrid?.Physics == null) // ignore projected and other non-physical grids
                return;
            if (IsServerOrHost)
            {
                IMyCubeGrid grid = Block.CubeGrid;
                Block.AppendingCustomInfo += AppendCustomInfo;
                grid.OnBlockRemoved += RegisterForPurge;
                grid.OnBlockAdded += RegisterForPurge;
                InitializeDrone();
                inited = true;
            }
        }

        private void AppendCustomInfo(IMyTerminalBlock block, StringBuilder text)
        {
            
        }

        public void Echo(string echoString)
        {
            EchoStrings.Add(echoString);
        }
    }
}
