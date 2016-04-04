using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common.DataElements
{
    /// <summary>
    /// Interface for a reference to a N64DataElement elsewhere. Mainly here to allow for
    ///  easy updating of reference pointers if the element location changes.
    /// </summary>
    public interface IN64DataElementReference
    {
        /// <summary>
        /// Reference object that is being pointed to
        /// </summary>
        N64DataElement Reference { get; set; }

        /// <summary>
        /// Grab the file offset of the reference and update the stored value of this object
        /// </summary>
        void UpdateReference();
    }
}
