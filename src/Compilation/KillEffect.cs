using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class KillEffect : IEffect
    {
        public string Reason { get; }

        public bool RecipientOnly {get;set;}

        public KillEffect(string reason)
        {
            Reason = reason;
        }

        public void Apply(SystemArgs args)
        {
            var o = RecipientOnly ? args.Recipient : args.AggressorIfAny ?? args.Recipient;
            
            if(o is IActor a)
                a.Kill(args.UserInterface,args.Round,Reason);
        }
    }
}