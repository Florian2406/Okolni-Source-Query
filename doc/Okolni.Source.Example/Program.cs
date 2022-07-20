using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Okolni.Source.Query;
using Okolni.Source.Query.Source;

namespace Okolni.Source.Example;

public class Program
{
    private static async Task Main(string[] args)
    {
        string Host = "185.239.211.62";
        int Port = 39215;
        using IQueryConnection conn = new QueryConnection();
        conn.Host = "185.239.211.62";
        conn.Port = 39215;

        conn.Setup();

        var info = await conn.GetInfoAsync();
        Console.WriteLine($"Server info: {info}");
        var players = await conn.GetPlayersAsync();
        Console.WriteLine($"Current players: {string.Join("; ", players.Players.Select(p => p.Name))}");
        var rules = await conn.GetRulesAsync();
        Console.WriteLine($"Rules: {string.Join("; ", rules.Rules.Select(r => $"{r.Key}: {r.Value}"))}");




        using IQueryConnectionPool connPool = new QueryConnectionPool();
        connPool.ReceiveTimeout = 5000;
        connPool.SendTimeout = 5000;
        var serverEndpoint1 = new IPEndPoint(IPAddress.Parse(Host), Port);
        var serverEndpoint2 = new IPEndPoint(IPAddress.Parse("176.57.181.146"), 28915);
        var serverEndpoint3 = new IPEndPoint(IPAddress.Parse("23.109.144.148"), 28215);
        var serverEndpoint4 = new IPEndPoint(IPAddress.Parse("147.135.30.212"), 5746);
        var serverEndpoint5 = new IPEndPoint(IPAddress.Parse("176.57.140.69"), 28915);

        var infoTask1 = connPool.GetInfoAsync(serverEndpoint1);
        var infoTask2 = connPool.GetInfoAsync(serverEndpoint2);
        var infoTask3 = connPool.GetInfoAsync(serverEndpoint3);
        var infoTask4 = connPool.GetInfoAsync(serverEndpoint4);
        var infoTask5 = connPool.GetInfoAsync(serverEndpoint5);
        await Task.WhenAll(infoTask1, infoTask2, infoTask3, infoTask4, infoTask5);
        var (info1, info2, info3, info4, info5) = (infoTask1.Result, infoTask2.Result, infoTask3.Result, infoTask4.Result, infoTask5.Result);
        Console.WriteLine($"Server info: {info1}");
        Console.WriteLine($"Server info: {info2}");
        Console.WriteLine($"Server info: {info3}");
        Console.WriteLine($"Server info: {info4}");
        Console.WriteLine($"Server info: {info5}");
    }
}