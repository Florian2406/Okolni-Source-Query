using Okolni.Source.Query;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Okolni.Source.Example
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            IQueryConnection conn = new QueryConnection();

            while (true)
            {
                conn.Host = "185.239.211.62";
                conn.Port = 39215;

                conn.Setup();

                var info = await conn.GetInfoAsync();
                Console.WriteLine($"Server info: {info.ToString()}");
                var players = await conn.GetPlayersAsync();
                Console.WriteLine($"Current players: {string.Join("; ", players.Players.Select(p => p.Name))}");
                var rules = await conn.GetRulesAsync();
                Console.WriteLine($"Rules: {string.Join("; ", rules.Rules.Select(r => $"{r.Key}: {r.Value}"))}");
                await Task.Delay(2000);
            }
        }
    }
}
