using System.Diagnostics.CodeAnalysis;
using Codexus.Base1200.Plugin.Utils.Patch.Codec;
using OpenNEL.SDK.Extensions;
using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Metadata;

public class DataValue<TValue>(int id, IEntityDataSerializer<TValue> serializer, TValue value) : IDataValue
{
	public int Id { get; } = id;

	public IEntityDataSerializer<TValue> Serializer { get; } = serializer;

	public TValue Value
	{
		[return: NotNull]
		get;
	} = value;

	public static IDataValue Read(IByteBuffer buffer, int id)
	{
		dynamic serializer = EntityDataSerializers.GetSerializer(buffer.ReadVarIntFromBuffer());
		dynamic val = serializer.Codec();
		dynamic val2 = val.Decode(buffer);
		return DataValue<TValue>.CreateDataValue(id, serializer, val2);
	}

	private static DataValue<T> CreateDataValue<T>(int id, IEntityDataSerializer<T> serializer, T value)
	{
		return new DataValue<T>(id, serializer, value);
	}

	public void Write(IByteBuffer buffer)
	{
		int serializedId = EntityDataSerializers.GetSerializedId(Serializer);
		buffer.WriteByte((int)(byte)Id);
		buffer.WriteVarInt(serializedId);
		Serializer.Codec().Encode(buffer, Value);
	}
}
