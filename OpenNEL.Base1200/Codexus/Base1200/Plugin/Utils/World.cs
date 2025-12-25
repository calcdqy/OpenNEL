using System.Collections.Generic;
using System.Linq;

namespace Codexus.Base1200.Plugin.Utils;

public class World
{
	private List<Entity> Entities { get; } = new List<Entity>();

	public void AddEntity(Entity entity)
	{
		Entities.Add(entity);
	}

	public void RemoveEntity(Entity entity)
	{
		Entities.Remove(entity);
	}

	public void RemoveEntity(int entityId)
	{
		Entities.RemoveAll((Entity x) => x.EntityId == entityId);
	}

	public Entity? GetEntity(int entityId)
	{
		return Entities.FirstOrDefault((Entity x) => x.EntityId == entityId);
	}
}
