using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Utils;

namespace BengalDroneControlBlock.NetworkProtobuf
{
    // An example packet extending another packet.
    // Note that it must be ProtoIncluded in PacketBase for it to work.
    [ProtoContract]
    public class ControlUpdatePacket : PacketBase
    {
        // tag numbers in this class won't collide with tag numbers from the base class
        [ProtoMember(1)]
        public float Value;

        [ProtoMember(2)]
        public long Block;

        [ProtoMember(3)]
        public string Name;

        public ControlUpdatePacket() { } // Empty constructor required for deserialization

        public ControlUpdatePacket(string name, float value, long block)
        {
            Name = name;
            Block = block;
            Value = value;
        }

        public override bool Received()
        {
            var Sess = Session.Session.Instance;
            if (Sess.DroneBlocks.ContainsKey(Block))
            {
                if (Name == "YawSensitivity")
                    Sess.terminalProperties.YawSensitivity.NoEchoSet(Sess.DroneBlocks[Block].Block, Value);
                else if (Name == "PitchSensitivity")
                    Sess.terminalProperties.PitchSensitivity.NoEchoSet(Sess.DroneBlocks[Block].Block, Value);
                else if (Name == "RollSensitivity")
                    Sess.terminalProperties.RollSensitivity.NoEchoSet(Sess.DroneBlocks[Block].Block, Value);
            }
            //Sess.terminalProperties.ThrustSensitivity.Set(Sess.DroneBlocks[Block].Block, Value[3]);
            return true; // relay packet to other clients (only works if server receives it)
        }
    }
}