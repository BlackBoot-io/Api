using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avn.Domain.Enums
{
    public enum EmailTemplate : byte
    {
        Verification = 1,
        Registration = 2,
        ForgetPassword = 3
    }
}
