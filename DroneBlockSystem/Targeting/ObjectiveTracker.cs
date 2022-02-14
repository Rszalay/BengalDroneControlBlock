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
using System.Linq;
using DroneBlockSystem.Controls;
using DroneBlockSystem.PolyMath;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections;
using VRage.Collections;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ObjectBuilders.Definitions;

namespace DroneBlockSystem.Targeting
{
    class ObjectiveTracker
    {
        public long EntityId { get; private set; }
        public string Name { get; private set; }
        public MotionCurve PhysicsCurve;
        public float Threat { get; set; }


        public ObjectiveTracker(MyDetectedEntityInfo detectedEntityInfo)
        {
            PhysicsCurve = new MotionCurve(detectedEntityInfo.Position);
            EntityId = detectedEntityInfo.EntityId;
            Name = detectedEntityInfo.Name;
        }

        public ObjectiveTracker(IMyEntity entity)
        {
            EntityId = entity.EntityId;
            PhysicsCurve = new MotionCurve(entity.WorldMatrix.Translation);
            Name = entity.Name;
        }

        public override int GetHashCode()
        {
            return (int)EntityId;
        }

        public void Update(Vector3D position, int ticks)
        {
            PhysicsCurve.Update(position, ticks);
        }

        public Vector3D PredictPosition(int ticks, double speedLimit)
        {
            return PhysicsCurve.SpeedLimitSolve(ticks, speedLimit);
        }
    }
}
