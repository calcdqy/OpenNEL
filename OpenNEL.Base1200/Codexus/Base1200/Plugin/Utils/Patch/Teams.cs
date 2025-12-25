using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Serilog;

namespace Codexus.Base1200.Plugin.Utils.Patch;

public class Teams
{
	private readonly ConcurrentDictionary<string, HashSet<string>> _teamMembers = new ConcurrentDictionary<string, HashSet<string>>();

	private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

	public void CreateTeam(string teamName)
	{
		_teamMembers[teamName] = new HashSet<string>();
	}

	public void RemoveTeam(string teamName)
	{
		_teamMembers.Remove<string, HashSet<string>>(teamName, out var _);
	}

	public bool ContainsTeam(string teamName)
	{
		return _teamMembers.ContainsKey(teamName);
	}

	public bool AddPlayerToTeam(string teamName, string playerName)
	{
		_lock.EnterWriteLock();
		try
		{
			return _teamMembers[teamName].Add(playerName);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public bool RemovePlayerFromTeam(string teamName, string playerName)
	{
		_lock.EnterWriteLock();
		try
		{
			HashSet<string> value;
			return _teamMembers.TryGetValue(teamName, out value) && value.Remove(playerName);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public string? FindPlayerTeam(string playerName)
	{
		_lock.EnterReadLock();
		try
		{
			foreach (var (result, hashSet2) in _teamMembers)
			{
				if (hashSet2.Contains(playerName))
				{
					return result;
				}
			}
			return null;
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public void Clear()
	{
		Log.Debug("Clearing teams...");
		_teamMembers.Clear();
	}
}
