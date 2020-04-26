[![Nuget](https://img.shields.io/nuget/v/Wanderer)](https://www.nuget.org/packages/Wanderer/) [![Build Status](https://travis-ci.com/tznind/Wanderer.svg?branch=master)](https://travis-ci.org/tznind/Wanderer) [![codecov](https://codecov.io/gh/tznind/Wanderer/branch/master/graph/badge.svg)](https://codecov.io/gh/tznind/Wanderer) [![Coverage Status](https://coveralls.io/repos/github/tznind/Wanderer/badge.svg?branch=master)](https://coveralls.io/github/tznind/Wanderer?branch=master) [![Total alerts](https://img.shields.io/lgtm/alerts/g/tznind/Wanderer.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/tznind/Wanderer/alerts/)

Wanderer is a game engine for developing dialogue rich scene based exploration games.

## Goals

Technical Goals:

1. Simple yaml/lua game files (See [Cookbook] and [Tutorial])
2. [Super thin interface layer](./src/IUserinterface.cs)
3. [Maximum Test coverage](https://codecov.io/gh/tznind/Wanderer)

## Contributing

Creating content is designed to be as simple as possible (see below):

![Screenshot of development autocomple][coding]

Autocomplete is supported via yaml schemas.  The easiest way to get this working is to use Visual Studio Code and install the [Redhat YAML plugin](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml).  When you open the [workspace](./Wanderer.code-workspace) autocomplete and hover comments should automatically appear.

A good place to start is the [Tutorial] and/or [Cookbook].

## Building

Build and publish with the `dotnet` command (requires installing [Dot Net 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1))

```bash
dotnet build
dotnet test
```

## Getting Started

Create a new C# console application (e.g. dotnet core 3.1):

```bash
dotnet new console
dotnet add package Wanderer
```

Open Program.cs and add the following to main:

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

Create a new file "rooms.yaml" and mark it as Content, Copy to output directory (this should update your csproj file with):

```xml
<ItemGroup>
  <Content Include="rooms.yaml">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

In "rooms.yaml" add exactly the following (including hyphens and whitespace):

```yaml
- Name: Endless Plains
  MandatoryActors:
    - Name: Girafe
```

Now when you run the game you should have a companion in your lonely world.  Add more content by following the [Tutorial] then take a look at the [Cookbook] for more recipes.

[IUserInterface] is a very simple interface to implement so building a more advanced user interface is easy, you could even create a full GUI in GTK, WinForms etc.

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
