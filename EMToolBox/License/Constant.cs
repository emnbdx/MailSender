using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMToolBox.License
{
    public class Constant
    {  
        public static int MachineCode
        {
            get
            {
                SKGL.SerialKeyConfiguration skc = new SKGL.SerialKeyConfiguration();
                SKGL.Generate generate = new SKGL.Generate(skc);

                return generate.MachineCode;
            }
        }
    }
}
