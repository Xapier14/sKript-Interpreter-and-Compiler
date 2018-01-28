using System;
using System.Collections.Generic;
using System.Text;

namespace Skript_Compiler
{
    /*
     * 
     * sKript Compiler v1.0
     * 
     * Notes:
     *      -Mostly the same funcs as Interpreter.cs but instead of executing them, it writes it to a *.cs file.
     *      -Variable tables are no longer needed as those are handled by cs. (Will cause issues when using int when string is required, ex: sysout(Writeline();).)
     * 
     */
    class Compiler
    {
        //Skript -> C# -> EXE
        public static void MakeCSFile(string address)
        {
            
        }
        public static int GetCurrentLine(string address)
        {
            return 0;
        }
        public static void InsertLine(string address, int line)
        {

        }
        public static void SetCurrentLine(string address)
        {

        }
        public static void FinalizeFile(string address)
        {

        }
    }
    class SysOp
    {

    }
    class StrOp
    {

    }
}
