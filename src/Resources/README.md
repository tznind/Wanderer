# Resources

This file describes the relationship between classes and resource files

|   Yaml File      |  Class           |   Notes                   |
|------------------|------------------|---------------------------|
| /Dialogue/*.yaml | `DialogueNode[]` | Contains unique dialogue trees.  Each node consists of 1 main piece of text and 0 or more options which can lead to other nodes or end dialogue |
| /InjurySystems/*.yaml | `InjurySystem` | Describes a method of inflicting damage upon Rooms and Actors (e.g. fire, plague, tissue damage etc)|
| /[Main.lua](./Main.lua) | N\A | Defines custom global methods and helper functions for use in scripting blocks |
| /[Plans.yaml](./Plans.yaml) | `Plan` | Contains AI Plans for NPC Actors.  These can be influenced by Leadership Actions of others|
| /Actors.yaml  | `ActorBlueprint[]` | Describes how to create `Actor` instances that fit thematically with any `Faction` / `Room` |
| /Rooms.yaml | `RoomBlueprint[]` | Contains descriptions of rooms that can be generated |
| /Items.yaml | `ItemBlueprint[]` | Contains generic items that would fit in any room generated (regardless of `Faction` |
| /Slots.yaml | `SlotCollection` | Contains default item slots (e.g. 1 Head, 2 Hand etc) that all `Actor` start with (unless the blueprint lists explicit slots).  Note also that this can be overridden with a faction Slots.yaml|
| /Factions/X/Faction.yaml | `Faction` | Contains description of the faction (name, role etc) |
| /Factions/X/Forenames.txt | `NameFactory` | If present then unamed Npc generated in this faction have random selections from this list |
| /Factions/X/Surnames.txt | `NameFactory` | As above but for surnames |
| /Factions/X/Slots.yaml | `SlotCollection` | Overrides the default system wide item slots for the specific faction (used where the actor blueprint doesn't explicitly list it's own slots)|

## Subdirectories

Once a project gets too big it can help to use subdirectories.  To this end you can have:

```
./Rooms.yaml
./Rooms/Level1/MyCoolRoom.yaml
./Rooms/Tutorial/MyOtherRoom.yaml
```

Anywhere you could have a given yaml file (e.g. `Dialogue.yaml`, `Actors.yaml`, `Rooms.yaml` or `Items.yaml`) you can instead/aswell have a directory.

All directories are evaluated and the Type of blueprint is infered from the last named folder in the hierarchy e.g.

```
#Would be interpreted as Items
./Level1/Rooms/MyCoolRoom/Items/Torch.yaml

#Would be interpreted as Room(s)
./Level1/Rooms/MyCoolRoom/CoolRoom.yaml
```

This lets you group your objects together however you wish e.g. keep room specific Dialogue in the same area as the Room definition.


## Faction Specific Rooms/Items/Actors

If yaml file or directory (e.g. `Rooms.yaml`) exists under a faction e.g. (`./Factions/Lowlifes/Rooms/Bar.yaml`) then the room will be associated with the `Faction` (in this case `LowLifes`).  This feature requires a `Faction.yaml` to exist (e.g. `./Factions/Lowlifes/Faction.yaml`).

Being associated with a specific Faction ensures that NPC appear in places thematically appropriate to them (e.g. wild monsters don't appear in the middle of towns - unless you want them to!)


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