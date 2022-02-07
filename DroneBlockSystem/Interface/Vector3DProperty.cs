using System.Collections.Generic;
using VRageMath;
using Sandbox.ModAPI;

namespace DroneBlockSystem.Interface
{
    class Vector3DProperty
    {
        readonly Dictionary<long, Vector3D> blockPropValues = new Dictionary<long, Vector3D>();

        internal Vector3DProperty(string propId)
        {
            var vectorProp = MyAPIGateway.TerminalControls.CreateProperty<Vector3D, IMyRemoteControl>(propId);
            vectorProp.Getter = Get;
            vectorProp.Setter = Set;
            MyAPIGateway.TerminalControls.AddControl<IMyRemoteControl>(vectorProp);
        }

        internal Vector3D Get(IMyTerminalBlock myTerminalBlock)
        {
            if (blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                return blockPropValues[myTerminalBlock.EntityId];
            else
                return
                    Vector3D.Zero;
        }

        internal void Set(IMyTerminalBlock myTerminalBlock, Vector3D vector)
        {
            if (blockPropValues.ContainsKey(myTerminalBlock.EntityId))
            {
                blockPropValues[myTerminalBlock.EntityId] = vector;
            }
            else
                blockPropValues.Add(myTerminalBlock.EntityId, vector);
        }

        internal void CloseBlock(IMyTerminalBlock myTerminalBlock)
        {
            if(blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                blockPropValues.Remove(myTerminalBlock.EntityId);
        }

        internal void OpenBlock(IMyTerminalBlock myTerminalBlock)
        {
            if (!blockPropValues.ContainsKey(myTerminalBlock.EntityId))
                blockPropValues.Add(myTerminalBlock.EntityId, Vector3D.Zero);
        }
    }
}
