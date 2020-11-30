using System.Collections.Generic;
using System.Linq;
using Laba_3.Enums;

namespace Laba_3
{
    public partial class Rule
    {
        private readonly List<Element> _rules;

        public Rule(params Element[] rules)
        {
            _rules = rules.ToList();
        }

        public List<Element> SplitReverse()
        {
            var t = _rules.ToList();
            t.Reverse();
            return t;
        }
    }

    public partial class Rule
    {
        public static readonly List<Rule> All = new List<Rule>()
        {
            // 0: S->for(A;B;A)do T
            new Rule(
                new Element(TokenNames.KEY_WORD_FOR), new Element(TokenNames.DELIMITER, "("),
                new Element(NTerminals.A), new Element(TokenNames.DELIMITER, ";"),
                new Element(NTerminals.B), new Element(TokenNames.DELIMITER, ";"),
                new Element(NTerminals.A), new Element(TokenNames.DELIMITER, ")"),
                new Element(TokenNames.KEY_WORD_DO), new Element(NTerminals.T)
            ),
            // 1: S-> $
            new Rule(new Element(TokenNames.END)),
            // 2: A->ident:=V 
            new Rule(new Element(TokenNames.IDENT), new Element(TokenNames.ASSIGN), new Element(NTerminals.V)),
            // 3: B->V operation V
            new Rule(new Element(NTerminals.V), new Element(TokenNames.OPERATION), new Element(NTerminals.V)),
            // 4: V->ident
            new Rule(new Element(TokenNames.IDENT)),
            // 5: V->num
            new Rule(new Element(TokenNames.NUM)),
            // 6: T->S
            new Rule(new Element(NTerminals.S)),
            // 7: T->V C V ; S
            new Rule(new Element(NTerminals.V), new Element(NTerminals.C), new Element(NTerminals.V),
                new Element(TokenNames.DELIMITER, ";"), new Element(NTerminals.S)),
            // 8: T->B
            new Rule(new Element(NTerminals.B),
                new Element(TokenNames.DELIMITER, ";"), new Element(NTerminals.S)),
            // 9: C->operation
            new Rule(new Element(TokenNames.OPERATION)),
            // 10: C-> :=
            new Rule(new Element(TokenNames.ASSIGN))
        };
    }
}