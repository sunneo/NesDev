using Interfaces;
using Serializers;
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
        Dictionary<String, Action<IBaseRPCPackage>> Handlers = new Dictionary<string, Action<IBaseRPCPackage>>();
        public void RegisterHandler(string FunctionName, Action<IBaseRPCPackage> pkg)
        {
            Handlers[FunctionName] = pkg;
        }

        public virtual void Start()
        {
            while (true)
            {
                String txt = Reader.ReadLine();
                if (!String.IsNullOrEmpty(txt))
                {
                    try
                    {
                        RPCPackage<StringValue, StringValue> header = Utility.DeserializeString<RPCPackage<StringValue, StringValue>>(txt);
                        if (header != null)
                        {
                            header.RawString = txt;
                            if (Handlers.ContainsKey(header.method))
                            {
                                Handlers[header.method](header);
                                header.Params = null;
                                header.RawString = null;
                                String response = Utility.SerializeToString(header);
                                Writer.WriteLine(response);
                                Writer.Flush();
                            }
                            else
                            {
                                Console.Error.WriteLine("Not Found Function Handler for {0}",header.method);
                            }
                        }
                    }
                    catch(Exception ee)
                    {
                        Console.Error.WriteLine(ee.ToString());
                    }
                }
            }
        }

        public void Stop()
        {
            Reader.Close();
        }
    }
}
