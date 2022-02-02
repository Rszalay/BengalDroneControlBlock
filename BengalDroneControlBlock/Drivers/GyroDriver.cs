using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using VRage;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Utils;
using VRageMath;
using ProtoBuf;
using BengalDroneControlBlock.Controls;
using BengalDroneControlBlock.Settings;
using BengalDroneControlBlock.DroneBlocks;

namespace BengalDroneControlBlock.Drivers
{
    class GyroDriver
    {
        DroneBlock ThisController;
        Dictionary<MyBlockOrientation, List<IMyGyro>> gyroSets;
        Ideal Yaw, Pitch, Roll;
        Vector3D _tangentVector;
        Vector3D _normalVector;

        float OffsetYaw = 1;
        float OffsetPitch = 1;
        float OffsetRoll = 1;

        public GyroDriver(List<IMySlimBlock> slimBlocks, DroneBlock thisController, DroneSettings settings)
        {
            ThisController = thisController;
            Yaw = new Ideal(settings.Yaw, ThisController);
            Pitch = new Ideal(settings.Pitch, ThisController);
            Roll = new Ideal(settings.Roll, ThisController);
            Purge(slimBlocks);
        }

        public void UpdateOffsets(float yaw, float pitch, float roll)
        {
            OffsetYaw = yaw;
            OffsetPitch = pitch;
            OffsetRoll = roll;
        }

        public void Purge(List<IMySlimBlock> slimBlocks)
        {
            gyroSets?.Clear();
            gyroSets = new Dictionary<MyBlockOrientation, List<IMyGyro>>();
            foreach (var block in slimBlocks)
            {
                if (block.FatBlock is IMyGyro)
                {
                    if (block == null)
                        continue;
                    if (!gyroSets.ContainsKey(block.Orientation))
                    {
                        gyroSets.Add(block.Orientation, new List<IMyGyro>());
                        gyroSets[block.Orientation].Add(block.FatBlock as IMyGyro);
                    }
                    else
                        gyroSets[block.Orientation].Add(block.FatBlock as IMyGyro);
                }
            }
        }

        public void ApplyGyroOverride(Vector3D gyroRpms)
        {
            //Modified from Whip's ApplyGyroOverride Method v9 - 8/19/17
            // X : Pitch, Y : Yaw, Z : Roll
            //gyroRpms.X = -gyroRpms.X;
            var relativeRotationVec = Vector3D.TransformNormal(gyroRpms, ThisController.Block.Entity.WorldMatrix);
            foreach (var orientation in gyroSets)
            {
                IMyGyro firstGyro = orientation.Value.First();
                var transformedRotationVec = Vector3D.TransformNormal(relativeRotationVec, Matrix.Transpose(firstGyro.WorldMatrix));
                foreach (var gyro in orientation.Value)
                {
                    gyro.Pitch = (float)transformedRotationVec.X;
                    gyro.Yaw = (float)transformedRotationVec.Y;
                    gyro.Roll = (float)transformedRotationVec.Z;
                    gyro.GyroOverride = true;
                }
            }
        }

        public void Load(Vector3D tangentVector, Vector3D nearNormalVector)
        {
            _tangentVector = tangentVector;
            _normalVector = nearNormalVector;
            Vector3D biTangentVector = Vector3D.Cross(tangentVector, nearNormalVector);
            _normalVector = Vector3D.Cross(tangentVector, biTangentVector);

            Vector3D transformedTangent = Vector3D.TransformNormal(_tangentVector, MatrixD.Transpose(ThisController.Entity.WorldMatrix));
            Vector3D transformedNormal = Vector3D.TransformNormal(_normalVector, MatrixD.Transpose(ThisController.Entity.WorldMatrix));

            double pitchError = Math.Asin(-transformedTangent.Y);
            double YawError = Math.Asin(transformedTangent.X);
            double rollError = Math.Asin(transformedNormal.X);
            ThisController.Echo("KP: " + OffsetYaw);

            Yaw.Load(YawError * OffsetYaw);
            Pitch.Load(pitchError * OffsetPitch);
            Roll.Load(rollError * OffsetRoll);
        }

        public void Run()
        {
            Vector3D gyroRpms = Vector3D.Zero;
            gyroRpms.X = Pitch.Run();
            gyroRpms.Y = Yaw.Run();
            gyroRpms.Z = Roll.Run();
            ApplyGyroOverride(gyroRpms);
        }

        public void Tick()
        {
            Pitch.Tick();
            Yaw.Tick();
            Roll.Tick();
        }

        public string GyroData()
        {
            string data = "Gyro Data: \n";
            foreach(var orientation in gyroSets)
            {
                data += orientation.Key.ToString() + ":  " + orientation.Value.Count;
            }
            return data;
        }
    }
}