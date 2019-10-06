using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clangd
{
    public class Program
    {
        public class ClangContext
        {
            
            public void Open(String filename)
            {

            }
            public void Change()
            {

            }
            public void Config()
            {

            }
            public void Definition()
            {

            }
            public void Complete()
            {

            }
        }
        static void DidOpen(Interfaces.IBaseRPCPackage package)
        {

        }
        static void DidChange(Interfaces.IBaseRPCPackage package)
        {

        }
        static void Definition(Interfaces.IBaseRPCPackage package)
        {

        }
        static void Config(Interfaces.IBaseRPCPackage package)
        {

        }
        static void Complete(Interfaces.IBaseRPCPackage package)
        {

        }
        public static void Main(string[] args)
        {
            var factory = JSONRPC.JsonRPC.Factory();
            factory.Reader = Console.In;
            factory.Writer = Console.Out;
            var server = factory.NewServer();
            server.RegisterHandler("document/didOpen", DidOpen);
            server.RegisterHandler("document/didChange", DidChange);
            server.RegisterHandler("document/definition", Definition);
            server.RegisterHandler("clangd/config", Config);
            server.RegisterHandler("clangd/complete", Complete);
            server.Start();
        }
    }
}
