using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Factories;
using Terminal.Gui;

namespace Game.UI
{
    public class MainWindow : Window, IUserinterface
    {
        private readonly WorldFactory _worldFactory;
        public const int DLG_WIDTH = 78;
        public const int DLG_HEIGHT = 18;
        public const int DLG_BOUNDARY = 2;

        private const int WIN_WIDTH = 80;
        private const int WIN_HEIGHT = 21;

        private const int MAP_WIDTH = 40;
        private const int MAP_HEIGHT = WIN_HEIGHT - 7;

        private const int ROOM_FRAME_X = MAP_WIDTH;
        private const int ROOM_FRAME_Y = -1;
        private const int ROOM_FRAME_WIDTH = WIN_WIDTH - (MAP_WIDTH + 1);
        private const int ROOM_FRAME_HEIGHT = MAP_HEIGHT + 3;


        public IWorld World { get; set; }
        public EventLog Log { get; }

        private readonly Label _lblMap;
        private readonly SplashScreen _splash;


        private readonly FrameView _roomFrame;
        private readonly FrameView _actionFrame;

        public MainWindow(WorldFactory worldFactory):base(new Rect(0,1,WIN_WIDTH,WIN_HEIGHT + 1),null)
        {
            _worldFactory = worldFactory;

            Log = new EventLog();
            Log.Register();

            var top = Application.Top;

            var menu = new MenuBar (new MenuBarItem [] {
                new MenuBarItem ("_Game (F9)", new MenuItem [] {
                    new MenuItem("_New",null, NewGame), 
                    new MenuItem("_Save",null,SaveGame), 
                    new MenuItem("_Load",null,LoadGame),
                    new MenuItem ("_Quit", null, () => { top.Running = false; })
                }),
                new MenuBarItem("_View",new MenuItem[]{
                    new MenuItem ("_Log", null, ViewLog),
                    new MenuItem ("_Character", null, () => { ShowActorStats(World?.Player); })
                })
            });
            top.Add (menu);

            /****** Menu ***********/
            // 15 x 80      |  rightFrame
            // ******** Actions *****
            // 6 x 80  actionFrame
            //***********************
            
            _roomFrame = new FrameView(new Rect(ROOM_FRAME_X,ROOM_FRAME_Y ,ROOM_FRAME_WIDTH,ROOM_FRAME_HEIGHT), "Contents");
            _actionFrame = new FrameView(new Rect(-1, 15, 80, 6),"Actions");
            
            _splash = new SplashScreen(){X = 4,Y=4};
            Add(_splash);

            _lblMap = new Label(new Rect(0, 0, MAP_WIDTH, MAP_HEIGHT), " ") {LayoutStyle = LayoutStyle.Absolute};

            _actionFrame.FocusFirst();
        }
        public void NewGame()
        {
            var newWorld = _worldFactory.Create();

            var dlg = new NewPlayerDialog(newWorld.Player,new AdjectiveFactory());

            try
            {
                Application.Run(dlg);
            }
            catch (Exception e)
            {
                ShowException("Error Creating World",e);
            }

            if(!dlg.Ok)
                return;

            SetWorld(newWorld);
        }

        private void SetWorld(IWorld newWorld)
        {
            World = newWorld;
            Log.Clear();
            
            Remove(_splash);
            Add(_lblMap);
            Add(_actionFrame);
            Add(_roomFrame);

            Refresh();
        }

        private void ShowException(string msg, Exception e)
        {
            var e2 = e;
            const string stackTraceOption = "Stack Trace";
            StringBuilder sb = new StringBuilder();

            while (e2 != null)
            {
                sb.AppendLine(e2.Message);
                e2 = e2.InnerException;
            }

            if(GetChoice(msg, sb.ToString(), out string chosen, "Ok", stackTraceOption))
                if(string.Equals(chosen,stackTraceOption))
                    ShowMessage("Stack Trace",e.ToString());

        }

        private void LoadGame()
        {
            var ofd = new OpenDialog("Load Game", "Enter file path to load")
            {
                AllowedFileTypes = new[] {".json"}, 
                CanChooseDirectories = false,
                AllowsMultipleSelection = false
            };

            Application.Run(ofd);

            var f = ofd.FilePaths.SingleOrDefault();
            if ( f != null)
            {
                try
                {
                    
                    var json = File.ReadAllText(f);
                    
                    var newWorld = JsonConvert.DeserializeObject<IWorld>(json, StarshipWanderer.World.GetJsonSerializerSettings());
                    
                    SetWorld(newWorld);
                }
                catch (Exception e)
                {
                    ShowException("Failed to Load",e);
                }
            }
        }
        
