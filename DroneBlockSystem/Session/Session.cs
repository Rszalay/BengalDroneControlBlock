using System;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Utils;
using DroneBlockSystem.ControlBlock.DroneBlocks;
using DroneBlockSystem.Interface;
using DroneBlockSystem.NetworkProtobuf;
using DroneBlockSystem.TargetingBlock.TargetingBlocks;
using DroneBlockSystem.TargetingBlock.CoreSystems.Api;

namespace DroneBlockSystem.Session
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    partial class Session : MySessionComponentBase
    {
        public static Session Instance;
        public Dictionary<long,DroneBlock> DroneBlocks = new Dictionary<long, DroneBlock>();
        public Dictionary<long,TargetingComputer> TargetingBlocks = new Dictionary<long, TargetingComputer>();
        public TerminalProperties terminalProperties;
        readonly bool inited = false;
        public Networking Networking = new Networking(5568);
        public WcApi WeaponCore;
        public Tracker TargetTracker = new Tracker();
        public bool WcInited { get; private set; } = false;

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
                if(inited)
                {
                    WeaponCore = new WcApi();
                    Action x = () => { WcInited = true; };
                    WeaponCore.Load(x);
                }
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
            WeaponCore?.Unload();
        }

        public void OnEntityCreate(IMyEntity ent)
        {
            if(!inited)
                MyAPIGateway.TerminalControls.CustomControlGetter += TerminalBuilder;
        }

        public override void UpdateBeforeSimulation()
        {
            Run();
            if (WcInited)
            {
                TargetTracker.Run();
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
