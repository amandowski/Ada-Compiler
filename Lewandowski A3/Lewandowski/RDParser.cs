/**************************************************************
 ** Name        : Amanda Lewandowski                         **
 ** Due Date    : February 22, 2020                          **
 ** Course      : CSc 446 - Compiler Construction            **
 ** Instructor  : George Hamer                               **
 ** Assign No.  : 3 - RD Parser                              **
 ** File Name   : RDParser.CS                                **
 **************************************************************
 ** Description : This is the RD parser for assign 3         **
 *************************************************************/
 
 
 using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LewandowskiA3
{
    public class RDParser
    {
        Lexical myLex;
        public RDParser(Lexical Lex)
        {
            myLex = Lex;
        }


        /*************************************************************
        ** Function    : Match                                      **
        ** Inputs      : Symbol                                     **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Checks if the wantedSymbol is equal to the **
        **               currentObject Symbol, else Error Message.  **
        *************************************************************/
        public void Match(Symbol desired)
        {
            //Console.WriteLine("Token: " + myLex.token + "   match");
            if (myLex.token == desired)
                myLex.GetNextToken();
            else
            {
                Console.WriteLine("Error Expected: " + myLex.token + " and instead got: " + desired);
                Environment.Exit(1);
                return;
            }
        }

        /*************************************************************
        ** Function    : Prog                                       **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **               Prog -->  procedure idt Args is            **
		**		                              DeclarativePart       **
        **                                    Procedures            **
        **                                    begin                 **
        **                                    SeqOfStatements       **
        **                                    end idt;              **
        *************************************************************/
        public void Prog()
        {
            Match(Symbol.proceduret);
            Match(Symbol.idt);
            Args();
            Match(Symbol.ist);
            DeclarativePart();
            Procedures();
            Match(Symbol.begint);
            SeqofStats();
            Match(Symbol.endt);
            Match(Symbol.idt);
            Match(Symbol.semit);
            
        }


        /*************************************************************
        ** Function    : DeclarativePart                            **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **               DeclarativePart -->  IdentifierList :      **
		**		                              TypeMark ;            **
        **                                    DeclarativePart | e   **
        *************************************************************/
        public void DeclarativePart()
        {
            if (myLex.token == Symbol.idt)
            {
                IdentifierList();
                Match(Symbol.colont);
                TypeMark();
                Match(Symbol.semit);
                DeclarativePart();
               /* if (myLex.token == Symbol.colont)
                {
                    Match(Symbol.colont);
                    TypeMark();
                    if (myLex.token == Symbol.semit)
                    {
                        Match(Symbol.semit);
                        DeclarativePart();
                    }
                }*/
            }
            else
            {
                return; //null term
            }
        }

        /*************************************************************
        ** Function    : IdentifierList                             **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **               IdentifierList -->   idt |                 **
		**		                              IdentifierList , idt  **
        *************************************************************/
        public void IdentifierList()
        {
            Match(Symbol.idt);
            if (myLex.token == Symbol.commat)
            {
                Match(Symbol.commat);
                IdentifierList();
            }
        }

        /*************************************************************
        ** Function    : TypeMark                                   **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **               TypeMark --> integert, realt, chart,       **
        **                            const assignop Value          **
        *************************************************************/
        public void TypeMark()
        {

            switch(myLex.token)
            {
                case Symbol.integert:
                    Match(Symbol.integert);
                    break;
                case Symbol.floatt:
                    Match(Symbol.floatt);
                    break;
                case Symbol.chart:
                    Match(Symbol.chart);
                    break;
                case Symbol.constantt:
                    Match(Symbol.constantt);
                    break;
                default:
                    Console.WriteLine("Error Unepected type on line: " + myLex.lineNo + "with type: " + myLex.token);
                    Environment.Exit(1);
                    break;
            }
            //Console.WriteLine("it got to type mark before crashing");
            /*if (myLex.token == Symbol.integert)
            {
                Match(Symbol.integert);
            }
            else if (myLex.token == Symbol.floatt)
            {
                Match(Symbol.floatt);
            }
            else if (myLex.token == Symbol.chart)
            {
                Match(Symbol.chart);
            }
            else if (myLex.token == Symbol.constantt)
            {
                Match(Symbol.constantt);
            }
            else
            {
                Console.WriteLine("Expecting either interger, real, char, or assignop token at line #" + myLex.lineNo + ", but found " + myLex.token + " instead");
                //Environment.Exit(1);
            }*/
            if(myLex.token == Symbol.assignopt)
            {
                Match(Symbol.assignopt);
                Value();
            }
            else
            {
                return;
            }
                
        }

        /*************************************************************
        ** Function    : Value                                      **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Starts the Grammar:                        **
        **                     Value --> NumericalLiteral           **
        *************************************************************/
        public void Value()
        {
            NumericalLiteral();
        }

        /*************************************************************
        ** Function    : Value                                      **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     Value --> NumericalLiteral           **
        *************************************************************/
        public void NumericalLiteral()
        {
            Match(Symbol.integert);
        }

        /*************************************************************
        ** Function    : Procedures                                 **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     Procedures -> Prog Procedures | e    **
        *************************************************************/
        public void Procedures()
        {
            if (myLex.token == Symbol.proceduret)
            {
                Prog();
                Procedures();
            }
            else
            { } //null
        }

        /*************************************************************
        ** Function    : Args                                       **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     Args -> ( ArgList ) | e              **
        *************************************************************/
        public void Args()
        {
            if (myLex.token == Symbol.lparent)
            {
                Match(Symbol.lparent);
                ArgList();
                Match(Symbol.rparent);
            }
            else
                return; //null
        }

        /*************************************************************
        ** Function    : ArgList                                    **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     ArgList -> Mode IdentifierList :     **
        **                                TypeMark MoreArgs         **
        *************************************************************/
        public void ArgList()
        {
            Mode();
            IdentifierList();
            Match(Symbol.colont);
            TypeMark();
            MoreArgs();
        }

        /*************************************************************
        ** Function    : MoreArgs                                   **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     MoreArgs -> ; ArgList | e            **
        *************************************************************/
        public void MoreArgs()
        {
            if (myLex.token == Symbol.semit)
            {
                Match(Symbol.semit);
                ArgList();
            }
            else
                return; //null

        }

        /*************************************************************
        ** Function    : Mode                                       **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     Mode -> in | out | inout | e         **
        *************************************************************/
        public void Mode()
        {
            switch(myLex.token)
            {
                case Symbol.intt:
                    Match(Symbol.intt);
                    break;
                case Symbol.outt:
                    Match(Symbol.outt);
                    break;
                case Symbol.inoutt:
                    Match(Symbol.inoutt);
                    break;
                default:
                    break; // null

            }
        }

        /*************************************************************
        ** Function    : SeqofStats                                 **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **               SeqofStats --> empty                       **
        *************************************************************/
        public void SeqofStats()
        {
            //null
        }
    }
}
