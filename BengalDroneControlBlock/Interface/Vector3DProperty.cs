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
    class Vector3DProperty
    {
        Dictionary<long, Vector3D> blockProps = new Dictionary<long, Vector3D>();

        public Vector3DProperty(string propId)
        {
            var vectorProp = MyAPIGateway.TerminalControls.CreateProperty<Vector3D, IMyRemoteControl>(propId);
            vectorProp.Getter = Get;
            vectorProp.Setter = Set;
            MyAPIGateway.TerminalControls.AddControl<IMyRemoteControl>(vectorProp);
        }

        public Vector3D Get(IMyTerminalBlock myTerminalBlock)
        {
            if (blockProps.ContainsKey(myTerminalBlock.EntityId))
                return blockProps[myTerminalBlock.EntityId];
            else
                return
                    Vector3D.Zero;
        }

        public void Set(IMyTerminalBlock myTerminalBlock, Vector3D vector)
        {
            if (blockProps.ContainsKey(myTerminalBlock.EntityId))
                blockProps[myTerminalBlock.EntityId] = vector;
            else
                blockProps.Add(myTerminalBlock.EntityId, vector);
        }

        public void CloseBlock(IMyTerminalBlock myTerminalBlock)
        {
            if(blockProps.ContainsKey(myTerminalBlock.EntityId))
                blockProps.Remove(myTerminalBlock.EntityId);
        }

        public void OpenBlock(IMyTerminalBlock myTerminalBlock)
        {
            if (!blockProps.ContainsKey(myTerminalBlock.EntityId))
                blockProps.Add(myTerminalBlock.EntityId, Vector3D.Zero);
        }
    }
}
