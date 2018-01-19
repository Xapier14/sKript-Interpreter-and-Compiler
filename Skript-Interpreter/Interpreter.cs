using System;
using System.Collections.Generic;

using Skript_Compiler;

using sysop = Skript_Interpreter.SystemOperation;
using strop = Skript_Interpreter.StringOperation;
using v = Skript_Interpreter.Variables;
using dict = Skript_Interpreter.Dictionary;
using t = Skript_Interpreter.Tools;
using fw = Skript_Interpreter.FileWrite;

/* About
 *          sKript Interpreter v0.3
 *      Copyright(c) 2018 - Lance Crisang
 *      Do not redistribute source code on other websites.
 *      Author: Lance Crisang
 *      
 *      Version Log:
 *      0.3 - 1/19/2028 - Added Int Variables, Now supports string and int variables!
 *      0.2 - 1/18/2018 - Added Partial Variable Support (Can set and get strings) may have some bugs, havent tested yet...
 *      0.1 - 1/9/2018 - Added Variable Support(Can't Store or Get yet...) and simple sysop functions(unfinished).
 *      
 */
 /* Notes
 *      To Do:
 *          -Add VarSubstitute();
 *          -Finish Variable table functions;
 *          -Math functions;
 *          -Int to String & vice-versa function.
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
        public static bool InterpretCommand(string line, IDictionary<string,string> string_table, IDictionary<string, int> int_table)
        {
            LinkedList<string> dic = Dictionary.MakeDictionary();
            bool result = false;
            string func = strop.GetWord(line, 1);
            if (!line.StartsWith("//")){
                if (Dictionary.CheckWord(dic, func) || Dictionary.IsComment(line))
                {
                    switch (func.ToLower())
                    { //Input Commands here and to MakeDictionary().
                        case "sysout":
                            string msg = strop.GetWord(line, 2);
                            if (strop.HasSubstring(msg, '"'.ToString()))
                            {
                                msg = strop.TrimEnds(msg, 1);
                            }
                            msg = v.SubstituteVars(string_table, int_table, msg, 3);
                            sysop.sysout(msg);
                            break;
                        case "sysout.":
                            sysop.sysout("");
                            break;
                        case "title":
                            sysop.title(v.SubstituteVars(string_table,int_table,t.RemoveQuotes(strop.strdiv(line, func)),3));
                            break;
                        case "beep":
                            string amount = v.SubstituteVars(string_table, int_table, strop.GetWord(line, 2), 1);
                            sysop.beep(Convert.ToInt32(amount));
                            break;
                        case "pause":

                            break;
                        case "setint":
                            string var_name = strop.GetWord(line, 2);
                            int value = Convert.ToInt32(strop.GetWord(line, 3));
                            Variables.StoreInt(int_table, var_name, value);
                            break;
                        case "setstr":
                            string var = strop.GetWord(line, 2);
                            string valuee = strop.GetWord(line, 3);
                            Variables.StoreString(string_table, var, valuee);
                            break;
                    }
                    result = true;
                } else
                {
                    //sysop.sysout("Command '" + strop.GetWord(line, 1) + "' not recognized...");
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
        public static string SubstituteVars(IDictionary<string, string> string_table, IDictionary<string, int> int_table, string str, int type)
        {
            string ret = str;
            int now = strop.GetWordCount(str);
            string word = "";
            //bool word_found = false;
            for (int i = 1; i <= now; i++)
            {
                //word_found = false;
                word=strop.GetWord(Tools.RemoveQuotes(str), i);
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
}