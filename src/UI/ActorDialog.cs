﻿using StarshipWanderer.Actors;
using Terminal.Gui;

namespace StarshipWanderer.UI
{
    public class ActorDialog : Dialog
    {
        public ActorDialog(IActor actor)
            :base(actor.Name, MainWindow.DLG_WIDTH, MainWindow.DLG_HEIGHT)
        {
            Button btn = new Button("Close",true);
            AddButton(btn);
            
            int y = 1;

            var finalStats = actor.GetFinalStats();

            foreach (var baseStat in actor.BaseStats)
            {
                string line = baseStat.Key + ":" + baseStat.Value;

                if (baseStat.Value != finalStats[baseStat.Key])
                    line += " (" + finalStats[baseStat.Key] + ")";

                var lbl = new Label(line) {Y = y++};
                Add(lbl);
            }

            btn.Clicked = () => { Running = false;};
        }
    }
}