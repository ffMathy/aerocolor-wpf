using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroColor.Exceptions
{
    class AeroMessageHookInstallException : Exception
    {
        public AeroMessageHookInstallException()
            : base(
                "Could not install a message hook to any window running right now in order for Aero color to be determined."
                )
        {
            
        }
    }
}
