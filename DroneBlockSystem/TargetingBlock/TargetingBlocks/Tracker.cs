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
using DroneBlockSystem.TargetingBlock.CoreSystems.Api;
using Sandbox.Common;
using Sandbox.Game;
using SpaceEngineers.Game;
using SpaceEngineers.ObjectBuilders;
using VRage.Game;
using Sandbox.Game.Entities;
using VRage.Utils;
using DroneBlockSystem.ControlBlock.DroneBlocks;
using DroneBlockSystem.Interface;
using DroneBlockSystem.NetworkProtobuf;
using DroneBlockSystem.TargetingBlock.TargetingBlocks;
using ProtoBuf;
using VRage;
using VRage.Game.Entity;
using VRageMath;

namespace DroneBlockSystem.TargetingBlock.TargetingBlocks
{
    class Tracker
    {
        private IMyEntities myEntities;
        public Dictionary<long, long> GridsWithControlBlocks = new Dictionary<long, long>();//blockID, gridID
        public Dictionary<long, List<long>> ThreatIdsByBlock = new Dictionary<long, List<long>>();//blockId, threat entityIds<>
        Dictionary<long, ObjectiveTracker> Objectives = new Dictionary<long, ObjectiveTracker>();
        ICollection<MyTuple<IMyEntity, float>> tempThreats;

        public void Run()
        {
            foreach(var grid in GridsWithControlBlocks)
            {
                IMyEntity thisGrid = myEntities.GetEntityById(grid.Value);
                tempThreats?.Clear();
                tempThreats = new List<MyTuple<IMyEntity, float>>();
                Session.Session.Instance.WeaponCore.GetSortedThreats(thisGrid, tempThreats);
                ThreatIdsByBlock[grid.Value]?.Clear();
                ThreatIdsByBlock[grid.Value] = new List<long>();
                foreach (var threat in tempThreats)
                {
                    ThreatIdsByBlock[grid.Value].Add(threat.Item1.EntityId);
                    if(!Objectives.ContainsKey(threat.Item1.EntityId))
                    {
                        Objectives.Add(threat.Item1.EntityId, new ObjectiveTracker(threat.Item1));
                        Action<IMyEntity> x = (IMyEntity ent) => { Objectives.Remove(ent.EntityId); };
                        threat.Item1.OnMarkForClose += x;
                    }    
                }
                foreach(var objective in Objectives)
                {
                    objective.Value.Update(myEntities.GetEntityById(objective.Key).GetPosition(), 1);
                }
            }
        }

        public void GetThreatTrackers(long targetingGrid, ref List<ObjectiveTracker> objectiveTrackers)
        {
            foreach(var objective in ThreatIdsByBlock[targetingGrid])
            {
                objectiveTrackers.Add(Objectives[objective]);
            }
        }

        public void AddGridWithControlBlock(IMyTerminalBlock controlBlock)
        {
            GridsWithControlBlocks.Add(controlBlock.EntityId, controlBlock.CubeGrid.EntityId);
        }

        public void RemoveGridWithControlBlock(IMyTerminalBlock controlBlock)
        {
            GridsWithControlBlocks.Remove(controlBlock.EntityId);
        }
    }
}
