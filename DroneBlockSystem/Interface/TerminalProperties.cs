using System.Collections.Generic;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;

namespace DroneBlockSystem.Interface
{
    class TerminalProperties
    {
        internal Vector3DProperty PV_Tangent;
        internal Vector3DProperty PV_Normal;
        internal Vector3DProperty PV_Motion;
        internal Vector3DProperty SP_Tangent;
        internal Vector3DProperty SP_Normal;
        internal Vector3DProperty SP_Motion;

        internal SliderControl YawSensitivity;
        internal SliderControl PitchSensitivity;
        internal SliderControl RollSensitivity;
        internal SliderControl ThrustSensitivity;

        internal readonly List<string> PropertyNames = new List<string>{
            "YawSensitivity",
            "PitchSensitivity",
            "RollSensitivity",
            "ThrustSensitivity"
        };

        IMyTerminalAction RunOnce;

        internal bool AreTerminalPropertiesSet { get; private set; }

        internal void SetTerminalProperties()
        {
            PV_Tangent = new Vector3DProperty("PV_Tangent");
            PV_Normal = new Vector3DProperty("PV_Normal");
            PV_Motion = new Vector3DProperty("PV_Motion");
            SP_Tangent = new Vector3DProperty("SP_Tangent");
            SP_Normal = new Vector3DProperty("SP_Normal");
            SP_Motion = new Vector3DProperty("SP_Motion");

            YawSensitivity = new SliderControl("YawSensitivity", "Yaw Sensitivity");
            PitchSensitivity = new SliderControl("PitchSensitivity", "Pitch Sensitivity");
            RollSensitivity = new SliderControl("RollSensitivity", "Roll Sensitivity");
            ThrustSensitivity = new SliderControl("ThrustSensitivity", "Thrust Sensitivity");

            RunOnce = MyAPIGateway.TerminalControls.CreateAction<IMyRemoteControl>("RunOnce");
            RunOnce.Action = Run;
            MyAPIGateway.TerminalControls.AddAction<IMyRemoteControl>(RunOnce);

            AreTerminalPropertiesSet = true;
        }

        internal void CloseBlock(IMyTerminalBlock myTerminalBlock)
        {
            PV_Tangent.CloseBlock(myTerminalBlock);
            PV_Normal.CloseBlock(myTerminalBlock);
            PV_Motion.CloseBlock(myTerminalBlock);
            SP_Tangent.CloseBlock(myTerminalBlock);
            SP_Normal.CloseBlock(myTerminalBlock);
            SP_Motion.CloseBlock(myTerminalBlock);

            YawSensitivity.CloseBlock(myTerminalBlock);
            PitchSensitivity.CloseBlock(myTerminalBlock);
            RollSensitivity.CloseBlock(myTerminalBlock);
            ThrustSensitivity.CloseBlock(myTerminalBlock);
        }

        internal void OpenBlock(IMyTerminalBlock myTerminalBlock)
        {
            PV_Tangent.OpenBlock(myTerminalBlock);
            PV_Normal.OpenBlock(myTerminalBlock);
            PV_Motion.OpenBlock(myTerminalBlock);
            SP_Tangent.OpenBlock(myTerminalBlock);
            SP_Normal.OpenBlock(myTerminalBlock);
            SP_Motion.OpenBlock(myTerminalBlock);

            YawSensitivity.OpenBlock(myTerminalBlock);
            PitchSensitivity.OpenBlock(myTerminalBlock);
            RollSensitivity.OpenBlock(myTerminalBlock);
            ThrustSensitivity.OpenBlock(myTerminalBlock);
        }

        internal void Run(IMyTerminalBlock droneBlock)
        {
            Session.Session.Instance.DroneBlocks[droneBlock.EntityId].Run();
        }
    }
}
