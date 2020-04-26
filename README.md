[![Nuget](https://img.shields.io/nuget/v/Wanderer)](https://www.nuget.org/packages/Wanderer/) [![Build Status](https://travis-ci.com/tznind/Wanderer.svg?branch=master)](https://travis-ci.org/tznind/Wanderer) [![codecov](https://codecov.io/gh/tznind/Wanderer/branch/master/graph/badge.svg)](https://codecov.io/gh/tznind/Wanderer) [![Coverage Status](https://coveralls.io/repos/github/tznind/Wanderer/badge.svg?branch=master)](https://coveralls.io/github/tznind/Wanderer?branch=master) [![Total alerts](https://img.shields.io/lgtm/alerts/g/tznind/Wanderer.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/tznind/Wanderer/alerts/)

Wanderer is a game engine for developing dialogue rich scene based exploration games.

![Screenshot of development autocomple][coding]

## Goals

Technical Goals:

1. Simple yaml/lua game files (See [Cookbook] and [Tutorial])
2. [Super thin interface layer](./src/IUserinterface.cs)
3. [Maximum Test coverage](https://codecov.io/gh/tznind/Wanderer)

## Getting Started

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

Fill it with some dialogue using the [online dialogue editor](https://tznind.github.io/WandererTools/counter) or just use the following:

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


Add more content by following the [Tutorial] then take a look at the [Cookbook] for more recipes.

[IUserInterface] is a very simple interface to implement so building a more advanced user interface is easy, you could even create a full GUI in GTK, WinForms etc.



## Auto-Complete

Autocomplete is supported via yaml schemas.  The easiest way to get this working is to use Visual Studio Code and install the [Redhat YAML plugin](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml).  When you open the [workspace](./Wanderer.code-workspace) autocomplete and hover comments should automatically appear.

A good place to start is the [Tutorial] and/or [Cookbook].

## Building

Build and publish with the `dotnet` command (requires installing [Dot Net 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1))

```bash
dotnet build
dotnet test
```

## Class Diagram

![Overview of classes in game][classDiagram]

[classDiagram]: ./src/Overview.cd.png
[screenshot1]: ./src/Screen1.png
[screenshot2]: ./src/Screen2.png
[screenshot3]: ./src/Screen3.png
[gameplay]: ./src/gameplay.gif
[coding]: ./WandererCoding.gif
[Cookbook]: ./Cookbook.md
[Tutorial]: ./Resources.md
[Splash]: ./src/splash.png
[IUserInterface]: ./src/IUserInterface.cs
