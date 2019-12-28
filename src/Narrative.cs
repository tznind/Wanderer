using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer
{
    /// <summary>
    /// Builds up a string of fluff text with log entries / technical summary.
    /// </summary>
    public class Narrative
    {
        public IActor Actor { get; }
        private readonly string _title;

        private readonly List<Tuple<string, bool>> _text = new List<Tuple<string, bool>>();

        /// <summary>
        /// Creates a new instance primed with the given <paramref name="fluff"/>
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="title"></param>
        /// <param name="fluff"></param>
        public Narrative(IActor actor,string title, string fluff)
        {
            Actor = actor;
            _title = title;

            Add(fluff);
        }

        public void Add(string fluff)
        {
            _text.Add(Tuple.Create(fluff,true));
        }

        public void Add(string fluff, string technical)
        {
            _text.Add(Tuple.Create(fluff,true));
            _text.Add(Tuple.Create(technical,false));
        }

        /// <summary>
        /// Outputs all technical text to the log and fluff to the <paramref name="ui"/>
        /// (if <see cref="Actor"/> is player or <paramref name="forceShowMessage"/> is true) 
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="forceShowMessage"></param>
        public void Show(IUserinterface ui, bool forceShowMessage)
        {
            //output the fluff to the ui if you are the actor
            if(forceShowMessage || Actor is You)
                ui.ShowMessage(_title,string.Join('\n',_text.Where(t=>t.Item2).Select(t=>t.Item1)),false);

            //log the technical stuff
            ui.Log.Info(string.Join('\n',_text.Where(t=>!t.Item2).Select(t=>t.Item1)));
        }

        public void Changed(string fluff, Stat stat, int change)
        {
            Add(fluff, stat + (change < 0 ? "Decreased" : "Increased") + " by " + Math.Abs(change));
        }
    }
}