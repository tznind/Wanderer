using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Relationships;
using Terminal.Gui;

namespace Game.UI
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
        public void InitializeComponent(IActor observer,IHasStats o)
        {
            List<string> lines = new List<string>();
            const int maxLines = MainWindow.DLG_HEIGHT - 6;

            lines.Add("Name:" + o.Name);

            if(o.Adjectives.Any())
                lines.Add("Adjectives:" + string.Join(',', o.Adjectives));

            var asActor = o as IActor;

            if(asActor != null)
                lines.Add("Factions:" + string.Join(',',asActor.FactionMembership));

            //output stats
            var finalStats = o.GetFinalStats(asActor ?? observer);

            foreach (var baseStat in o.BaseStats)
            {
                string line = baseStat.Key + ":" + baseStat.Value;

                if (Math.Abs(baseStat.Value - finalStats[baseStat.Key]) > 0.0001)
                    line += " (" + finalStats[baseStat.Key] + ")";

                lines.Add(line);
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
                var view = new ScrollView(new Rect(0, 0, MainWindow.DLG_WIDTH-3, MainWindow.DLG_HEIGHT- 6))
                {
                    ContentSize = new Size(MainWindow.DLG_WIDTH, lines.Count + 1),
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
                    yield return $"{relationship}";
                else
                    yield return $"{relationship} ({total})";
            }
        }
    }
}