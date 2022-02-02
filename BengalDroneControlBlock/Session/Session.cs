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
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Utils;
using BengalDroneControlBlock.Interface;
using BengalDroneControlBlock.DroneBlocks;
using BengalDroneControlBlock.NetworkProtobuf;

namespace BengalDroneControlBlock.Session
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    partial class Session : MySessionComponentBase
    {
        public static Session Instance;
        public Dictionary<long,DroneBlock> DroneBlocks = new Dictionary<long, DroneBlock>();
        public TerminalProperties terminalProperties;
        bool inited = false;
        public Networking Networking = new Networking(5568);

        public override void LoadData()
        {
            Instance = this;
            MyEntities.OnEntityCreate += OnEntityCreate;
        }

        public override void BeforeStart()
        {
            if ((MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated) || !MyAPIGateway.Utilities.IsDedicated)
            {
                if (terminalProperties == null)
                    terminalProperties = new TerminalProperties();
                if (!terminalProperties.AreTerminalPropertiesSet)
                    terminalProperties.SetTerminalProperties();
                Networking.Register();
            }
        }

        protected override void UnloadData()
        {
            terminalProperties = null;
            Instance = null;
            Networking?.Unregister();
            Networking = null;
            MyEntities.OnEntityCreate -= OnEntityCreate;
            MyAPIGateway.TerminalControls.CustomControlGetter -= TerminalBuilder;
        }

        public void OnEntityCreate(IMyEntity ent)
        {
            if(!inited)
                MyAPIGateway.TerminalControls.CustomControlGetter += TerminalBuilder;
        }

        public override void UpdateBeforeSimulation()
        {
            if ((MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated) || !MyAPIGateway.Utilities.IsDedicated)
            {
                foreach (var block in DroneBlocks)
                    if (block.Value != null)
                    {
                        block.Value.Tick();
                        block.Value.UpdatePV();
                        block.Value.Run();
                    }
            }
        }

        public override void Simulate()
        {
            
        }

        public override void UpdateAfterSimulation()
        {
            try 
            {
                

            }
            catch (Exception e)
            {
                MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");

                if (MyAPIGateway.Session?.Player != null)
                    MyAPIGateway.Utilities.ShowNotification($"[ ERROR: {GetType().FullName}: {e.Message} | Send SpaceEngineers.Log to mod author ]", 10000, MyFontEnum.Red);
            }
        }

        public override void Draw()
        {
            
        }

        public override void SaveData()
        {
            
        }

        public override void UpdatingStopped()
        {

        }
    }
}
