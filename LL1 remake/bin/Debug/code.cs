using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Program{
class Program{
public static string[] prop;
public static int poz;
public static void I ()
{
if (prop[poz]=="repeat") 
{
poz++;
I();
if (prop[poz]=="until") 
{
poz++;
E();
}
 else throw new Exception("Eroare");
}
else if (prop[poz]=="id")
{
F();
if (prop[poz]==":=") 
{
poz++;
F();
}
 else throw new Exception("Eroare");
}
else if (prop[poz]=="if") 
{
poz++;
E();
if (prop[poz]=="then") 
{
poz++;
if (prop[poz]=="(") 
{
poz++;
I();
if (prop[poz]==")") 
{
poz++;
I1();
}
 else throw new Exception("Eroare");
}
 else throw new Exception("Eroare");
}
 else throw new Exception("Eroare");
}
 else throw new Exception("Eroare");
}
public static void E ()
{
if (prop[poz]=="id")
{
F();
E1();
}
 else throw new Exception("Eroare");
}
public static void F ()
{
if (prop[poz]=="id") 
{
poz++;
F1();
}
 else throw new Exception("Eroare");
}
public static void L ()
{
if (prop[poz]=="id") 
{
poz++;
L1();
}
 else throw new Exception("Eroare");
}
public static void I1 ()
{
if (prop[poz]==")"||prop[poz]=="until"||prop[poz]=="$")
{
return;
}
else if (prop[poz]=="else") 
{
poz++;
I();
}
 else throw new Exception("Eroare");
}
public static void E1 ()
{
if (prop[poz]=="<") 
{
poz++;
F();
}
else if (prop[poz]=="<=") 
{
poz++;
F();
}
 else throw new Exception("Eroare");
}
public static void F1 ()
{
if (prop[poz]=="(") 
{
poz++;
L();
if (prop[poz]==")") 
{
poz++;
return;
}
 else throw new Exception("Eroare");
}
else if (prop[poz]=="then"||prop[poz]==")"||prop[poz]=="until"||prop[poz]==":="||prop[poz]=="<"||prop[poz]=="<="||prop[poz]=="$")
{
return;
}
 else throw new Exception("Eroare");
}
public static void L1 ()
{
if (prop[poz]==",") 
{
poz++;
if (prop[poz]=="id") 
{
poz++;
L1();
}
 else throw new Exception("Eroare");
}
else if (prop[poz]==")")
{
return;
}
 else throw new Exception("Eroare");
}
static void Main(string[] args){
char delimiter = ' ';
prop=args[0].Split(delimiter);
try {
poz=0;
I();
if (prop[poz]=="$")
Console.WriteLine("Propozitie corecta");
else Console.WriteLine("Eroare");
}
catch (Exception e){
Console.WriteLine(e.Message);}
} } }
