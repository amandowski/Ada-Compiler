/**************************************************************
 ** Name        : Amanda Lewandowski                         **
 ** Due Date    : February 22, 2020                          **
 ** Course      : CSc 446 - Compiler Construction            **
 ** Instructor  : George Hamer                               **
 ** Assign No.  : 3 - RD Parser                              **
 ** File Name   : Lexical.CS                                 **
 **************************************************************
 ** Description : This is the lexical analzyer for assign 3  **
 *************************************************************/


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace LewandowskiA3
{
    public enum Symbol
    {
        begint, modulet, constantt, proceduret,
        ist, ift, thent, elset, elsift, whilet, loopt,
        floatt, integert, chart, gett, putt, endt, unknownt,
        eoft, addopt, multop, assignopt, lparent, rparent, commat,
        colont, semit, periodt, quotet, idt, relopt, literalt, numt,
        intt, outt, inoutt, realt
    };

    public class Lexical
    {
        enum Resword
        {
            begint, modulet, constantt, proceduret,
            ist, ift, thent, elset, elsift, whilet, loopt,
            floatt, integert, chart, gett, putt, endt
        };

        enum Addop { plus, minus, or, OR };

        enum Mulop { star, div, rem, REM, mod, MOD, and, AND };

        enum Relop { equal, notequal, less, lessequal, greater, greaterequal };

        //global variables
        public Symbol token;
        public string lexeme;
        char ch;
        int positionNo;
        public int lineNo;
        public int value;
        public float valueR;
        public string literal;
        string filePathName;
        string codeLine;
        StreamReader sourceFile;

        String[] symbolArray = { "begint", "modulet", "constantt", "proceduret", "ist",
           "ift", "thent", "elset", "elsift", "whilet", "loopt", "floatt", "integert",
            "chart", "gett", "putt", "endt", "unknownt", "eoft", "addopt", "multop", "assignopt",
            "lparent", "rparent", "commat", "colont", "semit", "periodt", "quotet", "idt", "relopt", 
            "literalt", "intt", "outt", "inoutt" };

        String[] resArray = { "begint", "modulet", "constantt", "proceduret",
            "ist", "ift", "thent", "elset", "elsift", "whilet", "loopt",
            "floatt", "integert", "chart", "gett", "putt", "intt", "outt", "inoutt", "endt" };

        String[] addOpArray = { "+", "-", "or" };

        String[] mulOpArray = { "*", "/", "rem", "mod", "and" };

        String[] relOpArray = { "=", "/=", "<", "<=", ">", ">=" };


        /*************************************************************
        ** Function    : Lexical (Constructor)                      **
        ** Inputs      : String (filename)                          **
        ** Return      : Nothing for a constructor                  **
        **************************************************************
        ** Description : Starts a StreamReader for the input file   **
        **               then runs through and Display's the Tokens **
        *************************************************************/
        public Lexical(string filePath)
        {
            filePathName = Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\..\\" + filePath));
            sourceFile = new StreamReader(filePathName);
            codeLine = sourceFile.ReadLine();
            positionNo = 0;
            lineNo = 1;
            GetNextCh();
        }

        /*************************************************************
        ** Function    : ProcessToken                               **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Checks the first char and sorts it into the**
        **               correct ProcessingToken method.            **
        *************************************************************/

        public void ProcessToken()
        {
            token = Symbol.unknownt;

            GetNextCh();
            while (lexeme[0] == ' ' || lexeme[0] == '\n' || lexeme[0] == '\t')
            {
                lexeme = "";
                lexeme = lexeme + ch;
                GetNextCh();
                if (token == Symbol.eoft)
                {
                    return;
                }
            }

            if ((lexeme[0] >= '0' && lexeme[0] <= '9') || (lexeme[0] == '.' && (ch >= '0' || ch <= '9')))
                ProcessNumToken();
            else if (lexeme[0] >= 'a' && lexeme[0] <= 'z')
                ProcessWordToken();
            else if (lexeme[0] >= 'A' && lexeme[0] <= 'Z')
                ProcessWordToken();
            else if (lexeme[0] == '<' || lexeme[0] == '>' || lexeme[0] == '/' || lexeme[0] == ':')
            {
                if (ch == '=')
                    ProcessDoubleToken();
                else
                    ProcessSingleToken();
            }
            else if (lexeme[0] == '-' && ch == '-')
            {
                ProcessComment();
            }
            else
            {
                ProcessSingleToken();
            }
        }


        /*************************************************************
        ** Function    : GetNextTokens                              **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Gets the next token and checks if EOFToken.**
        *************************************************************/
        public void GetNextToken()
        {
            lexeme = "";
            while (ch == ' ' && ch == '\t' && ch == '\n')
            {
                GetNextCh();
            }
            lexeme = lexeme + ch;
            if (sourceFile.Peek() >= 0 || codeLine != null)
            {
                ProcessToken();
            }
            else
            {
                sourceFile.Close();
                lexeme = "";
                token = Symbol.eoft;
            }
        }


        /*************************************************************
        ** Function    : GetNextCh                                  **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Gets the next character for the variables  **
        **               and checks to make sure its not the \n ch. **
        *************************************************************/
        public void GetNextCh()
        {
            try
            {
                if (ch.Equals('\n') || (codeLine.EndsWith(ch) && codeLine.Length == positionNo))
                {
                    codeLine = sourceFile.ReadLine();
                    lineNo++;
                    positionNo = 0;
                    if (codeLine == null)
                    {
                        token = Symbol.eoft;
                        return;
                    }
                    else
                    {
                        if (codeLine.Length == 0)
                        {
                            codeLine = sourceFile.ReadLine();
                            lineNo++;
                            positionNo = 0;
                            GetNextCh();
                        }
                        else
                        {
                            ch = codeLine[positionNo];
                            positionNo++;
                        }
                    }
                }
                else if (positionNo >= codeLine.Length)
                {
                    codeLine = sourceFile.ReadLine();
                    lineNo++;
                    positionNo = 0;
                    ch = codeLine[positionNo];
                    positionNo++;
                }
                else
                {
                    ch = codeLine[positionNo];
                    positionNo++;
                }
            }
            catch (Exception ex) { }
        }


        /*************************************************************
        ** Function    : ProcessWordToken                           **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes Word Tokens and Finds Literals.  **
        *************************************************************/
        public void ProcessWordToken()
        {
            Symbol sym;

            ReadRest();
            if(lexeme.Length > 17)
            {
                token = Symbol.unknownt;
                Console.WriteLine("Error Idenitifers can only be up to 17 chars");
                return;
            }

            string uppercaseLexeme = lexeme.ToUpper();

            var foundToken = ProcessReservedWordToken(uppercaseLexeme);

            if (foundToken != Symbol.unknownt)
            {
                token = foundToken;
                return;
            }
            else if (uppercaseLexeme == "OR")
                token = Symbol.addopt;
            else if (uppercaseLexeme == "REM" || uppercaseLexeme == "MOD" || uppercaseLexeme == "AND")
                token = Symbol.multop;
            else
                token = Symbol.idt;

          /*  //if(sym >= Symbol.begint )
           // for (sym = Symbol.begint; sym <= Symbol.endt; sym++)
           // {
                if (lexeme.Length > 17)
                {
                    token = Symbol.unknownt;
                    Console.WriteLine("Error Idenitifers can only be up to 17 chars");
                    return;
                }
                else
                    token = Symbol.idt;
                ProcessReservedWordToken();

                //if (token.Equals(resArray[(int)sym]))
                //{
                   // token = sym;
                   // return;
                //}
           // }*/
        }

        public Symbol ProcessReservedWordToken(string reservedWord)
        {
            switch (reservedWord)
            {
                case "BEGIN": return Symbol.begint;
                case "CHAR": return Symbol.chart;
                case "CONSTANT": return Symbol.constantt;
                case "ELSE": return Symbol.elset;
                case "ELSIF": return Symbol.elsift;
                case "END": return Symbol.endt;
                case "FLOAT": return Symbol.floatt;
                case "GET": return Symbol.gett;
                case "IF": return Symbol.ift;
                case "INTEGER": return Symbol.integert;  
                case "REAL": return Symbol.realt;
                case "IS": return Symbol.ist;
                case "LOOP": return Symbol.loopt;
                case "MODULE":  return Symbol.modulet;
                case "PROCEDURE": return Symbol.proceduret;
                case "PUT": return Symbol.putt;
                case "THEN": return Symbol.thent;
                case "WHILE": return Symbol.whilet;
                case "IN": return Symbol.intt;
                case "OUT": return Symbol.outt;
                case "INOUT": return Symbol.inoutt;
                default: 
                    return Symbol.unknownt;
                    
            }
        }

        /*************************************************************
        ** Function    : ProcessNumTokens                           **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the numbers string values.                 **
        *************************************************************/
        public void ProcessNumToken()
        {
            while ((ch >= '0' && ch <= '9') || ch == '.')
            {
                lexeme = lexeme + ch;
                if (codeLine.Length < positionNo + 1)
                {
                    ch = '\n';
                }
                else
                {
                    ch = codeLine[positionNo];
                    positionNo++;
                }
            }
            token = Symbol.numt;
            if (lexeme.Contains('.'))
            {
                if (lexeme.EndsWith('.'))
                {
                    token = Symbol.unknownt;
                    Console.WriteLine("Error must have a value after the decimal point.");
                }
                else if (lexeme.StartsWith('.'))
                {
                    token = Symbol.unknownt;
                    Console.WriteLine("Error must have a value before the decimal point");
                }
                else
                {
                    //valueR = float.Parse(lexeme, CultureInfo.InvariantCulture.NumberFormat);
                    //valueR = float.Parse(lexeme, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    valueR = float.Parse(lexeme);
                    token = Symbol.floatt;
                }
            }
            else
            {
                value = int.Parse(lexeme);
                token = Symbol.integert;
            }
        }


        /*************************************************************
        ** Function    : ReadRest                                   **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Loops through to find the rest of letters  **
        **               and puts into Lexeme string.               **
        *************************************************************/
        public void ReadRest()
        {
            while ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || ch == '_')
            {
                lexeme = lexeme + ch;
                if (codeLine.Length < positionNo + 1)
                {
                    GetNextCh();
                    return;
                }
                ch = codeLine[positionNo];
                positionNo++;
            }
        }


        /*************************************************************
        ** Function    : ProcessSingleToken                         **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the single token string values.            **
        *************************************************************/
        public void ProcessSingleToken()
        {
            if (lexeme.Equals(addOpArray[(int)Addop.plus]))
                token = Symbol.addopt;
            else if (lexeme.Equals(addOpArray[(int)Addop.minus]))
                token = Symbol.addopt;
            else if (lexeme.Equals(addOpArray[(int)Addop.or]))
                token = Symbol.addopt;
            else if (lexeme.Equals(mulOpArray[(int)Mulop.star]))
                token = Symbol.multop;
            else if (lexeme.Equals(mulOpArray[(int)Mulop.div]))
                token = Symbol.multop;
            else if (lexeme.Equals(mulOpArray[(int)Mulop.rem]))
                token = Symbol.multop;
            else if (lexeme.Equals(mulOpArray[(int)Mulop.mod]))
                token = Symbol.multop;
            // else if (lexeme.Equals(mulOpArray[(int)Mulop.and]))
            //   token = Symbol.multop;
            else if (lexeme.Equals(relOpArray[(int)Relop.less]))
                token = Symbol.relopt;
            else if (lexeme.Equals(relOpArray[(int)Relop.greater]))
                token = Symbol.relopt;
            else if (lexeme.Equals(relOpArray[(int)Relop.equal]))
                token = Symbol.relopt;
            else if (lexeme.Equals(";"))
                token = Symbol.semit;
            else if (lexeme.Equals(","))
                token = Symbol.commat;
            else if (lexeme.Equals("("))
                token = Symbol.lparent;
            else if (lexeme.Equals(")"))
                token = Symbol.rparent;
            else if (lexeme.Equals(":"))
                token = Symbol.colont;
            else if (lexeme.Equals("\""))
                ProcessStringLiteral();
            else
            {
                Console.WriteLine("Error Expected a single character and got:" + lexeme);
            }
        }

        /*************************************************************
        ** Function    : ProcessStringLiteral                       **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the literal string values.                 **
        *************************************************************/
        public void ProcessStringLiteral()
        {
            token = Symbol.quotet;
            while (ch != '"')
            {
                lexeme = lexeme + ch;
                literal = lexeme;
                if ((codeLine.EndsWith(ch) && codeLine.Length == positionNo) || codeLine[positionNo].Equals('\n'))
                {
                    codeLine = sourceFile.ReadLine();
                    lineNo++;
                    positionNo = 0;
                    if (codeLine.Length == 0)
                    {
                        codeLine = sourceFile.ReadLine();
                        lineNo++;
                        positionNo = 0;
                        GetNextCh();
                    }
                    else
                    {
                        GetNextCh();
                    }
                    token = Symbol.unknownt;
                    Console.WriteLine("Error Literal has no terminating symbol");
                    return;
                }
                else
                {
                    ch = codeLine[positionNo];
                    positionNo++;
                }
            }
            lexeme = lexeme + ch;
            GetNextCh();
            return;
        }


        /*************************************************************
        ** Function    : ProcessDoubleToken                         **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the double token string values.            **
        *************************************************************/
        public void ProcessDoubleToken()
        {
            if (lexeme == "<" && ch == '=')
            {
                lexeme = lexeme + ch;
                token = Symbol.relopt;
            }
            else if (lexeme == ">" && ch == '=')
            {
                lexeme = lexeme + ch;
                token = Symbol.relopt;
            }
            else if (lexeme == "/" && ch == '=')
            {
                lexeme = lexeme + ch;
                token = Symbol.relopt;
            }
            else if (lexeme == ":" && ch == '=')
            {
                lexeme = lexeme + ch;
                token = Symbol.assignopt;
            }
            else
            {
                Console.WriteLine("Error Expected a double character and instead got: " + lexeme);
            }
            GetNextCh();
        }


        /*************************************************************
        ** Function    : ProcessComment                             **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the comment string values.                 **
        *************************************************************/
        public void ProcessComment()
        {
            if (lexeme == "-" && ch == '-')
            {
                codeLine = sourceFile.ReadLine();
                lineNo++;
                if (!(codeLine == null) && codeLine.Length == 0)
                {
                    codeLine = sourceFile.ReadLine();
                    lineNo++;
                    positionNo = 0;
                    GetNextCh();
                }

                if (codeLine == null)
                {
                    lexeme = "";
                    token = Symbol.eoft;
                    return;
                }
                else
                {
                    positionNo = 0;
                    ch = codeLine[positionNo];
                    positionNo++;
                    lexeme = "" + ch;
                    ProcessToken();
                }
            }
            else
                return;
        }


        /*************************************************************
        ** Function    : PrintToken                                 **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               proper to be printed and displayed.        **
        *************************************************************/
        public void PrintToken()
        {
            if (token == Symbol.floatt)
            {
                // NEEDS A NUMBER NOT 0 AFTER THE . OR WILL NOT SHOW
                Console.WriteLine("Token: " + symbolArray[(int)token].PadRight(20, ' ') + "Lexeme: " + lexeme.PadRight(25, ' ') + "ValueR: " + valueR);
            }
            else if (token == Symbol.integert)
            {
                Console.WriteLine("Token: " + symbolArray[(int)token].PadRight(20, ' ') + "Lexeme: " + lexeme.PadRight(25, ' ') + "Value: " + value);
            }
            else if (lexeme == "" && token != Symbol.eoft)
            {
                //file ended with comment
            }
            else
            {
                Console.WriteLine("Token: " + symbolArray[(int)token].PadRight(20, ' ') + "Lexeme: " + lexeme);
            }
        }

    }
}
