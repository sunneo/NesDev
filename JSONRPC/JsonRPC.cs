using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONRPC
{
    public class JsonRPC
    {
        public static IRPCBuilder Factory()
        {
            JsonRPCBuilder ret = new JsonRPCBuilder();
            return ret;
        }
    }
}
