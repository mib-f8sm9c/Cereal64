using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class F3DZEXCommandCollection : N64DataElement
    {
        private List<IF3DZEXCommand> _commands = new List<IF3DZEXCommand>();

        public ReadOnlyCollection<IF3DZEXCommand> Commands { get { return _commands.AsReadOnly(); } }

        public F3DZEXCommandCollection(int index, byte[] rawBytes)
            : base(index, rawBytes)
        {

        }

        public F3DZEXCommandCollection(int index, List<IF3DZEXCommand> commands)
            : base (index, null)
        {
            _commands = commands;
        }

        public override byte[] RawData 
        { 
            get
            {
                return null;
            }
            set
            {
                if (value == null) //Use null to allow for different constructor
                    return;

                byte[] bytes = new byte[0x8];
                for (int i = 0; i < value.Length - 0x7; i += 0x8)
                {
                    Array.Copy(value, i, bytes, 0, 0x8);

                    IF3DZEXCommand command = F3DZEXCommandFactory.ReadCommand(FileOffset + i, bytes);
                    if (command != null)
                        _commands.Add(command);
                }
            }
        }

        public override int RawDataSize { get { return _commands.Count * 0x8; } }

    }
}
