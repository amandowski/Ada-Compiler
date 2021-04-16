/**************************************************************
 ** Name        : Amanda Lewandowski                         **
 ** Due Date    : March 31, 2020                             **
 ** Course      : CSc 446 - Compiler Construction            **
 ** Instructor  : George Hamer                               **
 ** Assign No.  : 5 - Depth                                  **
 ** File Name   : RDParser.CS                                **
 **************************************************************
 ** Description : This is the RD parser                      **
 *************************************************************/

using System;
using System.Collections.Generic;

namespace LewandowskiA5
{
    class Parser
    {
        public bool isProc = false;
        public bool isConst = false;
        public SymbolTable ObjTable = new SymbolTable();
        public List<string> names = new List<string>();
        public ProcedureEntry tempP;
        public Stack<ProcedureEntry> procNames = new Stack<ProcedureEntry>();

        public List<int> paramTotalSize = new List<int>(0);
        public List<int> varTotalSize = new List<int>(0);
        public bool isParam = false;
        public bool isSOS = false;

        public Parser()
        {
            Vari.myLex.getNextToken();    // prime the parser
            prog();
            ObjTable.A5Display();

            if (Vari.Token != Vari.Symbol.eoftt)
            {
                Console.WriteLine("Error: Unused tokens on line " + Vari.lineNo);
            }
            else
            {
                Console.WriteLine("\nCompiled");
            }
        }

