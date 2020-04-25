using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories;
using Terminal.Gui;

namespace Wanderer.TerminalGui
{
    public class NewPlayerDialog : Dialog
    {
        public AdjectiveFactory AdjectiveFactory { get; }
        public bool Ok { get; set; }
        public NewPlayerDialog(MainWindow ui, IActor player,AdjectiveFactory adjectiveFactory) 
            :base("New Character", ui.DlgWidth, ui.DlgHeight)
        {
            AdjectiveFactory = adjectiveFactory;
            var btn = new Button("Finish", true)
            {
                Clicked = () =>
                {
                    Running = false;
                    Ok = true;
                }
            };

            var lblName = new Label("Name:")
            {
                X = 0,
                Y = 0
            };

            var tbName = new TextField("Wanderer")
            {
                X = Pos.Right(lblName),
                Y = lblName.Y,
                Width = 20
            };
            tbName.Changed += (o, e) => player.Name = tbName.Text?.ToString() ?? "Wanderer";

            Add(lblName);
            Add(tbName);

            //if(!RunDialog("Select Adjective","Choose an adjective for your character",out IAdjective chosen, adjectiveFactory.GetAvailableAdjectives(newWorld.Player).ToArray()))
              //  return;

            //newWorld.Player.Adjectives.Add(chosen);

            AddButton(btn);
        }

        public sealed override void Add(View view)
        {
            base.Add(view);
        }
    }
}
