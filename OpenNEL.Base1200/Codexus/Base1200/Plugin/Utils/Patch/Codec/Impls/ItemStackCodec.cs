using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils.Minecraft;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;

public class ItemStackCodec : IStreamCodec<IByteBuffer, ItemStack>
{
	public ItemStack Decode(IByteBuffer buffer)
	{
		int num = buffer.ReadVarIntFromBuffer();
		if (num <= 0)
		{
			return ItemStack.Empty;
		}
		int itemId = buffer.ReadVarIntFromBuffer();
		int num2 = buffer.ItemStackLength();
		if (num2 <= -1)
		{
			return new ItemStack
			{
				Count = num,
				ItemId = itemId
			};
		}
		byte[] array = new byte[num2];
		buffer.ReadBytes(array);
		return new ItemStack
		{
			Count = num,
			ItemId = itemId,
			RawData = array
		};
	}

	public void Encode(IByteBuffer buffer, ItemStack value)
	{
		if (value.IsEmpty)
		{
			buffer.WriteVarInt(0);
			return;
		}
		buffer.WriteVarInt(value.Count);
		buffer.WriteVarInt(value.ItemId);
		if (value.RawData == null)
		{
			buffer.WriteVarInt(0);
			buffer.WriteVarInt(0);
		}
		else
		{
			buffer.WriteBytes(value.RawData);
		}
	}
}
