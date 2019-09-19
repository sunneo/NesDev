using NesDev.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesDev
{
    public class Utility
    {
        public static IMacroExpander Expander;
        public static T DeserializeString<T>(String str)
        {
            try
            {
                using (TextReader r = new StringReader(str))
                using (Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader(r))
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    return serializer.Deserialize<T>(reader);
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(null, ee);
                return default(T);
            }
        }
        public static T Deserialize<T>(String filename)
        {
            try
            {
                if (!File.Exists(filename)) return default(T);
                return DeserializeString<T>(File.ReadAllText(filename));
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(null, ee);
                return default(T);
            }
        }
        public static String SerializeToString(object o, Type t = null)
        {
            try
            {
                StringBuilder strb = new StringBuilder();
                using (TextWriter w = new StringWriter(strb))
                using (Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(w))
                {
                    Newtonsoft.Json.JsonSerializer s = new Newtonsoft.Json.JsonSerializer();
                    if (t == null)
                    {
                        t = o.GetType();
                    }
                    s.Serialize(writer, o, t);
                }
                return strb.ToString();
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(null, ee);
                return "";
            }
        }
        public static void Serialize(String filename, object o, Type t = null)
        {
            try
            {
                File.WriteAllText(filename, SerializeToString(o, t));
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(null, ee);
            }
        }
    }
}
