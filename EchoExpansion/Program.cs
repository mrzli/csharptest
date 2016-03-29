using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoExpansion
{
    class Program
    {
        static void Main(string[] args)
        {
            // abc->abc
            // {a,b}{c,d} -> ac ad bc bd
            // {a,b}{c,g{e,m}}p{q,r} -> acpq acpr agepq agepr agmpq agmpr bcpq bcpr bgepq bgepr bgmpq bgmpr

            string input = "{a,b}{c,g{e,m}}p{q,r}"; //Console.ReadLine();

            int i = 0;
            List<Element> elements = GetElements(input, ref i);
            //TraverseAsChoiceTree(elements, 0, false);
        }

        private static List<Element> GetElements(string input, ref int i)
        {
            List<Element> elements = new List<Element>();
            while (i < input.Length)
            {
                if (input[i] == '{')
                {
                    Choice childChoice = GetChoice(input, ref i);
                    elements.Add(childChoice);
                }
                else if (char.IsLetter(input[i]))
                {
                    Lexeme lexeme = GetLexeme(input, ref i);
                    elements.Add(lexeme);
                }
                else if (input[i] == '}' || input[i] == ',')
                {
                    break;
                }
            }

            return elements;
        }

        private static Choice GetChoice(string input, ref int i)
        {
            i++;
            Choice choice = new Choice();
            while (i < input.Length)
            {
                if (input[i] == '}')
                {
                    i++;
                    break;
                }
                else if (input[i] == ',')
                {
                    i++;
                }
                else
                {
                    choice.Values.Add(GetElements(input, ref i));
                }
            }

            return choice;
        }

        private static Lexeme GetLexeme(string input, ref int i)
        {
            StringBuilder sb = new StringBuilder();
            while (i < input.Length && char.IsLetter(input[i]))
            {
                sb.Append(input[i]);
                i++;
            }

            return new Lexeme { Value = sb.ToString() };
        }

        private static void TraverseAsChoiceTree(List<Element> elements, int i, string current, bool parent)
        {
            if (i >= elements.Count)
            {
                if (parent) Console.Write(" ");
                return;
            }

            if (elements[i] is Lexeme)
            {
                Console.Write(((Lexeme)elements[i]).Value);
                //TraverseAsChoiceTree(elements, i + 1, false);
            }
            else if (elements[i] is Choice)
            {
                Choice choice = (Choice)elements[i];

                foreach (var childElements in choice.Values)
                {
                    //TraverseAsChoiceTree(childElements, 0, string false);
                }
            }
        }
    }

    class Element
    {

    }

    class Lexeme : Element
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }

    class Choice : Element
    {
        public List<List<Element>> Values { get; private set; }
        public int CurrentChoice { get; set; }

        public Choice()
        {
            Values = new List<List<Element>>();
        }
    }
}
