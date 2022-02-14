using ProtoBuf;
using System.Collections.Generic;

namespace DroneBlockSystem.NetworkProtobuf
{
    // An example packet extending another packet.
    // Note that it must be ProtoIncluded in PacketBase for it to work.
    [ProtoContract]
    public class EchoUpdatePacket : PacketBase
    {
        // tag numbers in this class won't collide with tag numbers from the base class
        [ProtoMember(1)]
        internal long Block;

        [ProtoMember(2)]
        internal string EchoString;

        internal EchoUpdatePacket() { } // Empty constructor required for deserialization

        internal EchoUpdatePacket(string echoString, long block)
        {
            EchoString = echoString;
            Block = block;
        }

        public override bool Received()
        {
            var Sess = Session.Session.Instance;
            if (Sess.AllDroneBlocks.ContainsKey(Block))
            {
                Sess.AllDroneBlocks[Block].EchoStrings.Clear();
                Sess.AllDroneBlocks[Block].EchoStrings = new List<string>();
                Sess.AllDroneBlocks[Block].Echo(EchoString);
            }
            return false; // relay packet to other clients (only works if server receives it)
        }
    }
}