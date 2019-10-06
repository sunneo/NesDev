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
    internal class JsonRPCClient:IRPCClient
    {
        TextReader Reader;
        TextWriter Writer;
        bool IsConnected = false;
        
        internal JsonRPCClient(TextReader reader, TextWriter writer)
        {
            this.Reader = reader;
            this.Writer = writer;
            this.IsConnected = (this.Reader != null && this.Writer != null);
        }
        public bool IsValid
        {
            get 
            {
                return this.IsConnected;
            }
        }

        public bool Invoke<Targ, Tresult>(RPCPackage<Targ, Tresult> arg)
            where Targ : Interfaces.ValueType
            where Tresult : Interfaces.ValueType
        {
            if (!IsConnected) return false;
            String sendData = Utility.SerializeToString(arg, typeof(RPCPackage<Targ, Tresult>));
            Writer.Write(sendData);
            Writer.Flush();
            String retstr = Reader.ReadToEnd();
            if(String.IsNullOrEmpty(retstr)) return false;
            RPCPackage<Targ, Tresult> ret = Utility.DeserializeString<RPCPackage<Targ, Tresult>>(retstr);
            if (ret == null)
            {
                return false;
            }
            arg.Results = ret.Results;
            return true;
        }


        public void Close()
        {
            this.IsConnected = false;
            this.Writer.Close();
            this.Reader.Close();
            this.Writer = null;
            this.Reader = null;
        }
    }
}
