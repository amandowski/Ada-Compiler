/**************************************************************
 ** Name        : Amanda Lewandowski                         **
 ** Due Date    : March 31, 2020                             **
 ** Course      : CSc 446 - Compiler Construction            **
 ** Instructor  : George Hamer                               **
 ** Assign No.  : 6 - Seq Of Stats                           **
 ** File Name   : SymbolTable.CS                             **
 **************************************************************
 ** Description : This is the Symbol Table                   **
 *************************************************************/

using System;
using System.Collections.Generic;   // need for list;

namespace LewandowskiA5
{
    public enum EntryType { varEntry, constEntry, procEntry }
    public enum VarType { charType, intType, floatType }
    public enum ModeType { inMode = 1, outMode, inoutMode }
    public class EntryNode
    {
        public string lexeme;
        public Vari.Symbol token;
        public int depth = 0;
        public EntryType type;
    }
    public class VarEntry : EntryNode
    {
        public VarType variableType;
        public int offset;
        public int size;
    }
    public class ConstEntry : EntryNode
    {
        public int iValue;
        public double fValue;
    }
    public class ProcedureEntry : EntryNode
    {
        public int solocals;
        public int soparams;
        public int noparams;
        public List<VarType> toparams;
        public List<ModeType> passing;
    }


    public class SymbolTable
    {
        public const int TABLESIZE = 211;
        public List<EntryNode>[] theSymbolTable = new List<EntryNode>[TABLESIZE];


        /*************************************************************
        ** Function    : SymbolTable                                **
        ** Inputs      : None                                       **
        ** Return      : Nothing for Constructor                    **
        **************************************************************
        ** Description : Basic Constructor. Inits the Records.      **
        **************************************************************/
        public SymbolTable()
        {
            for (int i = 0; i < TABLESIZE; ++i)
            {
                theSymbolTable[i] = new List<EntryNode>();
            }
        }

        /*************************************************************
        ** Function    : insert                                     **
        ** Inputs      : lex, token, and depth                      **
        ** Return      : void                                       **
        **************************************************************
        ** Description : Inserts the lexeme, token and depth into a **
		**               record in the symbol table.                **
        **************************************************************/
        public void insert(string lex, Vari.Symbol Token, int depth)
        {
            int x = hash(lex);
            EntryNode entry = new EntryNode();
            entry.lexeme = lex;
            entry.token = Token;
            entry.depth = depth;
            entry.type = Vari.entryType;

            if (theSymbolTable[x].Count != 0)
            { theSymbolTable[x].Insert(0, entry); }
            else
            { theSymbolTable[x].Add(entry); }

        }

        /*************************************************************
        ** Function    : lookup                                     **
        ** Inputs      : lex                                        **
        ** Return      : int                                        **
        **************************************************************
        ** Description : Lookup uses the lexeme to find the entry & **
		**               returns the Interface to that entry.       **
        **************************************************************/
        public int lookup(string lex)
        {
            int location = hash(lex);

            for (int i = 0; i < theSymbolTable[location].Count; ++i)
            {
                if (lex == theSymbolTable[location][i].lexeme)
                {
                    return i;
                }
            }
            return -1;  // not in table
        }

        /*************************************************************
        ** Function    : deleteDepth                                **
        ** Inputs      : depth                                      **
        ** Return      : void                                       **
        **************************************************************
        ** Description : Delete is passed the depth and deletes all **
		**               records that are in the table at that depth**
        **************************************************************/
        public void deleteDepth(int depth)
        {
            for (int listI = 0; listI < TABLESIZE; ++listI)
            {
                for (int arrayI = 0; arrayI < theSymbolTable[listI].Count - 1; ++arrayI)
                {
                    if (theSymbolTable[listI][arrayI].depth == depth)
                    {
                        theSymbolTable[listI].RemoveAt(arrayI);
                    }
                }
            }
        }

        /*************************************************************
        ** Function    : writeTable                                 **
        ** Inputs      : depth                                      **
        ** Return      : void                                       **
        **************************************************************
        ** Description : Includes a procedure that will write out   **
		**               all lexemes that are in the table at depth **
        **************************************************************/
        public void writeTable(int depth)
        {
            Console.WriteLine("------------------------------------------");
            for (int listIndex = 0; listIndex < TABLESIZE; ++listIndex)
            {
                for (int arrayIndex = 0; arrayIndex < theSymbolTable[listIndex].Count; ++arrayIndex)
                {
                    if (theSymbolTable[listIndex][arrayIndex].depth == depth)
                    {
                        Console.WriteLine("Lexeme: " + theSymbolTable[listIndex][arrayIndex].lexeme + " Token: " + theSymbolTable[listIndex][arrayIndex].token);
                    }
                }
            }
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("\n");
        }

        /*************************************************************
        ** Function    : hashWrap                                   **
        ** Inputs      : lexeme                                     **
        ** Return      : int                                        **
        **************************************************************
        ** Description : Wrapper for hash so it can be used outside **
        **************************************************************/
        public int hashWrap(string lexeme)
        {
            return hash(lexeme);
        }

        /*************************************************************
        ** Function    : hash                                       **
        ** Inputs      : lexeme                                     **
        ** Return      : int                                        **
        **************************************************************
        ** Description : Passed a lexeme and returns the loc of it  **
        **************************************************************/
        private int hash(string lexeme)
        {
            //PJW Hash in C# to work with this compiler
            const uint BitsInUnsignedInt = (uint)(sizeof(uint) * 8);
            const uint ThreeQuarters = (uint)((BitsInUnsignedInt * 3) / 4);
            const uint OneEighth = (uint)(BitsInUnsignedInt / 8);
            const uint HighBits = (uint)(0xFFFFFFFF) << (int)(BitsInUnsignedInt - OneEighth);
            uint hash = 0;
            uint test = 0;
            uint i = 0;

            for (i = 0; i < lexeme.Length; i++)
            {
                hash = (hash << (int)OneEighth) + ((byte)lexeme[(int)i]);

                if ((test = hash & HighBits) != 0)
                {
                    hash = ((hash ^ (test >> (int)ThreeQuarters)) & (~HighBits));
                }
            }

            return (int)hash % TABLESIZE;
        }

        /*************************************************************
        ** Function    : A5Display                                  **
        ** Inputs      : none                                       **
        ** Return      : vod                                        **
        **************************************************************
        ** Description : Displays the depth for assignment 5        **
        **************************************************************/
        public void A5Display()
        {
            EntryNode temp = new EntryNode();
            Console.WriteLine("\n Depth: " + Vari.depth);
            Console.WriteLine("-----------------------------------");
            for (int listIndex = 0; listIndex < TABLESIZE; ++listIndex)
            {
                for (int arrayIndex = 0; arrayIndex < theSymbolTable[listIndex].Count; ++arrayIndex)
                {
                    if (theSymbolTable[listIndex][arrayIndex].depth == Vari.depth)
                    {
                        temp = theSymbolTable[listIndex][arrayIndex];
                        Console.WriteLine("Lexeme: " + temp.lexeme + "   Type: " + temp.type.ToString());
                    }
                }
            }
            Console.WriteLine("---------------------------------------------");
        }
    }
}
