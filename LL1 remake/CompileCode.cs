using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL1_remake
{
    class CompileCode
    {
        public ParsingTable lookahead;
        public Grammar rulesconstruct;
        StreamWriter code = new StreamWriter("code.cs");

        public void NonterminalFunction(string functionName)
        {
            int count;
            bool firstNonterminal;
            bool isFirstIf = true;
            bool trigger;
            code.WriteLine($"public static void {functionName} ()");
            code.WriteLine("{");
            foreach (List<string> rule in rulesconstruct.ProductionRules)
            {
                trigger = true;
                if (rule[0]==functionName)
                {
                    count = 0;
                    firstNonterminal = true;
                    for (int i = 2; i < rule.Count(); i++)
                    {
                        if (i > 2) trigger = false;
                        if (rulesconstruct.Terminals.Contains(rule[i]))
                        {
                            if (isFirstIf == false && trigger == true) code.WriteLine($"else if (prop[poz]==\"{rule[i]}\") ");
                            else code.WriteLine($"if (prop[poz]==\"{rule[i]}\") ");
                            code.WriteLine("{");
                            count++;
                            firstNonterminal = false;
                            if (rulesconstruct.StartSymbol == functionName && i == rule.Count() - 1) code.WriteLine("return;");
                            else if (i == rule.Count() - 1) 
                            {
                                code.WriteLine("poz++;");
                                code.WriteLine("return;");
                            }
                            else code.WriteLine("poz++;");
                        }
                        else if(rulesconstruct.Nonterminals.Contains(rule[i]))
                        {
                            if (firstNonterminal == true)
                            {
                                string temp = string.Empty;
                                for (int j = 0; j < rulesconstruct.Terminals.Count(); j++)
                                    if (lookahead.parsingtable[rulesconstruct.Nonterminals.IndexOf(functionName)][j] == rulesconstruct.ProductionRules.IndexOf(rule))
                                    {
                                        temp = temp + "prop[poz]==\"";
                                        temp = temp + rulesconstruct.Terminals[j];
                                        temp = temp + "\"||";
                                    }
                                temp = temp.Remove(temp.Count() - 2);
                                if (isFirstIf == false && trigger == true) code.WriteLine($"else if ({temp})");
                                else code.WriteLine($"if ({temp})");
                                count++;
                                code.WriteLine("{");
                                firstNonterminal = false;
                            }
                            code.WriteLine($"{rule[i]}();");
                        }
                    }
                    if (rule.Count() == 2)
                    {
                        string temp = string.Empty;
                        for (int j = 0; j < rulesconstruct.Terminals.Count(); j++)
                            if (lookahead.parsingtable[rulesconstruct.Nonterminals.IndexOf(functionName)][j] == rulesconstruct.ProductionRules.IndexOf(rule))
                            {
                                temp = temp + "prop[poz]==\"";
                                temp = temp + rulesconstruct.Terminals[j];
                                temp = temp + "\"||";
                            }
                        temp = temp.Remove(temp.Count() - 2);
                        if (isFirstIf == false && trigger == true) code.WriteLine($"else if ({temp})");
                        else code.WriteLine($"if ({temp})");
                        count++;
                        code.WriteLine("{");
                        code.WriteLine("return;");
                    }
                    for (int i = 0; i < count; i++)
                    {
                        code.WriteLine("}");
                        if (i != count - 1)
                            code.WriteLine(" else throw new Exception(\"Eroare\");");
                    }
                    trigger = true;
                    isFirstIf = false;
                }
            }
            code.WriteLine(" else throw new Exception(\"Eroare\");");
            code.WriteLine("}");
        }

        public void GenerateCode()
        {
            code.WriteLine("using System;");
            code.WriteLine("using System.Collections.Generic;");
            code.WriteLine("using System.Text;");
            code.WriteLine("using System.Threading.Tasks;");
            code.WriteLine("namespace Program{");
            code.WriteLine("class Program{");
            code.WriteLine("public static string[] prop;");
            code.WriteLine("public static int poz;");
            foreach (string functionName in rulesconstruct.Nonterminals)
                NonterminalFunction(functionName);
            code.WriteLine("static void Main(string[] args){");
            code.WriteLine("char delimiter = ' ';");
            code.WriteLine("prop=args[0].Split(delimiter);");
            code.WriteLine("try {");
            code.WriteLine("poz=0;");
            code.WriteLine($"{rulesconstruct.StartSymbol}();");
            code.WriteLine("if (prop[poz]==\"$\")");
            code.WriteLine("Console.WriteLine(\"Propozitie corecta\");");
            code.WriteLine("else Console.WriteLine(\"Eroare\");");
            code.WriteLine("}");
            code.WriteLine("catch (Exception e){");
            code.WriteLine("Console.WriteLine(e.Message);}");
            code.WriteLine("} } }");
            code.Close();
        }

        public void CompileGeneratedCode()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = "SentenceChecker.exe";
            parameters.IncludeDebugInformation = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.GenerateInMemory = false;
            parameters.WarningLevel = 3;
            parameters.TreatWarningsAsErrors = false;
            parameters.CompilerOptions = "/optimize";
            parameters.TempFiles = new TempFileCollection(".", false);

            if (provider.Supports(GeneratorSupport.EntryPointMethod))
            {
                parameters.MainClass = "Program.Program";
            }

            if (Directory.Exists("Resources"))
            {
                if (provider.Supports(GeneratorSupport.Resources))
                {
                    parameters.EmbeddedResources.Add("Resources\\Default.resources");
                    parameters.LinkedResources.Add("Resources\\nb-no.resources");
                }
            }

            CompilerResults cr = provider.CompileAssemblyFromFile(parameters, "code.cs");

            if (cr.Errors.Count > 0)
            {
                Console.WriteLine("Errors building {0} into {1}",
                     "code.cs", cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Source {0} built into {1} successfully.",
                    "code.cs", cr.PathToAssembly);
                Console.WriteLine("{0} temporary files created during the compilation.",
                    parameters.TempFiles.Count.ToString());
            }
        }
    }
}
