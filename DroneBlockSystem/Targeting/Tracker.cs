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
using DroneBlockSystem.CoreSystems.Api;
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
using Sandbox.ModAPI.Ingame;

namespace DroneBlockSystem.Targeting
{
    class Tracker
    {
        public Dictionary<long, List<long>> ThreatIdsByBlock = new Dictionary<long, List<long>>();//GridId, threat entityIds<>
        Dictionary<long, IMyEntity> Objectives = new Dictionary<long, IMyEntity>();
        ICollection<MyTuple<IMyEntity, float>> tempThreats;

        public void Run()
        {/*
            foreach(var block in Session.Session.Instance.TargetingBlocks)
            {
                IMyEntity thisGrid = block.Value.Block.CubeGrid;
                tempThreats?.Clear();
                tempThreats = new List<MyTuple<IMyEntity, float>>();
                Session.Session.Instance.WeaponCore.GetSortedThreats(thisGrid, tempThreats);
                ThreatIdsByBlock[thisGrid.EntityId]?.Clear();
                ThreatIdsByBlock[thisGrid.EntityId] = new List<long>();
                foreach (var threat in tempThreats)
                {
                    ThreatIdsByBlock[thisGrid.EntityId].Add(threat.Item1.EntityId);
                    if(!Objectives.ContainsKey(threat.Item1.EntityId))
                    {
                        Objectives.Add(threat.Item1.EntityId, new ObjectiveTracker(threat.Item1));
                        Action<IMyEntity> x = (IMyEntity ent) => { Objectives.Remove(ent.EntityId); };
                        threat.Item1.OnMarkForClose += x;
                    }    
                }
                foreach(var objective in Objectives)
                {
                    objective.Value.Update(MyAPIGateway.Entities.GetEntityById(objective.Key).GetPosition(), 1);
                }
            }*/
        }

        public void GetThreatTrackers(long targetingGrid, out List<IMyEntity> objectiveTrackers)
        {
            objectiveTrackers = new List<IMyEntity>();
            tempThreats?.Clear();
            tempThreats = new List<MyTuple<IMyEntity, float>>();
            Session.Session.Instance.WeaponCore.GetSortedThreats(MyAPIGateway.Entities.GetEntityById(targetingGrid), tempThreats);
            foreach (var threat in tempThreats)
                objectiveTrackers.Add(threat.Item1);
        }
    }
}
