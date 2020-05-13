[![Nuget](https://img.shields.io/nuget/v/Wanderer)](https://www.nuget.org/packages/Wanderer/) [![Build Status](https://travis-ci.com/tznind/Wanderer.svg?branch=master)](https://travis-ci.org/tznind/Wanderer) [![codecov](https://codecov.io/gh/tznind/Wanderer/branch/master/graph/badge.svg)](https://codecov.io/gh/tznind/Wanderer) [![Coverage Status](https://coveralls.io/repos/github/tznind/Wanderer/badge.svg?branch=master)](https://coveralls.io/github/tznind/Wanderer?branch=master) [![Total alerts](https://img.shields.io/lgtm/alerts/g/tznind/Wanderer.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/tznind/Wanderer/alerts/)

# Wanderer

Wanderer is a game engine for developing dialogue rich scene based exploration games.

![Screenshot of development autocomple][coding]

## Goals

1. Simple yaml/lua game files (See [Cookbook])
2. [Super thin interface layer](./src/IUserinterface.cs)
3. [Maximum Test coverage](https://codecov.io/gh/tznind/Wanderer)
4. All the systems (with simple class interfaces to replace / inject your own)
   1. Injury systems (model fire, [hunger](./src/Resources/InjurySystems/Hunger.injury.yaml), [slashing / piercing](./src/Resources/InjurySystems/TissueInjury.injury.yaml) etc all directly from yaml)
   2. Negotiation system (coerce npcs to do perform actions)
   3. Relationship system (build love/hate with npc based on actions/dialogue)
   4. Factions
   5. Planning system (player influenced AI e.g. with leadership actions)   

## Getting Started

If you have trouble with this section you can refer to the [Wanderer Template Repository](https://github.com/tznind/WandererTemplate) which was produced by following this tutorial.

Create a new C# console application (requires [dotnet 3.1 sdk](https://dotnet.microsoft.com/download)):

```bash
dotnet new console
dotnet add package Wanderer
dotnet add package Wanderer.TerminalGui
```

Open Program.cs and add the following to main:

```csharp
using Terminal.Gui;
using Wanderer.Factories;
using Wanderer.TerminalGui;

namespace TestWandererInstructions
{
    class Program
    {
        static void Main(string[] args)
        {
            var worldFactory = new WorldFactory();
            
            Application.Init();
                            
            var ui = new MainWindow(worldFactory);
            Application.Top.Add(ui);
            Application.Run();             
        }
    }
}
```

Run the game (Use the dll name of your project):

```bash
dotnet build
dotnet ./bin/Debug/netcoreapp3.1/TestWandererInstructions.dll
```

At this point starting a new game should give an error about a missing Resources directory.  Edit your csproj file and add the following (this will mark all files under Resources as copy to bin directory).

```xml
  <ItemGroup>
    <Content Include="Resources\**\*">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
```

Now create a Resources folder in your project directory and add a new file "rooms.yaml".  Add exactly the following (including hyphens and whitespace):

```yaml
- Name: Endless Plains
  MandatoryActors:
    - Name: Girafe
```

To talk to the Girafe we can create some dialogue.  Create a file in the Resources directory called `dialogue.yaml`.

Fill it with some dialogue using the [online dialogue editor](https://tznind.github.io/WandererTools/) or just use the following:

```
- Identifier: b270c715-84e1-49b6-90df-91ea9e34075c
  Body:
  - Text: Hello there
  Options:
  - Text: hello yourself
    Destination: 8cd356a8-13e4-4ebe-9fb5-0f8d9899ff26
  - Text: that's enough of that!
- Identifier: 8cd356a8-13e4-4ebe-9fb5-0f8d9899ff26
  Body:
  - Text: How are you this fine morning?
  Options:
  - Text: Good
    Destination: 88d3f46a-69ef-4fd5-b7a6-98486d8cacea
  - Text: Bad
    Destination: 1101116d-34c4-48c5-a76b-353aa138dafa
- Identifier: 88d3f46a-69ef-4fd5-b7a6-98486d8cacea
  Body:
  - Text: Me too
- Identifier: 1101116d-34c4-48c5-a76b-353aa138dafa
  Body:
  - Text: Sorry to hear that
```

Finally update `rooms.yaml` to associate the dialogue with the Girafe

```yaml
- Name: Endless Plains
  MandatoryActors:
    - Name: Girafe
      Dialogue:
         Next: b270c715-84e1-49b6-90df-91ea9e34075c
```


Add more content by following the recipes in the [Cookbook].

## Creating your own UI

The above example uses the `Wanderer.TerminalGui` package (and `MainWindow` class) for the game user interface.  The engine itself is written in dotnet standard so is not tied to the Console.  It can run as a Blazor web app, WinForms, ETO etc.

[IUserInterface] is a very simple interface to implement so building a more advanced user interface is easy.  The best way to start is to run the game with the bare bones [ExampleUserInterface] (see below) then swap in your own implementation of [IUserInterface].

```csharp
var factory = new WorldFactory(Environment.CurrentDirectory);
var world = factory.Create();
var ui = new ExampleUserInterface();

while (!world.Player.Dead)
{
    // get user to pick an action
    ui.GetChoice("Pick Action", null, out IAction chosen, world.Player.GetFinalActions().ToArray());
    Console.Clear();

    // run the chosen action in the world
    world.RunRound(ui,chosen);   

    // tell player what happened
    ui.ShowMessage("Results", string.Join(Environment.NewLine,ui.Log.RoundResults));
}
```

## Auto-Complete

Yaml autocomplete is supported via schemas.  The easiest way to get this working is to use Visual Studio Code and install the [Redhat YAML plugin](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml).  Download the [latest schemas](./src/Resources/Schemas) and reference them from your [workspace](./Wanderer.code-workspace).

```json
"settings": {

		"yaml.schemas": {
			"./src/Resources/Schemas/injury.schema.json": [ "/**/*injury.yaml" ],
			"./src/Resources/Schemas/dialogue.schema.json": [ "/**/*dialogue.yaml" ],
			"./src/Resources/Schemas/adjectives.schema.json": [ "/**/*adjectives.yaml" ],
			"./src/Resources/Schemas/actions.schema.json": [ "/**/*actions.yaml" ],
			"./src/Resources/Schemas/items.schema.json": [ "/**/*items.yaml" ],
			"./src/Resources/Schemas/rooms.schema.json": [ "/**/*rooms.yaml" ],
			"./src/Resources/Schemas/actors.schema.json": [ "/**/*actors.yaml" ]
		}
    }
```

## Validation

You can validate your resource files using the `WorldValidator` class.  This tests not only that the yaml files can be deserialized but also that there are no missing references (e.g. dialogue) and that conditions, effects and behaviours have valid Lua code which can be executed.

For example:

```csharp
var worldFactory = new WorldFactory();
                            
var validator = new WorldValidator();
validator.IncludeStackTraces = true;
validator.Validate(worldFactory);

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
```

## Building

Build and publish with the `dotnet` command (requires installing [Dot Net 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1))

```bash
dotnet build
dotnet test
```

## Games built with Wanderer

- [Starship Wanderer](https://github.com/tznind/StarshipWanderer/)

## Class Diagram

![Overview of classes in game][classDiagram]

[classDiagram]: ./src/Overview.cd.png
[screenshot1]: ./src/Screen1.png
[screenshot2]: ./src/Screen2.png
[screenshot3]: ./src/Screen3.png
[gameplay]: ./src/gameplay.gif
[coding]: ./WandererCoding.gif
[Cookbook]: ./Cookbook.md
[Splash]: ./src/splash.png
[IUserInterface]: ./src/IUserinterface.cs
[ExampleUserInterface]: ./src/ExampleUserInterface.cs
