using Wanderer;
using Wanderer.Actors;
using Terminal.Gui;

namespace Wanderer.TerminalGui
{
    public class ModalDialog : Dialog
    {
        public ModalDialog(MainWindow ui,string title, View subview)
            :base(title, ui.DlgWidth, ui.DlgHeight)
        {
            Button btn = new Button("Close",true);
            AddButton(btn);
            
            Add(subview);
            
            btn.Clicked = () => { Running = false;};
            btn.FocusFirst();
        }
    }
}
