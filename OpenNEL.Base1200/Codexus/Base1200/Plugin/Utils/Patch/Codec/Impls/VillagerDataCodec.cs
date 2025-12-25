using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class VillagerDataCodec : IStreamCodec<IByteBuffer, VillagerData>
{
	public VillagerData Decode(IByteBuffer buffer)
	{
		int villagerType = buffer.ReadVarIntFromBuffer();
		int villagerProfession = buffer.ReadVarIntFromBuffer();
		int level = buffer.ReadVarIntFromBuffer();
		return new VillagerData(villagerType, villagerProfession, level);
	}

	public void Encode(IByteBuffer buffer, VillagerData value)
	{
		buffer.WriteVarInt(value.VillagerType);
		buffer.WriteVarInt(value.VillagerProfession);
		buffer.WriteVarInt(value.Level);
	}
}
