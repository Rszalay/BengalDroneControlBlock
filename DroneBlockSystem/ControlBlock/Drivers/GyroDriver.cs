using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using Sandbox.Game.Entities;
using VRageMath;
using DroneBlockSystem.ControlBlock.DroneBlocks;
using DroneBlockSystem.Controls;
using DroneBlockSystem.ControlBlock.Settings;

namespace DroneBlockSystem.ControlBlock.Drivers
{
    class GyroDriver
    {
        readonly DroneBlock ThisController;
        Dictionary<MyBlockOrientation, List<IMyGyro>> gyroSets;
        readonly Ideal Yaw, Pitch, Roll;
        Vector3D _tangentVector;
        Vector3D _normalVector;

        float totalGridTorque = 0;

        internal GyroDriver(List<IMySlimBlock> slimBlocks, DroneBlock thisController, DroneSettings settings)
        {
            ThisController = thisController;
            Yaw = new Ideal(settings.Yaw, ThisController);
            Pitch = new Ideal(settings.Pitch, ThisController);
            Roll = new Ideal(settings.Roll, ThisController);
            Purge(slimBlocks);
        }

        internal void UpdateOffsets(float yaw = -1, float pitch = -1, float roll = -1)
        {
            Yaw.UpdateOffset(yaw);
            Pitch.UpdateOffset(pitch);
            Roll.UpdateOffset(roll);
        }

        internal void Purge(List<IMySlimBlock> slimBlocks)
        {
            gyroSets?.Clear();
            gyroSets = new Dictionary<MyBlockOrientation, List<IMyGyro>>();
            totalGridTorque = 0;
            foreach (var block in slimBlocks)
            {
                if (block.FatBlock is IMyGyro)
                {
                    if (block == null)
                        continue;
                    IMyGyro thisGyro = block.FatBlock as IMyGyro;
                    totalGridTorque += (thisGyro as MyGyro).MaxGyroForce;
                    if (!gyroSets.ContainsKey(block.Orientation))
                    {
                        gyroSets.Add(block.Orientation, new List<IMyGyro>());
                        gyroSets[block.Orientation].Add(block.FatBlock as IMyGyro);
                    }
                    else
                        gyroSets[block.Orientation].Add(block.FatBlock as IMyGyro);
                }
            }
            float mass = (ThisController.Block.CubeGrid as MyCubeGrid).GetCurrentMass();
            float Loa = (ThisController.Block.CubeGrid.LocalAABB.Size * ThisController.Block.LocalMatrix.Forward).Length();
            float Woa = (ThisController.Block.CubeGrid.LocalAABB.Size * ThisController.Block.LocalMatrix.Left).Length();
            float Hoa = (ThisController.Block.CubeGrid.LocalAABB.Size * ThisController.Block.LocalMatrix.Up).Length();
            float MoIYaw = (mass / 12f) * (Loa * Loa + Woa * Woa);
            float MoIPitch = (mass / 12f) * (Loa * Loa + Hoa * Hoa);
            float MoIRoll = (mass / 12f) * (Woa * Woa + Hoa * Hoa);
            float LimitYaw = MoIYaw / totalGridTorque;
            float LimitPitch = MoIPitch / totalGridTorque;
            float LimitRoll = MoIRoll / totalGridTorque;
            IdealSettings newYaw = new IdealSettings(LimitYaw * 10f, (float)Math.Sqrt(LimitYaw) * .005f, (float)Math.Pow(LimitYaw, 2) * 50, 0);
            IdealSettings newPitch = new IdealSettings(LimitPitch * 10f, (float)Math.Sqrt(LimitPitch) * .005f, (float)Math.Pow(LimitPitch, 2) * 50, 0);
            IdealSettings newRoll = new IdealSettings(LimitRoll * 10f, (float)Math.Sqrt(LimitRoll) * .005f, (float)Math.Pow(LimitRoll, 2) * 50, 0);
            Yaw.UpdateGains(newYaw);
            Pitch.UpdateGains(newPitch);
            Roll.UpdateGains(newRoll);
        }

        internal void ApplyGyroOverride(Vector3D gyroRpms)
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

        internal void Load(Vector3D tangentVector, Vector3D nearNormalVector)
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

            Yaw.Load(YawError);
            Pitch.Load(pitchError);
            Roll.Load(rollError);
        }

        internal void Run()
        {
            Vector3D gyroRpms = Vector3D.Zero;
            gyroRpms.X = Pitch.Run();
            gyroRpms.Y = Yaw.Run();
            gyroRpms.Z = Roll.Run();
            ApplyGyroOverride(gyroRpms);
        }

        internal void UpdateGains(DroneSettings droneSettings)
        {
            Yaw.UpdateGains(droneSettings.Yaw);
            Pitch.UpdateGains(droneSettings.Pitch);
            Roll.UpdateGains(droneSettings.Roll);
        }

        internal void Tick()
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