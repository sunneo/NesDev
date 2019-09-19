using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesDev.Config
{
    public class CompilerConfig
    {
        /// <summary>
        /// crt0
        /// </summary>
        public String CRTAssembly = "";
        /// <summary>
        /// rumtime.lib
        /// </summary>
        public String RuntimeLib = "";
        /// compiler
        /// </summary>
        public String CC = "";
        /// <summary>
        /// include path to compiler
        /// </summary>
        public List<String> CCINC = new List<string>();
        /// <summary>
        /// ccflags to compiler
        /// </summary>
        public String CCFLAGS = "";

        /// <summary>
        /// assembler
        /// </summary>
        public String CA = "";
        /// <summary>
        /// include path to assembly
        /// </summary>
        public List<String> ASMINC = new List<string>();
        /// <summary>
        /// include path to assembly
        /// </summary>
        public String ASMFLAGS = "";

        /// <summary>
        /// linker
        /// </summary>
        public String LD = "";

        /// <summary>
        /// linker configure file
        /// </summary>
        public String LDCFG = "";

    }
    public class AppConfig
    {
        public String LauncherListPath = "customizeCmds.json";
        public CompilerConfig Configure = new CompilerConfig();
        
    }
}
