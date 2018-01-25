﻿using System;
using System.Collections.Generic;

using Skript_Compiler;

using sysop = Skript_Interpreter.SystemOperation;
using strop = Skript_Interpreter.StringOperation;
using v = Skript_Interpreter.Variables;
using dict = Skript_Interpreter.Dictionary;
using t = Skript_Interpreter.Tools;
using fw = Skript_Interpreter.FileWrite;
/* About
 *          sKript Interpreter v0.4
 *      Copyright(c) 2018 - Lance Crisang
 *      Do not redistribute source code on other websites.
 *      GitHub: https://github.com/Xapier14/sKript-Interpreter-and-Compiler
 *      Author: Lance Crisang
 *      
 *      Version Log:
 *      0.4ext - 1/25/2018 - Fixed Typos, setperm now works properly. Added Flags.(Commands not yet influenced by flags)
 *      0.4 - 1/24/2018 - Added Permissions, now needs permissions for certain functions.
 *      0.3 - 1/19/2018 - Added Int Variables, Now supports string and int variables!
 *      0.2 - 1/18/2018 - Added Partial Variable Support (Can set and get strings) may have some bugs, havent tested yet...
 *      0.1 - 1/9/2018 - Added Variable Support(Can't Store or Get yet...) and simple sysop functions(unfinished).
 *      
 */
/* Notes
*      To Do:
*          -Flag Implementation
*          -Math functions;
*/

namespace Skript_Interpreter
{
    class Interpreter
    {
        
