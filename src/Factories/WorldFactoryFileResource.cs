using System;
using System.IO;

namespace Wanderer.Factories
{
    public class WorldFactoryFileResource:WorldFactoryResource
    {
        public FileInfo FileInfo { get; }

        public WorldFactoryFileResource(FileInfo f): base(f.FullName,File.ReadAllText(f.FullName))
        {
            FileInfo = f;
        }

        public override bool SharesPath(WorldFactoryResource other)
        {
            return other is WorldFactoryFileResource f ?
                SharesPath(f) : throw new NotSupportedException("Expected other resource to be a WorldFactoryFileResource");
        }

        public bool SharesPath(WorldFactoryFileResource other)
        {
            return FileInfo.Directory.FullName.StartsWith(
                other.FileInfo.Directory.FullName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}