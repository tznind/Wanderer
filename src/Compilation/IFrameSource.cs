using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public interface IFrameSource
    {
        Frame GetFrame(SystemArgs args);
    }
}