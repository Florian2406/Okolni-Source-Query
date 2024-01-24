using System;
using System.Collections.Generic;
using System.Text;

namespace Okolni.Source.Query.Responses
{
    public class RuleResponse
    {
        /// <summary>
        /// Always equal to 'E' (0x45)
        /// </summary>
        public byte Header { get; set; }

        /// <summary>
        /// Dictionary of Rules on the Server
        /// </summary>
        public Dictionary<string, string> Rules { get; set; }
        
        /// <summary>
        /// The response obtained from the server without any processing.
        /// </summary>
        public byte[] RawResponse { get; set; }
    }
}
