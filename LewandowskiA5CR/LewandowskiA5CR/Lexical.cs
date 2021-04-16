/**************************************************************
 ** Name        : Amanda Lewandowski                         **
 ** Due Date    : March 31, 2020                             **
 ** Course      : CSc 446 - Compiler Construction            **
 ** Instructor  : George Hamer                               **
 ** Assign No.  : 5 - Depth                                  **
 ** File Name   : Lexical.CS                                 **
 **************************************************************
 ** Description : This is the Lexical Analyzer               **
 *************************************************************/

using System;
using System.IO;
using System.Collections.Generic;

namespace LewandowskiA5
{
    public class Lexical
    {
        public static StreamReader reader;
        public static List<string> reservedWords;

        public Lexical(string fileName)
        {
            reader = new StreamReader(fileName);
            Vari.ch = (char)reader.Read();
            reservedWords = new List<string> { "begin", "module", "constant",
                                                "procedure", "is", "if", "then",
                                                "else", "elseif", "while", "loop",
                                                "float", "integer", "char", "get",
                                                "put", "end" };
        }



        /*************************************************************
        ** Function    : GetNextTokens                              **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Gets the next token and checks if EOFToken.**
        *************************************************************/
        public void getNextToken()
        {
            while (Vari.ch <= 32)
            {
                getNextChar();
            }
            if (!reader.EndOfStream)
                processToken();
            else
            {
                Vari.Token = Vari.Symbol.eoftt;
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
        public void getNextChar()
        {
            if (Vari.ch == 10)
                Vari.lineNo++;
            Vari.ch = (char)reader.Read();
        }

        /*************************************************************
        ** Function    : ProcessToken                               **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes single Tokens                    **
        *************************************************************/
        public void processToken()
        {
            Vari.Lexeme = Vari.ch.ToString();
            getNextChar(); // one character lookahead

            if (Vari.Lexeme[0] >= 'A' && Vari.Lexeme[0] <= 'Z' || Vari.Lexeme[0] >= 'a' && Vari.Lexeme[0] <= 'z')
                processWordToken();
            else if (Vari.Lexeme[0] >= '0' && Vari.Lexeme[0] <= '9')
                processNumToken();
            else if (Vari.Lexeme[0] == '-' && Vari.ch == '-')
            {
                processComment();
                getNextToken();
            }
            else if (Vari.Lexeme[0] == '"')
                processStringLiteral();
            else if (Vari.Lexeme[0] == '<' || Vari.Lexeme[0] == '>' || Vari.Lexeme[0] == ':' || Vari.Lexeme[0] == '/')
            {
                if (Vari.ch == '=')
                    processDoubleToken();
                else
                    processSingleToken();
            }
            else
                processSingleToken();
        }

        /*************************************************************
        ** Function    : ProcessWordToken                           **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes Word Tokens and Finds Literals.  **
        *************************************************************/
        public void processWordToken()
        {
            int length = 1;

            while (Vari.ch >= 'A' && Vari.ch <= 'Z' || Vari.ch >= 'a' && Vari.ch <= 'z' || Vari.ch == '_' || Vari.ch >= '0' && Vari.ch <= '9')
            {
                length++;
                Vari.Lexeme += Vari.ch;
                getNextChar();
            }

            switch (Vari.Lexeme.ToLower())
            {
                case "begin":
                    Vari.Token = Vari.Symbol.begint;
                    break;
                case "module":
                    Vari.Token = Vari.Symbol.modulet;
                    break;
                case "constant":
                    Vari.Token = Vari.Symbol.constantt;
                    break;
                case "procedure":
                    Vari.Token = Vari.Symbol.proceduret;
                    break;
                case "is":
                    Vari.Token = Vari.Symbol.ist;
                    break;
                case "if":
                    Vari.Token = Vari.Symbol.ift;
                    break;
                case "then":
                    Vari.Token = Vari.Symbol.thent;
                    break;
                case "else":
                    Vari.Token = Vari.Symbol.elset;
                    break;
                case "elseif":
                    Vari.Token = Vari.Symbol.elseift;
                    break;
                case "while":
                    Vari.Token = Vari.Symbol.whilet;
                    break;
                case "loop":
                    Vari.Token = Vari.Symbol.loopt;
                    break;
                case "float":
                    Vari.Token = Vari.Symbol.floatt;
                    break;
                case "integer":
                    Vari.Token = Vari.Symbol.integert;
                    break;
                case "char":
                    Vari.Token = Vari.Symbol.chart;
                    break;
                case "get":
                    Vari.Token = Vari.Symbol.gett;
                    break;
                case "put":
                    Vari.Token = Vari.Symbol.putt;
                    break;
                case "end":
                    Vari.Token = Vari.Symbol.endt;
                    break;
                case "or":
                    Vari.Token = Vari.Symbol.addopt;
                    break;
                case "rem":
                    Vari.Token = Vari.Symbol.mulopt;
                    break;
                case "mod":
                    Vari.Token = Vari.Symbol.mulopt;
                    break;
                case "and":
                    Vari.Token = Vari.Symbol.mulopt;
                    break;
                case "in":
                    Vari.Token = Vari.Symbol.intt;
                    break;
                case "out":
                    Vari.Token = Vari.Symbol.outt;
                    break;
                case "inout":
                    Vari.Token = Vari.Symbol.inoutt;
                    break;
                case "not":
                    Vari.Token = Vari.Symbol.nott;
                    break;
                default:
                    Vari.Token = Vari.Symbol.idt;
                    break;
            }

            if (Vari.Token == Vari.Symbol.idt && length >= 17)
            {
                Console.WriteLine("Error Invalid Identifier");
            }
        }

        /*************************************************************
        ** Function    : display                                    **
        ** Inputs      : void                                       **
        ** Return      : void                                       **
        **************************************************************
        ** Description : displays the tokens                        **
        *************************************************************/
        public void display()
        {
            if (Vari.Token == Vari.Symbol.whitespace)
                return;

            if (Vari.Token == Vari.Symbol.eoftt)
                Vari.Lexeme = "eoft";

            Console.Write("token: " + Vari.Token.ToString().PadRight(17, ' ') + "lexeme: " + Vari.Lexeme.PadRight(19, ' '));

            if (Vari.Token == Vari.Symbol.integert)
                Console.Write("int: " + Vari.IntValue);

            else if (Vari.Token == Vari.Symbol.floatt)
                Console.Write("real: " + Vari.RealValue);

            else if (Vari.Token == Vari.Symbol.literal)
                Console.Write("literal: " + "\"" + Vari.LiteralValue + "\"");

            else if (Vari.Token == Vari.Symbol.unknownt)
                Console.Write("Error unknown token");

            Console.Write("\n");
        }


        /*************************************************************
        ** Function    : ProcessNumTokens                           **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the numbers string values.                 **
        *************************************************************/
        public void processNumToken()
        {
            int numOfDecimals = 0;

            while (Vari.ch >= '0' && Vari.ch <= '9' || (Vari.ch == '.' && numOfDecimals < 1))
            {
                if (Vari.ch == '.')
                {
                    numOfDecimals += 1;
                }

                Vari.Lexeme += Vari.ch;
                getNextChar();
            }

            if (Vari.Lexeme[Vari.Lexeme.Length - 1] == '.')
            {
                Vari.Token = Vari.Symbol.unknownt;
                Console.WriteLine("Error on line: " + Vari.lineNo + ", incorrect token");
                return;
            }
            else if (numOfDecimals == 1)
            {
                Vari.RealValue = System.Convert.ToDouble(Vari.Lexeme);
                Vari.Token = Vari.Symbol.floatt;
            }
            else
            {
                Vari.IntValue = System.Convert.ToInt32(Vari.Lexeme);
                Vari.Token = Vari.Symbol.integert;
            }
        }

        /*************************************************************
        ** Function    : ProcessComment                             **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the comment string values.                 **
        *************************************************************/
        public void processComment()
        {
            while (Vari.ch != 10 && !reader.EndOfStream)
            {
                getNextChar();
            }

            Vari.Token = Vari.Symbol.whitespace;
        }

        /*************************************************************
       ** Function    : ProcessStringLiteral                       **
       ** Inputs      : None                                       **
       ** Return      : Void                                       **
       **************************************************************
       ** Description : Processes the Tokens which are considered  **
       **               the literal string values.                 **
       *************************************************************/
        public void processStringLiteral()
        {
            bool hasEnding = false;
            Vari.LiteralValue = "";

            while (Vari.ch != 10 && !reader.EndOfStream && Vari.ch != '"')
            {
                if (Vari.Lexeme.Length < 17)
                    Vari.Lexeme += Vari.ch;

                Vari.LiteralValue += Vari.ch;
                getNextChar();

                if (Vari.ch == '"')
                {
                    hasEnding = true;
                    getNextChar();
                }
            }

            if (!hasEnding)
            {
                Vari.Token = Vari.Symbol.unknownt;
                Console.WriteLine("Error on line " + Vari.lineNo + ": incomplete literal, missing closing");
            }
            else
            {
                Vari.Token = Vari.Symbol.literal;
            }
        }

        /*************************************************************
        ** Function    : ProcessDoubleToken                         **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Processes the Tokens which are considered  **
        **               the double token string values.            **
        *************************************************************/
        public void processDoubleToken()
        {
            if (Vari.Lexeme[0] == '<' || Vari.Lexeme[0] == '>' || Vari.Lexeme[0] == '/')
                Vari.Token = Vari.Symbol.relopt;
            else if (Vari.Lexeme[0] == ':')
                Vari.Token = Vari.Symbol.assignopt;

            getNextChar();
        }

        /*************************************************************
         ** Function    : ProcessSingleToken                         **
         ** Inputs      : None                                       **
         ** Return      : Void                                       **
         **************************************************************
         ** Description : Processes the Tokens which are considered  **
         **               the single token string values.            **
         *************************************************************/
        public void processSingleToken()
        {
            if (Vari.Lexeme[0] == '+' || Vari.Lexeme[0] == '-')
                Vari.Token = Vari.Symbol.signopt;
            else if (Vari.Lexeme[0] == '<' || Vari.Lexeme[0] == '>' || Vari.Lexeme[0] == '=')
                Vari.Token = Vari.Symbol.relopt;
            else if (Vari.Lexeme[0] == '*' || Vari.Lexeme[0] == '/')
                Vari.Token = Vari.Symbol.mulopt;
            else if (Vari.Lexeme[0] == '.')
                Vari.Token = Vari.Symbol.period;
            else if (Vari.Lexeme[0] == '(')
                Vari.Token = Vari.Symbol.openParent;
            else if (Vari.Lexeme[0] == ')')
                Vari.Token = Vari.Symbol.closeParent;
            else if (Vari.Lexeme[0] == ',')
                Vari.Token = Vari.Symbol.commat;
            else if (Vari.Lexeme[0] == ':')
                Vari.Token = Vari.Symbol.colont;
            else if (Vari.Lexeme[0] == ';')
                Vari.Token = Vari.Symbol.semit;
            else if (Vari.Lexeme[0] == '"')
                Vari.Token = Vari.Symbol.quotet;
            else
                Vari.Token = Vari.Symbol.unknownt;
        }
    }
}

