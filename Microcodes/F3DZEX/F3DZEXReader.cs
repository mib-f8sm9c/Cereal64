using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DZEX.DataElements;
using Cereal64.Common.Utils;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;

namespace Cereal64.Microcodes.F3DZEX
{
    public class F3DZEXReaderPackage
    {
        public List<Palette> Palettes = new List<Palette>();
        public List<Texture> Textures = new List<Texture>();
        public List<VertexCollection> Vertices = new List<VertexCollection>();
        public List<F3DZEXCommandCollection> Commands = new List<F3DZEXCommandCollection>();
    }

    public static class F3DZEXReader
    {
        //public static 
        public static F3DZEXReaderPackage ReadF3DZEXAt(byte[] data, int offset)
        {
            F3DZEXReaderPackage package = new F3DZEXReaderPackage();

            if (offset < 0 || offset >= data.Length || offset % 8 != 0)
                return package;

            //Reset/Initialize what needs to be initialized/reset

            byte[] command = new byte[8];

            while (offset < data.Length)
            {
                //read the command
                Array.Copy(data, offset, command, 0, 8);

                IF3DZEXCommand f3Command =  F3DZEXCommandFactory.ReadCommand(offset, command);
                if (f3Command == null)
                    break;

                //package.Commands.Add(f3Command);
                ParseCommand(f3Command);

                offset += 8;
            }

            return package;
        }

        private static void ParseCommand(IF3DZEXCommand command)
        {
            //Here handle extra command things (like if it's referencing textures and such)
            switch (command.CommandID)
            {
                case F3DZEXCommandID.G_NOOP: //don't actually use this one

                    break;
            }
        }
    }
}
