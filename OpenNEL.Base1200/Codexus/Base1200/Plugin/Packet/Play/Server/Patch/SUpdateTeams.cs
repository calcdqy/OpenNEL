using System.Collections.Generic;
using Codexus.Base1200.Plugin.Extensions;
using Codexus.Base1200.Plugin.Utils.Patch;
using OpenNEL.SDK.Connection;
using OpenNEL.SDK.Enums;
using OpenNEL.SDK.Extensions;
using OpenNEL.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Base1200.Plugin.Packet.Play.Server.Patch;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 96, new EnumProtocolVersion[] { EnumProtocolVersion.V1206 }, false)]
public class SUpdateTeams : IPacket
{
	private string TeamName { get; set; } = string.Empty;

	private byte Method { get; set; }

	private byte[]? RemainingBytes { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		TeamName = buffer.ReadStringFromBuffer(32767);
		Method = buffer.ReadByte();
		RemainingBytes = buffer.ReadByteArrayReadableBytes();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteStringToBuffer(TeamName);
		buffer.WriteByte((int)Method);
		buffer.WriteBytes(RemainingBytes);
	}

	public bool HandlePacket(GameConnection connection)
	{
		Teams teams = connection.GetTeams();
		return Method switch
		{
			0 => HandleCreateTeam(teams), 
			1 => HandleRemoveTeam(teams), 
			3 => UpdateTeamEntities(teams, add: true, connection), 
			4 => UpdateTeamEntities(teams, add: false, connection), 
			_ => false, 
		};
	}

	private bool HandleCreateTeam(Teams teams)
	{
		if (teams.ContainsTeam(TeamName))
		{
			Log.Debug<string>("Team {TeamName} already exists", TeamName);
			return true;
		}
		Log.Debug<string>("Creating team {TeamName}", TeamName);
		teams.CreateTeam(TeamName);
		return false;
	}

	private bool HandleRemoveTeam(Teams teams)
	{
		if (!teams.ContainsTeam(TeamName))
		{
			Log.Debug<string>("Team {TeamName} does not exist", TeamName);
			return true;
		}
		Log.Debug<string>("Removing team {TeamName}", TeamName);
		teams.RemoveTeam(TeamName);
		return false;
	}

	private bool UpdateTeamEntities(Teams teams, bool add, GameConnection connection)
	{
		if (!teams.ContainsTeam(TeamName) || RemainingBytes == null)
		{
			Log.Debug<string>("Team {TeamName} does not exist", TeamName);
			return true;
		}
		List<string> list = new List<string>();
		IByteBuffer buffer = Unpooled.WrappedBuffer(RemainingBytes);
		int num = buffer.ReadVarIntFromBuffer();
		for (int i = 0; i < num; i++)
		{
			string text = buffer.ReadStringFromBuffer(32767);
			if (add)
			{
				string text2 = teams.FindPlayerTeam(text);
				if (text2 != null && text2 != TeamName)
				{
					Log.Debug<string, string, string>("Player {EntityId} is already in team {CurrentTeam}, will transfer to {NewTeam}", text, text2, TeamName);
					teams.RemovePlayerFromTeam(text2, text);
				}
				bool num2 = teams.AddPlayerToTeam(TeamName, text);
				Log.Debug<string, string>("{EntityId} added to team {TeamName}", text, TeamName);
				if (num2)
				{
					list.Add(text);
				}
			}
			else
			{
				bool num3 = teams.RemovePlayerFromTeam(TeamName, text);
				Log.Debug<string, string>("{EntityId} removed from team {TeamName}", text, TeamName);
				if (num3)
				{
					list.Add(text);
				}
			}
		}
		IByteBuffer buffer2 = Unpooled.Buffer();
		buffer2.WriteVarInt(list.Count);
		foreach (string item in list)
		{
			buffer2.WriteStringToBuffer(item);
		}
		RemainingBytes = buffer2.ReadByteArrayReadableBytes();
		return false;
	}
}
