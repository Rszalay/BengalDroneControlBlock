using System.Collections.Generic;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace DroneBlockSystem.Session
{
    partial class Session : MySessionComponentBase
    {
        public Queue<long> PurgeQueue = new Queue<long>();
        public Queue<long> RunQueue = new Queue<long>();
        readonly int dronesToRun = 10;

        public void Purge()
        {
            //only one drone gets to purge each tick
            if(PurgeQueue.Count > 0)
            {
                long thisDrone = PurgeQueue.Dequeue();
                if(AllDroneBlocks.ContainsKey(thisDrone))
                    AllDroneBlocks[thisDrone].Purge();
            }
        }

        public void RegisterForPurge(long thisDrone)
        {
            if (!PurgeQueue.Contains(thisDrone))
                PurgeQueue.Enqueue(thisDrone);
        }

        public void Run()
        {
            if (IsServerOrHost)
            {
                if (AllDroneBlocks == null)
                    return;
                if (AllDroneBlocks.Count > 0)
                {
                    Purge();
                    if (RunQueue.Count < dronesToRun * 2)
                        foreach (var drone in AllDroneBlocks)
                            RunQueue.Enqueue(drone.Key);
                    for (int i = 0; i < dronesToRun; i++)
                    {
                        AllDroneBlocks[RunQueue.Dequeue()].Run();
                        if (i >= AllDroneBlocks.Count - 1)
                            break;
                    }
                }
                foreach (var drone in AllDroneBlocks)
                    drone.Value.Tick();
            }
        }
    }
}
