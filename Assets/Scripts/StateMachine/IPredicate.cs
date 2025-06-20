using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ComboProject.Predicates
{
    public interface IPredicate
    {
        bool Evaluate();
    }

    public class And : IPredicate
    {
        [SerializeField] private List<IPredicate> _rules = new List<IPredicate>();
        
        public bool Evaluate() => _rules.All(r => r.Evaluate());
    }

    public class Or : IPredicate
    {
        [SerializeField] private List<IPredicate> _rules = new List<IPredicate>();
        
        public bool Evaluate() => _rules.Any(r => r.Evaluate());
    }

    public class Not : IPredicate
    {
        [SerializeField] private IPredicate _rule;
        
        public bool Evaluate() => !_rule.Evaluate();
    }
}