using System;
using System.Collections.Generic;
using System.Text;

namespace Okolni.Source.Common
{
    [Serializable]
    public class SourceQueryException : Exception
    {
        public SourceQueryException() : base() { }
        public SourceQueryException(string message) : base(message) { }
        public SourceQueryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
