using StarshipWanderer;
using StarshipWanderer.Actors;
using Terminal.Gui;

namespace Game.UI
{
    public class HasStatsDialog : Dialog
    {
        public HasStatsDialog(IActor observer,IHasStats actor)
            :base(actor.Name, MainWindow.DLG_WIDTH, MainWindow.DLG_HEIGHT)
        {
            Button btn = new Button("Close",true);
            AddButton(btn);

            var v = new HasStatsView();
            v.InitializeComponent(observer,actor);
            Add(v);
            
            btn.Clicked = () => { Running = false;};
            btn.FocusFirst();
        }
    }
}