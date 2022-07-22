using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Okolni.Source.Query;
using Okolni.Source.Common;
using Okolni.Source.Query.Source;

namespace Okolni.Source.Query.Test
{
    [TestClass]
    
    public class FunctionalTests
    {
        [TestMethod]
        [DataRow("64.44.28.18", 28016)]
        [DataRow("45.235.99.86", 9702)]
        [DataRow("131.196.197.89", 9877)]
        [DataRow("176.57.181.146", 28915)]
        [DataRow("23.109.144.148", 28215)]
        [DataRow("147.135.30.212", 5746)]
        [DataRow("176.57.140.69", 28915)]
        [DataRow("185.239.211.116", 32915)]
        [DataRow("176.57.173.121", 31715)]
        [DataRow("185.189.255.36", 27802)]
        [DataRow("193.164.16.98", 27822)]

        public void QueryTestSync(string Host, int Port)
        {
            using IQueryConnection conn = new QueryConnection();

            conn.Host = Host;
            conn.Port = Port;
            conn.ReceiveTimeout = 500;
            conn.SendTimeout = 500;

            conn.Setup();
            var players = conn.GetPlayers();
            var rules = conn.GetRules();
            var info = conn.GetInfo();
            Assert.IsNotNull(players);
            Assert.IsNotNull(rules);
            Assert.IsNotNull(info);

        }

        [TestMethod]
        [DataRow("64.44.28.18", 28016)]

        [DataRow("45.235.99.86", 9702)]
        [DataRow("131.196.197.89", 9877)]
        [DataRow("176.57.181.146", 28915)]
        [DataRow("23.109.144.148", 28215)]
        [DataRow("147.135.30.212", 5746)]
        [DataRow("176.57.140.69", 28915)]
        [DataRow("185.239.211.116", 32915)]
        [DataRow("176.57.173.121", 31715)]
        [DataRow("185.189.255.36", 27802)]
        [DataRow("193.164.16.98", 27822)]
        public async Task QueryTestASync(string Host, int Port)
        {
            using IQueryConnection conn = new QueryConnection();

            conn.Host = Host;
            conn.Port = Port;
            conn.ReceiveTimeout = 500;
            conn.SendTimeout = 500;

            conn.Setup();
            var players = await conn.GetPlayersAsync();
            var rules = await conn.GetRulesAsync();
            var info = await conn.GetInfoAsync();
            Assert.IsNotNull(players);
            Assert.IsNotNull(rules);
            Assert.IsNotNull(info);
        }
        [TestMethod]
        public async Task QueryTestPool()
        {
            using IQueryConnectionPool connPool = new QueryConnectionPool();
            connPool.ReceiveTimeout = 5000;
            connPool.SendTimeout = 5000;
            var serverEndpoint1 = new IPEndPoint(IPAddress.Parse("185.239.211.62"), 39215);
            var serverEndpoint2 = new IPEndPoint(IPAddress.Parse("176.57.181.146"), 28915);
            var serverEndpoint3 = new IPEndPoint(IPAddress.Parse("23.109.144.148"), 28215);
            var serverEndpoint4 = new IPEndPoint(IPAddress.Parse("147.135.30.212"), 5746);
            var serverEndpoint5 = new IPEndPoint(IPAddress.Parse("176.57.140.69"), 28915);


            var infotaskTest = await connPool.GetInfoAsync(serverEndpoint1);


            var infoTask1 = connPool.GetInfoAsync(serverEndpoint1);
            var infoTask2 = connPool.GetInfoAsync(serverEndpoint2);
            var infoTask3 = connPool.GetInfoAsync(serverEndpoint3);
            var infoTask4 = connPool.GetInfoAsync(serverEndpoint4);
            var infoTask5 = connPool.GetInfoAsync(serverEndpoint5);
            await Task.WhenAll(infoTask1, infoTask2, infoTask3, infoTask4, infoTask5);
            var (info1, info2, info3, info4, info5) = (infoTask1.Result, infoTask2.Result, infoTask3.Result, infoTask4.Result, infoTask5.Result);
            Assert.IsNotNull(info1);
            Assert.IsNotNull(info2);
            Assert.IsNotNull(info3);
            Assert.IsNotNull(info4);
            Assert.IsNotNull(info5);
        }
    }
}