        private void SaveGame()
        {
            if(World == null)
            {
                ShowMessage("No World","No game is currently loaded");
                return;
            }

            var sf = new SaveDialog("Save Game", "Enter save file path") {AllowedFileTypes = new[] {".json"}};

            Application.Run(sf);

            if (sf.FileName != null)
            {
                var f =
                    sf.FileName.EndsWith(".json") ? sf.FileName : sf.FileName + ".json";
                
                try
                {
                    var json = JsonConvert.SerializeObject(World, StarshipWanderer.World.GetJsonSerializerSettings());
                    File.WriteAllText(f.ToString(),json);
                }
                catch (Exception e)
                {
                    ShowException("Failed to Save",e);
                }
            }
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

                var msg = Wrap(message, width-1).TrimEnd();

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

                if(options.Length == 1)
                    dlg.FocusFirst();
            }

            Application.Run(dlg);

            chosen = result;
            return optionChosen;
        }

        public void ShowActorStats(IActor actor)
        {
            if (actor == null)
            {
                ShowMessage("No World","No game is currently loaded");
                return;
            }

            var dlg = new ActorDialog(actor);
            Application.Run(dlg);
        }

        public bool GetChoice<T>(string title, string body, out T chosen, params T[] options)
        {
            return RunDialog(title, body, out chosen, options);
        }

        public void Refresh()
        {
            UpdateActions();

            UpdateRoomFrame();

            RedrawMap();
        }

        public void ShowMessage(string title, LogEntry showThenLog)
        {
            ShowMessage(title,showThenLog.Message);
            Log.Info(showThenLog);
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

                    if (World.Map.ContainsKey(pointToRender) && World.Map[pointToRender].IsExplored)
                        sb.Append(World.Map[pointToRender].Tile);
                    else
                        sb.Append(' ');
                }

                sb.Append('\n');
            }

            _lblMap.Text = sb.ToString();
        }

        public void ShowMessage(string title, string body)
        {
            RunDialog(title,body,out _,"Ok");
        }

        readonly List<Button> _oldButtons = new List<Button>();

        readonly List<Point> _buttonLocations = new List<Point>()
        {
            new Point(0,16),
            new Point(0,17),
            new Point(0,18),
            new Point(0,19),
            new Point(12,16),
            new Point(12,17),
            new Point(12,18),
            new Point(12,19),
            new Point(24,16),
            new Point(24,17),
            new Point(24,18),
            new Point(24,19),
            new Point(36,16),
            new Point(36,17),
            new Point(36,18),
            new Point(36,19),
            new Point(48,16),
            new Point(48,17),
            new Point(48,18),
            new Point(48,19),

        };

        


        public void UpdateActions()
        {
            Title = $"Map ({World.Player.CurrentLocation.GetPoint()})";

            foreach(Button b in _oldButtons)
                Remove(b);
            
            _oldButtons.Clear();

            int buttonLoc = 0;

            var allActions = World.Player.GetFinalActions().Where(a=>a.HasTargets(World.Player));

            //don't run out of UI spaces! (maybe we can page this later on if we get too many unique actions to render)
            foreach (var action in allActions.Take(_buttonLocations.Count))
            {
                var btn = new Button(_buttonLocations[buttonLoc].X, _buttonLocations[buttonLoc].Y, action.Name, false)
                {
                    Width = 10,
                    Height = 1,
                    Clicked = () =>
                    {
                        try
                        {
                            World.RunRound(this, action);
                        }
                        catch (Exception e)
                        {
                            ShowException("Error During Round",e);
                        }
                    }
                };

                _oldButtons.Add(btn);
                this.Add(btn);
                buttonLoc++;
            }
        }
        private void UpdateRoomFrame()
        {

            _roomFrame.Title = World.Player.CurrentLocation.ToString();
            _roomFrame.RemoveAll();
            
            List<string> contents = new List<string>();

            contents.Add("Faction:" + (World.Player.CurrentLocation.ControllingFaction?.Name ?? "None"));

            contents.AddRange(World.Player.GetCurrentLocationSiblings(true).Select(s=>s.ToString()));
            contents.AddRange(World.Player.CurrentLocation.Items.Select(s=>s.ToString()));

            View addLabelsTo;

            //if it is too many items
            if (contents.Count > ROOM_FRAME_HEIGHT - 3)
            {
                //use a scroll view
                var view = new ScrollView(new Rect(0, 0, ROOM_FRAME_WIDTH, ROOM_FRAME_HEIGHT-2))
                {
                    ContentSize = new Size(ROOM_FRAME_WIDTH, contents.Count + 1),
                    ContentOffset = new Point(0, 0),
                    ShowVerticalScrollIndicator = true,
                    ShowHorizontalScrollIndicator = false
                };

                addLabelsTo = view;
                _roomFrame.Add(view);
            }
            else
                addLabelsTo = _roomFrame; //otherwise just labels

            for (int i = 0; i < contents.Count; i++)
                addLabelsTo.Add(new Label(0, i, contents[i]));
        }

        public string Wrap(string s, int width)
        {
            var r = new Regex(@"(?:((?>.{1," + width + @"}(?:(?<=[^\S\r\n])[^\S\r\n]?|(?=\r?\n)|$|[^\S\r\n]))|.{1,16})(?:\r?\n)?|(?:\r?\n|$))");
            return r.Replace(s, "$1\n");
        }
    }
}