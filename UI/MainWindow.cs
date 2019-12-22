using System;
using System.Collections;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using Terminal.Gui;

namespace StarshipWanderer.UI
{
    public class MainWindow : Window, IUserinterface
    {
        public World World { get; }
        public ListView ListActions { get; set; }

        public MainWindow(World world):base(new Rect(0,1,80,21),null)
        {
            World = world;
            var top = Application.Top;

            var menu = new MenuBar (new MenuBarItem [] {
                new MenuBarItem ("_Game (F9)", new MenuItem [] {

                    new MenuItem ("_Character", "", () => { ShowActorStats(World.Player); }),
                    new MenuItem ("_Quit", "", () => { top.Running = false; })
                })
            });
            top.Add (menu);
            
            /****** Menu ***********/
            // 15 x 80
            // ******** Actions *****
            // 5 x 80
            //***********************
            var frame = new FrameView(new Rect(-1, 15, 80, 5),"Actions");

            ListActions = new ListView();

            frame.Add(ListActions);
            frame.FocusFirst();

            Add(frame);    
            
            UpdateActions();
        }

        private const int DLG_WIDTH = 60;
        private const int DLG_HEIGHT = 15;

        public void ShowActorStats(IActor actor)
        {
            Button btn;
            var dlg = new Dialog(actor.Name, DLG_WIDTH, DLG_HEIGHT,btn = new Button("Close",true));

            var type = actor.GetType();

            int y = 1;

            foreach (var prop in type.GetProperties())
            {
                object val;

                try
                {
                    val = prop.GetValue(actor);
                }
                catch (Exception)
                {
                    val = "unknown";
                }

                var lbl = new Label(prop.Name + ":" + val);
                lbl.Y = y++;
                dlg.Add(lbl);
            }

            btn.Clicked = () => { dlg.Running = false;};
            Application.Run(dlg);
        }

        public T GetOption<T>(string title) where T : Enum
        {
            var result = default(T);
            var dlg = new Dialog(title, DLG_WIDTH, DLG_HEIGHT);

            foreach (var value in Enum.GetValues(typeof(T)))
            {
                T v1 = (T) value;
                var btn = new Button(value.ToString());
                btn.Clicked = ()=>
                {
                    result = v1;
                    dlg.Running = false;
                };

                dlg.AddButton(btn);
            }

            Application.Run(dlg);
            return result;
        }

        public void Refresh()
        {
            UpdateActions();
        }

        List<Button> _oldButtons = new List<Button>();
        List<Point> _buttonLocations = new List<Point>()
        {
            new Point(0,16),
            new Point(0,17),
            new Point(0,18),
            new Point(0,19),
            new Point(0,20),
        };
        public void UpdateActions()
        {
            Title = World.CurrentLocation.Title;

            foreach(Button b in _oldButtons)
                Remove(b);
            
            _oldButtons.Clear();

            int buttonLoc = 0;

            foreach (var action in World.CurrentLocation.GetActions())
            {
                var btn = new Button(_buttonLocations[buttonLoc].X, _buttonLocations[buttonLoc].Y, action.Name, false);
                btn.Width = 10;
                btn.Height = 1;
                btn.Clicked = ()=>action.Perform(this);
                _oldButtons.Add(btn);
                this.Add(btn);
                buttonLoc++;
            }
        }
    }
}