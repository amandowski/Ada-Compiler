/**********************************************************************************
 ** Name	    : Amanda Lewandowski                                             **
 ** Assignment  : 4 - Symbol Table                                               **
 ** Due Date    : March 5th, 2020                                                **
 ** Instructor  : George Hamer                                                   **
 ** File        : SymbolTable.cs                                                 **
 **********************************************************************************
 ** Description : Write a module that will keep a symbol table for our compiler. **
 **********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;


namespace LewandowskiA4
{
	public class SymbolTable
	{
		public const int TABLESIZE = 211;
		public List<ITableEntry>[] theTable = new List<ITableEntry>[TABLESIZE];
		public int depth;
		public enum ProcedureType { callByValue, callByResult, callByValueResult }
		public enum VariableType { intType, realType, charType, }

		public enum EntryType { varType, constantType }
		public class Variable : ITableEntry
		{
			public string Token { get; set; } //Temporary string until we put all the assignments together
			public string Lexeme { get; set; }
			public int Depth { get; set; }
			public VariableType variableType { get; set; } //Keeps track of type of variable
			public EntryType entryType { get; set; }
			public int offset { get; set; }
			public int size { get; set; }
		}

		public class emptySpot : ITableEntry
		{
			public string Token { get; set; } //Temporary string until we put all the assignments together
			public string Lexeme { get; set; }
			public int Depth { get; set; }
			public EntryType entryType { get; set; }
		}

		public interface ITableEntry
		{
			string Token { get; set; } //Temporary string until we put all the assignments together
			string Lexeme { get; set; }
			int Depth { get; set; }
			EntryType entryType { get; set; }

		}


		/*************************************************************
        ** Function    : SymbolTable                                **
        ** Inputs      : None                                       **
        ** Return      : Nothing for Constructor                    **
        **************************************************************
        ** Description : Basic Constructor. Inits the Records.      **
        **************************************************************/
		public SymbolTable()
		{
			//Initialize table to empty
			depth = 0;
			theTable = new List<ITableEntry>[TABLESIZE];
			for (int i = 0; i < TABLESIZE; i++)
			{
				theTable[i] = new List<ITableEntry>();
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
		public void insert(string lex, string token, int depth)
		{
			int location;
			location = hash(lex);
			ITableEntry tempSTR = lookup(lex);
			tempSTR.Token = token;
			tempSTR.Lexeme = lex;
			tempSTR.Depth = depth;
			theTable[location].Add(tempSTR);
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
			bool foundValueAtDepth = false;
			int tempDepth;
			for (int i = 0; i < theTable.Length; i++)
			{
				try
				{
					foreach (ITableEntry entry in theTable[i])
					{
						tempDepth = entry.Depth;
						if (tempDepth == depth)
						{
							//Console.WriteLine("Depth " + depth + " has values found."); error testing
							theTable[i].Remove(entry);
							foundValueAtDepth = true;
						}
					}
				}
				catch (Exception ex)
				{
				}
			}
			if (foundValueAtDepth == false)
			{
				Console.WriteLine("Nothing at Depth " + depth + " was found.");
			}
			else
			{
				//Console.WriteLine("End of Deleting at Depth " + depth + ".");
			}
		}

		/*************************************************************
        ** Function    : lookup                                     **
        ** Inputs      : lex                                        **
        ** Return      : void                                       **
        **************************************************************
        ** Description : Lookup uses the lexeme to find the entry & **
		**               returns the Interface to that entry.       **
        **************************************************************/
		public ITableEntry lookup(string lex)
		{
			String tempLex;
			for (int i = 0; i < theTable.Length; i++)
			{
				try
				{
					foreach (ITableEntry entry in theTable[i])
					{
						tempLex = entry.Lexeme;
						if (tempLex.Equals(lex))
						{
							Console.WriteLine("Lexeme \"" + lex + "\" is found in the Table.");
							return entry;
						}
					}
				}
				catch (Exception ex)
				{
				}
			}
			//Console.WriteLine("Lexeme \"" + lex + "\" is not found in the Table."); error testing
			emptySpot blank = new emptySpot();
			return blank;
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
			bool foundValueAtDepth = false;
			for (int i = 0; i < theTable.Length; i++)
			{
				try
				{
					foreach (ITableEntry entry in theTable[i])
					{
						int tempDepth = entry.Depth;
						if (tempDepth == depth)
						{
							Console.WriteLine(entry.Lexeme);
							foundValueAtDepth = true;
						}
					}
				}
				catch (Exception ex)
				{

				}
			}
			if (foundValueAtDepth == false)
			{
				Console.WriteLine("No Values found at Depth " + depth + ".");
			}
			else
			{
				//Console.WriteLine("End of Printing at Depth " + depth + ".");
			}
		}

		/*************************************************************
        ** Function    : hash                                       **
        ** Inputs      : lexeme                                     **
        ** Return      : int                                        **
        **************************************************************
        ** Description : Passed a lexeme and returns the loc of it  **
        **************************************************************/
		int hash(string lexeme)
		{
			//PJW Hash in C# to work with this compiler
			const uint bitSizeUInt = (uint)(sizeof(uint) * 8);
			const uint threeFourthUInt = (uint)((bitSizeUInt * 3) / 4);
			const uint oneEighthUInt = (uint)(bitSizeUInt / 8);
			const uint highBitsUInt = (uint)(0xFFFFFFFF) << (int)(bitSizeUInt - oneEighthUInt);
			uint hash = 0;
			uint test = 0;
			uint i = 0;

			for (i = 0; i < lexeme.Length; i++)
			{
				hash = (hash << (int)oneEighthUInt) + ((byte)lexeme[(int)i]);

				if ((test = hash & highBitsUInt) != 0)
				{
					hash = ((hash ^ (test >> (int)threeFourthUInt)) & (~highBitsUInt));
				}
			}

			return (int)hash % TABLESIZE;
		}
	}
}
