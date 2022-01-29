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
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Utils;

namespace BengalDroneControlBlock.Interface
{
    class TerminalProperties
    {
        public Vector3DProperty PV_Tangent;
        public Vector3DProperty PV_Normal;
        public Vector3DProperty PV_Motion;
        public Vector3DProperty SP_Tangent;
        public Vector3DProperty SP_Normal;
        public Vector3DProperty SP_Motion;

        IMyTerminalAction RunOnce;

        public bool AreTerminalPropertiesSet { get; private set; }

        public void SetTerminalProperties()
        {
            PV_Tangent = new Vector3DProperty("PV_Tangent");
            PV_Normal = new Vector3DProperty("PV_Normal");
            PV_Motion = new Vector3DProperty("PV_Motion");
            SP_Tangent = new Vector3DProperty("SP_Tangent");
            SP_Normal = new Vector3DProperty("SP_Normal");
            SP_Motion = new Vector3DProperty("SP_Motion");

            RunOnce = MyAPIGateway.TerminalControls.CreateAction<IMyRemoteControl>("RunOnce");
            RunOnce.Action = Run;
            MyAPIGateway.TerminalControls.AddAction<IMyRemoteControl>(RunOnce);

            AreTerminalPropertiesSet = true;
        }

        public void CloseBlock(IMyTerminalBlock myTerminalBlock)
        {
            PV_Tangent.CloseBlock(myTerminalBlock);
            PV_Normal.CloseBlock(myTerminalBlock);
            PV_Motion.CloseBlock(myTerminalBlock);
            SP_Tangent.CloseBlock(myTerminalBlock);
            SP_Normal.CloseBlock(myTerminalBlock);
            SP_Motion.CloseBlock(myTerminalBlock);
        }

        public void OpenBlock(IMyTerminalBlock myTerminalBlock)
        {
            PV_Tangent.OpenBlock(myTerminalBlock);
            PV_Normal.OpenBlock(myTerminalBlock);
            PV_Motion.OpenBlock(myTerminalBlock);
            SP_Tangent.OpenBlock(myTerminalBlock);
            SP_Normal.OpenBlock(myTerminalBlock);
            SP_Motion.OpenBlock(myTerminalBlock);
        }

        public void Run(IMyTerminalBlock droneBlock)
        {
            Session.Session.Instance.DroneBlocks[droneBlock.EntityId].Run();
        }
    }
}
