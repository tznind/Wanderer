using System;
using System.IO;
using Wanderer.Actions;
using Wanderer.Stats;
using YamlDotNet.Serialization;

namespace Wanderer.Compilation
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
                .WithTypeConverter(new YamlTypeConverter<IEffect>())
                .WithTypeConverter(new YamlTypeConverter<IFrameSource>())
                .WithTypeConverter(new YamlTypeConverter<Stat>())
                .Build();

        private Compiler()
        {

        }


        public static string GetDefaultResourcesDirectory()
        {
            string entry = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            return Path.Combine(entry == null ? Environment.CurrentDirectory : Path.GetDirectoryName(entry) ?? ".","Resources");
        }
    }
}
