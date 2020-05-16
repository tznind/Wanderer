using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Relationships;
using Terminal.Gui;
using System.Text;
using Wanderer.Stats;

namespace Wanderer.TerminalGui
{
    public class HasStatsView : View
    {
        public bool AllowScrolling { get; set; } = true;

        /// <summary>
        /// Setup the control to display the stats of <paramref name="o"/> as percieved
        /// by <paramref name="observer"/>
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="o"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void InitializeComponent(IActor observer,IHasStats o, int width, int height)
        {
            var stats = observer.CurrentLocation.World.AllStats.All;

            List<string> lines = new List<string>();
            int maxLines = height - 6;

            lines.Add("Name:" + o.Name);

            if(o.Adjectives.Any())
                lines.Add("Adjectives:" + string.Join(',', o.Adjectives));

            var asActor = o as IActor;

            if(asActor != null)
                lines.Add("Factions:" + string.Join(',',asActor.FactionMembership));

            if(o.InjurySystem?.Name != null)
                lines.Add("Damage:" + o.InjurySystem.Name);

            //output stats
            var finalStats = o.GetFinalStats(asActor ?? observer);

            var sbStatLine1 = new StringBuilder();
            var sbStatLine2 = new StringBuilder();
            var sbStatLine3 = new StringBuilder();

            foreach (var s in stats)
            {
                if(s == Stat.Value)
                    continue;

                string stat = s.ToString().Substring(0,2);
                string val = o.BaseStats[s].ToString();
                string valFinal = "";

                if (Math.Abs(o.BaseStats[s] - finalStats[s]) > 0.0001)
                    valFinal = $"({finalStats[s]:N0})";

                int maxWidth = Math.Max(3,Math.Max(val.Length,valFinal.Length));

                stat = stat.PadRight(maxWidth);
                val = val.PadRight(maxWidth);
                valFinal = valFinal.PadRight(maxWidth);

                sbStatLine1.Append(stat);
                sbStatLine2.Append(val);
                sbStatLine3.Append(valFinal);
            }

            lines.Add(sbStatLine1.ToString());
            lines.Add(sbStatLine2.ToString());
            lines.Add(sbStatLine3.ToString());

            if(o.BaseActions.Count > 0)
            {
                lines.Add("Actions:");
                foreach(var action in o.BaseActions)
                    lines.Add(action.ToString());
            }


            if (asActor != null)
            {
                lines.Add("Items:");

                if(!asActor.Items.Any())
                    lines.Add("None");
                else
                    //output items
                    lines.AddRange(asActor.Items.Select(i => i.ToString()));

                var relationships = GetRelationships(asActor).ToArray();

                if (relationships.Any())
                {
                    lines.Add("Relationships:");
                    lines.AddRange(relationships);
                }
            }
            
            View addLabelsTo;
            //if it is too many items
            if (lines.Count > maxLines && AllowScrolling)
            {
                //use observer scroll view
                var view = new ScrollView(new Rect(0, 0, width-3, height- 6))
                {
                    ContentSize = new Size(width, lines.Count + 1),
                    ContentOffset = new Point(0, 0),
                    ShowVerticalScrollIndicator = true,
                    ShowHorizontalScrollIndicator = false
                };

                addLabelsTo = view;
                Add(view);
            }
            else
                addLabelsTo = this; //otherwise just labels

            for (int i = 0; i < lines.Count; i++)
                if (i == maxLines && !AllowScrolling)
                {
                    addLabelsTo.Add(new Label(0, i,"..."));
                    break;
                }
                else
                    addLabelsTo.Add(new Label(0, i, lines[i]));
        }
        private IEnumerable<string> GetRelationships(IActor actor)
        {
            foreach (var relationship in actor.CurrentLocation.World.Relationships.OfType<PersonalRelationship>().Where(r => r.Observer == actor))
            {
                //personal relationship then in brackets the sum total (including inherited faction relationships etc)
                var total = actor.CurrentLocation.World.Relationships.SumBetween(relationship.Observer,relationship.Observed);

                //sometimes they are the same
                if(Math.Abs(total - relationship.Attitude) < 0.001)
                    yield return $"To {relationship.Observed} {relationship.Attitude}";
                else
                    yield return $"To {relationship.Observed} {relationship.Attitude} ({total:N0})";
            }
        }
    }
}
