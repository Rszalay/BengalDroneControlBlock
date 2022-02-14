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

namespace DroneBlockSystem.ControlBlock.DroneBlocks
{
    partial class DroneBlock : MyGameLogicComponent
    {
        List<IMyDBSBlock> AttachedComps = new List<IMyDBSBlock>();
        TargetingComputer TargetingBlock = new TargetingComputer();
        //CommsArray CommsBlock;
        //AISwitch SwitchBlock;
        //List<AIScript> ScriptBlock;

        bool GetAttachedComps(long gridId)
        {
            //Get the drone components for this grid, if missing components would render the system non functional retur false
            if (Session.Session.Instance.AllBlocks.Count == 0)
                return false;
            AttachedComps = Session.Session.Instance.AllBlocks.Where(x => x.GetGridId() == gridId).ToList();
            if (AttachedComps.Count == 0)
                return false;
            TargetingBlock = AttachedComps.Where(x => x.GetBlockType() == MyBlockType.Targeting).First() as TargetingComputer ?? null;
            if (TargetingBlock == null)
                return false;
            AttachedComps.Clear();
            return true;
        }
    }
}
