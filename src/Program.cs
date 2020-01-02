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
            
            var mainWindow = new MainWindow(new WorldFactory());
            Application.Top.Add(mainWindow);
            Application.Run();
        }
    }
}
