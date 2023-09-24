using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using SharpEngine.Network.Internal;

namespace SharpEngine.Network;

/// <summary>
/// SharpEngine Network Client
/// </summary>
public class Client
{
    /// <summary>
    /// Delegate for PacketReceived Event
    /// </summary>
    public delegate void ReceivePacket(dynamic packet);

    /// <summary>
    /// Delegate for PeerConnected Event
    /// </summary>
    public delegate void Connected();

    /// <summary>
    /// Delegate for ErrorReceived Event
    /// </summary>
    public delegate void NetworkError(IPEndPoint endPoint, SocketError error);

    /// <summary>
    /// Delegate for PeerDisconnected Event
    /// </summary>
    public delegate void Disconnected(NetPeer peer, DisconnectInfo info);

    /// <summary>
    /// List of all packets unknown by Client
    /// </summary>
    public List<Type> PacketTypes { get; } = new();

    /// <summary>
    /// If Client is Running
    /// </summary>
    public bool IsRunning { get; set; }

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

    private readonly EventBasedNetListener _listener = new();
    private readonly NetManager _client;
    private readonly NetPeer _server;

    /// <summary>
    /// Create Client
    /// </summary>
    /// <param name="ip">Ip</param>
    /// <param name="port">Port</param>
    /// <param name="key">Connection Key</param>
    public Client(string ip, int port, string key = "")
    {
        _client = new NetManager(_listener);
        _client.Start();
        _server = _client.Connect(ip, port, key);

        _listener.NetworkReceiveEvent += ReceiveEvent;
        _listener.PeerConnectedEvent += _ => PeerConnected?.Invoke();
        _listener.NetworkErrorEvent += (endPoint, error) => ErrorReceived?.Invoke(endPoint, error);
        _listener.PeerDisconnectedEvent += (peer, info) => PeerDisconnected?.Invoke(peer, info);

        IsRunning = true;
    }

    /// <summary>
    /// Send Packet to Server
    /// </summary>
    /// <param name="packet">Packet</param>
    /// <typeparam name="T">Type of Packet</typeparam>
    /// <exception cref="UnknownPacketException">Exception thrown when packet is unknown</exception>
    public void SendPacket<T>(T packet)
        where T : notnull
    {
        if (!PacketTypes.Contains(packet.GetType()))
            throw new UnknownPacketException($"Unknown Packet : {packet.GetType()}");
        Common.SendPacket(_server, packet);
    }

    /// <summary>
    /// Update Client
    /// </summary>
    public void Update() => _client.PollEvents();

    /// <summary>
    /// Shutdown Client
    /// </summary>
    public void Shutdown()
    {
        _client.Stop();
        IsRunning = false;
    }

    private void ReceiveEvent(
        NetPeer peer,
        NetPacketReader reader,
        byte channel,
        DeliveryMethod deliveryMethod
    )
    {
        var packetType = reader.GetString();
        foreach (var type in PacketTypes)
        {
            if (packetType == type.Name)
            {
                PacketReceived?.Invoke(Common.ReadPacket(reader, packetType, type));
                break;
            }
        }
    }
}
