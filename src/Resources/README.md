# Resources

This file describes the relationship between classes and resource files

|   Yaml File      |  Class           |   Notes                   |
|------------------|------------------|---------------------------|
| /Dialogue/*.yaml | `DialogueNode[]` | Contains unique dialogue trees.  Each node consists of 1 main piece of text and 0 or more options which can lead to other nodes or end dialogue |
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
