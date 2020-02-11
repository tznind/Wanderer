using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Effects
{

    public interface IEffect
    {
        void Apply(SystemArgs args);
    }
}