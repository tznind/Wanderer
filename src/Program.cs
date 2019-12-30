using System;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;
using StarshipWanderer.UI;
using Terminal.Gui;

namespace StarshipWanderer
{
    class Program
    {


        static void Main(string[] args)
        {
            Application.Init();
            var log = new EventLog();
            log.Register();
            
            var f = new WorldFactory();

            var mainWindow = new MainWindow(f.Create(),log);
            Application.Top.Add(mainWindow);
            Application.Run();
        }
    }
}
