using System;
using System.Linq;
using Game.UI;
using Wanderer;
using Wanderer.Factories;
using Wanderer.Validation;
using Terminal.Gui;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {

            var f = new WorldFactory();

            if(args.Any(a=>string.Equals(a,"--validate",StringComparison.CurrentCultureIgnoreCase)))
            {
                var validator = new WorldValidator();
                validator.IncludeStackTraces = false;
                validator.Validate(f);
                
                Console.WriteLine(validator.Warnings);

                if(validator.Errors.Length > 0)
                    Console.WriteLine(validator.Errors);
                else
                    Console.WriteLine("Validation Passed");
            }
            else
            {
                Application.Init();
                var mainWindow = new MainWindow(f);
                Application.Top.Add(mainWindow);
                Application.Run();                
            }

        }
    }
}
