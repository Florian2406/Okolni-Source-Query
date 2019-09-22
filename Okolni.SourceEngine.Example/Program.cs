using Okolni.SourceEngine.Query;
using Okolni.SourceEngine.Common;
using System;

namespace Okolni.SourceEngine.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            IQueryConnection conn = new QueryConnection();

            // VALGUERO
            conn.Host = "89.163.146.91";
            conn.Port = 27023;

            // THE SHIP
            //conn.Host = "178.251.24.141";
            //conn.Port = 27017;

            // CS:GO Multi
            //conn.Host = "94.250.219.40";
            //conn.Port = 27015;
            conn.Connect();
            var info = conn.GetInfo();
            var players = conn.GetPlayers();
            var rules = conn.GetRules();
            //conn.Disconnect();
            Console.WriteLine(info.ToString());
        }
    }
}
