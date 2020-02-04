using System;
using System.Collections.Generic;
using System.Text;

namespace StarshipWanderer.Compilation
{
    public sealed class Compiler
    {
        private static readonly Lazy<Compiler>
            Lazy =
                new Lazy<Compiler>
                    (() => new Compiler());

        public static Compiler Instance => Lazy.Value;

        public TypeCollectionFactory TypeFactory { get; set; } = new TypeCollectionFactory(typeof(Compiler).Assembly);

        private Compiler()
        {
        }
    }
}
