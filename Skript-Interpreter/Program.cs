using System;
using System.Collections.Generic;
/*
 * Skript Interpreter CMD Interface & Line Reader
 * Version 0.4
 * 
 */
namespace Skript_Interpreter
{
    class Splash
    {
        public static void Show()
        {
            Console.Clear();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string ver = fvi.FileVersion;
            Console.WriteLine("sKript Interpreter v" + ver + ", Copyright (c) 2018 - Lance Crisang");
            Console.WriteLine("");
        }
    }
    class Interp
    {
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
            System.IO.StreamReader file = new System.IO.StreamReader(@file_location);
            while (file.ReadLine() != null)
            {
                ret++;
            }
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

        static string lineinterpret(string line,IDictionary<string, string> s_table, IDictionary<string, int> i_table, IDictionary<int, bool> permissions, IDictionary<int, bool> flags)
        {
            //Parse and interpret lines here.
            if (Interpreter.InterpretCommand(line,s_table,i_table,permissions,flags))
            {

            } else
            {
                //sysout("Command not recognized...");
            }
            return "";
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
                if (args[1].ToLower() == "compile")
                {
                    string destination = "";
                    if (args.Length > 2)
                    {
                        destination = args[2];
                    }
                    else
                    {
                        destination = sysin("File Destination:");
                    }
                    FileWrite.Compile(file, destination);
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
                    while (interp != numLine(file))
                    {
                        curline = loadFile(file)[interp];
                        //sysout(curline);
                        //sysout(curline);
                        lineinterpret(curline,s_t,i_t,perms,flags);
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
