using Okolni.Source.Query;
using Okolni.Source.Common;
using System;
using System.Linq;

namespace Okolni.Source.Example
{
    public class Program
    {
        static void Main(string[] args)
        {
            IQueryConnection conn = new QueryConnection();

            // Exampleserver: ARK VALGUERO Server
            conn.Host = "127.0.0.1";
            conn.Port = 27015;

            conn.Connect();

            var info = conn.GetInfo();
            var players = conn.GetPlayers();
            var rules = conn.GetRules();

            //conn.Disconnect();
            Console.WriteLine($"Server info: {info.ToString()}");
            Console.WriteLine($"Current players: {string.Join("; ", players.Players.Select(p => p.Name))}");
            Console.WriteLine($"Rules: {string.Join("; ", rules.Rules.Select(r => $"{r.Key}: {r.Value}"))}");
        }
    }
}
