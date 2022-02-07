using System.Collections.Generic;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Utils;
using DroneBlockSystem.NetworkProtobuf;

namespace DroneBlockSystem.Interface
{
    class SliderControl
    {
        public Dictionary<long, float> blockPropValues = new Dictionary<long, float>();
        public IMyTerminalControlSlider slider;

        internal SliderControl(string propId, string title)
        {
            slider = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyRemoteControl>(propId);
            slider.Getter = Get;
            slider.Setter = Set;
            slider.Title = MyStringId.GetOrCompute(title);
            slider.Visible = Istrue;
            slider.Enabled = Isenabled;
            slider.SetLogLimits(.1f, 10f);
            MyAPIGateway.TerminalControls.AddControl<IMyRemoteControl>(slider);
        }

        internal float Get(IMyTerminalBlock myTerminalBlock)
        {
            if (blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                return blockPropValues[myTerminalBlock.EntityId];
            else return 0f;
        }

        internal void NoEchoSet(IMyTerminalBlock myTerminalBlock, float value)
        {
            if (blockPropValues.ContainsKey(myTerminalBlock.EntityId))
            {
                blockPropValues[myTerminalBlock.EntityId] = value;
            }
        }

        internal void Set(IMyTerminalBlock myTerminalBlock, float value)
        {
            if (blockPropValues.ContainsKey(myTerminalBlock.EntityId))
            {
                ControlUpdatePacket updatePacket = new ControlUpdatePacket((slider as IMyTerminalControl).Id, value, myTerminalBlock.EntityId);
                Session.Session.Instance.Networking.SendToServer(updatePacket);
                blockPropValues[myTerminalBlock.EntityId] = value;
            }
        }

        internal void CloseBlock(IMyTerminalBlock myTerminalBlock)
        {
            if(blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                blockPropValues.Remove(myTerminalBlock.EntityId);
        }

        internal void OpenBlock(IMyTerminalBlock myTerminalBlock)
        {
            if (!blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                blockPropValues.Add(myTerminalBlock.EntityId, 1f);
        }

        internal static bool Istrue(IMyTerminalBlock block)
        {
            return true;
        }

        internal static bool Isenabled(IMyTerminalBlock block)
        {
            return true;
        }
    }
}
