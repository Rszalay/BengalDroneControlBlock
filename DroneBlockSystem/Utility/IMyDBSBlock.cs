using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.Common.ObjectBuilders;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.ModAPI;
using VRage.Game.ModAPI.Ingame.Utilities;
using DroneBlockSystem.ControlBlock.Settings;
using DroneBlockSystem.TargetingBlock.TargetingBlocks;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Linq;
using System.Threading.Tasks;
using VRage.Game.ModAPI.Network;
using VRage.Network;
using VRage.Utils;
using DroneBlockSystem.NetworkProtobuf;
using DroneBlockSystem.Utility;
using Sandbox.Game.EntityComponents;

namespace DroneBlockSystem.Utility
{
    enum MyBlockType
    {
        None = 0,
        Controller = 1,
        Switch = 2,
        Targeting = 3,
        Comms = 4,
        Script = 5,
        Coprocessor = 6
    }
    interface IMyDBSBlock
    {
        MyBlockType GetBlockType();
        long GetGridId();
    }
}
