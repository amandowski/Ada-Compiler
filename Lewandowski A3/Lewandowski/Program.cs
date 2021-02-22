/**************************************************************
 ** Name        : Amanda Lewandowski                         **
 ** Due Date    : February 22, 2021                          **
 ** Course      : CSc 446 - Compiler Construction            **
 ** Instructor  : George Hamer                               **
 ** Assign No.  : 3 - Recursive Descent Parser               **
 ** File Name   : Program.CS                                 **
 **************************************************************
 ** Description : Main program for RD parser                 ** 
 *************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LewandowskiA3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            String filename = "";
            int tokenCount = 0;

            if (args.Length == 1 && File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\..\\" + args[0]))))
            {
                filename = args[0];
            }
            else
            {
                Console.WriteLine("Invalid Arguments - Only Ada file should be the argument");
                Console.WriteLine("Enter the Ada File Name: ");
                filename = Console.ReadLine();
            }

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\..\\" + filename))))
            {
                Lexical myLex = new Lexical(filename);
                RDParser parser = new RDParser(myLex);

                while (myLex.token != Symbol.eoft)
                {
                    myLex.GetNextToken();
                    //Console.WriteLine("Token: " + myLex.token);
                    parser.Procedures();
                    

                }
                
                Console.WriteLine("Compiled with no errors");
                /*while (myLex.token != Symbol.eoft)
                {
                    if (tokenCount <= 24)
                    {
                        myLex.GetNextToken();
                        myLex.PrintToken();
                        tokenCount++;
                    }
                    else
                    {
                        Console.WriteLine("Press any key to Continue");
                        Console.ReadLine();
                        tokenCount = 0;
                    }
                }*/
            }
            else
            {
                Console.WriteLine("Incorrect Ada File Path. Please try again ");
                Environment.Exit(0);
            }
        }
    }
}
