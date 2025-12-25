using System.Collections.Generic;

namespace Codexus.Base1200.Plugin.Utils.Patch;

public class IdBiMap<T> where T : notnull
{
	private readonly Dictionary<int, T> _idToObject = new Dictionary<int, T>();

	private readonly Dictionary<T, int> _objectToId = new Dictionary<T, int>();

	private int _nextId;

	public int Add(T obj)
	{
		if (_objectToId.TryGetValue(obj, out var value))
		{
			return value;
		}
		int num = _nextId++;
		_idToObject[num] = obj;
		_objectToId[obj] = num;
		return num;
	}

	public T GetObject(int id)
	{
		return _idToObject[id];
	}

	public int GetId(T obj)
	{
		return _objectToId[obj];
	}
}
