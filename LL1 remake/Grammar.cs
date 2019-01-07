using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL1_remake
{
    class Grammar
    {
        public string StartSymbol;
        public List<string> Nonterminals = new List<string>();
        public List<string> Terminals = new List<string>();
        public int RulesNumber;
        public List<List<string>> ProductionRules = new List<List<string>>();

        private char delimiterChar;
        private string newNonterminal;
        private List<List<string>> alfa;
        private List<List<string>> beta;
        public bool test_grammar = false;

        public void CreateRules (string rule)
        {
            List<String> entityrule = new List<string>();
            delimiterChar = ':';
            string[] temp = rule.Split(new char[] { ':' }, 2);
            entityrule.Add(temp[0]);
            entityrule.Add(":");
            if (rule.Count() > 2)
            {
                delimiterChar = ' ';
                entityrule.AddRange(temp[1].Split(delimiterChar));
            }
            ProductionRules.Add(entityrule);
        }

        public void PrintInformation (ref Information myWindow)
        {
            myWindow.Your_code.AppendText($"Simbolul de start este:\n{StartSymbol}\n");
            myWindow.Your_code.AppendText("\nNeterminalele sunt:\n");
            foreach (string i in Nonterminals) myWindow.Your_code.AppendText($"{i}\n");
            myWindow.Your_code.AppendText("\nTerminalele sunt:\n");
            foreach (string i in Terminals) myWindow.Your_code.AppendText($"{i}\n");
            myWindow.Your_code.AppendText("\nRegulile de productie sunt:\n");
            foreach (List<string> i in ProductionRules)
            {
                foreach (string j in i)
                {
                    myWindow.Your_code.AppendText($"{j} ");
                }
                myWindow.Your_code.AppendText("\n");
            }
        }

        public bool LeftRecursion ()
        {
            List<List<string>> copyRules = new List<List<string>>(ProductionRules);
            bool found;
            bool state = false;
            int index = 0;
            for (int i=0; i<RulesNumber; i++)
            {
                alfa = new List<List<string>>();
                beta = new List<List<string>>();
                found = false;
                if (ProductionRules[i].Count() < 3) continue;
                if (ProductionRules[i][0]==ProductionRules[i][2])
                {
                    found = true;
                    state = true;
                    List<string> temp = new List<string>();
                    for (int j = 3; j < ProductionRules[i].Count(); j++)
                        temp.Add(ProductionRules[i][j]);
                    alfa.Add(temp);
                }
                if (found == true)
                {
                    for (int j = 0; j < RulesNumber; j++)
                    {
                        if (i == j) continue;
                        if (ProductionRules[i][0] == ProductionRules[j][0])
                        {
                            if (ProductionRules[j].Count() < 3) continue;
                            if (ProductionRules[j][0] == ProductionRules[j][2])
                            {
                                List<string> temp = new List<string>();
                                for (int k = 3; j < ProductionRules[j].Count(); k++)
                                    temp.Add(ProductionRules[j][k]);
                                alfa.Add(temp);
                            }
                            else
                            {
                                List<string> temp = new List<string>();
                                for (int k = 2; k < ProductionRules[j].Count(); k++)
                                    temp.Add(ProductionRules[j][k]);
                                beta.Add(temp);
                            }
                            copyRules.Remove(ProductionRules[j]);
                            index = j;
                        }
                    }
                }
                if (found == true)
                {
                    copyRules.Remove(ProductionRules[i]);
                    List<string> temp;

                    foreach (List<string> b in beta)
                    {
                        temp = new List<string>();
                        temp.Add(ProductionRules[i][0]);
                        temp.Add(":");
                        temp.AddRange(b);
                        newNonterminal = ProductionRules[i][0] + "1";
                        temp.Add(newNonterminal);
                        copyRules.Add(temp);
                    }

                    foreach (List<string> a in alfa)
                    {
                        temp = new List<string>();
                        newNonterminal = ProductionRules[i][0] + "1";
                        temp.Add(newNonterminal);
                        temp.Add(":");
                        temp.AddRange(a);
                        temp.Add(newNonterminal);
                        copyRules.Add(temp);
                    }

                    temp = new List<string>();
                    temp.Add(newNonterminal.ToString());
                    temp.Add(":");
                    copyRules.Add(temp);

                    Nonterminals.Add(newNonterminal);
                }
            }
            ProductionRules = copyRules;
            RulesNumber = ProductionRules.Count();
            return state;
        }

        public bool CommonPrefixes()
        {
            List<List<string>> copyRules = new List<List<string>>(ProductionRules);
            bool found;
            bool state = false;
            int index = 0;
            for (int i = 0; i < RulesNumber; i++)
            {
                if (ProductionRules[i].Count() < 3) continue;
                alfa = new List<List<string>>();
                beta = new List<List<string>>();
                found = false;
                for (int j = 0; j < RulesNumber; j++)
                {
                    if (ProductionRules[j].Count() < 3) continue;
                    if (i == j) continue;
                    if (ProductionRules[i][0]==ProductionRules[j][0])
                    {
                        if (found == false)
                        {
                            if (ProductionRules[i][2] == ProductionRules[j][2])
                            {
                                found = true;
                                state = true;
                                List<string> temp = new List<string>();
                                int k = 2;
                                int p = 0;
                                int min;
                                if (ProductionRules[i].Count() > ProductionRules[j].Count()) min = ProductionRules[j].Count() - 1;
                                else min = ProductionRules[i].Count() - 1;
                                while (k <= min)
                                {
                                    if (ProductionRules[i][k] == ProductionRules[j][k])
                                    {
                                        temp.Add(ProductionRules[i][k]);
                                        k++;
                                        p = k;
                                    }
                                    else break;
                                }
                                beta.Add(temp);

                                temp = new List<string>();
                                k = p;
                                while (k<ProductionRules[i].Count())
                                {
                                    temp.Add(ProductionRules[i][k]);
                                    k++;
                                }
                                alfa.Add(temp);

                                temp = new List<string>();
                                k = p;
                                while (k < ProductionRules[j].Count())
                                {
                                    temp.Add(ProductionRules[j][k]);
                                    k++;
                                }
                                alfa.Add(temp);

                                copyRules.Remove(ProductionRules[j]);
                                index = j;
                            }
                        }
                        else
                        {
                            int k = 2;
                            bool ok = true;
                            for (k = 2; k < beta[0].Count() + 2; k++) 
                            {
                                if (ProductionRules[j].Count() - 2 < beta[0].Count()) 
                                {
                                    ok = false;
                                    break;
                                }
                                if (ProductionRules[j][k] != beta[0][k - 2]) ok = false;
                            }
                            if (ok == true)
                            {
                                List<string> temp = new List<string>();
                                k = beta[0].Count() + 2;
                                while (k < ProductionRules[j].Count())
                                {
                                    temp.Add(ProductionRules[j][k]);
                                    k++;
                                }
                                alfa.Add(temp);

                                copyRules.Remove(ProductionRules[j]);
                                index = j;
                            }
                        }
                    }
                }
                if (found == true)
                {
                    copyRules.Remove(ProductionRules[i]);
                    List<string> temp;


                    temp = new List<string>();
                    temp.Add(ProductionRules[i][0]);
                    temp.Add(":");
                    temp.AddRange(beta[0]);
                    newNonterminal = ProductionRules[i][0] + "1";
                    temp.Add(newNonterminal);
                    copyRules.Add(temp);

                    foreach (List<string> a in alfa)
                    {
                        temp = new List<string>();
                        temp.Add(newNonterminal);
                        temp.Add(":");
                        temp.AddRange(a);
                        copyRules.Add(temp);
                    }


                    Nonterminals.Add(newNonterminal);
                    i = index + 1;
                }
            }
            ProductionRules = copyRules;
            RulesNumber = ProductionRules.Count();
            return state;
        }
    }
}
