using System;
using System.Collections.Generic;
using System.IO;
/*
 * Skript Interpreter CMD Interface & Line Reader
 * Version 0.6
 * 
 *  Log:
 *      0.6 - 2/7/2018 - New Splash, Re-did Compile mode.
 *      0.5.1 - 1/28/2018 - Fixed LineReader Memory Issue
 */
namespace Skript_Interpreter
{
    class Splash
    {
        static void WriteBlank(int length)
        {
            int i = 0;
            string str = "";
            if (length > 0)
            {
                while (i < length)
                {
                    str = str + " ";
                    i++;
                }
                Console.WriteLine(str);
            }
        }
        public static void Show()
        {
            Console.Clear();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string ver = fvi.FileVersion;
            Console.WriteLine("");
            var dc = Console.ForegroundColor;
            var bc = Console.BackgroundColor;
            var fc = ConsoleColor.Cyan;
            var vc = ConsoleColor.Green;
            string s1 = "  sKript Interpreter(";
            string s2 = "v0.5r1";
            string s3 = ") + Compiler(";
            string s4 = "v0.1";
            string s5 = ") ";
            string s6 = "v" + ver;
            string s7 = ", Copyright(c) 2018 - Lance Crisang  ";
            string fulls = s1 + s2 + s3 + s4 + s5 + s6 + s7;
            int slength = fulls.Length;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            WriteBlank(slength);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            WriteBlank(slength);
            Console.Write(s1);
            Console.ForegroundColor = vc;
            Console.Write(s2);
            Console.ForegroundColor = fc;
            Console.Write(s3);
            Console.ForegroundColor = vc;
            Console.Write(s4);
            Console.ForegroundColor = fc;
            Console.Write(s5);
            Console.ForegroundColor = vc;
            Console.Write(s6);
            Console.ForegroundColor = fc;
            Console.WriteLine(s7);
            WriteBlank(slength);
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            WriteBlank(slength);
            Console.ForegroundColor = dc;
            Console.BackgroundColor = bc;
            Console.WriteLine("");
        }
    }
    class Interp
    {
        public static string Re_Execute(string linecommand, int goline, int interp, string _2ndexec, IDictionary<string, string> str_t, IDictionary<string, int> int_t, IDictionary<int,bool> perm_t, IDictionary<int,bool> flag_t)
        {
            string ret = "";
            if (!(linecommand == ""))
            {
                if (linecommand.StartsWith("|go|"))
                {
                    goline = Convert.ToInt32(linecommand.Remove(0, 4));
                    if (goline > -1)
                    {
                        interp = goline - 1;
                    }
                }
                if (linecommand.StartsWith("|exec|"))
                {
                    ret = Program.lineinterpret(linecommand.Remove(0, 6), str_t, int_t, perm_t, flag_t);
                }
            }
            return ret;
        }
        public static IDictionary<string, string> MakeStringTable()
        {
            IDictionary<string, string> table = new Dictionary<string, string>();
            table.Clear();
            return table;
        }
        public static IDictionary<string, int> MakeIntTable()
        {
            IDictionary<string, int> table = new Dictionary<string, int>();
            table.Clear();
            return table;
        }
        public static IDictionary<int, bool> MakePermissions()
        {
            IDictionary<int, bool> table = new Dictionary<int, bool>();
            table.Clear();
            return table;
        }
        public static IDictionary<int, bool> MakeFlags()
        {
            IDictionary<int, bool> table = new Dictionary<int, bool>();
            table.Clear();
            return table;
        }
        public static void SetDefaultFlags(IDictionary<int, bool> flag_table)
        {
            /*
             * Flags
             * 00 - Suppress Error Messages
             * 01 - Enable logging
             * 02 - Disable Input
             * 03 - Auto-Compile
             * 04 - Experimental Features
             * 05 - Suppress Debug Messages
             * 06 - Disable Int Table
             * 07 - Disable String Table
             * 08 - Disable Variable Tables
             * 09 - Disable Beep function
             * 10 - Auto Exception Search to StackExchange
             */
            flag_table.Clear();
            flag_table.Add(00, true);
            flag_table.Add(05, true);
            flag_table.Add(09, true);
            flag_table.Add(04, true);
        }
    }
    class Program
    {
        static int numLine(string file_location)
        {
            int ret = 0;
            StreamReader file = new StreamReader(@file_location);
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
            StreamReader file = new StreamReader(@file_location);
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

        static void sysout(string msg)
        {
            System.Console.WriteLine(msg);
        }

        static string sysin(string msg)
        {
            String ret = "";
            sysout(msg);
            ret = System.Console.ReadLine();
            return ret;
        }

        static void pause(string msg)
        {
            System.Console.Write(msg);
            System.Console.ReadKey();
            sysout("");
        }

        public static string lineinterpret(string line,IDictionary<string, string> s_table, IDictionary<string, int> i_table, IDictionary<int, bool> permissions, IDictionary<int, bool> flags)
        {
            //Parse and interpret lines here.
            string linecom = Interpreter.InterpretCommand(line, s_table, i_table, permissions, flags);
            
            return linecom;
        }
        static void Main(string[] args)
        {
            Splash.Show();
            //Ask for File
            
            IDictionary<string, int> i_t = Interp.MakeIntTable();
            IDictionary<string, string> s_t = Interp.MakeStringTable();
            IDictionary<int, bool> perms = Interp.MakePermissions();
            IDictionary<int, bool> flags = Interp.MakeFlags();
            Interp.SetDefaultFlags(flags);
            string file = "";
            bool arguments = false;
            int alength = args.Length;
            if (args.Length == 0)
            {
                file = sysin("File Location:");
            } else {
                file = args[0];
                arguments = true;
            }
            bool exists = System.IO.File.Exists(file);
            if (arguments & alength > 1)
            {
                switch (args[1].ToLower())
                {
                    case "compile":
                        sysout("SKF Translate & Compile Mode.");
                        sysout("    -Be sure to use an existing directory for the destination address.");
                        sysout("");
                        string dest = Directory.GetCurrentDirectory() + "\\output.exe";
                        if (arguments & alength > 2)
                        {
                            dest = args[2];
                        } else
                        {
                            dest = sysin("File Destination:");
                        }
                        Console.WriteLine("Compiling...");
                        if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\temp"))
                        {
                            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\temp");
                        }
                        if (File.Exists(Directory.GetCurrentDirectory() + "\\temp\tmp.cs"))
                        {
                            File.Delete(Directory.GetCurrentDirectory() + "\\temp\\tmp.cs");
                        }
                        if (File.Exists(Directory.GetCurrentDirectory() + "\\temp\tmp.exe"))
                        {
                            File.Delete(Directory.GetCurrentDirectory() + "\\temp\\tmp.exe");
                        }
                        Skript_Compiler.Compiler.StartCompile(file, Directory.GetCurrentDirectory() + "\\temp\\tmp.cs", dest);
                        Console.Write("Press enter to continue...");
                        Console.ReadLine();
                        Environment.Exit(0);
                        break;
                    case "maketest":
                        StreamWriter testfile = new StreamWriter("test.txt");
                        testfile.WriteLine("1| This is a test file.");
                        testfile.WriteLine("2| This translator uses StreamWriter & StreamReader.");
                        testfile.Close();
                        Environment.Exit(0);
                        break;
                }
            }
            else
            {
                if (exists == true)
                {
                    //File exists
                    //sysout("Reading file...");
                    //string[] filearray = loadFile(file);
                    //sysout("[FILE_OPER] " + loadFile(file));
                    int interp = 0;
                    string curline = "";
                    string linecommand = "";
                    int goline = -1;
                    string _2ndexec = "";
                    while (interp != numLine(file))
                    {
                        curline = loadFile(file)[interp];
                        //sysout(curline);
                        //sysout(curline);
                        linecommand = lineinterpret(curline, s_t, i_t, perms, flags);
                        if (!(linecommand == ""))
                        {
                            if (linecommand.StartsWith("|go|"))
                            {
                                goline = Convert.ToInt32(linecommand.Remove(0, 4));
                                if (goline > -1)
                                {
                                    interp = goline - 1;
                                }
                            }
                            if (linecommand.StartsWith("|exec|"))
                            {
                                _2ndexec = lineinterpret(linecommand.Remove(0, 6), s_t, i_t, perms, flags);
                                if (_2ndexec.StartsWith("|go|"))
                                {
                                    goline = Convert.ToInt32(_2ndexec.Remove(0, 4));
                                    if (goline > -1)
                                    {
                                        interp = goline - 1;
                                    }
                                }
                            }
                        }
                        
                        interp++;
                    }
                    sysout("");
                    pause("End of script, press any key to continue...");

                }
                else
                {
                    //File does not exist.
                    sysout("File does not exist.");
                    pause("Press any key to exit...");
                }
            }
            Main(args);
        }  
    }
}
