/**************************************************************
 ** Name        : Amanda Lewandowski                         **
 ** Due Date    : March 31, 2020                             **
 ** Course      : CSc 446 - Compiler Construction            **
 ** Instructor  : George Hamer                               **
 ** Assign No.  : 6 - Seq Of Stats                           **
 ** File Name   : Program.CS                                 **
 **************************************************************
 ** Description : This is the main driver                    **
 *************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LewandowskiA5;

namespace LewandowskiA5
{
    class Program
    {
        static void Main(string[] args)
        {
            string file_dir = Directory.GetCurrentDirectory() + "\\";
            string file_name = "";

            if (args.Length == 0)
            {
                do
                {
                    Console.WriteLine("Enter the Ada File Name: ");
                    file_name = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(file_name));
            }

            if (args.Length > 0 && File.Exists(file_dir + args[0]))
                file_name = file_dir + args[0];
            else if (File.Exists(file_dir + file_name))
                file_name = file_dir + file_name;
            else
            {
                Console.WriteLine("ERROR: File not found.");
                Environment.Exit(1);
            }

            Vari.myLex = new Lexical(file_name);
            var myParse = new LewandowskiA5.Parser();
        }
    }

    public class Vari
    {
        public enum Symbol
        {
            begint, modulet, constantt, proceduret, ist, ift, thent, elset,
            elseift, whilet, loopt, floatt, integert, chart, gett, putt, endt,
            idt, eoftt, relopt, assignopt, signopt, addopt, mulopt, unknownt,
            literal, period, commat, lparent, rparent, colont, semit,
            quotet, whitespace, or, mod, and, rem, intt, outt, inoutt, nott
        }

        public static Symbol Token;
        public static string Lexeme;
        public static int lineNo = 1;
        public static int IntValue;
        public static double RealValue;
        public static string LiteralValue;

        public static char ch;
        public static Lexical myLex;

        public static EntryType entryType;
        public static VarType varType;
        public static ModeType modeType;
        public static List<int> offset = new List<int>();
        public static int depth;
    }
}
