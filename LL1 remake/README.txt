grammar.cs = recursivitate stanga si reguli de productie, pentru un neterminal, ce contin acelasi prefix
parsingtable.cs = FIRST+, FOLLOW+, tabela
compilecode = generare fisier .cs, compilare, executie

~ gramaticile exemplu sunt cele din curs
~ fara spatiu dupa delimitatorul ":" din regulile de productie; pentru lista (ne)terminale delimitatorii sunt " " si "\t"
~ code.cs codul generat (in /bin/debug)
~ sentencechecker.exe aplicatia generata (in /bin/debug), poate fi apelat si in linie de comanda cu argument

mainwindow.cs:

FilePath_MouseDoubleClick = deschide fisierul selectat si initializeaza gramatica
TestGrammarButton_Click = corecteaza gramatica
TestLL1Button_Click = pe baza gramaticii corectate calculeaza FIRST+ si FOLLOW+
CreateTableButton_Click = tabela
SeeTableButton_Click = tabela de la functia anterioara
StartButton_Click = genereaza, compileaza cod; codul se regaseste in acelasi director cu executabilul principal
VerifyButton_Click = executa cod si da drept argument propozitia scrisa

grammar.cs:

createrules = populeaza in mod convenabil tabela cu regulile de productie
printinformation = afiseaza intr-o noua fereastra gramatica
leftrecursion = recusivitate stanga
commonprefixes = regulile de productie care incep la fel

parsingtable.cs:

calculatefirst_a = popularea initiala cu FIRST+ (valorile directe)
calculatefirst_b = FIRST+ recursiv pe baza ce a fost calculat in _a
calculatefist = apeleaza _a si _b in ordinea buna
calculatefollow_a = FOLLOW+ pentru toate cazurile
calculatefollow = decide cand e nevoie de FOLLOW+
calculateintersections = conditia de LL1 (multimile pentru un neterminal sunt disjuncte)
calculateparsingtable = pune valorile in tabel
printparsetable = afiseaza tabela
printinformation = afiseaza FIRST+, FOLLOW+

compilecode.cs

nonterminalfunction = scrie functia pentru fiecare neterminal
generate code = scrie celalalte parti din cod
compilegeneratedcode = compileaza si genereaza solutia