using System.Net;
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
        public void MultipleQueriesTest()
        {
            IQueryConnection conn = new QueryConnection();

            // Random Server - Should find more stable one?
            conn.Host = "185.239.211.62";
            conn.Port = 39215;

            conn.Connect();
            var players1 = conn.GetPlayers();
            var players2 = conn.GetPlayers();
            Assert.IsNotNull(players1);
            Assert.IsNotNull(players2);
            conn.Disconnect();
            // Console.WriteLine(players1.ToString());
            // Console.WriteLine(players2.ToString());
            // Console.WriteLine(players3.ToString());
            // Console.WriteLine(players4.ToString());
            // Console.WriteLine(players5.ToString());
        }

        [TestMethod]
        public async Task AsyncQueryTest()
        {
            IQueryConnection conn = new QueryConnection();

            // Random Server - Should find more stable one?
            conn.Host = "185.239.211.62";
            conn.Port = 39215;

            conn.Connect();

            var players = await conn.GetPlayersAsync();
            var info = await conn.GetInfoAsync();
            var rules = await conn.GetRulesAsync();
            Assert.IsNotNull(players);
            Assert.IsNotNull(info);
            Assert.IsNotNull(rules);
            conn.Disconnect();
        }

        [TestMethod]
        //192.0.2.0/24 is reserved documentation range
        [DataRow("192.0.2.0", 27015)]
        public async Task QueryTestFailCondition(string Host, int Port)
        {
            IQueryConnection conn = new QueryConnection();
            conn.Host = Host;
            conn.Port = Port;

            conn.Connect();


            await Assert.ThrowsExceptionAsync<SourceQueryException>(async () =>
                await conn.GetInfoAsync());
            conn.Disconnect();
        }
    }
}
