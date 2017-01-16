using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common.DataElements
{
    public delegate void UpdateEvent();

    public interface IUpdatable
    {
        UpdateEvent Updated { get; set; }
    }
}
