using Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONRPC
{
    internal class JsonRPCServer:IRPCServer
    {
        TextReader Reader;
        TextWriter Writer;
        internal JsonRPCServer(TextReader reader, TextWriter writer)
        {
            this.Reader = reader;
            this.Writer = writer;
        }
        public void RegisterHandler(string FunctionName, Action<IBaseRPCPackage> pkg)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
