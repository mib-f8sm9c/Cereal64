using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public enum F3DZEXCommandID
    {
        G_NOOP = 0x00,
        G_VTX = 0x01,
        G_MODIFYVTX = 0x02,
        G_CULLDL = 0x03,
        G_BRANCH_Z = 0x04,
        G_TRI1 = 0x05,
        G_TRI2 = 0x06,
        G_QUAD = 0x07,
        G_SPECIAL_3 = 0xD3,
        G_SPECIAL_2 = 0xD4,
        G_SPECIAL_1 = 0xD5,
        G_DMA_IO = 0xD6,
        G_TEXTURE = 0xD7,
        G_POPMTX = 0xD8,
        G_GEOMETRYMODE = 0xD9,
        G_MTX = 0xDA,
        G_MOVEWORD = 0xDB,
        G_MOVEMEM = 0xDC,
        G_LOAD_UCODE = 0xDD,
        G_DL = 0xDE,
        G_ENDDL = 0xDF,
        G_SPNOOP = 0xE0,
        G_RDPHALF_1 = 0xE1,
        G_SETOTHERMODE_L = 0xE2,
        G_SETOTHERMODE_H = 0xE3,
        G_TEXRECT = 0xE4,
        G_TEXRECTFLIP = 0xE5,
        G_RDPLOADSYNC = 0xE6,
        G_RDPPIPESYNC = 0xE7,
        G_RDPTILESYNC = 0xE8,
        G_RDPFULLSYNC = 0xE9,
        G_SETKEYGB = 0xEA,
        G_SETKEYR = 0xEB,
        G_SETCONVERT = 0xEC,
        G_SETSCISSOR = 0xED,
        G_SETPRIMDEPTH = 0xEE,
        G_RDPSETOTHERMODE = 0xEF,
        G_LOADTLUT = 0xF0,
        G_RDPHALF_2 = 0xF1,
        G_SETTILESIZE = 0xF2,
        G_LOADBLOCK = 0xF3,
        G_LOADTILE = 0xF4,
        G_SETTILE = 0xF5,
        G_FILLRECT = 0xF6,
        G_SETFILLCOLOR = 0xF7,
        G_SETFOGCOLOR = 0xF8,
        G_SETBLENDCOLOR = 0xF9,
        G_SETPRIMCOLOR = 0xFA,
        G_SETENVCOLOR = 0xFB,
        G_SETCOMBINE = 0xFC,
        G_SETTIMG = 0xFD,
        G_SETZIMG = 0xFE,
        G_SETCIMG = 0xFF
    }

    public static class F3DZEXCommandFactory
    {
        public static IF3DZEXCommand ReadCommand(int index, byte[] bytes)
        {
            if (bytes.Length < 0x8)
                return null;

            if (!Enum.IsDefined(typeof(F3DZEXCommandID), (int)bytes[0]))
                return null;

            F3DZEXCommandID command = (F3DZEXCommandID)((int)bytes[0]);
            switch (command)
            {
                case F3DZEXCommandID.G_NOOP:
                    return new F3DZEX_G_NoOp(index, bytes);

                case F3DZEXCommandID.G_VTX:
                    return new F3DZEX_G_Vtx(index, bytes);

                case F3DZEXCommandID.G_MODIFYVTX:
                    return new F3DZEX_G_ModifyVtx(index, bytes);

                case F3DZEXCommandID.G_CULLDL:
                    return new F3DZEX_G_CullDL(index, bytes);

                case F3DZEXCommandID.G_BRANCH_Z:
                    return new F3DZEX_G_Branch_Z(index, bytes);

                case F3DZEXCommandID.G_TRI1:
                    return new F3DZEX_G_Tri1(index, bytes);

                case F3DZEXCommandID.G_TRI2:
                    return new F3DZEX_G_Tri2(index, bytes);

                case F3DZEXCommandID.G_QUAD:
                    return new F3DZEX_G_Quad(index, bytes);

                case F3DZEXCommandID.G_SPECIAL_3:
                    return new F3DZEX_G_Special_3(index, bytes);

                case F3DZEXCommandID.G_SPECIAL_2:
                    return new F3DZEX_G_Special_2(index, bytes);

                case F3DZEXCommandID.G_SPECIAL_1:
                    return new F3DZEX_G_Special_1(index, bytes);

                case F3DZEXCommandID.G_DMA_IO:
                    return new F3DZEX_G_DMA_IO(index, bytes);

                case F3DZEXCommandID.G_TEXTURE:
                    return new F3DZEX_G_Texture(index, bytes);

                case F3DZEXCommandID.G_POPMTX:
                    return new F3DZEX_G_PopMtx(index, bytes);

                case F3DZEXCommandID.G_GEOMETRYMODE:
                    return new F3DZEX_G_GeometryMode(index, bytes);

                case F3DZEXCommandID.G_MTX:
                    return new F3DZEX_G_Mtx(index, bytes);

                case F3DZEXCommandID.G_MOVEWORD:
                    return new F3DZEX_G_MoveWord(index, bytes);

                case F3DZEXCommandID.G_MOVEMEM:
                    return new F3DZEX_G_MoveMem(index, bytes);

                case F3DZEXCommandID.G_LOAD_UCODE:
                    return new F3DZEX_G_Load_UCode(index, bytes);

                case F3DZEXCommandID.G_DL:
                    return new F3DZEX_G_DL(index, bytes);

                case F3DZEXCommandID.G_ENDDL:
                    return new F3DZEX_G_EndDL(index, bytes);

                case F3DZEXCommandID.G_SPNOOP:
                    return new F3DZEX_G_SPNoOp(index, bytes);

                case F3DZEXCommandID.G_RDPHALF_1:
                    return new F3DZEX_G_RDPHalf_1(index, bytes);

                case F3DZEXCommandID.G_SETOTHERMODE_L:
                    return new F3DZEX_G_SetOtherMode_L(index, bytes);

                case F3DZEXCommandID.G_SETOTHERMODE_H:
                    return new F3DZEX_G_SetOtherMode_H(index, bytes);

                case F3DZEXCommandID.G_TEXRECT:
                    return new F3DZEX_G_TexRect(index, bytes);

                case F3DZEXCommandID.G_TEXRECTFLIP:
                    return new F3DZEX_G_TexRectFlip(index, bytes);

                case F3DZEXCommandID.G_RDPLOADSYNC:
                    return new F3DZEX_G_RDPLoadSync(index, bytes);

                case F3DZEXCommandID.G_RDPPIPESYNC:
                    return new F3DZEX_G_RDPPipeSync(index, bytes);

                case F3DZEXCommandID.G_RDPTILESYNC:
                    return new F3DZEX_G_RDPTileSync(index, bytes);

                case F3DZEXCommandID.G_RDPFULLSYNC:
                    return new F3DZEX_G_RDPFullSync(index, bytes);

                case F3DZEXCommandID.G_SETKEYGB:
                    return new F3DZEX_G_SetKeyGB(index, bytes);

                case F3DZEXCommandID.G_SETKEYR:
                    return new F3DZEX_G_SetKeyR(index, bytes);

                case F3DZEXCommandID.G_SETCONVERT:
                    return new F3DZEX_G_SetConvert(index, bytes);

                case F3DZEXCommandID.G_SETSCISSOR:
                    return new F3DZEX_G_SetScissor(index, bytes);

                case F3DZEXCommandID.G_SETPRIMDEPTH:
                    return new F3DZEX_G_SetPrimDepth(index, bytes);

                case F3DZEXCommandID.G_RDPSETOTHERMODE:
                    return new F3DZEX_G_RDPSetOtherMode(index, bytes);

                case F3DZEXCommandID.G_LOADTLUT:
                    return new F3DZEX_G_LoadTLut(index, bytes);

                case F3DZEXCommandID.G_RDPHALF_2:
                    return new F3DZEX_G_RDPHalf_2(index, bytes);

                case F3DZEXCommandID.G_SETTILESIZE:
                    return new F3DZEX_G_SetTileSize(index, bytes);

                case F3DZEXCommandID.G_LOADBLOCK:
                    return new F3DZEX_G_LoadBlock(index, bytes);

                case F3DZEXCommandID.G_LOADTILE:
                    return new F3DZEX_G_LoadTile(index, bytes);

                case F3DZEXCommandID.G_SETTILE:
                    return new F3DZEX_G_SetTile(index, bytes);

                case F3DZEXCommandID.G_FILLRECT:
                    return new F3DZEX_G_FillRect(index, bytes);

                case F3DZEXCommandID.G_SETFILLCOLOR:
                    return new F3DZEX_G_SetFillColor(index, bytes);

                case F3DZEXCommandID.G_SETFOGCOLOR:
                    return new F3DZEX_G_SetFogColor(index, bytes);

                case F3DZEXCommandID.G_SETBLENDCOLOR:
                    return new F3DZEX_G_SetBlendColor(index, bytes);

                case F3DZEXCommandID.G_SETPRIMCOLOR:
                    return new F3DZEX_G_SetPrimColor(index, bytes);

                case F3DZEXCommandID.G_SETENVCOLOR:
                    return new F3DZEX_G_SetEnvColor(index, bytes);

                case F3DZEXCommandID.G_SETCOMBINE:
                    return new F3DZEX_G_SetCombine(index, bytes);

                case F3DZEXCommandID.G_SETTIMG:
                    return new F3DZEX_G_SetTImg(index, bytes);

                case F3DZEXCommandID.G_SETZIMG:
                    return new F3DZEX_G_SetZImg(index, bytes);

                case F3DZEXCommandID.G_SETCIMG:
                    return new F3DZEX_G_SetCImg(index, bytes);

            }

            return null;
            
        }

    }
}
