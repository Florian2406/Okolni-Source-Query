using System;
using System.Collections.Generic;
using System.Text;

namespace Okolni.Source.Query.Responses
{
    interface IResponse
    {
        byte Header { get; set; }
    }
}
