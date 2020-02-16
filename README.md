[![Build Status](https://travis-ci.org/tznind/Wanderer.svg?branch=master)](https://travis-ci.org/tznind/Wanderer) [![codecov](https://codecov.io/gh/tznind/Wanderer/branch/master/graph/badge.svg)](https://codecov.io/gh/tznind/Wanderer) [![Coverage Status](https://coveralls.io/repos/github/tznind/Wanderer/badge.svg?branch=master)](https://coveralls.io/github/tznind/Wanderer?branch=master) [![Total alerts](https://img.shields.io/lgtm/alerts/g/tznind/Wanderer.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/tznind/Wanderer/alerts/)



# Wanderer

The goal of this project is to create a game in which you wander a city sized starship interacting with the inhabitants and generally getting up to trouble.  The following goals are intended for the project:

1. Any action the Player can do Non Player Characters (Npc) can do
2. Persuasion and Cunning should be as viable as Fighting
3. No dice rolls for actions (if you have Fight 10 and you fight someone with Fight 15 the outcome should always be the same)
4. Actions should have consequences (for relationships, long term injuries etc).

Flexibility and extensibility are also core aims.  To this end: 

1. All narrative, dialogues, blueprints etc should come from [external yaml/csharp scripting files](./Resources.md)
2. The library (`Wanderer`) must remain seperate from the UI (`Game.csproj`) with a very thin [interface layer](./src/IUserinterface.cs) to allow for any future UI formats.

![Screenshot of gameplay showing map][screenshot1]
![Screenshot of gameplay showing narrative][screenshot2]
![Screenshot of gameplay showing dialogue][screenshot3]

You can [download the compiled binaries in the Releases Section](https://github.com/tznind/Wanderer/releases) for both windows and linux console.

**The game is in very early development and not really ready for playing.  Open issues for suggestions or if you like the idea and what to get involved.**

## Building

Build and publish with the `dotnet` command (requires installing [Dot Net 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1))

```bash
dotnet build
dotnet publish -r win-x64
```
__Substitute win-x64 for linux-x64 etc depending on your OS__

After publishing you can run the game from the Game project bin directory

```
cd ./Game/bin/Debug/netcoreapp3.1/win-x64/publish/
./Game.exe
```
__For linux drop the .exe extension__

## Development Goals

All narrative elements driven by the [yaml configuration files](./src/Resources/README.md).  This includes Adjectives, Factions, Rooms, Item slots etc.  The [UI layer is super thin](./src/IUserinterface.cs), consisting of only a handful of methods.

- For every [interface](./src/Actors/IActor.cs), an [abstract](./src/Actors/Actor.cs)!
- For every abstract a [blueprint](./src/Factories/Blueprints/ActorBlueprint.cs)
- For every blueprint a [factory](./src/Factories/ActorFactory.cs)
- For every line of code a [Test](./Tests/Actors/YamlActorFactoryTests.cs)!

Other design patterns include:

- Guids for everyth object in the world (that matters)
- Everything [HasStats](./src/IHasStats.cs): an actor has stats, the room has stats, his items and the items Adjectives all [HasStats](./src/IHasStats.cs).  Yes that means the Light on the end of your Torch has the capability to talk to you (isn't that awesome?!)

## Dependencies

[Terminal.Gui](https://github.com/migueldeicaza/gui.cs) - Only the most awesome console gui ever!
[CommandLineParser](https://github.com/commandlineparser/commandline) - For parsing CLI args
[YamlDotNet](https://github.com/aaubry/YamlDotNet) - Markup language parser
[JSON.Net](https://github.com/JamesNK/Newtonsoft.Json) - For saving/loading
[Rosyln](https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples) - For parsing C# in script files

## Class Diagram

![Overview of classes in game][classDiagram]

[classDiagram]: ./src/Overview.cd.png
[screenshot1]: ./src/Screen1.png
[screenshot2]: ./src/Screen2.png
[screenshot3]: ./src/Screen3.png
