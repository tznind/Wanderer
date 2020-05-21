using System;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes how to create an <see cref="IFrameSource"/>
    /// </summary>
    public class FrameSourceBlueprint
    {
        /// <summary>
        /// Lua code to construct a new <see cref="Frame"/>.  This is the action that should be performed if the plan is selected by the AI.  A <see cref="Frame"/> includes both the action and the selected targets
        /// </summary>
        public string Lua {get;set;}


        /// <summary>
        /// Creates a new <see cref="IFrameSource"/> from the blueprint
        /// </summary>
        /// <returns></returns>
        public IFrameSource Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                return new FrameSourceCode(Lua);

            throw new NotImplementedException("FrameSourceBlueprint was blank");
        }
    }
}