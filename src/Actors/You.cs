using Newtonsoft.Json;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public class You : Actor
    {
        
        [JsonConstructor]
        protected You()
        {

        }

        public You(string name, IPlace currentLocation):base( name,currentLocation)
        {
            BaseStats[Stat.Loyalty] = 10;
            BaseStats[Stat.Fight] = 10;
            BaseStats[Stat.Coerce] = 10;
        }

        public override bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, int attitude)
        {
            //ask user through UI
            return ui.GetChoice(title,body,out chosen,options);
        }
        
        public override void Kill(IUserinterface ui)
        {
            ui.ShowMessage("Dead","You have tragically met your end.  Don't worry, many of your comrades will benefit from you sacrifice (at breakfast tomorrow).",false);
        }
    }
}
