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
using BengalDroneControlBlock.NetworkProtobuf;

namespace BengalDroneControlBlock.Interface
{
    class SliderControl
    {
        Dictionary<long, float> blockPropValues = new Dictionary<long, float>();
        public IMyTerminalControlSlider slider;

        public SliderControl(string propId, string title)
        {
            slider = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyRemoteControl>(propId);
            slider.Getter = Get;
            slider.Setter = Set;
            slider.Title = MyStringId.GetOrCompute(title);
            slider.Visible = Istrue;
            slider.Enabled = Isenabled;
            slider.SetLimits(0, 2);
            MyAPIGateway.TerminalControls.AddControl<IMyRemoteControl>(slider);
        }

        public float Get(IMyTerminalBlock myTerminalBlock)
        {
            if (blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                return blockPropValues[myTerminalBlock.EntityId];
            else return 0f;
        }

        public void Set(IMyTerminalBlock myTerminalBlock, float value)
        {
            if (blockPropValues.ContainsKey(myTerminalBlock.EntityId))
            {
                ControlUpdatePacket updatePacket = new ControlUpdatePacket((slider as IMyTerminalControl).Id, value, myTerminalBlock.EntityId);
                Session.Session.Instance.Networking.SendToServer(updatePacket);
                blockPropValues[myTerminalBlock.EntityId] = value;
            }
        }

        public void CloseBlock(IMyTerminalBlock myTerminalBlock)
        {
            if(blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                blockPropValues.Remove(myTerminalBlock.EntityId);
        }

        public void OpenBlock(IMyTerminalBlock myTerminalBlock)
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
