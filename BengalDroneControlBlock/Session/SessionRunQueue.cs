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
        public Queue<long> PurgeQueue = new Queue<long>();
        public Queue<long> RunQueue = new Queue<long>();
        int dronesToRun = 10;

        public void Purge()
        {
            //only one drone gets to purge each tick
            if(PurgeQueue.Count > 0)
            {
                long thisDrone = PurgeQueue.Dequeue();
                if(DroneBlocks.ContainsKey(thisDrone))
                    DroneBlocks[thisDrone].Purge();
            }
        }

        public void RegisterForPurge(long thisDrone)
        {
            if (!PurgeQueue.Contains(thisDrone))
                PurgeQueue.Enqueue(thisDrone);
        }

        public void Run()
        {
            if ((MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated) || !MyAPIGateway.Multiplayer.MultiplayerActive)
            {
                if (DroneBlocks == null)
                    return;
                if (DroneBlocks.Count > 0)
                {
                    Purge();
                    if (RunQueue.Count < dronesToRun * 2)
                        foreach (var drone in DroneBlocks)
                            RunQueue.Enqueue(drone.Key);
                    for (int i = 0; i < dronesToRun; i++)
                    {
                        DroneBlocks[RunQueue.Dequeue()].Run();
                        if (i >= DroneBlocks.Count - 1)
                            break;
                    }
                }
                foreach (var drone in DroneBlocks)
                    drone.Value.Tick();
            }
        }
    }
}
