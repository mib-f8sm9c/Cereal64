using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public interface IF3DZEXCommand
    {
        F3DZEXCommandID CommandID { get; }

        string CommandName { get; }

        string CommandDesc { get; }

        bool IsValid { get; }
    }
}
