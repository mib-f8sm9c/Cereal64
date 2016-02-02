using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common
{
    public interface IN64DataElement
    {
        int FileOffset { get; }

        byte[] RawData { get; }
    }
}
