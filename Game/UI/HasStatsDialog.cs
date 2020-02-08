using StarshipWanderer;
using StarshipWanderer.Actors;
using Terminal.Gui;

namespace Game.UI
{
    public class HasStatsDialog : Dialog
    {
        public HasStatsDialog(MainWindow ui,IActor observer,IHasStats actor)
            :base(actor.Name, ui.DlgWidth, ui.DlgHeight)
        {
            Button btn = new Button("Close",true);
            AddButton(btn);

            var v = new HasStatsView();
            v.InitializeComponent(observer,actor,ui.DlgWidth,ui.DlgHeight);
            Add(v);
            
            btn.Clicked = () => { Running = false;};
            btn.FocusFirst();
        }
    }
}