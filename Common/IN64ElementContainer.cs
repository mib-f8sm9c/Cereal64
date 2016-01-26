using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common
{
    public interface IN64ElementContainer
    {
        void AddElement(N64DataElement element);

        void RemoveElement(N64DataElement element);
    }
}
