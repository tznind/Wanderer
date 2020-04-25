using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wanderer;
using Wanderer.Relationships;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;
using Rune = System.Rune;

namespace Wanderer.TerminalGui
{
    class FactionsView : View
    {
        private List<IFaction> _factions;
        private List<Label> _relationshipBars;
        private ColorScheme _redScheme;
        private ColorScheme _greenScheme;

        Dictionary<IFaction,double[]> _factionFeelings = new Dictionary<IFaction, double[]>();
        private ListView _listview;

        public void InitializeComponent(IWorld world, in int dlgWidth, in int dlgHeight)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            _factions = world.Factions.ToList();
            _relationshipBars = new List<Label>();
            
            //for each faction
            foreach (var f in _factions)
            {
                _factionFeelings.Add(f,new double[_factions.Count]);

                for (int i = 0; i < _factions.Count; i++)
                {
                    //for each other faction
                    _factionFeelings[f][i] = world.Relationships
                        .OfType<FactionRelationship>()
                        .Where(r => r.HostFaction == f && r.AppliesTo(_factions[i]))
                        .Sum(r => r.Attitude);
                }
            }
            
            _listview = new ListView(world.Factions.ToList())
            {
                Width = Dim.Percent(40),
                Height = Dim.Fill()
            };
            _listview.SelectedChanged += UpdateGraphs;

            Add(_listview);

            for (var index = 0; index < _factions.Count; index++)
            {

                var pb = new Label("attitude")
                {
                    X = Pos.Right(_listview),
                    Y = index,
                    Width = Dim.Fill(),
                };
                _relationshipBars.Add(pb);
                Add(pb);
            }

            var green = Attribute.Make(Color.BrightGreen, Color.Black);
            var red = Attribute.Make(Color.BrightRed,Color.Black); 

            _redScheme = new ColorScheme()
            {
                Focus = red,
                HotFocus = red,
                HotNormal = red,
                Normal = red
            };

            _greenScheme = new ColorScheme()
            {
                Focus = green,
                HotFocus = green,
                HotNormal = green,
                Normal = green
            };

            UpdateGraphs();

        }

        private void UpdateGraphs()
        {
            var selectedFaction = _factions[_listview.SelectedItem];

            for (var index = 0; index < _relationshipBars.Count; index++)
            {
                
                //how strong
                var feeling = _factionFeelings[selectedFaction][index];
                
                if(Math.Abs(feeling) < 0.001)
                {
                    _relationshipBars[index].Text = "";
                }
                else
                if(feeling > 0)
                {
                    _relationshipBars[index].ColorScheme = _greenScheme;
                    _relationshipBars[index].Text = new string('+',Math.Min(10,Math.Max(1,(int)(feeling/10.0))));
                }
                else
                {
                    _relationshipBars[index].ColorScheme = _redScheme;
                    _relationshipBars[index].Text = new string('-',Math.Min(10,Math.Max(1,(int)(Math.Abs(feeling)/10.0))));
                }

                _relationshipBars[index].ColorScheme = feeling < 0 ? _redScheme : _greenScheme;
            }
        }
    }
}
