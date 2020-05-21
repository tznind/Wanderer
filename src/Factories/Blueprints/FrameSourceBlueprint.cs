using System;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
    public class FrameSourceBlueprint
    {
        public string Lua {get;set;}

        internal IFrameSource Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                return new FrameSourceCode(Lua);

            throw new NotImplementedException("FrameSourceBlueprint was blank");
        }
    }
}