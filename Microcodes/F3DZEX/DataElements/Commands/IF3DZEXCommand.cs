using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    // To do: add a thanks to CloudModding for the F3DZEX help info

    public interface IF3DZEXCommand : IN64DataElement
    {
        F3DZEXCommandID CommandID { get; }

        string CommandName { get; }

        string CommandDesc { get; }

        bool IsValid { get; }
    }
}
