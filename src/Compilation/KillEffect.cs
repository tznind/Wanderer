using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class KillEffect : Effect
    {
        public string Reason { get; }

        public KillEffect(string reason, SystemArgsTarget target) : base(target)
        {
            Reason = reason;
        }

        public override void Apply(SystemArgs args)
        {
            var o = args.GetTarget(Target);
            
            if(o is IActor a)
                a.Kill(args.UserInterface,args.Round,Reason);
        }
    }
}