using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;

namespace Skript_Compiler
{
    /*
     * 
     * sKript Compiler v0.1
     * 
     * Notes:
     *      -Mostly the same funcs as Interpreter.cs but instead of executing them, it writes it to a *.cs file.
     *      -Variable tables are no longer needed as those are handled by cs. (Will cause issues when using int when string is required, ex: sysout(Writeline();).)
     *      -Compiler base is done and executes perfectly. Just need to implement Main() and interpreting.
     */
    
    class Compiler
    {
        //Skript -> C# -> EXE
        public static void StartCompile(string address, string saveto, string exesave)
        {
            string sv = saveto;
            if (!sv.EndsWith(".cs"))
            {
                sv = sv + ".cs";
            }
            //Console.WriteLine("Compiling '" + address + "' to '" + sv + "'...");
            MakeCSFile(address, sv);
            BeginCompile(sv, exesave);
            //Console.WriteLine("Done compiling!");
            //Console.WriteLine("Raw Address: '" + saveto + "', New Address: '" + sv + "'.");
        }
        public static void MakeCSFile(string skfaddress, string addressto)
        {
            string[] linearray = FileRead.loadFile(skfaddress);
            using (StreamWriter file =
            new StreamWriter(@addressto))
            {
                var dc = Console.ForegroundColor;
                ConsoleColor wc = ConsoleColor.Magenta;
                Writer.WriteHeader(file, "SKF_Compiled");
                Writer.WriteFunctions(file);
                Writer.WriteMainHead(file);
                Console.ForegroundColor = wc;
                Console.Write("[WriterC#]");
                Console.ForegroundColor = dc;
                Console.WriteLine(" CS Header & Functions Written!");
                foreach (string line in linearray)
                {
                    //Interpret lines here.
                    Console.ForegroundColor = wc;
                    Console.Write("[WriterC#]");
                    Console.ForegroundColor = dc;
                    Console.WriteLine(" Interpreting lines...");
                    Writer.WriteMainLine(file, "Console.WriteLine(" + '"'.ToString() + "Hello World!" + '"'.ToString() +");");
                    Writer.WriteMainLine(file, "Console.Beep();");
                    Writer.WriteMainLine(file, "Console.ReadLine();");
                }
                Writer.WriteMainFoot(file);
                Console.ForegroundColor = wc;
                Console.Write("[WriterC#]");
                Console.ForegroundColor = dc;
                Console.WriteLine(" Main Function Written!");
                Writer.WriteFooter(file);
                Console.ForegroundColor = wc;
                Console.Write("[WriterC#]");
                Console.ForegroundColor = dc;
                Console.WriteLine(" CS Footer Written!");
                file.Close();
            }
            
        }
        public static void BeginCompile(string csfile, string dest)
        {
            //Console.WriteLine("CS Address: '" + csfile + "', EXE Address: '" + dest + "'.");
            //CSharpCodeProvider com = new CSharpCodeProvider();
            CodeDomProvider com = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters opt = new CompilerParameters();
            //ICodeCompiler icc = com.CreateCompiler();
            //opt.MainClass = "Program";
            opt.GenerateExecutable = true;
            opt.OutputAssembly = dest;
            string file = File.ReadAllText(csfile);
            var defcolor = Console.ForegroundColor;
            CompilerResults results = com.CompileAssemblyFromSource(opt, file);
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERR] Line number " + CompErr.Line + ", Error No: " + CompErr.ErrorNumber + ", '" + CompErr.ErrorText + "';");
                    Console.WriteLine("");
                    //Console.WriteLine("");
                    /*textBox2.Text = textBox2.Text +
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine + Environment.NewLine;
                                */
                }
                if (!(Console.ForegroundColor == defcolor))
                {
                    Console.ForegroundColor = defcolor;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[C#_COMPILER]");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Compile completed without any errors!");
                if (!(Console.ForegroundColor == defcolor))
                {
                    Console.ForegroundColor = defcolor;
                }
                Process.Start(dest);
            }

            /* [OBOSOLETE] uses seperate csc.exe to compile...
            string exe = csfile.Remove(csfile.Length - 3, 3) + ".exe";
            Process CSC = new Process();
            CSC.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\bin\\csc.exe";
            CSC.StartInfo.Arguments = "/nologo /out:" + dest + " " + '"'.ToString() + csfile + '"'.ToString();
            CSC.StartInfo.CreateNoWindow = false;
            CSC.StartInfo.UseShellExecute = false;
            CSC.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Console.WriteLine("[CSC] Starting csc.exe...");//(" + CSC.StartInfo.Arguments.ToString() + ")...");
            CSC.Start();
            CSC.WaitForExit();
            string ecod = CSC.ExitCode.ToString();
            CSC.Close();
            Console.WriteLine("[CSC] csc.exe has exited with code: " + ecod + "!");
            //Process.Start(Directory.GetCurrentDirectory()+"\\bin\\csc.exe", " /nologo /out:"+ dest + " " + csfile);
            //File.Move(csfile, exe);
            */
        }
        
    }
    class Writer
    {
        public static void WriteHeader(StreamWriter file, string nspace)
        {
            file.WriteLine("using System;");
            file.WriteLine("using System.Text;");
            file.WriteLine("using System.Diagnostics;");
            file.WriteLine("");
            file.WriteLine("//SKF Compiled from skript_interpreter by Lance Crisang.");
            file.WriteLine("");
            file.WriteLine("namespace " + nspace);
            file.WriteLine("{");

        }
        public static void WriteFooter(StreamWriter file)
        {
            file.WriteLine("}");
            file.WriteLine("//File was generated using skf_compiler.");
        }
        public static void WriteMainHead(StreamWriter file)
        {
            file.WriteLine("    class Program");
            file.WriteLine("    {");
            file.WriteLine("        static void Main(string[] args)");
            file.WriteLine("        {");

        }
        public static void WriteMainLine(StreamWriter file, string line)
        {
            file.WriteLine("            "+line);
        }
        public static void WriteMainFoot(StreamWriter file)
        {
            file.WriteLine("        }");
            file.WriteLine("    }");
        }
        public static void WriteFunctions(StreamWriter file)
        {
            file.WriteLine("    class Functions");
            file.WriteLine("    {");
            file.WriteLine("        public static void sysout(string msg){");
            file.WriteLine("            Console.WriteLine(msg);");
            file.WriteLine("        }");
            file.WriteLine("        public static void beep(int amount){");
            file.WriteLine("            for (int i = 1; i <= amount; i++)");
            file.WriteLine("            {");
            file.WriteLine("                Console.Beep();");
            file.WriteLine("            }");
            file.WriteLine("        }");

            file.WriteLine("    }");

        }
    }
    class SysOp
    {

    }
    class StrOp
    {

    }
    class FileRead
    {
        static int numLine(string file_location)
        {
            int ret = 0;
            System.IO.StreamReader file = new System.IO.StreamReader(@file_location);
            while (file.ReadLine() != null)
            {
                ret++;
            }
            file.Close();
            return ret;
        }
        public static string[] loadFile(string file_location)
        {

            int no = 0;
            string line = "";
            System.IO.StreamReader file = new System.IO.StreamReader(@file_location);
            string[] fileline = new string[numLine(file_location)];
            //sysout("Reading Lines...");
            while ((line = file.ReadLine()) != null)
            {
                fileline[no] = line;
                //sysout(fileline[no]);
                no++;
            }
            file.Close();
            return fileline;
        }
        public static string GetLine(string[] linearray, int pos)
        {
            return linearray[pos];
        }
    }
}
