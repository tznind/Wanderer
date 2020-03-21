using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Factories;
using Terminal.Gui;
using Wanderer.Actions;
using Wanderer.Items;

namespace Game.UI
{
    public class 
    MainWindow : Window, IUserinterface
    {
        private readonly WorldFactory _worldFactory;
        public int DlgWidth = 78;
        public int DlgHeight = 18;
        public int DlgBoundary = 2;
        
        public IWorld World { get; set; }
        public EventLog Log { get; }
        private readonly SplashScreen _splash;

        private ListView _roomContents;
        private List<IHasStats> _roomContentsObjects;
        private HasStatsView _detail;
        private MapView _mapView;
        private RoomContentsRenderer _roomContentsRenderer = new RoomContentsRenderer();
        ActionManager _actionManager = new ActionManager();
        public bool ShowMap => World?.Map != null && _detail == null;
        
        public MainWindow(WorldFactory worldFactory):base("Game")
        {
            X = 0;
            Y = 1;
            Width = Dim.Fill();
            Height = Dim.Fill();

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
                    new MenuItem ("_Character", null, () => { ShowStats(World?.Player); }),
                    new MenuItem ("_Factions", null, () => { ShowFactions(World); })
                })
            });
            top.Add (menu);
            
            _splash = new SplashScreen(){X = 4,Y=4};
            _mapView = new MapView()
            {
                X = 0,
                Y = -1,
                Width = Dim.Percent(70)-1,
                Height = Dim.Fill() - 5
            };

            Add(_splash);
        }

        private void ShowFactions(IWorld world)
        {
            if (world == null)
            {
                ShowMessage("No World","No game is currently loaded");
                return;
            }

            var v = new FactionsView();
            v.InitializeComponent(world,DlgWidth,DlgHeight);
            var dlg = new ModalDialog(this,"Factions",v);
            Application.Run(dlg);
        }


        public void NewGame()
        {

            try
            {
                var newWorld = _worldFactory.Create();

                var dlg = new NewPlayerDialog(this,newWorld.Player,new AdjectiveFactory());

                Application.Run(dlg);

                if(!dlg.Ok)
                    return;

                SetWorld(newWorld);
            }
            catch (Exception e)
            {
                ShowException("Error Creating World",e);
            }

        }

        private void SetWorld(IWorld newWorld)
        {
            World = newWorld;
            Log.Clear();
            
            Remove(_splash);
            
            _mapView.World = World;
            Add(_mapView);


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
                    
                    var newWorld = JsonConvert.DeserializeObject<IWorld>(json, Wanderer.World.GetJsonSerializerSettings());
                    
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
                    var json = JsonConvert.SerializeObject(World, Wanderer.World.GetJsonSerializerSettings());
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
                string.Join('\n',Log.Target.Logs.Reverse()/*most recent first*/),out _,"Ok");
        }

        bool RunDialog<T>(string title, string message,out T chosen, params T[] options)
        {
            var result = default(T);
            bool optionChosen = false;

            var dlg = new Dialog(title, DlgWidth, DlgHeight);
            
            var line = DlgHeight - (DlgBoundary)*2 - options.Length;

            if (!string.IsNullOrWhiteSpace(message))
            {
                int width = DlgWidth - (DlgBoundary * 2);

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
                T v1 = value;

                string name = value.ToString();

                var btn = new Button(0, line++, name)
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

        public void ShowStats(IHasStats of)
        {
            if (of == null)
            {
                ShowMessage("No World","No game is currently loaded");
                return;
            }

            var v = new HasStatsView();
            v.InitializeComponent(of as IActor ?? World.Player,of,DlgWidth,DlgHeight);
            var dlg = new ModalDialog(this,of.Name,v);
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

            TriggerTerminalResized();
            
        }

        private void TriggerTerminalResized()
        {
            typeof(Application).GetMethod("TerminalResized", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        }

        public void ShowMessage(string title, LogEntry showThenLog)
        {
            ShowMessage(title,showThenLog.Message);
            Log.Info(showThenLog);
        }

        public void ShowMessage(string title, string body)
        {
            RunDialog(title,body,out _,"Ok");
        }

        readonly List<Button> _oldButtons = new List<Button>();

        readonly List<Tuple<Pos,Pos>> _buttonLocations = new List<Tuple<Pos, Pos>>()
        {
            //TODO: this should expand infinitely (with optional scroll view)
            Tuple.Create(Pos.At(0),Pos.Percent(100)-4),
            Tuple.Create(Pos.At(0),Pos.Percent(100)-3),
            Tuple.Create(Pos.At(0),Pos.Percent(100)-2),
            Tuple.Create(Pos.At(0),Pos.Percent(100)-1),
            Tuple.Create(Pos.At(12),Pos.Percent(100)-4),
            Tuple.Create(Pos.At(12),Pos.Percent(100)-3),
            Tuple.Create(Pos.At(12),Pos.Percent(100)-2),
            Tuple.Create(Pos.At(12),Pos.Percent(100)-1),
            Tuple.Create(Pos.At(24),Pos.Percent(100)-4),
            Tuple.Create(Pos.At(24),Pos.Percent(100)-3),
            Tuple.Create(Pos.At(24),Pos.Percent(100)-2),
            Tuple.Create(Pos.At(24),Pos.Percent(100)-1),
            Tuple.Create(Pos.At(36),Pos.Percent(100)-4),
            Tuple.Create(Pos.At(36),Pos.Percent(100)-3),
            Tuple.Create(Pos.At(36),Pos.Percent(100)-2),
            Tuple.Create(Pos.At(36),Pos.Percent(100)-1),
            Tuple.Create(Pos.At(48),Pos.Percent(100)-4),
            Tuple.Create(Pos.At(48),Pos.Percent(100)-3),
            Tuple.Create(Pos.At(48),Pos.Percent(100)-2),
            Tuple.Create(Pos.At(48),Pos.Percent(100)-1),

        };

        public void UpdateActions()
        {
            Title = $"Map ({World.Player.CurrentLocation.GetPoint()})";

            foreach(Button b in _oldButtons)
                Remove(b);
            
            _oldButtons.Clear();

            int buttonLoc = 0;

            //don't run out of UI spaces! (maybe we can page this later on if we get too many unique actions to render)
            foreach (var actionDescription in _actionManager.GetTypes(World.Player,true).Take(_buttonLocations.Count))
            {
                var name = GetActionButtonName(actionDescription);

                var btn = new Button( name, false)
                {
                    X = _buttonLocations[buttonLoc].Item1, 
                    Y= _buttonLocations[buttonLoc].Item2,
                    Width = 10,
                    Height = 1,
                    Clicked = () =>
                    {
                        try
                        {
                            RunRound(actionDescription);
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

        private void RunRound(ActionDescription actionDescription)
        {
            var instances = _actionManager.GetInstances(World.Player, actionDescription, true);

            if(instances.Count == 1 && !(instances[0] is DialogueAction)) /* Always ask for this*/
                World.RunRound(this, instances.Single());
            else
            if(instances.Count == 0)
                ShowMessage("No targets","No valid targets");
            else
            if (GetChoice("Action", null, out IAction chosen, instances.ToArray())) 
                World.RunRound(this, chosen);


            this.Refresh();
        }

        private string GetActionButtonName(ActionDescription action)
        {
            //indicate hotkey by using underscore
            var idx = action.Name.ToLower().IndexOf(char.ToLower(action.HotKey));

            //the first upper case character becomes the key
            return idx > 0 
                //lower everything up to the index, upper the index then add the rest
                ? action.Name.Substring(0,idx).ToLower() + char.ToUpper(action.HotKey) +  (action.Name.Length == idx ? "":action.Name.Substring(idx+1)):
                action.Name;
        }

        private void UpdateRoomFrame()
        {
            var room = World.Player.CurrentLocation;

            _roomContentsObjects = new List<IHasStats>();
            _roomContentsObjects.Add( room);

            if(room.ControllingFaction != null)
                _roomContentsObjects.Add(room.ControllingFaction);
            
            _roomContentsObjects.AddRange(World.Player.GetCurrentLocationSiblings(true));
            _roomContentsObjects.AddRange(World.Player.CurrentLocation.Items);

            if (_roomContents != null)
            {
                Remove(_roomContents);
                _roomContents.SelectedChanged -= UpdateDetailPane;
            }

            //use a scroll view
             _roomContentsRenderer.SetCollection(_roomContentsObjects);
            _roomContents = new ListView( _roomContentsRenderer)
            {
                X=Pos.Percent(70),
                Width = Dim.Fill(),
                Height = Dim.Percent(75)
            };

            _roomContents.SelectedChanged += UpdateDetailPane;

            Add(_roomContents);
        }

        public override bool ProcessKey(KeyEvent keyEvent)
        {
            try
            {
                return base.ProcessKey(keyEvent);
            }
            finally
            {
                UpdateDetailPane();
            }
        }

        public override bool ProcessColdKey(KeyEvent keyEvent)
        {
            if (keyEvent.Key == Key.Enter && _roomContents != null)
            {
                if(_roomContents.SelectedItem < _roomContentsObjects.Count)
                {
                    var target = _roomContentsObjects[_roomContents.SelectedItem];

                    var options = World.Player.GetFinalActions()
                    .Where(a=> a.Owner == target ||
                    a.GetTargets(World.Player).Contains(target)
                    )
                    .ToArray();

                    if(options.Any() && GetChoice("Action",null,out IAction chosen,options))
                    {
                        if(chosen is FightAction f)
                            f.PrimeWithTarget = target as IActor;
                        if (chosen is PickUpAction p)
                            p.PrimeWithTarget = target as IItem;

                        World.RunRound(this, chosen);
                        Refresh();

                        SetFocus(_roomContents);
                        _roomContents.SelectedItem = 0;
                    }
                }
            }

            if (keyEvent.Key == Key.Esc && _detail != null )
            {
                HideDetailPane();
                var button = _oldButtons?.FirstOrDefault();
                if (button != null)
                    SetFocus(button);

                TriggerTerminalResized();
            }

            return base.ProcessColdKey(keyEvent);
        }

        private void UpdateDetailPane()
        {
            if(_roomContents == null)
                return;

            var selected = _roomContents.SelectedItem;
            
            if(_detail != null)
                Remove(_detail);

            if(_mapView != null)
                Remove(_mapView);
                
            if (_roomContents.HasFocus)
            {
                _detail = new HasStatsView()
                {
                    AllowScrolling = false
                };

                var o = _roomContentsObjects[selected];

                _detail.InitializeComponent(o as IActor ?? World.Player,o,Bounds.Width,Bounds.Height-3);
                _detail.X = 1;
                _detail.Y = 1;
                _detail.Width = Dim.Percent(70);
                _detail.Height = Dim.Fill() - 5;

                Add(_detail);
            }
            else
            {
                HideDetailPane();
            }

            TriggerTerminalResized();
        }

        private void HideDetailPane()
        {
            if(_detail != null)
                Remove(_detail);

            _detail = null;

            if(_mapView != null)
                Add(_mapView);
        }

        public string Wrap(string s, int width)
        {
            var r = new Regex(@"(?:((?>.{1," + width + @"}(?:(?<=[^\S\r\n])[^\S\r\n]?|(?=\r?\n)|$|[^\S\r\n]))|.{1,16})(?:\r?\n)?|(?:\r?\n|$))");
            return r.Replace(s, "$1\n");
        }
    }
}