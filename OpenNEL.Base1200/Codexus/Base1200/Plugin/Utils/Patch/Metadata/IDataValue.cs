using DotNetty.Buffers;

namespace Codexus.Base1200.Plugin.Utils.Patch.Metadata;

public interface IDataValue
{
	int Id { get; }

	void Write(IByteBuffer buffer);
}
