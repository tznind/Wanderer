using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Systems
{
    class DialogueSystem : IDialogueSystem
    {
        public void Apply(SystemArgs args)
        {
            if (args.AggressorIfAny == null)
                return;

            if (args.AggressorIfAny is You)
            {
                args.UserInterface.ShowMessage(args.Recipient + " Dialogue",$"Hello there {args.AggressorIfAny.Name}, are you enjoying wandering?");

                if(args.UserInterface.GetChoice("Answer","Are you enjoying yourself?",out bool chosen,new []{true,false}))
                    args.UserInterface.ShowMessage(args.Recipient + " Dialogue",chosen ? $"Glad to hear it {args.AggressorIfAny.Name}": "That's unfortunate");
            }
            else
            {
                args.UserInterface.Log.Info(new LogEntry($"{args.AggressorIfAny} talked to {args.Recipient} about their feelings",args.Round,args.AggressorIfAny));
            }

            
        }

        public bool CanTalk(IActor actor, IActor other)
        {
            return true;
        }

        public IEnumerable<IActor> GetAvailableTalkTargets(IActor actor)
        {
            return actor.GetCurrentLocationSiblings().Where(o => CanTalk(actor, o));
        }
    }
}