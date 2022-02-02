using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ObjectBuilders;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;
using Sandbox.ModAPI.Interfaces.Terminal;
using BengalDroneControlBlock.Interface;
using Sandbox;
using Sandbox.Common;
using Sandbox.Graphics;
using SpaceEngineers.Game;
using SpaceEngineers.ObjectBuilders;
using VRage;
using VRage.Input;
using VRage.Library;
using Sandbox.Common.ObjectBuilders;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using Sandbox.Definitions;
using BengalDroneControlBlock.DroneBlocks;

namespace BengalDroneControlBlock.Session
{
    partial class Session : MySessionComponentBase
    {
        public List<IMyTerminalControl> Controls;
        public void TerminalBuilder(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (!Instance.DroneBlocks.ContainsKey(block.EntityId))
            {
                for (int i = controls.Count - 1; i >= 0; i--)
                    if (terminalProperties.PropertyNames.Contains(controls[i].Id))
                        controls.RemoveAt(i);
            }
        }
    }
}
