using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Stats;

namespace Wanderer
{
    /// <summary>
    /// Builds up a string of fluff text with log entries / technical summary.
    /// </summary>
    public class Narrative
    {
        public IActor Actor { get; }
        public Guid Round { get; }
        private readonly string _title;

        private readonly List<Tuple<string, bool>> _text = new List<Tuple<string, bool>>();

        /// <summary>
        /// Creates a new instance primed with the given <paramref name="fluff"/>
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="title"></param>
        /// <param name="fluff"></param>
        /// <param name="technical"></param>
        /// <param name="round">The current round (see <see cref="ActionStack.Round"/>) or empty</param>
        public Narrative(IActor actor,string title, string fluff,string technical, Guid round)
        {
            Actor = actor;
            Round = round;
            _title = title;

            Add(fluff,technical);
        }
        
        public void Add(string fluff, string technical)
        {
            if(!string.IsNullOrWhiteSpace(fluff))
                _text.Add(Tuple.Create(fluff,true));
            
            if(!string.IsNullOrWhiteSpace(technical))
                _text.Add(Tuple.Create(technical,false));
        }

        /// <summary>
        /// Outputs all technical text to the log and fluff to the <paramref name="ui"/>
        /// (if <see cref="Actor"/> is player or <paramref name="forceShowMessage"/> is true) 
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="forceShowMessage"></param>
        public void Show(IUserinterface ui, bool forceShowMessage=false)
        {
            //output the fluff to the ui if you are the actor
            if(forceShowMessage || Actor is You)
                ui.ShowMessage(_title,string.Join("\n",_text.Where(t=>t.Item2).Select(t=>t.Item1)));

            //log the technical stuff
            var technical = string.Join("\n", _text.Where(t => !t.Item2).Select(t => t.Item1));

            if(!string.IsNullOrWhiteSpace(technical))
                ui.Log.Info(new LogEntry(technical,Round,Actor));
        }

        public void Changed(string fluff, Stat stat, double change)
        {
            Add(fluff, stat + (change < 0 ? "Decreased" : "Increased") + " by " + Math.Abs(change));
        }
    }
}