        public static bool testCommand(string line)
        {
            LinkedList<string> dic = Dictionary.MakeDictionary();
            bool result = false;
            if (Dictionary.CheckWord(dic, line))
            {
                Console.WriteLine("Command '" + line + "' recognized.");
                result = true;
            }
            return result;
        }
        public static bool InterpretCommand(string line, IDictionary<string,string> string_table, IDictionary<string, int> int_table, IDictionary<int, bool> permissions, IDictionary<int, bool> flags)
        {
            LinkedList<string> dic = Dictionary.MakeDictionary();
            bool result = false;
            string func = strop.GetWord(line, 1);
            if (!line.StartsWith("//")){
                if (Dictionary.CheckWord(dic, func) || Dictionary.IsComment(line))
                {
                    try
                    {
                        switch (func.ToLower())
                        { //Input Commands here and to MakeDictionary().
                            case "setperm":
                                string perm = v.SubstituteVars(string_table, int_table, strop.GetWord(line, 2), 1, permissions);
                                string val = v.SubstituteVars(string_table, int_table, strop.GetWord(line, 3), 1, permissions);
                                Permissions.SetPermission(Convert.ToInt32(perm), Convert.ToBoolean(Convert.ToInt32(val)), permissions);
                                if (!Flags.CheckFlag(Flags.SuppressDebugMsg, flags))
                                {
                                    sysop.sysout("[Permissions] Set Permission '" + perm + "' to " + val + ".");
                                }
                                break;
                            case "setflag":
                                string flag = v.SubstituteVars(string_table, int_table, strop.GetWord(line, 2), 1, permissions);
                                string fval = v.SubstituteVars(string_table, int_table, strop.GetWord(line, 3), 1, permissions);
                                Flags.SetFlag(Convert.ToInt32(flag), Convert.ToBoolean(Convert.ToInt32(fval)), flags);
                                if (!Flags.CheckFlag(Flags.SuppressDebugMsg, flags))
                                {
                                    sysop.sysout("[Flags] Set Flag '" + flag + "' to " + fval + ".");
                                }
                                break;
                            case "sysout":
                                string msg = strop.GetWord(line, 2);
                                if (strop.HasSubstring(msg, '"'.ToString()))
                                {
                                    msg = strop.TrimEnds(msg, 1);
                                }
                                msg = v.SubstituteVars(string_table, int_table, msg, 3, permissions);
                                sysop.sysout(msg);
                                break;
                            case "sysout.":
                                sysop.sysout("");
                                break;
                            case "title":
                                sysop.title(v.SubstituteVars(string_table, int_table, t.RemoveQuotes(strop.strdiv(line, func)), 3, permissions));
                                break;
                            case "beep":
                                string amount = v.SubstituteVars(string_table, int_table, strop.GetWord(line, 2), 1, permissions);
                                sysop.beep(Convert.ToInt32(amount));
                                break;
                            case "pause":

                                break;
                            case "setint":
                                if (Permissions.CheckPermission(Permissions.Variables, permissions))
                                {
                                    string var_name = strop.GetWord(line, 2);
                                    int value = Convert.ToInt32(strop.GetWord(line, 3));
                                    Variables.StoreInt(int_table, var_name, value);
                                }
                                else
                                {
                                    if (!Flags.CheckFlag(Flags.SuppressDebugMsg, flags))
                                    {
                                        sysop.sysout("[Permissions] Can't run command 'setint' due to invalid or unset permissions.");
                                    }
                                }
                                break;
                            case "setstr":
                                if (Permissions.CheckPermission(Permissions.Variables, permissions))
                                {
                                    string var = strop.GetWord(line, 2);
                                    string valuee = strop.GetWord(line, 3);
                                    Variables.StoreString(string_table, var, valuee);
                                }
                                else
                                {
                                    if (!Flags.CheckFlag(Flags.SuppressDebugMsg, flags))
                                    {
                                        sysop.sysout("[Permissions] Can't run command 'setstr' due to invalid or unset permissions.");
                                    }
                                }
                                break;
                        }
                    } catch (Exception ex) {
                        if (!Flags.CheckFlag(Flags.SuppressErrorMsg, flags))
                        {
                            sysop.sysout("[Error] Error executing '" + func.ToLower() + "'. Exception: '" + ex.ToString() + "'.");
                            sysop.sysout("[Error] Faulty line: '" + line + "'.");
                        }
                    }
                    result = true;
                } else
                {
                    if (!Flags.CheckFlag(Flags.SuppressDebugMsg, flags))
                    {
                        sysop.sysout("Command '" + strop.GetWord(line, 1) + "' not recognized...");
                    }
                }
            }
            return result;
        }
    }
    class SystemOperation
    {
        public static void beep(int amount)
        {
            for (int i = 1; i <= amount; i++)
            {
                Console.Beep();
            }
        }
        public static string sysin(string msg)
        {
            string input = "";
            Console.WriteLine(msg);
            input = Console.ReadLine();
            return input;
        }
        public static void sysout(string msg)
        {
            Console.WriteLine(msg);
            //Console.Beep();
        }
        public static void title(string title)
        {
            Console.Title = title;
            //Console.Beep();
        }
        public static void pause(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadKey();
        }
        public static void end(int ecode)
        {
            Environment.Exit(ecode);
        }
    }
    class FileRead
    {
        public static string GetLine(string file, int line)
        {
            string ret = "";

            return ret;
        }
    }
    class FileWrite
    {
        public static bool Compile(string file, string destination)
        {
            bool result = false;
            string[] filearray;
            filearray = Program.loadFile(file);
            string fileloc = destination + "\\" + file;

            //Write compile code here.
            Compiler.MakeCSFile(fileloc);
            int curline = Compiler.GetCurrentLine(fileloc);

            return result;
        }
    }
    class StringOperation
    {
        public static int GetWordCount(string sentence)
        {
            //shamelessly copied from https://stackoverflow.com/questions/8784517/counting-number-of-words-in-c-sharp
            int index = 0,wordcount=0;
            while (index < sentence.Length)
            {
                // check if current char is part of a word
                while (index < sentence.Length && !char.IsWhiteSpace(sentence[index]))
                    index++;

                wordcount++;

                // skip whitespace until next word
                while (index < sentence.Length && char.IsWhiteSpace(sentence[index]))
                    index++;
            }
            return wordcount;
        }
        public static bool HasSubstring(string sentence, string substring)
        {
            bool ret = false;
            ret = sentence.Contains(substring);
            return ret;
        }
        public static string TrimEnds(string sentence, int characters)
        {
            string ret = "";
            ret = sentence.Substring(characters, sentence.Length - (characters * 2));
            return ret;
        }
        public static string strdiv(string sentence,string startfrom)
        {
            string ret = sentence.Substring(startfrom.Length);
            return ret;
        }
        public static string GetWord(string sentence,int word)
        {
            //Gets a word from a sentence.
            //ex:
            // sentence = "a 'new world'";
            // word = 2;
            // 
            // Would return: 'new world'.
            int find = 0;
            int maxfind = sentence.Length;
            int curword = 1;
            string substr = "";
            string indstr = "";
            bool quote = false;
            char[] quo = new char[1];
            quo[0] = '"';
            while (find < maxfind)
            {
                indstr = sentence.Substring(find, 1);
                if (indstr.Contains('"'.ToString()))
                {
                    quote = !quote;
                }
                if (curword == word)
                {
                    if (char.IsWhiteSpace(indstr[0]) & quote == false) { }
                    else
                    {
                        substr = substr + indstr;
                    }
                }
                if (curword == word + 1)
                {
                    break;
                }
                
                
                if (indstr==" " & quote == false)
                {
                    curword++;
                }
                find++;
            }
            
            return substr;
        }
    }
    class Dictionary
    {
        public static LinkedList<string> MakeDictionary()
        {
            LinkedList<string> dic = new LinkedList<string>();
            //Add commands here.
            dic.AddFirst("getint");
            dic.AddFirst("setint");
            dic.AddFirst("setstr");
            dic.AddFirst("sysout");
            dic.AddFirst("sysout.");
            dic.AddFirst("setperm");
            dic.AddFirst("setflag");
            dic.AddFirst("title");
            dic.AddFirst("beep");
            dic.AddFirst("pause");
            return dic;
        }
        public static void DictionaryAdd(LinkedList<string> dic, string word)
        {
            dic.AddFirst(word.ToLower());
        }
        public static bool CheckWord(LinkedList<string> dictionary,string word)
        {
            bool result = false;
            result = dictionary.Contains(word);
            return result;
        }
        public static bool IsComment(string sentence)
        {
            bool ret = false;
            ret = sentence.StartsWith("//") | sentence.StartsWith("\\");
            return ret;
        }
    }
    class Tools
    {
        public static string RemoveQuotes(string sentence)
        {
            string msg = sentence;
            if (strop.HasSubstring(sentence, '"'.ToString()))
            {
                msg = msg.Replace('"'.ToString(),"");
            }
            return msg;
        }
    }
    class Variables
    {
        public static string SubstituteVars(IDictionary<string, string> string_table, IDictionary<string, int> int_table, string str, int type, IDictionary<int, bool> p_table)
        {
            string ret = str;
            if (Permissions.CheckPermission(Permissions.Variables, p_table))
            {
                int now = strop.GetWordCount(str);
                string word = "";
                //bool word_found = false;
                for (int i = 1; i <= now; i++)
                {
                    //word_found = false;
                    word = strop.GetWord(Tools.RemoveQuotes(str), i);
                    if (VarExists(int_table, string_table, word))
                    {
                        //word_found = true;
                        switch (type)
                        {
                            case 1:
                                ret = ret.Replace(word, GetInt(int_table, VarName(word)).ToString());
                                break;
                            case 2:
                                ret = ret.Replace(word, GetString(string_table, VarName(word)));
                                break;
                            case 3:
                                if (IntExists(int_table, word))
                                {
                                    ret = ret.Replace(word, GetInt(int_table, VarName(word)).ToString());
                                }
                                if (StringExists(string_table, word))
                                {
                                    ret = ret.Replace(word, GetString(string_table, VarName(word)));
                                }
                                break;
                        }

                    }
                    //sysop.sysout("[Debug_SubVars] @Word: " + i.ToString() + ", Word: " + word + ", String: " + ret + ", Var Exist?: "+word_found.ToString()+".");
                }
            }
            ret = Tools.RemoveQuotes(ret);
            return ret;
        }
        public static bool VarExists(IDictionary<string,int> int_table, IDictionary<string,string> string_table, string var)
        {
            bool ret = false;
            if ((IntExists(int_table,var) | StringExists(string_table,var)) & var.StartsWith("@")){
                ret = true;
            }
            return ret;
        }
        public static string VarName(string var_name)
        {
            string ret = var_name;
            if (var_name.StartsWith("@") & var_name.Length > 1)
            {
                ret = var_name.Substring(1);
            }
            return ret;
        }
        public static int GetType(IDictionary<string, int> int_table, IDictionary<string, string> string_table, string raw_var)
        {
            int type = 0;
            if (IntExists(int_table, raw_var) & !StringExists(string_table, raw_var)) { type = 1; } //If int
            if (!IntExists(int_table, raw_var) & StringExists(string_table, raw_var)) { type = 2; } //If string
            if (IntExists(int_table, raw_var) & StringExists(string_table, raw_var)) { type = 3; } //If both
            return type;
        }
        public static bool IntExists(IDictionary<string, int> int_table, string raw_var_name)
        {
            bool ret = false;
            if (raw_var_name.StartsWith("@") & int_table.ContainsKey(raw_var_name.Substring(1)))
            {
                ret = true;
            }
            return ret;
        }
        public static bool StringExists(IDictionary<string, string> string_table, string raw_var_name)
        {

            bool ret = false;
            if (raw_var_name.StartsWith("@") & string_table.ContainsKey(raw_var_name.Remove(0,1)))
            {
                ret = true;
            }
            //sysop.sysout("[Debug_StrExist] VarName: " + raw_var_name + ", RealName: " + raw_var_name.Remove(0,1) + ", StartsWith?: " + raw_var_name.StartsWith("@").ToString() + ", VarFound?: " + string_table.ContainsKey(raw_var_name.Remove(0, 1)) + ".");
            return ret;
        }
        public static string GetString(IDictionary<string,string> string_table,string var_name)
        {
            string ret;
            bool found = string_table.TryGetValue(var_name, out ret);
            if (!found) { ret = var_name; }
            return ret;
        }
        public static int GetInt(IDictionary<string,int> int_table, string var_name)
        {
            int ret = 0;
            int_table.TryGetValue(var_name, out ret);
            return ret;
        }
        public static void StoreString(IDictionary<string, string> string_table, string var_name, string value)
        {
            if (string_table.ContainsKey(var_name))
            {
                string_table.Remove(var_name);
            }
            string_table.Add(var_name, value);
            //sysop.sysout("[Debug_StoreString] key_exist: " + string_table.ContainsKey(var_name).ToString() + ", value: " + value);
        }
        public static void StoreInt(IDictionary<string, int> int_table, string var_name, int value)
        {
            if (int_table.ContainsKey(var_name))
            {
                int_table.Remove(var_name);
            }
            int_table.Add(var_name, value);
        }
        public static void DelString(IDictionary<string,string> string_table, string var_name)
        {
            string_table.Remove(var_name);
        }
        public static void DelInt(IDictionary<string,int> int_table, string var_name)
        {
            int_table.Remove(var_name);
        }
        public static void ClearTables(IDictionary<string,string> string_table,IDictionary<string, int> int_table)
        {
            string_table.Clear();
            int_table.Clear();
        }
    }
    class Flags
    {
        public const int SuppressErrorMsg = 00;
        public const int EnableLogging = 01;
        public const int DisableInput = 02;
        public const int AutoCompile = 03;
        public const int Experimental = 04;
        public const int SuppressDebugMsg = 05;
        public const int DisableInt = 06;
        public const int DisableString = 07;
        public const int DisableVars = 08;
        public const int DisableBeep = 09;
        public const int AutoSearchEx = 10;

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
        public static void SetFlag(int flag, bool value, IDictionary<int, bool> flag_table)
        {
            if (flag_table.ContainsKey(flag))
            {
                flag_table.Remove(flag);
            }
            flag_table.Add(flag, value);
        }
        public static bool CheckFlag(int flag, IDictionary<int, bool> flag_table)
        {
            bool ret = false;
            flag_table.TryGetValue(flag, out ret);
            return ret;
        }
    }
    class Permissions
    {
        public const int SysOp = 00;
        public const int FileWrite = 01;
        public const int FileRead = 02;
        public const int Variables = 03;
        /*
         *           Permission List
         *                 v1
         *      |===============|=======|
         *      | Permission    | Index |
         *      | ------------- | ----- |
         *      | SystemOps     | 00    |
         *      | FileWrite     | 01    |
         *      | FileRead      | 02    |
         *      | Variables     | 03    |
         *      |===============|=======|
         *      More may be added in the future.
         *      
         *      SystemOperations - These do not include SysOut, SysClear, and Title functions. These however, incude
         *                          SysRun, and SysShell functions
         */
        public static void SetPermission(int permission, bool value, IDictionary<int, bool> p_table)
        {
            if (p_table.ContainsKey(permission))
            {
                p_table.Remove(permission);
            }
            p_table.Add(permission, value);
            
        }
        public static bool CheckPermission(int permission, IDictionary<int, bool> p_table)
        {
            bool ret = false;
            p_table.TryGetValue(permission, out ret);
            return ret;
        }
    }
}