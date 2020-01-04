using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NStack;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using Terminal.Gui;

namespace StarshipWanderer.UI
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


        private FrameView _roomFrame;
        private FrameView _actionFrame;

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

            Application.Run(dlg);

            if(!dlg.Ok)
                return;

            World = newWorld;
            Log.Clear();
            
            Remove(_splash);
            Add(_lblMap);
            Add(_actionFrame);
            Add(_roomFrame);

            Refresh();
        }

        private void LoadGame()
        {
            var ofd = new OpenDialog("Load Game", "Enter file path to load");
            ofd.AllowedFileTypes = new[] {".json"};

            ofd.CanChooseDirectories = false;
            ofd.AllowsMultipleSelection = false;

            Application.Run(ofd);

            var f = ofd.FilePaths.SingleOrDefault();
            if ( f != null)
            {
                try
                {
                    
                    var json = File.ReadAllText(f);
                    
                    World = JsonConvert.DeserializeObject<IWorld>(json, StarshipWanderer.World.GetJsonSerializerSettings());

                    Refresh();
                }
                catch (Exception e)
                {
                    ShowMessage("Failed to Load",e.ToString(),false,Guid.Empty);
                }
            }
        }
        
        private void SaveGame()
        {
            if(World == null)
            {
                ShowMessage("No World","No game is currently loaded",false,Guid.Empty);
                return;
            }

            var sf = new SaveDialog("Save Game", "Enter save file path");
            sf.AllowedFileTypes = new[] {".json"};
            
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
                    ShowMessage("Failed to Save",e.ToString(),false,Guid.Empty);
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
            if (actor == null)
            {
                ShowMessage("No World","No game is currently loaded",false,Guid.Empty);
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
            new Point(15,16),
            new Point(15,17),
            new Point(15,18),
            new Point(15,19),

        };

        


        public void UpdateActions()
        {
            Title = $"Map ({World.Player.CurrentLocation.GetPoint()})";

            foreach(Button b in _oldButtons)
                Remove(b);
            
            _oldButtons.Clear();

            int buttonLoc = 0;

            var allActions = World.Player.GetFinalActions();

            foreach (var action in allActions)
            {
                var btn = new Button(_buttonLocations[buttonLoc].X, _buttonLocations[buttonLoc].Y, action.Name, false)
                {
                    Width = 10,
                    Height = 1,
                    Clicked = () =>
                    {
                        World.RunRound(this, action);
                    }
                };

                _oldButtons.Add(btn);
                this.Add(btn);
                buttonLoc++;
            }
        }
        private void UpdateRoomFrame()
        {

            _roomFrame.Title = World.Player.CurrentLocation.Name;
            _roomFrame.RemoveAll();

            int row = 0;

            List<string> contents = new List<string>();

            contents.AddRange(World.Player.CurrentLocation.Actors.Where(a => !(a is You)).Select(a => a.Name));

            contents.AddRange(World.Player.CurrentLocation.Items.Select(a => "[I]" + a.Name));

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
    }
}