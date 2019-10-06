using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONRPC
{
    internal class JsonRPCBuilder : IRPCBuilder
    {
        public System.IO.TextWriter Writer
        {
            get;
            set;
        }

        public System.IO.TextReader Reader
        {
            get;
            set;
        }

        public IRPCServer NewServer()
        {
            JsonRPCServer ret = new JsonRPCServer(Reader, Writer);
            return ret;
        }

        public IRPCClient NewClient()
        {
            JsonRPCClient ret = new JsonRPCClient(Reader, Writer);
            return ret;
        }
    }
}
