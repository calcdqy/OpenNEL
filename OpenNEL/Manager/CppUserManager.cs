using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Codexus.Cipher.Entities.G79;
using Codexus.Cipher.Protocol;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Manager;
using OpenNEL.Entities.Web;
using OpenNEL.Enums;
using Serilog;

namespace OpenNEL.Manager;

public class CppUserManager : IUserManager
{
	private const string UsersFilePath = "cppusers.json";

	private static readonly Lock Lock = new Lock();

	private static readonly Lock MaintainLock = new Lock();

	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		WriteIndented = true
	};

	private static CppUserManager? _instance;

	private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

	public static CppUserManager Instance
	{
		get
		{
			using (Lock.EnterScope())
			{
				return _instance ?? (_instance = new CppUserManager());
			}
		}
	}

	private List<EntityUser> Users { get; set; } = new List<EntityUser>();

	private List<EntityAvailableUser> AvailableUsers { get; } = new List<EntityAvailableUser>();

	private CppUserManager()
	{
		IUserManager.CppInstance = this;
	}

	public EntityAvailableUser? GetAvailableUser(string entityId)
	{
		return AvailableUsers.FirstOrDefault((EntityAvailableUser u) => u.UserId == entityId);
	}

	private async Task MaintainThread()
	{
		using (new G79())
		{
			_ = 1;
			try
			{
				while (!_cancellationTokenSource.Token.IsCancellationRequested)
				{
					try
					{
						long expirationThreshold = DateTimeOffset.UtcNow.AddMinutes(-30.0).ToUnixTimeMilliseconds();
						using (MaintainLock.EnterScope())
						{
							foreach (EntityAvailableUser item in AvailableUsers.Where((EntityAvailableUser u) => u.LastLoginTime < expirationThreshold).ToList())
							{
								_ = item;
							}
						}
						await Task.Delay(2000, _cancellationTokenSource.Token);
					}
					catch (Exception exception)
					{
						Log.Error(exception, "Error in MaintainThread");
						await Task.Delay(2000);
					}
				}
			}
			catch (OperationCanceledException)
			{
				Log.Logger.Information("Maintain thread cancelled");
			}
		}
	}

	public List<string> GetAvailableUsers()
	{
		using (MaintainLock.EnterScope())
		{
			return AvailableUsers.Select((EntityAvailableUser user) => user.UserId).ToList();
		}
	}

	public List<EntityUser> GetUsersNoDetails()
	{
		using (Lock.EnterScope())
		{
			return Users.Select((EntityUser u) => new EntityUser
			{
				UserId = u.UserId,
				Authorized = u.Authorized,
				AutoLogin = false,
				Channel = u.Channel,
				Type = u.Type,
				Details = "",
				Platform = u.Platform,
				Alias = u.Alias
			}).ToList();
		}
	}

	public EntityUser? GetUserByEntityId(string entityId)
	{
		return Users.FirstOrDefault((EntityUser u) => u.UserId == entityId);
	}

	public EntityAvailableUser? GetLastAvailableUser()
	{
		return AvailableUsers.LastOrDefault();
	}

	public void AddUserToMaintain(EntityAuthenticationOtp user)
	{
		AvailableUsers.Add(new EntityAvailableUser
		{
			UserId = user.EntityId,
			AccessToken = user.Token,
			LastLoginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
		});
	}

	public void AddUser(EntityUser entityUser, bool saveToDisk = true)
	{
		using (Lock.EnterScope())
		{
			ArgumentNullException.ThrowIfNull(entityUser, "entityUser");
			EntityUser entityUser2 = Users.FirstOrDefault((EntityUser u) => u.UserId == entityUser.UserId);
			if (entityUser2 != null)
			{
				entityUser2.Authorized = true;
				return;
			}
			entityUser.Platform = Platform.Mobile;
			Users.Add(entityUser);
			if (saveToDisk)
			{
				SaveUsersToDisk();
			}
		}
	}

	public void RemoveUser(string entityId)
	{
		using (Lock.EnterScope())
		{
			EntityUser entityUser = Users.FirstOrDefault((EntityUser u) => u.UserId == entityId);
			if (entityUser != null)
			{
				Users.Remove(entityUser);
				SaveUsersToDisk();
			}
		}
	}

	public void RemoveAvailableUser(string entityId)
	{
		using (MaintainLock.EnterScope())
		{
			EntityAvailableUser entityAvailableUser = AvailableUsers.FirstOrDefault((EntityAvailableUser u) => u.UserId == entityId);
			if (entityAvailableUser == null)
			{
				return;
			}
			AvailableUsers.Remove(entityAvailableUser);
		}
		using (Lock.EnterScope())
		{
			EntityUser entityUser = Users.FirstOrDefault((EntityUser u) => u.UserId == entityId);
			if (entityUser != null)
			{
				entityUser.Authorized = false;
				SaveUsersToDisk();
			}
		}
	}

	public void ReadUsersFromDisk()
	{
		using (Lock.EnterScope())
		{
			try
			{
				if (File.Exists("cppusers.json"))
				{
					string json = File.ReadAllText("cppusers.json");
					Users = JsonSerializer.Deserialize<List<EntityUser>>(json) ?? new List<EntityUser>();
					Users.ForEach(delegate(EntityUser user)
					{
						user.Authorized = false;
					});
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error reading users from disk: {Message}", ex.Message);
				Users = new List<EntityUser>();
			}
		}
	}

	public void SaveUsersToDisk()
	{
		try
		{
			string contents = JsonSerializer.Serialize(Users, JsonOptions);
			File.WriteAllText("cppusers.json", contents);
		}
		catch (Exception ex)
		{
			Log.Error("Error saving users to disk: {Message}", ex.Message);
			throw;
		}
	}
}
