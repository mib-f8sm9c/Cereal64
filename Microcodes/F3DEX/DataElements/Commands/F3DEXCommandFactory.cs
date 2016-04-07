using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Microcodes.F3DEX.DataElements.Commands
{
    public enum F3DEXCommandID
    {
        F3DZEX_G_NOOP = 0x00,
        F3DZEX_G_MTX = 0x01,
        F3DZEX_G_MOVEMEM = 0x03,
        F3DZEX_G_VTX = 0x04,
        F3DZEX_G_DL = 0x06,
        F3DZEX_G_BRANCH_Z = 0xB0,
        F3DZEX_G_TRI2 = 0xB1,
        F3DZEX_G_MODIFYVTX = 0xB2,
        F3DZEX_G_RDPHALF_2 = 0xB3,
        F3DZEX_G_RDPHALF_1 = 0xB4,
        F3DZEX_G_CLEARGEOMETRYMODE = 0xB6,
        F3DZEX_G_SETGEOMETRYMODE = 0xB7,
        F3DZEX_G_ENDDL = 0xB8,
        F3DZEX_G_SETOTHERMODE_L = 0xB9,
        F3DZEX_G_SETOTHERMODE_H = 0xBA,
        F3DZEX_G_TEXTURE = 0xBB,
        F3DZEX_G_MOVEWORD = 0xBC,
        F3DZEX_G_POPMTX = 0xBD,
        F3DZEX_G_CULLDL = 0xBE,
        F3DZEX_G_TRI1 = 0xBF,
        F3DZEX_G_TEXRECT = 0xE4,
        F3DZEX_G_TEXRECTFLIP = 0xE5,
        F3DZEX_G_RDPLOADSYNC = 0xE6,
        F3DZEX_G_RDPPIPESYNC = 0xE7,
        F3DZEX_G_RDPTILESYNC = 0xE8,
        F3DZEX_G_RDPFULLSYNC = 0xE9,
        F3DZEX_G_SETKEYGB = 0xEA,
        F3DZEX_G_SETKEYR = 0xEB,
        F3DZEX_G_SETCONVERT = 0xEC,
        F3DZEX_G_SETSCISSOR = 0xED,
        F3DZEX_G_SETPRIMDEPTH = 0xEE,
        F3DZEX_G_RDPSETOTHERMODE = 0xEF,
        F3DZEX_G_LOADTLUT = 0xF0,
        F3DZEX_G_SETTILESIZE = 0xF2,
        F3DZEX_G_LOADBLOCK = 0xF3,
        F3DZEX_G_LOADTILE = 0xF4,
        F3DZEX_G_SETTILE = 0xF5,
        F3DZEX_G_FILLRECT = 0xF6,
        F3DZEX_G_SETFILLCOLOR = 0xF7,
        F3DZEX_G_SETFOGCOLOR = 0xF8,
        F3DZEX_G_SETBLENDCOLOR = 0xF9,
        F3DZEX_G_SETPRIMCOLOR = 0xFA,
        F3DZEX_G_SETENVCOLOR = 0xFB,
        F3DZEX_G_SETCOMBINE = 0xFC,
        F3DZEX_G_SETTIMG = 0xFD,
        F3DZEX_G_SETZIMG = 0xFE,
        F3DZEX_G_SETCIMG = 0xFF
    }
    /*
    public static class F3DZEXCommandFactory
    {
        public static F3DZEXCommand ReadCommand(int index, byte[] bytes)
        {
            if (bytes.Length < 0x8)
                return null;

            if (!Enum.IsDefined(typeof(F3DZEXCommandID), (int)bytes[0]))
                return null;

            F3DZEXCommandID command = (F3DZEXCommandID)((int)bytes[0]);
            switch (command)
            {
                case F3DZEXCommandID.F3DZEX_G_NOOP:
                    return new F3DZEX_G_NoOp(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_VTX:
                    return new F3DZEX_G_Vtx(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_MODIFYVTX:
                    return new F3DZEX_G_ModifyVtx(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_CULLDL:
                    return new F3DZEX_G_CullDL(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_BRANCH_Z:
                    return new F3DZEX_G_Branch_Z(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_TRI1:
                    return new F3DZEX_G_Tri1(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_TRI2:
                    return new F3DZEX_G_Tri2(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_QUAD:
                    return new F3DZEX_G_Quad(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SPECIAL_3:
                    return new F3DZEX_G_Special_3(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SPECIAL_2:
                    return new F3DZEX_G_Special_2(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SPECIAL_1:
                    return new F3DZEX_G_Special_1(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_DMA_IO:
                    return new F3DZEX_G_DMA_IO(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_TEXTURE:
                    return new F3DZEX_G_Texture(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_POPMTX:
                    return new F3DZEX_G_PopMtx(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_GEOMETRYMODE:
                    return new F3DZEX_G_GeometryMode(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_MTX:
                    return new F3DZEX_G_Mtx(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_MOVEWORD:
                    return new F3DZEX_G_MoveWord(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_MOVEMEM:
                    return new F3DZEX_G_MoveMem(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_LOAD_UCODE:
                    return new F3DZEX_G_Load_UCode(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_DL:
                    return new F3DZEX_G_DL(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_ENDDL:
                    return new F3DZEX_G_EndDL(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SPNOOP:
                    return new F3DZEX_G_SPNoOp(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_RDPHALF_1:
                    return new F3DZEX_G_RDPHalf_1(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETOTHERMODE_L:
                    return new F3DZEX_G_SetOtherMode_L(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETOTHERMODE_H:
                    return new F3DZEX_G_SetOtherMode_H(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_TEXRECT:
                    return new F3DZEX_G_TexRect(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_TEXRECTFLIP:
                    return new F3DZEX_G_TexRectFlip(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_RDPLOADSYNC:
                    return new F3DZEX_G_RDPLoadSync(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_RDPPIPESYNC:
                    return new F3DZEX_G_RDPPipeSync(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_RDPTILESYNC:
                    return new F3DZEX_G_RDPTileSync(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_RDPFULLSYNC:
                    return new F3DZEX_G_RDPFullSync(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETKEYGB:
                    return new F3DZEX_G_SetKeyGB(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETKEYR:
                    return new F3DZEX_G_SetKeyR(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETCONVERT:
                    return new F3DZEX_G_SetConvert(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETSCISSOR:
                    return new F3DZEX_G_SetScissor(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETPRIMDEPTH:
                    return new F3DZEX_G_SetPrimDepth(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_RDPSETOTHERMODE:
                    return new F3DZEX_G_RDPSetOtherMode(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_LOADTLUT:
                    return new F3DZEX_G_LoadTLut(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_RDPHALF_2:
                    return new F3DZEX_G_RDPHalf_2(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETTILESIZE:
                    return new F3DZEX_G_SetTileSize(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_LOADBLOCK:
                    return new F3DZEX_G_LoadBlock(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_LOADTILE:
                    return new F3DZEX_G_LoadTile(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETTILE:
                    return new F3DZEX_G_SetTile(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_FILLRECT:
                    return new F3DZEX_G_FillRect(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETFILLCOLOR:
                    return new F3DZEX_G_SetFillColor(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETFOGCOLOR:
                    return new F3DZEX_G_SetFogColor(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETBLENDCOLOR:
                    return new F3DZEX_G_SetBlendColor(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETPRIMCOLOR:
                    return new F3DZEX_G_SetPrimColor(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETENVCOLOR:
                    return new F3DZEX_G_SetEnvColor(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETCOMBINE:
                    return new F3DZEX_G_SetCombine(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETTIMG:
                    return new F3DZEX_G_SetTImg(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETZIMG:
                    return new F3DZEX_G_SetZImg(index, bytes);

                case F3DZEXCommandID.F3DZEX_G_SETCIMG:
                    return new F3DZEX_G_SetCImg(index, bytes);

            }

            return null;
            
        }

    }
    */
}
