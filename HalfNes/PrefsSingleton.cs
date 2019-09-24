using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes
{
    public class PrefsSingleton
    {
        static Preferences instance;
        static object locker = new object();
        public class Preferences
        {
            internal object getObject(String name, object defaultVal)
            {
                try
                {
                    object obj = Properties.Settings.Default[name];
                    return (object)obj;
                }
                catch (Exception)
                {
                    return defaultVal;
                }
            }
            public int getInt(String name, int defaultVal = 0)
            {
                return (int)getObject(name, defaultVal);
            }
            public bool getbool(String name, bool defaultVal = false)
            {
                return (bool)getObject(name, defaultVal);
            }
            public bool getBoolean(String name, bool defaultVal = false)
            {
                return getbool(name, defaultVal);
            }
        }
        public static Preferences get()
        {
            if (instance == null)
            {
                instance = new Preferences();
            }
            return instance;
        }
    }
}
