using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common
{
    /// <summary>
    /// IN64ElementContainer is made to contain a list of a specific type of
    ///  N64Element. This is so when a RomFile is loading in an N64Element, it
    ///  can have auto-updated containers of certain element types (ex. Textures,
    ///  Unknown data sections, etc.)
    ///  
    /// Pretty simple concept, look to RomFile to see how it is used
    /// </summary>
    public interface IN64ElementContainer : ITreeNodeElement
    {
        /// <summary>
        /// Called whenever the parent RomFile adds an element. It's up to
        ///  the implemented Container to decide whether to add the element
        ///  or not.
        /// </summary>
        /// <param name="element">N64DataElement added to parent RomFile</param>
        void AddElement(N64DataElement element);

        /// <summary>
        /// Called whenever the parent RomFile removes an element. It's up to
        ///  the implemented Container to decide whether to remove the element
        ///  or not.
        /// </summary>
        /// <param name="element">N64DataElement removed from parent RomFile</param>
        void RemoveElement(N64DataElement element);
    }
}
