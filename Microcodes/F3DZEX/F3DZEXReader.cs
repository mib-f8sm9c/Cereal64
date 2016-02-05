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
            List<Palette> palettes = new List<Palette>();
            List<Texture> textures = new List<Texture>();
            List<Vertex> vertices = new List<Vertex>();
            List<F3DZEXCommand> commands = new List<F3DZEXCommand>();

            F3DZEXReaderPackage package = new F3DZEXReaderPackage();

            if (offset < 0 || offset >= data.Length || offset % 8 != 0)
                return package;

            //Reset/Initialize what needs to be initialized/reset

            byte[] command = new byte[8];

            while (offset < data.Length)
            {
                //read the command
                Array.Copy(data, offset, command, 0, 8);

                F3DZEXCommand f3Command =  F3DZEXCommandFactory.ReadCommand(offset, command);
                if (f3Command == null)
                    break;

                commands.Add(f3Command);
                ParseCommand(f3Command);

                offset += 8;

                if (f3Command is F3DZEX_G_EndDL)
                    break;
            }

            //Sort what needs to be sorted
            textures.Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));
            palettes.Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));
            vertices.Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));
            commands.Sort((s1, s2) => s1.FileOffset.CompareTo(s2.FileOffset));

            //Combine into the collections
            List<F3DZEXCommandCollection> commandColls = new List<F3DZEXCommandCollection>();
            List<VertexCollection> vertexColls = new List<VertexCollection>();

            //F3DZEX Commands
            int startColl = 0;
            for (int endColl = 0; endColl < commands.Count; endColl++)
            {
                if (endColl == commands.Count - 1 ||
                    commands[endColl + 1].FileOffset != commands[endColl].FileOffset + commands[endColl].RawDataSize)
                {
                    //Cut off the collection here
                    F3DZEXCommandCollection newColl = new F3DZEXCommandCollection(commands[startColl].FileOffset,
                        commands.GetRange(startColl, endColl - startColl + 1)); //NOTE: Shallow copying is done here

                    commandColls.Add(newColl);

                    startColl = endColl + 1;
                }
            }

            //Vertices
            startColl = 0;
            for (int endColl = 0; endColl < vertices.Count; endColl++)
            {
                if (endColl == vertices.Count - 1 ||
                    vertices[endColl + 1].FileOffset != vertices[endColl].FileOffset + vertices[endColl].RawDataSize)
                {
                    //Cut off the collection here
                    VertexCollection newVert = new VertexCollection(vertices[startColl].FileOffset,
                        vertices.GetRange(startColl, endColl - startColl + 1)); //NOTE: Shallow copying is done here

                    vertexColls.Add(newVert);

                    startColl = endColl + 1;
                }
            }

            package.Textures = textures;
            package.Palettes = palettes;
            package.Commands = commandColls;
            package.Vertices = vertexColls;

            return package;
        }

        private static void ParseCommand(F3DZEXCommand command)
        {
            //Here handle extra command things (like if it's referencing textures and such)
            switch (command.CommandID)
            {
                case F3DZEXCommandID.F3DZEX_G_NOOP: //don't actually use this one

                    break;
                case F3DZEXCommandID.F3DZEX_G_VTX:
                    //Need to laod in the vertex data here

                    break;
            }
        }
    }
}
