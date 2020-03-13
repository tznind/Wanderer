using System;
using System.Linq;
using Game.UI;
using Wanderer;
using Wanderer.Factories;
using Terminal.Gui;
using CommandLine;
using Wanderer.Systems.Validation;
using Wanderer.Editor;
using System.IO;
using NLog;
using NLog.Fluent;
using NLog.Targets;

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

            [Option('c', "compile", Required = false, HelpText = "Compile a simple proto dialogue tree into yaml")]
            public string CompileDialogue { get; set; }
            
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {

                       if(!string.IsNullOrWhiteSpace(o.CompileDialogue))
                       {
                           var b = new DialogueBuilder();
                           try
                           {
                                var result = b.BuildAndSerialize(new FileInfo(o.CompileDialogue));
                                
                                if(result == null)
                                    Console.WriteLine("No file created");
                                else
                                    Console.WriteLine("Created " + result.FullName);
                           }
                           catch(Exception ex)
                           {
                               Console.WriteLine(ex.Message);
                           }
                           return;
                       }

                       var f = new WorldFactory();
                       if(o.ResourcesDirectory != null)
                            f.ResourcesDirectory = o.ResourcesDirectory;

                       if (o.Validate)
                       {
                           var validator = new WorldValidator();
                            validator.IncludeStackTraces = false;
                            validator.Validate(f);
                            

                            if(validator.Warnings.Length > 0)
                            {
                                Console.WriteLine("WARNINGS:");
                                Console.WriteLine(validator.Warnings);
                            }

                            if(validator.Errors.Length > 0)
                            {
                                Console.WriteLine("ERRORS:");
                                Console.WriteLine(validator.Errors);
                            }


                            Console.WriteLine("Finished Validation:");
                            Console.WriteLine(validator.ErrorCount + " Errors");
                            Console.WriteLine(validator.WarningCount + " Warnings");
                       }
                       else
                       {
                            //Don't log to the console when Console Gui is running 
                            LogManager.Configuration.RemoveTarget("Console");

                            Application.Init();
                            var mainWindow = new MainWindow(f);
                            Application.Top.Add(mainWindow);
                            Application.Run();                
                       }
                   });
        }
    }
}
