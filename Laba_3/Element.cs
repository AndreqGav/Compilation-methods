using System.Collections.Generic;
using System.Text;
using Laba_3.Enums;

namespace Laba_3
{
    public class Element
    {
        public TokenNames? Type;
        public string value;
        public NTerminals? NTerminal;

        public List<Element> Children = new List<Element>();

        public Element()
        {
        }

        public Element(NTerminals nTerminals)
        {
            NTerminal = nTerminals;
        }

        public Element(TokenNames tokenName, string value = null)
        {
            Type = tokenName;
            this.value = value;
        }

        public bool IsTerminal()
        {
            return NTerminal == null;
        }

        public void AddChild(Element element)
        {
            Children.Add(element);
        }

        public string Print(StringBuilder prefix = null, StringBuilder childPrefix = null)
        {
            var buffer = new StringBuilder();
            buffer.AppendLine();
            buffer.Append(prefix);
            buffer.Append(NTerminal?.ToString() ?? Type.ToString());


            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();

            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var isLast = Children.Count == 0 || Children.Count - 1 == i;
                if (isLast)
                {
                    sb1.Append(childPrefix);
                    sb1.Append("└──");

                    sb2.Append(childPrefix);
                    sb2.Append("   ");
                }
                else
                {
                    sb1.Append(childPrefix);
                    sb1.Append("├──");

                    sb2.Append(childPrefix);
                    sb2.Append("│  ");
                }

                buffer.Append(child.Print(sb1, sb2));
                sb1.Clear();
                sb2.Clear();
            }

            if (Children.Count == 0 && value != null)
            {
                buffer.Append("──");
                buffer.Append(value);
            }

            return buffer.ToString();
        }
    }
}
