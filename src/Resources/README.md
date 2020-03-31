# Resources

This file describes the relationship between classes and resource files.  The filename determines how it is processed.  For example all files ending in `dialogue.yaml` are processed as dialogue.  This would include `myroom.dialogue.yaml`, `banter.dialogue.yaml` but also `dialogue.yaml` (i.e. with no prefix).

|   Yaml File      |  Class           | Subdirectory Support |  Notes                   |
|------------------|------------------|-----------------|---------------------------|
| *dialogue.yaml | `DialogueNode[]` | yes |Contains unique dialogue trees.  Each node consists of 1 main piece of text and 0 or more options which can lead to other nodes or end dialogue |
| *injury.yaml | `InjurySystem` | yes | Describes a method of inflicting damage upon Rooms and Actors (e.g. fire, plague, tissue damage etc)|
| *actors.yaml  | `ActorBlueprint[]` | yes | Describes how to create `Actor` instances that fit thematically with any `Faction` / `Room` |
| *rooms.yaml | `RoomBlueprint[]` | yes | Contains descriptions of rooms that can be generated |
| *items.yaml | `ItemBlueprint[]` | yes | Contains generic items that would fit in any room generated (regardless of `Faction` |
| /[Main.lua](./Main.lua) | N\A | no | Defines custom global methods and helper functions for use in scripting blocks |
| /slots.yaml | `SlotCollection` | no | Contains default item slots (e.g. 1 Head, 2 Hand etc) that all `Actor` start with (unless the blueprint lists explicit slots).  Note also that this can be overridden with a [faction slots.yaml](#factions-directory) |
| /[plans.yaml](./plans.yaml) | `Plan` | no | Contains AI Plans for NPC Actors.  These can be influenced by Leadership Actions of others|

## Factions directory

The `/Factions` directory is special.  Any folder under this directory should contain a `faction.yaml` which describes the faction (name, role in game etc).  All yaml files under this directory (including subdirectories) are automatically assigned to thematically fit the parent Faction.  For example creating `cultists.rooms.yaml` in a subdirectory of `/Factions/Cult` would associate the rooms declared inside with the Cult faction (assuming `/Factions/Cult/faction.yaml` exists).

Being associated with a specific Faction ensures that NPC appear in places thematically appropriate to them (e.g. wild monsters don't appear in the middle of towns - unless you want them to!)

|   Yaml File      |  Class           |   Notes                   |
|------------------|------------------|---------------------------|
| /Factions/X/faction.yaml | `Faction` | Contains description of the faction (name, role etc) |
| /Factions/X/forenames.txt | `NameFactory` | If present then unamed Npc generated in this faction have random selections from this list |
| /Factions/X/surnames.txt | `NameFactory` | As above but for surnames |
| /Factions/X/slots.yaml | `SlotCollection` | Overrides the default system wide item slots for the specific faction (used where the actor blueprint doesn't explicitly list it's own slots)|

## Subdirectories

Once a project gets too big it can help to use subdirectories.  To this end you can have:

```
./rooms.yaml
./Level1/MyCoolRoom.rooms.yaml
./Tutorial/MyOtherRoom.rooms.yaml
```

All that matters is the file extension.  The only exception to this rule are the fixed directory files e.g. `slots.yaml` (See table above)

## Proto Dialogue

Dialogues are tied together with Next / Identifier Guids.  But assembling this can be a pain.  For this reason the [DialogueCompiler] class exists.  To use it from the CLI create a text file with the dialogue you want.

The rules are super simple, type something the NPC says then indent your responses.  Repeat for whatever depth you want e.g.

```
The ageing barkeep rubs an oily cloth over a dirty cup and stares idly off into infinity. A death stick hangs from his lower lip occasionally spilling ash into whatever cup he is filling.  "Get you a drink cutter?" he mutters
  Ill have a mould ale
    Hrumph. Whatever, here’s your drink
  Make it moonshine, sunshine
    What’s that [sunshine]?
      Nevermind
      It’s bright and glows... dunno really
        Sounds like a stab light. But here you go, here’s your drink
  What’s a ‘cutter’?
    You are.
      Fair enough
  Heard any rumours lately?
    I heard a rumour there was an ugly cutter wandering around asking folks stupid questions
      Your going to regret saying that [Attack]
      Your a funny guy
      A simple no would have sufficed
  What’s there to do in a hole like this?
    Drinking, rutting and chukin dice. What’s your pleasure?
     Drinking
       What’ll it be? Moonshine or mould
     Rutting
       Better try your luck with one of the patrons then, you ain’t my type
     Dice
       Then go talk to Scrag over there she’s always up for a game
     Got a spare death stick?
       They will kill you sure as a shank but sure have one on the house.
```

Next run 'Game' with the `-c` option.  For example:
```
dotnet ./Game/bin/Debug/netcoreapp3.1/Game.dll -c /home/thomas/Documents/Barkeeper.txt
```

This will give us a file (Barkeeper.yaml) which contains the basic flow and can be copied to the Resources directory.  You will still need to add any looping and effects (e.g. spawning drinks/death sticks):

```
- Identifier: c9d833c6-c9d2-4821-a5e9-36e21867fc05
  Body:
  - Text: The ageing barkeep rubs an oily cloth over a dirty cup and stares idly off into infinity. A death stick hangs from his lower lip occasionally spilling ash into whatever cup he is filling.  "Get you a drink cutter?" he mutters
  Options:
  - Text: Ill have a mould ale
    Destination: 8482f515-8797-4d5f-a7d7-cea904a9ec40
  - Text: Make it moonshine, sunshine
    Destination: 3265fbfe-ec71-4036-b1da-64c539efe572
  - Text: What’s a ‘cutter’?
    Destination: 72c7a965-6b87-4043-9a87-2db3f65897bf
  - Text: Heard any rumours lately?
    Destination: b15c43e2-8b00-4c67-bbe8-b0f2dde27761
  - Text: What’s there to do in a hole like this?
    Destination: 97ddb94f-3968-451b-b597-121479dffc8a
- Identifier: 8482f515-8797-4d5f-a7d7-cea904a9ec40
  Body:
  - Text: Hrumph. Whatever, here’s your drink
- Identifier: 3265fbfe-ec71-4036-b1da-64c539efe572
  Body:
  - Text: What’s that [sunshine]?
  Options:
  - Text: Nevermind
  - Text: It’s bright and glows... dunno really
    Destination: 559ff191-45dd-4913-95a1-c5a1e0fd73f7
- Identifier: 559ff191-45dd-4913-95a1-c5a1e0fd73f7
  Body:
  - Text: Sounds like a stab light. But here you go, here’s your drink
- Identifier: 72c7a965-6b87-4043-9a87-2db3f65897bf
  Body:
  - Text: You are.
  Options:
  - Text: Fair enough
- Identifier: b15c43e2-8b00-4c67-bbe8-b0f2dde27761
  Body:
  - Text: I heard a rumour there was an ugly cutter wandering around asking folks stupid questions
  Options:
  - Text: Your going to regret saying that [Attack]
  - Text: Your a funny guy
  - Text: A simple no would have sufficed
- Identifier: 97ddb94f-3968-451b-b597-121479dffc8a
  Body:
  - Text: Drinking, rutting and chukin dice. What’s your pleasure?
  Options:
  - Text: Drinking
    Destination: 2ce9e104-97e8-451e-ab79-58f75932a892
  - Text: Rutting
    Destination: bf174bb7-ce3f-48bf-8b23-ea7f4d3c27ea
  - Text: Dice
    Destination: cf5a7f30-60cc-4ce9-a6e3-c349d54c396c
  - Text: Got a spare death stick?
    Destination: 55cba8c8-d7f0-41f8-846e-5227e87c86ef
- Identifier: 2ce9e104-97e8-451e-ab79-58f75932a892
  Body:
  - Text: What’ll it be? Moonshine or mould
- Identifier: bf174bb7-ce3f-48bf-8b23-ea7f4d3c27ea
  Body:
  - Text: Better try your luck with one of the patrons then, you ain’t my type
- Identifier: cf5a7f30-60cc-4ce9-a6e3-c349d54c396c
  Body:
  - Text: Then go talk to Scrag over there she’s always up for a game
- Identifier: 55cba8c8-d7f0-41f8-846e-5227e87c86ef
  Body:
  - Text: They will kill you sure as a shank but sure have one on the house.
```

You can validate that your Resources work by running the `-v` option

```
dotnet clean
dotnet build
dotnet ./Game/bin/Debug/netcoreapp3.1/Game.dll -v
```