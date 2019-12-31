using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NStack;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using Terminal.Gui;

namespace StarshipWanderer.UI
{
    public class MainWindow : Window, IUserinterface
    {
        private const int DLG_WIDTH = 78;
        private const int DLG_HEIGHT = 18;
        private const int DLG_BOUNDARY = 2;

        private const int WIN_WIDTH = 80;
        private const int WIN_HEIGHT = 20;

        private const int MAP_WIDTH = 40;
        private const int MAP_HEIGHT = WIN_HEIGHT - 5;


        public IWorld World { get; set; }
        public EventLog Log { get; }
        public ListView ListActions { get; set; }

        private readonly Label _lblMap;

        public MainWindow(IWorld world, EventLog log):base(new Rect(0,1,WIN_WIDTH,WIN_HEIGHT + 1),null)
        {
            World = world;
            Log = log;
            var top = Application.Top;

            var menu = new MenuBar (new MenuBarItem [] {
                new MenuBarItem ("_Game (F9)", new MenuItem [] {
                    
                    new MenuItem ("View _Log", "", () => { ViewLog(); }),
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

            _lblMap = new Label(new Rect(0, 0, MAP_WIDTH, MAP_HEIGHT), " ") {LayoutStyle = LayoutStyle.Absolute};

            Add(_lblMap);

            frame.Add(ListActions);
            frame.FocusFirst();

            Add(frame);    
            
            Refresh();
        }

        public sealed override void Add(View view)
        {
            base.Add(view);
        }

        private void ViewLog()
        {
            RunDialog("Log",
                string.Join('\n',Log.Target.Logs),out _,"Ok");
        }

        bool RunDialog<T>(string title, string message,out T chosen, params T[] options)
        {
            var result = default(T);
            bool optionChosen = false;

            var dlg = new Dialog(title, DLG_WIDTH, DLG_HEIGHT);
            
            var line = DLG_HEIGHT - (DLG_BOUNDARY)*2 - options.Length;

            if (!string.IsNullOrWhiteSpace(message))
            {
                int width = DLG_WIDTH - (DLG_BOUNDARY * 2);

                var msg = Wrap(message, width).TrimEnd();

                var text = new Label(0, 0, msg)
                {
                    Height = line - 1, Width = width
                };

                //if it is too long a message
                int newlines = msg.Count(c => c == '\n');
                if (newlines > line - 1)
                {
                    var view = new ScrollView(new Rect(0, 0, width, line - 1))
                    {
                        ContentSize = new Size(width, newlines + 1),
                        ContentOffset = new Point(0, 0),
                        ShowVerticalScrollIndicator = true,
                        ShowHorizontalScrollIndicator = false
                    };
                    view.Add(text);
                    dlg.Add(view);
                }
                else
                    dlg.Add(text);
            }
            
            foreach (var value in options)
            {
                T v1 = (T) value;
                var btn = new Button(0, line++, value.ToString())
                {
                    Clicked = () =>
                    {
                        result = v1;
                        dlg.Running = false;
                        optionChosen = true;
                    }
                };


                dlg.Add(btn);
            }

            Application.Run(dlg);

            chosen = result;
            return optionChosen;
        }

        private string Wrap(string message, int lineWidth)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < message.Length; i = Math.Min(message.Length, i + lineWidth))
            {
                string nextChunk = message.Substring(i, Math.Min(message.Length - i, lineWidth));

                sb.Append(nextChunk);

                if(!nextChunk.Contains('\n'))
                    sb.Append('\n');
            }
            
            return sb.ToString();
        }

        public void ShowActorStats(IActor actor)
        {
            Button btn;
            var dlg = new Dialog(actor.Name, DLG_WIDTH, DLG_HEIGHT,btn = new Button("Close",true));
            
            int y = 1;
            
            foreach (var stat in actor.BaseStats)
            {
                var lbl = new Label(stat.Key + ":" + stat.Value) {Y = y++};
                dlg.Add(lbl);
            }

            btn.Clicked = () => { dlg.Running = false;};
            Application.Run(dlg);
        }

        public bool GetChoice<T>(string title, string body, out T chosen, params T[] options)
        {
            return RunDialog(title, body, out chosen, options);
        }

        public void Refresh()
        {
            UpdateActions();

            RedrawMap();
        }

        private void RedrawMap()
        {
            if (World.Player == null)
                return;

            StringBuilder sb = new StringBuilder();
            
            var home = World.Map.GetPoint(World.Player.CurrentLocation);

            //start map drawing at the north most (biggest Y value) and count down (towards south)
            for (int y = (MAP_HEIGHT/2) - 1 ; y >= -MAP_HEIGHT/2 ; y--)
            {    
                //start each line ath the west most (smallest x value) and count up (towards east)
                for (int x = -MAP_WIDTH/2; x < MAP_WIDTH/2; x++)
                {
                    var pointToRender = home.Offset(new Point3(x, y, 0));

                    if (World.Map.ContainsKey(pointToRender))
                        sb.Append(World.Map[pointToRender].Tile);
                    else
                        sb.Append(' ');
                }

                sb.Append('\n');
            }

            _lblMap.Text = sb.ToString();
        }

        public void ShowMessage(string title, string body, bool log, Guid round)
        {
            if(log)
                Log.Info(body,round);

            RunDialog(title,body,out _,"Ok");
        }

        readonly List<Button> _oldButtons = new List<Button>();

        readonly List<Point> _buttonLocations = new List<Point>()
        {
            new Point(0,16),
            new Point(0,17),
            new Point(0,18),
            new Point(0,19),
            new Point(0,20),
        };


        public void UpdateActions()
        {
            Title = World.Player.CurrentLocation.Title;

            foreach(Button b in _oldButtons)
                Remove(b);
            
            _oldButtons.Clear();

            int buttonLoc = 0;

            var allActions = World.Player.CurrentLocation.GetActions(World.Player)
                .Union(World.Player.GetFinalActions());

            foreach (var action in allActions)
            {
                var btn = new Button(_buttonLocations[buttonLoc].X, _buttonLocations[buttonLoc].Y, action.Name, false)
                {
                    Width = 10,
                    Height = 1,
                    Clicked = () =>
                    {
                        var stack = new ActionStack();
                        var actionRun = stack.RunStack(this, action, World.Player,
                            World.Population.SelectMany(a => a.GetFinalBehaviours()));

                        if(actionRun)
                            World.RunNpcActions(this);

                        if(Log.RoundResults.Any())
                            ShowMessage("Round Results",string.Join('\n',Log.RoundResults),false,Guid.Empty);
                    }
                };

                _oldButtons.Add(btn);
                this.Add(btn);
                buttonLoc++;
            }
        }
    }
}