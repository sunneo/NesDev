using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ValueType
    {
        object Raw { get; set; }
    }

    public class ValueType<T> : ValueType
    {
        public object Raw
        {
            get;
            set;
        }
        public T Value
        {
            get
            {
                return (T)Raw;
            }
            set
            {
                Raw = value;
            }
        }
    }
    public class StringValue : ValueType<String> { }
    public class StringListValue : ValueType<List<String>> { }
    public class IntValue : ValueType<int> { }
    public class IntListValue : ValueType<List<int>> { }

    public interface IBaseRPCPackage
    {
        String method { get; set; }
        ValueType Params { get; set; }
        ValueType Results { get; set; }
    }

    public class RPCPackage<ParamType, ResultType> : IBaseRPCPackage
        where ParamType : ValueType
        where ResultType: ValueType
    {
        public String method { get; set; }
        public ParamType Params { get; set; }
        public ResultType Results { get; set; }


        ValueType IBaseRPCPackage.Params
        {
            get
            {
                return (ValueType)this.Params;
            }
            set
            {
                this.Params = (ParamType)value;
            }
        }

        ValueType IBaseRPCPackage.Results
        {
            get
            {
                return (ValueType)this.Results;
            }
            set
            {
                this.Results = (ResultType)value;
            }
        }
    }

    public interface IRPCClient
    {
        bool IsValid { get; }
        bool Invoke<Targ, Tresult>(RPCPackage<Targ, Tresult> arg)
            where Targ : ValueType
            where Tresult : ValueType;
        void Close();
       
    }
    public class IRPCClientAdapter : IRPCClient
    {
        public IRPCClient Client;
        public IRPCClientAdapter(IRPCClient client)
        {
            this.Client = client;
        }
        public bool IsValid 
        {
            get
            {
                if (Client == null) return false;
                return Client.IsValid;
            }
        }
        public bool Invoke<Targ, Tresult>(RPCPackage<Targ, Tresult> arg)
            where Targ : ValueType
            where Tresult : ValueType
        {
            if (Client == null) return false;
            return Client.Invoke<Targ, Tresult>(arg);
        }
        public bool Invoke<Targ, Tresult>(String method, RPCPackage<Targ, Tresult> arg)
            where Targ : ValueType
            where Tresult : ValueType
        {
            arg.method = method;
            return Invoke<Targ, Tresult>(arg);
        }
        bool Invoke<Targ, Tresult>(String method, Targ argtype, Tresult res) 
            where Targ: ValueType
            where Tresult: ValueType
        {
            RPCPackage<Targ, Tresult> arg = new RPCPackage<Targ, Tresult>();
            arg.method = method;
            arg.Params = argtype;
            bool ret = Invoke<Targ, Tresult>(arg);
            if (res != null && arg.Results != null)
            {
                res.Raw = arg.Results.Raw;
            }
            return ret;
        }


        public void Close()
        {
            if (Client != null)
            {
                Client.Close();
            }
        }
    }
    public interface IRPCServer
    {
        void RegisterHandler(String FunctionName, Action<IBaseRPCPackage> pkg);
        void Start();
        void Stop();
    }
    public interface IRPCBuilder
    {
        TextWriter Writer { get; set; }
        TextReader Reader { get; set; }
        IRPCServer NewServer();
        IRPCClient NewClient();
    }
}
