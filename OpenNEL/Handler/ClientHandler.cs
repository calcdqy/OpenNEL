using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Codexus.Cipher.Protocol;
using OpenNEL;
using OpenNEL.Entities;
using OpenNEL.Extensions;
using OpenNEL.HandleWebSocket.Connection;
using OpenNEL.Type;
using Serilog;

namespace OpenNEL.Handler;

public class ClientHandler : IDisposable
{
	public static readonly Com4399 Com4399 = new Com4399();

	public static readonly G79 G79 = new G79();

	public static readonly WPFLauncher X19 = new WPFLauncher();

	private readonly ConcurrentDictionary<Guid, SecureConnection> _connections = new ConcurrentDictionary<Guid, SecureConnection>();
	
	public void Dispose()
	{
		X19.Dispose();
		GC.SuppressFinalize(this);
	}
}
