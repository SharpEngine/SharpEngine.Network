using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using SharpEngine.Network.Internal;

namespace SharpEngine.Network;

/// <summary>
/// SharpEngine Network Server
/// </summary>
public class Server
{
    /// <summary>
    /// Delegate for PacketReceived Event
    /// </summary>
    public delegate void ReceivePacket(NetPeer peer, dynamic packet);

    /// <summary>
    /// Delegate for PeerConnected Event
    /// </summary>
    public delegate void Connected(NetPeer peer);

    /// <summary>
    /// Delegate for PeerDisconnected Event
    /// </summary>
    public delegate void Disconnected(NetPeer peer, DisconnectInfo info);

    /// <summary>
    /// Delegate for Update Event
    /// </summary>
    public delegate void UpdateHandler();

    /// <summary>
    /// Delegate for ErrorReceived Event
    /// </summary>
    public delegate void NetworkError(IPEndPoint endPoint, SocketError error);

    /// <summary>
    /// List of all packets unknown by Server
    /// </summary>
    public List<Type> PacketTypes { get; } = [];

    /// <summary>
    /// Event when packet is received
    /// </summary>
    public event ReceivePacket? PacketReceived;

    /// <summary>
    /// Event when client is connected
    /// </summary>
    public event Connected? PeerConnected;

    /// <summary>
    /// Event when client is disconnected
    /// </summary>
    public event Disconnected? PeerDisconnected;

    /// <summary>
    /// Event when error is received
    /// </summary>
    public event NetworkError? ErrorReceived;

    /// <summary>
    /// Event when update is made
    /// </summary>
    public event UpdateHandler? Update;

    private readonly EventBasedNetListener _listener = new();
    private readonly NetManager _server;

    /// <summary>
    /// Create Server
    /// </summary>
    /// <param name="port">Port</param>
    /// <param name="key">Connection Key</param>
    public Server(int port, string key = "")
    {
        _server = new NetManager(_listener);
        _server.Start(port);

        _listener.ConnectionRequestEvent += request => request.AcceptIfKey(key);
        _listener.PeerConnectedEvent += peer => PeerConnected?.Invoke(peer);
        _listener.PeerDisconnectedEvent += (peer, info) => PeerDisconnected?.Invoke(peer, info);
        _listener.NetworkErrorEvent += (endPoint, error) => ErrorReceived?.Invoke(endPoint, error);
        _listener.NetworkReceiveEvent += NetworkReceive;
    }

    /// <summary>
    /// Start Server
    /// </summary>
    public void Start()
    {
        while (!Console.KeyAvailable)
        {
            _server.PollEvents();
            Update?.Invoke();
            Thread.Sleep(15);
        }
        _server.Stop();
    }

    /// <summary>
    /// Send Packet to All Clients
    /// </summary>
    /// <param name="packet">Packet</param>
    /// <typeparam name="T">Type of Packet</typeparam>
    /// <exception cref="UnknownPacketException">Exception thrown when packet is unknown</exception>
    public void BroadcastPacket<T>(T packet)
        where T : notnull
    {
        foreach (var peer in _server.ConnectedPeerList)
            SendPacket(packet, peer);
    }

    /// <summary>
    /// Send Packet to Client
    /// </summary>
    /// <param name="packet">Packet</param>
    /// <param name="peer">Client</param>
    /// <typeparam name="T">Type of Packet</typeparam>
    /// <exception cref="UnknownPacketException">Exception thrown when packet is unknown</exception>
    public void SendPacket<T>(T packet, NetPeer peer)
        where T : notnull
    {
        if (!PacketTypes.Contains(packet.GetType()))
            throw new UnknownPacketException($"Unknown Packet : {packet.GetType()}");
        Common.SendPacket(peer, packet);
    }

    private void NetworkReceive(
        NetPeer peer,
        NetPacketReader reader,
        byte channel,
        DeliveryMethod deliverymethod
    )
    {
        var packetType = reader.GetString();
        foreach (var type in PacketTypes)
        {
            if (packetType == type.Name)
            {
                PacketReceived?.Invoke(peer, Common.ReadPacket(reader, packetType, type));
                break;
            }
        }
    }
}
