using System;
using System.Collections.Generic;
using Wanderer.Adjectives;

namespace Wanderer.Actions
{
    public interface IActionCollection : ISwCollection<IAction>
    {
        bool HasAction(Type t);
        bool HasAction(string actionTypeName);

        IAction GetAction(Type t);
        IAction GetAction(string actionTypeName);
    }
}