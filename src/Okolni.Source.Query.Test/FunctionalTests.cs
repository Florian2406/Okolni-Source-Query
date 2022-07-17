using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Okolni.Source.Query;
using Okolni.Source.Common;

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
    }
}
