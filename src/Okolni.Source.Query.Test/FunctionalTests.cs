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

            // Exampleserver: ARK VALGUERO Server
            conn.Host = "89.163.146.91";
            conn.Port = 27023;

            conn.Connect();
            var players1 = conn.GetPlayers();
            var players2 = conn.GetPlayers();

            //conn.Disconnect();
            // Console.WriteLine(players1.ToString());
            // Console.WriteLine(players2.ToString());
            // Console.WriteLine(players3.ToString());
            // Console.WriteLine(players4.ToString());
            // Console.WriteLine(players5.ToString());
        }
    }
}
