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
            var mainWindow = new MainWindow(new World(new You(),new RoomFactory(new ActorFactory())));
            Application.Top.Add(mainWindow);
            Application.Run();
        }
    }
}
