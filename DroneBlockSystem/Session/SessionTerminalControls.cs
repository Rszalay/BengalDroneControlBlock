using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using VRage.Game.Components;
using Sandbox.ModAPI.Interfaces.Terminal;

namespace DroneBlockSystem.Session
{
    partial class Session : MySessionComponentBase
    {
        public void TerminalBuilder(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (!Instance.AllDroneBlocks.ContainsKey(block.EntityId))
            {
                for (int i = controls.Count - 1; i >= 0; i--)
                    if (terminalProperties.PropertyNames.Contains(controls[i].Id))
                        controls.RemoveAt(i);
            }
        }
    }
}
