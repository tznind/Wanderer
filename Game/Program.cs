using System;
using System.Linq;
using Game.UI;
using Wanderer;
using Wanderer.Factories;
using Wanderer.Validation;
using Terminal.Gui;
using CommandLine;

namespace Game
{
    class Program
    {



        public class Options
        {
            [Option('v', "validate", Required = false, HelpText = "Validate yaml game files.")]
            public bool Validate { get; set; }


            [Option('r', "resources", Required = false, HelpText = "Location of yaml game files")]
            public string ResourcesDirectory { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {

                       var f = new WorldFactory();
                       if(o.ResourcesDirectory != null)
                            f.ResourcesDirectory = o.ResourcesDirectory;

                       if (o.Validate)
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
                   });
        }
    }
}
