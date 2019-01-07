using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL1_remake
{
    class ParsingTable
    {
        public Grammar GrammarToBoChecked;
        private List<List<string>> first = new List<List<string>>();
        private List<List<string>> follow = new List<List<string>>();
        public List<List<int>> parsingtable = new List<List<int>>();

        private bool isTerminal(string element)
        {
            return GrammarToBoChecked.Terminals.Contains(element);
        }

        private bool isNonterminal(string element)
        {
            return GrammarToBoChecked.Nonterminals.Contains(element);
        }

        public void CalculateFirst_a()
        {
            List<string> temp;
            for (int i = 0; i < GrammarToBoChecked.RulesNumber; i++)
            {
                if (GrammarToBoChecked.ProductionRules[i].Count() == 2)
                {
                    temp = new List<string>();
                    temp.Add(i.ToString());
                    temp.Add(GrammarToBoChecked.ProductionRules[i][0]);
                    temp.Add("Empty");
                    first.Add(temp);
                    continue;
                }
                if (isTerminal(GrammarToBoChecked.ProductionRules[i][2]))
                {
                    temp = new List<string>();
                    temp.Add(i.ToString());
                    temp.Add(GrammarToBoChecked.ProductionRules[i][0]);
                    temp.Add(GrammarToBoChecked.ProductionRules[i][2]);
                    first.Add(temp);
                }
                else
                {
                    temp = new List<string>();
                    temp.Add(i.ToString());
                    temp.Add(GrammarToBoChecked.ProductionRules[i][0]);
                    temp.Add("Empty");
                    first.Add(temp);
                }
            }
        }

        public void CalculateFirst_b(string element, int k, int p)
        {
            List<string> temp;
            for (int i=0; i<GrammarToBoChecked.RulesNumber; i++)
            {

                if (GrammarToBoChecked.ProductionRules[i].Count() == 2) continue;
                if (GrammarToBoChecked.ProductionRules[i][0] == element)
                {
                    if (isTerminal(GrammarToBoChecked.ProductionRules[i][2]) && first[k][2] == "Empty" && p > 0)
                    {
                        temp = new List<string>();
                        temp.Add(GrammarToBoChecked.ProductionRules[i][2]);
                        first[k].AddRange(temp);
                    }
                    else if (isNonterminal(GrammarToBoChecked.ProductionRules[i][2]))
                    {
                        p++;
                        CalculateFirst_b(GrammarToBoChecked.ProductionRules[i][2], k, p);
                        p--;
                    }
                }
            }
        }

        public void CalculateFirst()
        {
            int attempt = 0;
            CalculateFirst_a();
            for (int i=0; i<GrammarToBoChecked.RulesNumber; i++)
            {
                if (GrammarToBoChecked.ProductionRules[i].Count() == 2)
                {
                    first[i].RemoveAt(2);
                    continue;
                }
                if (first[i][2] == "Empty")
                {
                    CalculateFirst_b(GrammarToBoChecked.ProductionRules[i][0], i, attempt);
                    first[i].RemoveAt(2);
                }
            }
        }

        public void CalculateFollow_a(string element, int k, int count)
        {
            for (int i=0; i<GrammarToBoChecked.RulesNumber; i++)
            {
                if (element==GrammarToBoChecked.StartSymbol)
                {
                    List<string> temp = new List<string>();
                    temp.Add("$");
                    follow[k].AddRange(temp);
                }
                if (GrammarToBoChecked.ProductionRules[i].Contains(element))
                {
                    if (count == GrammarToBoChecked.RulesNumber) continue;
                    int p = GrammarToBoChecked.ProductionRules[i].LastIndexOf(element);
                    if (p < 2) continue;
                    for (p=2; p<GrammarToBoChecked.ProductionRules[i].Count(); p++)
                    {
                        if (GrammarToBoChecked.ProductionRules[i][p] == element)
                        {
                            if (p == GrammarToBoChecked.ProductionRules[i].Count() - 1)
                            {
                                count++;
                                CalculateFollow_a(GrammarToBoChecked.ProductionRules[i][0], k, count);
                                count--;
                                continue;
                            }
                            p++;
                            if (isTerminal(GrammarToBoChecked.ProductionRules[i][p]))
                            {
                                List<string> temp = new List<string>();
                                temp.Add(GrammarToBoChecked.ProductionRules[i][p]);
                                follow[k].AddRange(temp);
                            }
                            else
                            {
                                bool isEmpty = true;
                                for (int l = 0; l < GrammarToBoChecked.RulesNumber; l++)
                                    if (first[l][1] == GrammarToBoChecked.ProductionRules[i][p])
                                    {
                                        List<string> temp = new List<string>();
                                        for (int m = 2; m < first[l].Count(); m++)
                                        {
                                            temp.Add(first[l][m]);
                                            isEmpty = false;
                                        }
                                        follow[k].AddRange(temp);
                                    }
                                if (isEmpty == false)
                                {
                                    count++;
                                    CalculateFollow_a(GrammarToBoChecked.ProductionRules[i][0], k, count);
                                    count--;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CalculateFollow()
        {
            for (int i=0; i<GrammarToBoChecked.RulesNumber; i++)
            {
                if(first[i].Count()==2)
                {
                    List<string> temp = new List<string>();
                    temp.Add(i.ToString());
                    temp.Add(GrammarToBoChecked.ProductionRules[i][0]);
                    follow.Add(temp);
                    CalculateFollow_a(GrammarToBoChecked.ProductionRules[i][0], i,0);
                }
                else
                {
                    List<string> temp = new List<string>();
                    temp.Add(i.ToString());
                    temp.Add(GrammarToBoChecked.ProductionRules[i][0]);
                    follow.Add(temp);
                }
                follow[i] = follow[i].Distinct().ToList();
            }
        }

        public bool CalculateIntersections()
        {
            bool ok = true;
            List<string> temp = new List<string>();
            for (int i=0; i<GrammarToBoChecked.RulesNumber; i++)
            {
                temp = first[i].Intersect(follow[i]).ToList();
                if (temp.Count() > 2) 
                {
                    ok = false;
                    return ok;
                }
                for (int j = 0; j < GrammarToBoChecked.RulesNumber; j++)
                {
                    if (i == j) continue;
                    if (first[i][1]==first[j][1])
                    {
                        temp = first[i].Intersect(first[j]).ToList();
                        if (temp.Count() > 1) 
                        {
                            ok = false;
                            return ok;
                        }
                        temp = first[i].Intersect(follow[j]).ToList();
                        if (temp.Count() > 1)
                        {
                            ok = false;
                            return ok;
                        }
                    }
                }
            }
            return ok;
        }

        public void CalculateParsingTable()
        {
            for (int m = 0; m < GrammarToBoChecked.Nonterminals.Count(); m++)
            {
                List<int> temp = new List<int>();
                for (int n = 0; n < GrammarToBoChecked.Terminals.Count; n++)
                    temp.Add(-1);
                parsingtable.Add(temp);
            }
            for (int i = 0; i < GrammarToBoChecked.RulesNumber; i++)
            {
                for (int j = 2; j < first[i].Count(); j++)
                {
                    int col1 = GrammarToBoChecked.Terminals.IndexOf(first[i][j]);
                    int lin = GrammarToBoChecked.Nonterminals.IndexOf(first[i][1]);
                    parsingtable[lin][col1] = i;
                }
                for (int j = 2; j < follow[i].Count(); j++)
                {
                    int col2 = GrammarToBoChecked.Terminals.IndexOf(follow[i][j]);
                    int lin = GrammarToBoChecked.Nonterminals.IndexOf(follow[i][1]);
                    parsingtable[lin][col2] = i;
                }
            }
        }

        public void PrintParseTable(ref Parse myWindow)
        {
            myWindow.Message.Text = "Your parsing table is:";
            myWindow.Your_big_text.AppendText("\t");
            foreach (string i in GrammarToBoChecked.Terminals)
                myWindow.Your_big_text.AppendText($"{i}\t");
            myWindow.Your_big_text.AppendText("\n");
            for (int i=0; i<GrammarToBoChecked.Nonterminals.Count(); i++)
            {
                myWindow.Your_big_text.AppendText($"{GrammarToBoChecked.Nonterminals[i]}\t");
                for (int j = 0; j < GrammarToBoChecked.Terminals.Count(); j++)
                    myWindow.Your_big_text.AppendText($"{parsingtable[i][j]}\t");
                myWindow.Your_big_text.AppendText("\n");
            }
        }

        public void PrintInformation(ref Information myWindow)
        {
            myWindow.Your_code.AppendText("Multimile simbolurilor directoare sunt:\n");
            myWindow.Your_code.AppendText("\nFIRST:\n");
            foreach (List<string> i in first)
            {
                foreach (string j in i)
                    myWindow.Your_code.AppendText($"{j} ");
                myWindow.Your_code.AppendText("\n");
            }
            myWindow.Your_code.AppendText("\nFOLLOW:\n");
            foreach (List<string> i in follow)
            {
                foreach (string j in i)
                    myWindow.Your_code.AppendText($"{j} ");
                myWindow.Your_code.AppendText("\n");
            }
        }
    }
}
