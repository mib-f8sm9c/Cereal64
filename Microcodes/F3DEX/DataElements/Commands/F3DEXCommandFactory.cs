using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Microcodes.F3DEX.DataElements.Commands
{
    public enum F3DEXCommandID
    {
        F3DEX_G_NOOP = 0x00,
        F3DEX_G_MTX = 0x01,
        F3DEX_G_MOVEMEM = 0x03,
        F3DEX_G_VTX = 0x04,
        F3DEX_G_DL = 0x06,
        F3DEX_G_MK64_ENDDL = 0x80,
        F3DEX_G_LOAD_UCODE = 0xAF,
        F3DEX_G_BRANCH_Z = 0xB0,
        F3DEX_G_TRI2 = 0xB1,
        F3DEX_G_MODIFYVTX = 0xB2,
        F3DEX_G_RDPHALF_2 = 0xB3,
        F3DEX_G_RDPHALF_1 = 0xB4,
        F3DEX_G_CLEARGEOMETRYMODE = 0xB6,
        F3DEX_G_SETGEOMETRYMODE = 0xB7,
        F3DEX_G_ENDDL = 0xB8,
        F3DEX_G_SETOTHERMODE_L = 0xB9,
        F3DEX_G_SETOTHERMODE_H = 0xBA,
        F3DEX_G_TEXTURE = 0xBB,
        F3DEX_G_MOVEWORD = 0xBC,
        F3DEX_G_POPMTX = 0xBD,
        F3DEX_G_CULLDL = 0xBE,
        F3DEX_G_TRI1 = 0xBF,
        F3DEX_G_TEXRECT = 0xE4,
        F3DEX_G_TEXRECTFLIP = 0xE5,
        F3DEX_G_RDPLOADSYNC = 0xE6,
        F3DEX_G_RDPPIPESYNC = 0xE7,
        F3DEX_G_RDPTILESYNC = 0xE8,
        F3DEX_G_RDPFULLSYNC = 0xE9,
        F3DEX_G_SETKEYGB = 0xEA,
        F3DEX_G_SETKEYR = 0xEB,
        F3DEX_G_SETCONVERT = 0xEC,
        F3DEX_G_SETSCISSOR = 0xED,
        F3DEX_G_SETPRIMDEPTH = 0xEE,
        F3DEX_G_RDPSETOTHERMODE = 0xEF,
        F3DEX_G_LOADTLUT = 0xF0,
        F3DEX_G_SETTILESIZE = 0xF2,
        F3DEX_G_LOADBLOCK = 0xF3,
        F3DEX_G_LOADTILE = 0xF4,
        F3DEX_G_SETTILE = 0xF5,
        F3DEX_G_FILLRECT = 0xF6,
        F3DEX_G_SETFILLCOLOR = 0xF7,
        F3DEX_G_SETFOGCOLOR = 0xF8,
        F3DEX_G_SETBLENDCOLOR = 0xF9,
        F3DEX_G_SETPRIMCOLOR = 0xFA,
        F3DEX_G_SETENVCOLOR = 0xFB,
        F3DEX_G_SETCOMBINE = 0xFC,
        F3DEX_G_SETTIMG = 0xFD,
        F3DEX_G_SETZIMG = 0xFE,
        F3DEX_G_SETCIMG = 0xFF
    }
    
    public static class F3DEXCommandFactory
    {
        public static F3DEXCommand ReadCommand(int index, byte[] bytes)
        {
            if (bytes.Length < 0x8)
                return null;

            if (!Enum.IsDefined(typeof(F3DEXCommandID), (int)bytes[0]))
                return null;

            F3DEXCommandID command = (F3DEXCommandID)((int)bytes[0]);
            switch (command)
            {
                case F3DEXCommandID.F3DEX_G_NOOP:
                    return new F3DEX_G_NoOp(index, bytes);

                case F3DEXCommandID.F3DEX_G_VTX:
                    return new F3DEX_G_Vtx(index, bytes);

                case F3DEXCommandID.F3DEX_G_MODIFYVTX:
                    return new F3DEX_G_ModifyVtx(index, bytes);

                case F3DEXCommandID.F3DEX_G_CULLDL:
                    return new F3DEX_G_CullDL(index, bytes);

                case F3DEXCommandID.F3DEX_G_BRANCH_Z:
                    return new F3DEX_G_Branch_Z(index, bytes);

                case F3DEXCommandID.F3DEX_G_TRI1:
                    return new F3DEX_G_Tri1(index, bytes);

                case F3DEXCommandID.F3DEX_G_TRI2:
                    return new F3DEX_G_Tri2(index, bytes);

                case F3DEXCommandID.F3DEX_G_TEXTURE:
                    return new F3DEX_G_Texture(index, bytes);

                case F3DEXCommandID.F3DEX_G_POPMTX:
                    return new F3DEX_G_PopMtx(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETGEOMETRYMODE:
                    return new F3DEX_G_SetGeometryMode(index, bytes);

                case F3DEXCommandID.F3DEX_G_CLEARGEOMETRYMODE:
                    return new F3DEX_G_ClearGeometryMode(index, bytes);

                case F3DEXCommandID.F3DEX_G_MTX:
                    return new F3DEX_G_Mtx(index, bytes);

                case F3DEXCommandID.F3DEX_G_MOVEWORD:
                    return new F3DEX_G_MoveWord(index, bytes);

                case F3DEXCommandID.F3DEX_G_MOVEMEM:
                    return new F3DEX_G_MoveMem(index, bytes);

                case F3DEXCommandID.F3DEX_G_LOAD_UCODE:
                    return new F3DEX_G_Load_UCode(index, bytes);

                case F3DEXCommandID.F3DEX_G_DL:
                    return new F3DEX_G_DL(index, bytes);

                case F3DEXCommandID.F3DEX_G_MK64_ENDDL:
                    return new F3DEX_G_MK64_EndDL(index, bytes);

                case F3DEXCommandID.F3DEX_G_ENDDL:
                    return new F3DEX_G_EndDL(index, bytes);

                case F3DEXCommandID.F3DEX_G_RDPHALF_1:
                    return new F3DEX_G_RDPHalf_1(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETOTHERMODE_L:
                    return new F3DEX_G_SetOtherMode_L(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETOTHERMODE_H:
                    return new F3DEX_G_SetOtherMode_H(index, bytes);

                case F3DEXCommandID.F3DEX_G_TEXRECT:
                    return new F3DEX_G_TexRect(index, bytes);

                case F3DEXCommandID.F3DEX_G_TEXRECTFLIP:
                    return new F3DEX_G_TexRectFlip(index, bytes);

                case F3DEXCommandID.F3DEX_G_RDPLOADSYNC:
                    return new F3DEX_G_RDPLoadSync(index, bytes);

                case F3DEXCommandID.F3DEX_G_RDPPIPESYNC:
                    return new F3DEX_G_RDPPipeSync(index, bytes);

                case F3DEXCommandID.F3DEX_G_RDPTILESYNC:
                    return new F3DEX_G_RDPTileSync(index, bytes);

                case F3DEXCommandID.F3DEX_G_RDPFULLSYNC:
                    return new F3DEX_G_RDPFullSync(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETKEYGB:
                    return new F3DEX_G_SetKeyGB(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETKEYR:
                    return new F3DEX_G_SetKeyR(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETCONVERT:
                    return new F3DEX_G_SetConvert(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETSCISSOR:
                    return new F3DEX_G_SetScissor(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETPRIMDEPTH:
                    return new F3DEX_G_SetPrimDepth(index, bytes);

                case F3DEXCommandID.F3DEX_G_RDPSETOTHERMODE:
                    return new F3DEX_G_RDPSetOtherMode(index, bytes);

                case F3DEXCommandID.F3DEX_G_LOADTLUT:
                    return new F3DEX_G_LoadTLut(index, bytes);

                case F3DEXCommandID.F3DEX_G_RDPHALF_2:
                    return new F3DEX_G_RDPHalf_2(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETTILESIZE:
                    return new F3DEX_G_SetTileSize(index, bytes);

                case F3DEXCommandID.F3DEX_G_LOADBLOCK:
                    return new F3DEX_G_LoadBlock(index, bytes);

                case F3DEXCommandID.F3DEX_G_LOADTILE:
                    return new F3DEX_G_LoadTile(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETTILE:
                    return new F3DEX_G_SetTile(index, bytes);

                case F3DEXCommandID.F3DEX_G_FILLRECT:
                    return new F3DEX_G_FillRect(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETFILLCOLOR:
                    return new F3DEX_G_SetFillColor(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETFOGCOLOR:
                    return new F3DEX_G_SetFogColor(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETBLENDCOLOR:
                    return new F3DEX_G_SetBlendColor(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETPRIMCOLOR:
                    return new F3DEX_G_SetPrimColor(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETENVCOLOR:
                    return new F3DEX_G_SetEnvColor(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETCOMBINE:
                    return new F3DEX_G_SetCombine(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETTIMG:
                    return new F3DEX_G_SetTImg(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETZIMG:
                    return new F3DEX_G_SetZImg(index, bytes);

                case F3DEXCommandID.F3DEX_G_SETCIMG:
                    return new F3DEX_G_SetCImg(index, bytes);

            }

            return null;
            
        }
    }
    
}
