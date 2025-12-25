using System.Collections.Generic;
using Codexus.Base1200.Plugin.Utils.Patch.Codec;
using Codexus.Base1200.Plugin.Utils.Patch.Codec.Impls;
using DotNetty.Buffers;
using Minecraft = Codexus.Base1200.Plugin.Utils.Minecraft;

namespace Codexus.Base1200.Plugin.Utils.Patch.Metadata;

public static class EntityDataSerializers
{
	private static readonly IdBiMap<object> Serializers;

	public static readonly IEntityDataSerializer<Minecraft.Byte> Byte;

	public static readonly IEntityDataSerializer<Minecraft.Integer> Int;

	public static readonly IEntityDataSerializer<Minecraft.Long> Long;

	public static readonly IEntityDataSerializer<Minecraft.Float> Float;

	public static readonly IEntityDataSerializer<Minecraft.String> StringSerializer;

	public static readonly IEntityDataSerializer<Minecraft.Component> Component;

	public static readonly IEntityDataSerializer<Optional<Minecraft.Component>> OptionalComponent;

	public static readonly IEntityDataSerializer<Minecraft.ItemStack> ItemStack;

	public static readonly IEntityDataSerializer<Minecraft.BlockState> BlockState;

	public static readonly IEntityDataSerializer<Optional<Minecraft.BlockState>> OptionalBlockState;

	public static readonly IEntityDataSerializer<Minecraft.Boolean> Boolean;

	public static readonly IEntityDataSerializer<Minecraft.ParticleOptions> Particle;

	public static readonly IEntityDataSerializer<List<Minecraft.ParticleOptions>> Particles;

	public static readonly IEntityDataSerializer<Minecraft.Rotations> Rotations;

	public static readonly IEntityDataSerializer<Minecraft.BlockPos> BlockPos;

	public static readonly IEntityDataSerializer<Optional<Minecraft.BlockPos>> OptionalBlockPos;

	public static readonly IEntityDataSerializer<Minecraft.Direction> Direction;

	public static readonly IEntityDataSerializer<Optional<byte[]>> OptionalUuid;

	public static readonly IEntityDataSerializer<Optional<Minecraft.GlobalPos>> OptionalGlobalPos;

	public static readonly IEntityDataSerializer<Minecraft.CompoundTag> CompoundTag;

	public static readonly IEntityDataSerializer<Minecraft.VillagerData> VillagerData;

	public static readonly IEntityDataSerializer<Minecraft.OptionalInt> OptionalUnsignedInt;

	public static readonly IEntityDataSerializer<Minecraft.Pose> Pose;

	public static readonly IEntityDataSerializer<Minecraft.Holder<Minecraft.CatVariant>> CatVariant;

	public static readonly IEntityDataSerializer<Minecraft.Holder<Minecraft.WolfVariant>> WolfVariant;

	public static readonly IEntityDataSerializer<Minecraft.Holder<Minecraft.FrogVariant>> FrogVariant;

	public static readonly IEntityDataSerializer<Minecraft.Holder<Minecraft.PaintingVariant>> PaintingVariant;

	public static readonly IEntityDataSerializer<Minecraft.Armadillo.ArmadilloState> ArmadilloState;

	public static readonly IEntityDataSerializer<Minecraft.Sniffer.State> SnifferState;

	public static readonly IEntityDataSerializer<Minecraft.Vector3F> Vector3;

	public static readonly IEntityDataSerializer<Minecraft.Quaternionf> Quaternion;

	static EntityDataSerializers()
	{
		Serializers = new IdBiMap<object>();
		Byte = Register(new ByteCodec());
		Int = Register(new IntegerCodec());
		Long = Register(new LongCodec());
		Float = Register(new FloatCodec());
		StringSerializer = Register(new StringCodec());
		Component = Register(new ComponentCodec());
		OptionalComponent = Register(new OptionalComponentCodec());
		ItemStack = Register(new ItemStackCodec());
		Boolean = Register(new BooleanCodec());
		Rotations = Register(new RotationsCodec());
		BlockPos = Register(new BlockPosCodec());
		OptionalBlockPos = Register(new OptionalBlockPosCodec());
		Direction = Register(new DirectionCodec());
		OptionalUuid = Register(new OptionalUuidCodec());
		BlockState = Register(new BlockStateCodec());
		OptionalBlockState = Register(new OptionalBlockStateCodec());
		CompoundTag = Register(new CompoundTagCodec());
		Particle = Register(new ParticleCodec());
		Particles = Register(new ParticleListCodec());
		VillagerData = Register(new VillagerDataCodec());
		OptionalUnsignedInt = Register(new OptionalUnsignedIntCodec());
		Pose = Register(new PoseCodec());
		CatVariant = Register(new CatVariantCodec());
		WolfVariant = Register(new WolfVariantCodec());
		FrogVariant = Register(new FrogVariantCodec());
		OptionalGlobalPos = Register(new OptionalGlobalPosCodec());
		PaintingVariant = Register(new PaintingVariantCodec());
		SnifferState = Register(new SnifferStateCodec());
		ArmadilloState = Register(new ArmadilloStateCodec());
		Vector3 = Register(new Vector3FCodec());
		Quaternion = Register(new QuaternionCodec());
	}

	public static object GetSerializer(int i)
	{
		return Serializers.GetObject(i);
	}

	public static int GetSerializedId(object serializer)
	{
		return Serializers.GetId(serializer);
	}

	private static IEntityDataSerializer<T> Register<T>(IStreamCodec<IByteBuffer, T> codec)
	{
		IEntityDataSerializer<T> entityDataSerializer = IEntityDataSerializer<T>.ForValueType(codec);
		Serializers.Add(entityDataSerializer);
		return entityDataSerializer;
	}
}
