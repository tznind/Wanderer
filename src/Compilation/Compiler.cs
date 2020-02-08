using StarshipWanderer.Actions;
using StarshipWanderer.Conditions;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Compilation
{
    public sealed class Compiler
    {
        private static object oInstance = new object();
        private static Compiler _instance;

        public static Compiler Instance
        {
            get
            {
                if (_instance == null)
                    lock (oInstance)
                        if (_instance == null)
                            _instance = new Compiler();

                return _instance;
            }
        }

        public TypeCollectionFactory TypeFactory { get; set; } = new TypeCollectionFactory(typeof(Compiler).Assembly);
        public IDeserializer Deserializer =>
            new DeserializerBuilder()
                .WithTypeConverter(new YamlTypeConverter<ICondition>())
                .WithTypeConverter(new YamlTypeConverter<IAction>())
                .Build();

        private Compiler()
        {

        }


    }
}
