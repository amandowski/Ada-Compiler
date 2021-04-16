/**********************************************************************************
 ** Name	    : Amanda Lewandowski                                             **
 ** Assignment  : 4 - Symbol Table                                               **
 ** Due Date    : March 5th, 2020                                                **
 ** Instructor  : George Hamer                                                   **
 ** File        : Program.cs                                                     **
 **********************************************************************************
 ** Description : Write a module that will keep a symbol table for our compiler. **
 **********************************************************************************/
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace LewandowskiA4
{
    public class Program
    {
        static void Main(string[] args)
        {
            SymbolTable symTab = new SymbolTable();
            symTab.insert("2", "int", 1);
            symTab.insert("3.5", "float", 1);
            symTab.insert("temp", "idt", 2);
            symTab.insert("TEMP", "idt", 3);
            symTab.insert("Int", "ResWord", 3);
            symTab.insert("87", "int", 1);
            symTab.insert("123.5", "float", 1);
            symTab.insert("223", "int", 2);
            symTab.insert("3234.5", "float", 2);


            Console.WriteLine("\nThe Table: ");
            symTab.writeTable(1);
            symTab.writeTable(2);
            symTab.writeTable(3);

            Console.WriteLine("\nLooking up Lexeme \"Int\"");
            symTab.lookup("Int");

            Console.WriteLine("\nDeleting Depth 3");
            symTab.deleteDepth(3);

            Console.WriteLine("\nThe Table: ");
            symTab.writeTable(1);
            symTab.writeTable(2);
            symTab.writeTable(3);

        }
    }
}

