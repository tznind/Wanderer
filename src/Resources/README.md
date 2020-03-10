# Resources

This file describes the relationship between classes and resource files

|   Yaml File      |  Class           |   Notes                   |
|------------------|------------------|---------------------------|
| /Dialogue/*.yaml | `DialogueNode[]` | Contains unique dialogue trees.  Each node consists of 1 main piece of text and 0 or more options which can lead to other nodes or end dialogue |
| /InjurySystems/*.yaml | `InjurySystem` | Describes a method of inflicting damage upon Rooms and Actors (e.g. fire, plague, tissue damage etc)|
| /[Main.lua](./Main.lua) | N\A | Defines custom global methods and helper functions for use in scripting blocks |
| /[Plans.yaml](./Plans.yaml) | `Plan` | Contains AI Plans for NPC Actors.  These can be influenced by Leadership Actions of others|
| /Rooms.yaml | `RoomBlueprint[]` | Contains descriptions of rooms that can be generated |
| /Items.yaml | `ItemBlueprint[]` | Contains generic items that would fit in any room generated (regardless of `Faction` |
| /Slots.yaml | `SlotCollection` | Contains default item slots (e.g. 1 Head, 2 Hand etc) that all `Actor` start with (unless the blueprint lists explicit slots).  Note also that this can be overridden with a faction Slots.yaml|
| /Factions/X/Faction.yaml | `Faction` | Contains description of the faction (name, role etc) |
| /Factions/X/Rooms.yaml  | `RoomBlueprint[]` | Describes how to create `Room` instances that fit thematically with the `Faction` whose folder they are in |
| /Factions/X/Actors.yaml  | `ActorBlueprint[]` | Describes how to create `Actor` instances that fit thematically with the `Faction` whose folder they are in |
| /Factions/X/Items.yaml  | `ItemBlueprint[]` | Describes how to create `Item` instances that fit thematically with the `Faction` whose folder they are in |
| /Factions/X/Slots.yaml | `SlotCollection` | Overrides the default system wide item slots for the specific faction (used where the actor blueprint doesn't explicitly list it's own slots)|
| /Factions/X/Forenames.txt | `NameFactory` | If present then unamed Npc generated in this faction have random selections from this list |
| /Factions/X/Surnames.txt | `NameFactory` | As above but for surnames |


## Subdirectories

Once a project gets too big it can help to use subdirectories.  To this end you can have:

```
./Rooms.yaml
./Rooms/Level1/MyCoolRoom.yaml
./Rooms/Tutorial/MyOtherRoom.yaml
```
_Anywhere you could have Rooms.yaml you can also have a Rooms directory.  Files under this directory are loaded as rooms_

You can also create a folder `Dialogue` under any `Rooms` directory:

```
./Rooms.yaml
./Rooms/Level1/MyCoolRoom.yaml
./Rooms/Level1/Dialogue/SomeoneCool.yaml
./Rooms/Level1/Dialogue/DescriptionOfMyCoolRoom.yaml
./Rooms/Tutorial/MyOtherRoom.yaml
./Rooms/Tutorial/Dialogue/Descripions/RoomDescriptionDialogue.yaml
./Rooms/Tutorial/Dialogue/Actors/Dude1.yaml
./Rooms/Tutorial/Dialogue/Actors/Dude2.yaml
```
_Any file under a folder called 'Dialogue' will be loaded as Dialogue_
