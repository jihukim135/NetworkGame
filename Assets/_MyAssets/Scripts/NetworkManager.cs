using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public struct Packet
{
    public const int Size = 24;
    public long Timestamp;
    public int PacketType; // 1: 틱, 2: 이동, 3: 발사, 4: 점프, 5: 게임오버
    public int PlayerX;
    public int DestX;
    public int Uuid;
}

public class NetworkManager : Singleton<NetworkManager>
{
    private TcpClient _server;
    private NetworkStream _netStream;

    [SerializeField] private string address;
    [SerializeField] private int port;

    private Queue<Packet> _packets;
    private Dictionary<int, Car> _players;

    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2Int initPosition;
    [SerializeField] private int localUuid;

    private bool _isTicked;

    private void Awake()
    {
        _packets = new Queue<Packet>();
        _players = new Dictionary<int, Car>();
    }

    private void Start()
    {
        try
        {
            _server = new TcpClient(address, port);
            _server.LingerState = new LingerOption(true, 0);

            _netStream = _server.GetStream();
            Debug.Log("connected to server");

            new Thread(ReceiveLoop).Start();
        }
        catch (SocketException)
        {
            Debug.Log("Unable to connect to server");
        }
    }

    private void OnApplicationQuit()
    {
        if (_netStream == null)
        {
            return;
        }
        
        _netStream.Close();
        _server.Close();
    }

    private void Update()
    {
        if (!_isTicked)
        {
            return;
        }
        
        RunQueue();
        _isTicked = false;
    }

    public void SpawnLocalPlayer()
    {
        localUuid = Random.Range(int.MinValue, int.MaxValue);

        GameObject instance = Instantiate(localPlayerPrefab);
        instance.GetComponent<FixedPosTransform>().position = initPosition;
        Car newPlayer = instance.GetComponent<Car>();
        _players.Add(localUuid, newPlayer);

        SendDatagram(2, initPosition, initPosition);
    }

    public void ReceiveLoop()
    {
        while (true)
        {
            byte[] data = new byte[Packet.Size];
            _netStream.Read(data, 0, data.Length);

            Packet p = new Packet
            {
                Timestamp = System.BitConverter.ToInt64(data, 0),
                PacketType = System.BitConverter.ToInt32(data, 8),
                PlayerX = System.BitConverter.ToInt32(data, 12),
                DestX = System.BitConverter.ToInt32(data, 16),
                Uuid = System.BitConverter.ToInt32(data, 20)
            };

            if (p.PacketType == 1)
            {
                _isTicked = true;
            }
            else
            {
                _packets.Enqueue(p);
            }
        }
    }

    public void SendDatagram(int type, Vector2Int player, Vector2Int dest)
    {
        byte[] dgram = new byte[Packet.Size];

        System.BitConverter.GetBytes(UnixTimeNow()).CopyTo(dgram, 0);
        System.BitConverter.GetBytes(type).CopyTo(dgram, 8);
        System.BitConverter.GetBytes(player.x).CopyTo(dgram, 12);
        System.BitConverter.GetBytes(dest.x).CopyTo(dgram, 20);
        System.BitConverter.GetBytes(localUuid).CopyTo(dgram, 28);

        _netStream.Write(dgram, 0, 32);
        _netStream.Flush();
    }

    private void RunQueue()
    {
        while (_packets.Count > 0)
        {
            Packet p = _packets.Dequeue();
            Vector2Int pos = new Vector2Int(p.PlayerX, initPosition.y);
            Vector2Int dest = new Vector2Int(p.DestX, initPosition.y);

            if (!_players.ContainsKey(p.Uuid))
            {
                GameObject instance = Instantiate(playerPrefab);
                instance.GetComponent<FixedPosTransform>().position = initPosition;
                Car newPlayer = instance.GetComponent<Car>();
                _players.Add(p.Uuid, newPlayer);
            }

            Car player = _players[p.Uuid];

            // if (p.PacketType == 2)
            // {
            //     player.Move(pos, dest);
            // }
            // else if (p.PacketType == 3)
            // {
            //     player.Shoot(pos, dest);
            // }
            // else if (p.PacketType == 4)
            // {
            //     player.Jump(pos, dest);
            // }
            // else if (p.PacketType == 5)
            // {
            //     _players.Remove(p.Uuid);
            // }
        }
    }

    private static long UnixTimeNow()
    {
        var timeSpan = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
        return timeSpan.Ticks;
    }
}