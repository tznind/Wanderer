using System.Collections.Generic;
using System.Linq;
using StarshipWanderer;
using StarshipWanderer.Actors;
using Terminal.Gui;

namespace Game.UI
{
    public class ActorDialog : Dialog
    {
        public ActorDialog(IActor actor)
            :base(actor.Name, MainWindow.DLG_WIDTH, MainWindow.DLG_HEIGHT)
        {
            Button btn = new Button("Close",true);
            AddButton(btn);

            List<string> lines = new List<string>();
            
            if(actor.Adjectives.Any())
                lines.Add("Adjectives:" + string.Join(',', actor.Adjectives));

            //output stats
            var finalStats = actor.GetFinalStats();

            foreach (var baseStat in actor.BaseStats)
            {
                string line = baseStat.Key + ":" + baseStat.Value;

                if (baseStat.Value != finalStats[baseStat.Key])
                    line += " (" + finalStats[baseStat.Key] + ")";

                lines.Add(line);
            }

            lines.Add("Items:");

            if(!actor.Items.Any())
                lines.Add("None");
            else
                //output items
                lines.AddRange(actor.Items.Select(i => i.ToString()));
            
            View addLabelsTo;

            //if it is too many items
            if (lines.Count > MainWindow.DLG_HEIGHT - 6)
            {
                //use a scroll view
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
                addLabelsTo.Add(new Label(0, i, lines[i]));
            
            btn.Clicked = () => { Running = false;};
            btn.FocusFirst();
        }
    }
}