        /*************************************************************
         ** Function    : Match                                      **
         ** Inputs      : Symbol                                     **
         ** Return      : Void                                       **
         **************************************************************
         ** Description : Checks if the wantedSymbol is equal to the **
         **               currentObject Symbol, else Error Message.  **
         *************************************************************/
        void Match(Vari.Symbol desiredSym)
        {
            if (Vari.Token == desiredSym)
            {
                if (!isSOS)
                {
                    int namesSize;
                    switch (Vari.Token)
                    {
                        case Vari.Symbol.proceduret:
                            isProc = true;
                            break;
                        case Vari.Symbol.idt:
                            if (isProc)
                            {
                                checkDuplicates(Vari.Lexeme);
                                Vari.entryType = EntryType.procEntry;
                                ObjTable.insert(Vari.Lexeme, Vari.Token, Vari.depth);
                                newProc(Vari.Lexeme);
                            }
                            else
                            { names.Add(Vari.Lexeme); }
                            break;
                        case Vari.Symbol.integert:
                            if (isConst)
                            {
                                Vari.entryType = EntryType.constEntry;
                                namesSize = names.Count;
                                for (int i = 0; i < namesSize; ++i)
                                {
                                    checkDuplicates(names[0]);
                                    ObjTable.insert(names[0], Vari.Token, Vari.depth);
                                    newConst(names[0], "integer");
                                    names.RemoveAt(0);
                                }
                                isConst = false;
                            }
                            else
                            {
                                Vari.entryType = EntryType.varEntry;
                                Vari.varType = VarType.intType;
                                namesSize = names.Count;
                                for (int i = 0; i < namesSize; ++i)
                                {
                                    checkDuplicates(names[0]);
                                    ObjTable.insert(names[0], Vari.Token, Vari.depth);
                                    newVar(names[0]);
                                    names.RemoveAt(0);
                                }
                            }
                            break;
                        case Vari.Symbol.floatt:
                            if (isConst)
                            {
                                Vari.entryType = EntryType.constEntry;
                                namesSize = names.Count;
                                for (int i = 0; i < namesSize; ++i)
                                {
                                    checkDuplicates(names[0]);
                                    ObjTable.insert(names[0], Vari.Token, Vari.depth);
                                    newConst(names[0], "real");
                                    names.RemoveAt(0);
                                }
                                isConst = false;
                            }
                            else
                            {
                                Vari.entryType = EntryType.varEntry;
                                Vari.varType = VarType.floatType;
                                namesSize = names.Count;
                                for (int i = 0; i < namesSize; ++i)
                                {
                                    checkDuplicates(names[0]);
                                    ObjTable.insert(names[0], Vari.Token, Vari.depth);
                                    newVar(names[0]);
                                    names.RemoveAt(0);
                                }
                            }
                            break;
                        case Vari.Symbol.chart:
                            Vari.entryType = EntryType.varEntry;
                            Vari.varType = VarType.charType;
                            namesSize = names.Count;
                            for (int i = 0; i < namesSize; ++i)
                            {
                                checkDuplicates(names[0]);
                                ObjTable.insert(names[0], Vari.Token, Vari.depth);
                                newVar(names[0]);

                                names.RemoveAt(0);
                            }
                            break;
                        case Vari.Symbol.constantt:
                            isConst = true;
                            break;
                        case Vari.Symbol.ist:
                            if (isProc)
                            {
                                incrementDepth();
                                isProc = false;
                            }
                            break;
                        case Vari.Symbol.openParent:
                            if (isProc)
                            {
                                incrementDepth();
                                isProc = false;
                                isParam = true;
                            }
                            break;
                        case Vari.Symbol.closeParent:
                            isParam = false;
                            break;
                        case Vari.Symbol.intt:
                            tempP.passing.Add(ModeType.inMode);
                            break;
                        case Vari.Symbol.inoutt:
                            tempP.passing.Add(ModeType.inoutMode);
                            break;
                        case Vari.Symbol.outt:
                            tempP.passing.Add(ModeType.outMode);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    if (Vari.Token == Vari.Symbol.idt)
                    {
                        int row = ObjTable.hashWrap(Vari.Lexeme);
                        int col = ObjTable.lookup(Vari.Lexeme);

                        if (col == -1)
                        {
                            Console.WriteLine("Error: there is an undeclared variable: " + Vari.Lexeme + " on line: " + Vari.lineNo);
                        }
                    }

                }

                Vari.myLex.getNextToken();
            }
            else if (Vari.Token == Vari.Symbol.eoftt)
            {
                return;
            }
            else
            {
                Console.WriteLine("Received: " + Vari.Token + " but expected: " + desiredSym);
                Environment.Exit(1);
            }
        }

        /*************************************************************
        ** Function    : Mode                                       **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     Mode -> in | out | inout | e         **
        *************************************************************/
        void mode()
        {
            switch (Vari.Token)
            {
                case Vari.Symbol.intt:
                    Match(Vari.Symbol.intt);
                    break;
                case Vari.Symbol.outt:
                    Match(Vari.Symbol.outt);
                    break;
                case Vari.Symbol.inoutt:
                    Match(Vari.Symbol.inoutt);
                    break;
                default:
                    break;      // null terminal
            }
        }

        /*************************************************************
        ** Function    : MoreArgs                                   **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     MoreArgs -> ; ArgList | e            **
        *************************************************************/
        void moreArgs()
        {
            if (Vari.Token == Vari.Symbol.semit)
            {
                Match(Vari.Symbol.semit);
                argList();
            }
            else
            {
                return; // null terminal
            }
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
        void argList()
        {
            mode();
            IdentifierList();
            Match(Vari.Symbol.colont);
            typeMark();
            moreArgs();
        }

        /*************************************************************
        ** Function    : Args                                       **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     Args -> ( ArgList ) | e              **
        *************************************************************/
        void Args()
        {
            if (Vari.Token == Vari.Symbol.openParent)
            {
                Match(Vari.Symbol.openParent);
                argList();
                Match(Vari.Symbol.closeParent);
            }
            else
            {
                return;     // null
            }
        }

        /*************************************************************
        ** Function    : Procedures                                 **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **                     Procedures -> Prog Procedures | e    **
        *************************************************************/
        void Procedures()
        {
            // procedures -> prog procedures | e
            if (Vari.Token == Vari.Symbol.proceduret)
            {
                prog();
                Procedures();
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
        ** Description : Completes the Grammar:                     **
        **                     Value --> NumericalLiteral           **
        *************************************************************/
        void value()
        {
            switch (Vari.Token)
            {
                case Vari.Symbol.integert:
                    Match(Vari.Symbol.integert);
                    break;
                case Vari.Symbol.floatt:
                    Match(Vari.Symbol.floatt);
                    break;
                case Vari.Symbol.literal:
                    Match(Vari.Symbol.literal);
                    break;
                case Vari.Symbol.chart:
                    Match(Vari.Symbol.chart);
                    break;
                default:
                    Console.WriteLine(" !! ERROR: Unexpected type on line " + Vari.lineNo);
                    Environment.Exit(1);
                    break;
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
        void typeMark()
        {
            switch (Vari.Token)
            {
                case Vari.Symbol.integert:
                    Match(Vari.Symbol.integert);
                    break;
                case Vari.Symbol.floatt:
                    Match(Vari.Symbol.floatt);
                    break;
                case Vari.Symbol.chart:
                    Match(Vari.Symbol.chart);
                    break;
                case Vari.Symbol.constantt:
                    Match(Vari.Symbol.constantt);
                    break;
                default:
                    Console.WriteLine("!! ERROR: Unexpected type on line " + Vari.lineNo);
                    Environment.Exit(1);
                    break;
            }

            if (Vari.Token == Vari.Symbol.assignopt)
            {
                Match(Vari.Symbol.assignopt);
                value();
            }
            else
                return;     // null term
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
        void IdentifierList()
        {
            // idList -> idt | idList , idt
            Match(Vari.Symbol.idt);

            if (Vari.Token == Vari.Symbol.commat)
            {
                Match(Vari.Symbol.commat);
                IdentifierList();
            }
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
        void DeclarativePart()
        {
            if (Vari.Token == Vari.Symbol.idt)
            {
                IdentifierList();
                Match(Vari.Symbol.colont);
                typeMark();
                Match(Vari.Symbol.semit);
                DeclarativePart();
            }
            else
                return;
        }

        /*************************************************************
        ** Function    : SeqofStats                                 **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Completes the Grammar:                     **
        **               SeqofStats --> empty                       **
        *************************************************************/
        void SeqOfStats()
        {
            // sos -> Statement ; StatTail | e
            if (Vari.Token == Vari.Symbol.idt)
            {
                isSOS = true;
                statement();
                match(Vari.Symbol.semit);
                statTail();
                isSOS = false;
            }
            else { return; }
        }
        
        void statTail()
        {
            // statTail -> Statement ; StatTail | e
            if (Vari.Token == Vari.Symbol.idt)
            {
                statement();
                Match(Vari.Symbol.semit);
                statTail();
            }
            else { return; }
        }

        void statement()
        {
            // statement -> assignStat | IOStat
            if(Vari.Token == Vari.Symbol.idt)
            {
                assignStat();
            } else
            {
                ioStat();
            }
        }

        void assignStat()
        {
            // assignStat -> idt := Expr
            Match(Vari.Symbol.idt);
            Match(Vari.Symbol.assignopt);
            expr();
        }
        void ioStat()
        {
            // ioStat -> e
            return;
        }
        void expr()
        {
            // expr -> relation
            relation();
        }

        void relation()
        {
            // relation -> simpleExpr
            simpleExpr();
        }

        void simpleExpr()
        {
            // simpleExpr -> term moreTerm
            term();
            moreTerm();
        }

        void moreTerm()
        {
            // moreTerm -> addopt Term MoreTerm | e
            if(Vari.Token == Vari.Symbol.addopt || Vari.Token == Vari.Symbol.signopt)
            {
                if(Vari.Token == Vari.Symbol.addopt)
                {
                    Match(Vari.Symbol.addopt);
                } else
                {
                    Match(Vari.Symbol.signopt);
                }
                term();
                moreTerm();
            }
            else { return; }
        }

        void term()
        {
            // term -> Factor MoreFactor
            factor();
            moreFactor();
        }

        void moreFactor()
        {
            // moreFactor -> mulopt Factor MoreFactor | e
            if (Vari.Token == Vari.Symbol.mulopt)
            {
                Match(Vari.Symbol.mulopt);
                factor();
                moreFactor();
            }
            else { return; }
        }

        // TODO: last two conditions
        void factor()
        {
            // factor ->    idt |
            //              numt|
            //              (Expr) |
            //              nott Facotr |
            //              signopt Factor

            if (Vari.Token == Vari.Symbol.idt)
            {
                Match(Vari.Symbol.idt);
            }
            else if (Vari.Token == Vari.Symbol.floatt)
            {
                Match(Vari.Symbol.floatt);
            }
            else if (Vari.Token == Vari.Symbol.integert)
            {
                Match(Vari.Symbol.integert);
            } else if(Vari.Token == Vari.Symbol.openParent)
            {
                Match(Vari.Symbol.openParent);
                expr();
                Match(Vari.Symbol.closeParent);
            } else if(Vari.Token == Vari.Symbol.nott)
            {
                Match(Vari.Symbol.nott);
                factor();
            }
            else if(Vari.Token == Vari.Symbol.signopt)
            {
                Match(Vari.Symbol.signopt);
                factor();
            }
            
        }

        // Prog() is responsible for directing the grammar and calls several other
        // grammar variable functions, along with matching syntaxes for the program.
        void prog()
        {
            Match(Vari.Symbol.proceduret);
            Match(Vari.Symbol.idt);
            Args();
            Match(Vari.Symbol.ist);

            DeclarativePart();
            Procedures();

            Match(Vari.Symbol.begint);
            SeqOfStats();
            Match(Vari.Symbol.endt);
            endProc();
            Match(Vari.Symbol.idt);
            Match(Vari.Symbol.semit);

            decrementDepth();
        }

        /*************************************************************
        ** Function    : IncrementDepth                             **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Increments the depth                       **
        *************************************************************/
        void incrementDepth()
        {
            Vari.offset.Add(0);
            paramTotalSize.Add(0);
            varTotalSize.Add(0);
            Vari.depth++;
        }

        /*************************************************************
        ** Function    : DecrementDepth                             **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : Decrements the depth                       **
        *************************************************************/
        void decrementDepth()
        {
            ObjTable.A5Display();
            ObjTable.deleteDepth(Vari.depth);
            Vari.offset.RemoveAt(Vari.depth - 1);
            Vari.depth--;
        }

        /*************************************************************
        ** Function    : CheckDuplicates                            **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : checks for duplicates                      **
        *************************************************************/
        void checkDuplicates(string lex)
        {
            int x = ObjTable.hashWrap(lex);
            for (int i = 0; i < ObjTable.theSymbolTable[x].Count; ++i)
            {
                EntryNode temp = ObjTable.theSymbolTable[x][i];
                if (temp.depth == Vari.depth && lex == temp.lexeme)
                {
                    Console.WriteLine("Error: There is a duplicate id: " + lex + " on line: " + Vari.lineNo);
                }
            }
        }

        /*************************************************************
        ** Function    : newVar                                     **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : makes a new variable entry                 **
        *************************************************************/
        void newVar(string lex)
        {
            VarEntry temp = new VarEntry();
            int subList = ObjTable.lookup(lex);
            int mainList = ObjTable.hashWrap(lex);
            temp.token = Vari.Token;
            temp.lexeme = lex;
            temp.depth = Vari.depth;
            temp.type = Vari.entryType;

            switch (Vari.varType)
            {
                case VarType.charType:
                    temp.size = 1;
                    temp.variableType = VarType.charType;
                    break;
                case VarType.intType:
                    temp.size = 2;
                    temp.variableType = VarType.intType;
                    break;
                case VarType.floatType:
                    temp.size = 4;
                    temp.variableType = VarType.floatType;
                    break;
                default:
                    break;
            }

            if (isParam)
            {
                paramTotalSize[Vari.depth - 1] += temp.size;
                tempP.toparams.Add(temp.variableType);
            }
            else
            {
                temp.offset = Vari.offset[Vari.depth - 1];
                Vari.offset[Vari.depth - 1] += temp.size;
            }

            ObjTable.theSymbolTable[mainList][subList] = temp;
        }

        /*************************************************************
        ** Function    : newConst                                   **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : makes new const entry                      **
        *************************************************************/
        void newConst(string lex, string numType)
        {
            ConstEntry temp = new ConstEntry();
            int subList = ObjTable.lookup(lex);
            int mainList = ObjTable.hashWrap(lex);
            temp.token = Vari.Token;
            temp.lexeme = lex;
            temp.depth = Vari.depth;
            temp.type = Vari.entryType;

            switch (numType)
            {
                case "integer":
                    temp.iValue = Vari.IntValue;
                    break;
                case "real":
                    temp.fValue = Vari.RealValue;
                    break;
            }

            ObjTable.theSymbolTable[mainList][subList] = temp;
        }

        /*************************************************************
        ** Function    : newProc                                    **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : makes a new procedure entry                **
        *************************************************************/
        void newProc(string lex)
        {
            tempP = new ProcedureEntry();

            int subList = ObjTable.lookup(lex);
            int mainList = ObjTable.hashWrap(lex);

            List<int> tempLocs = new List<int>();
            tempLocs.Add(mainList);
            tempLocs.Add(subList);

            tempP.token = Vari.Token;
            tempP.lexeme = lex;
            tempP.depth = Vari.depth;
            tempP.type = Vari.entryType;

            tempP.solocals = 0;
            tempP.soparams = 0;
            tempP.noparams = 0;
            tempP.toparams = new List<VarType>();
            tempP.passing = new List<ModeType>();

            procNames.Push(tempP);
            ObjTable.theSymbolTable[mainList][subList] = tempP;
        }

        /*************************************************************
        ** Function    : endProc                                    **
        ** Inputs      : None                                       **
        ** Return      : Void                                       **
        **************************************************************
        ** Description : closes the procedure entry                 **
        *************************************************************/
        void endProc()
        {
            tempP.noparams = tempP.toparams.Count;
            tempP.solocals = Vari.offset[Vari.depth - 1];
            for (int i = 0; i < tempP.toparams.Count; ++i)
            {
                VarType type = tempP.toparams[i];
                if (type == VarType.charType)
                {
                    tempP.soparams += 1;
                }
                else if (type == VarType.intType)
                {
                    tempP.soparams += 2;
                }
                else if (type == VarType.floatType)
                {
                    tempP.soparams += 4;
                }
            }

            int subList = ObjTable.lookup(tempP.lexeme);
            int mainList = ObjTable.hashWrap(tempP.lexeme);

            while (subList < ObjTable.theSymbolTable[mainList].Count && tempP.depth != ObjTable.theSymbolTable[mainList][subList].depth)
            {
                ++subList;
            }


            ObjTable.theSymbolTable[mainList][subList] = tempP;

            if (Vari.Lexeme != tempP.lexeme)
            {
                Console.WriteLine("Error: The Procedure name: " + tempP.lexeme + " does not match the end procedure name: " + Vari.Lexeme);
                Environment.Exit(1);
            }

            procNames.Pop();
            if (procNames.Count != 0)
            {
                tempP = procNames.Peek();
            }
        }
    }
}
