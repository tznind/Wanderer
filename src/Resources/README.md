# Resources

This file describes the relationship between classes and resource files

|   Yaml File      |  Class           |   Notes                   |
|------------------|------------------|---------------------------|
| /Dialogue/*.yaml | `DialogueNode[]` | Contains unique dialogue trees.  Each node consists of 1 main piece of text and 0 or more options which can lead to other nodes or end dialogue |
| /Factions/X/Faction.yaml | `Faction` | Contains description of the faction (name, role etc) |
| /Factions/X/Actors.yaml  | `ActorBlueprint` | Describes how to create `Actor` instances that fit thematically with the `Faction` whose folder they are in |
| /Factions/X/Forenames.txt | `NameFactory` | If present then unamed Npc generated in this faction have random selections from this list |
| /Factions/X/Surnames.txt | `NameFactory` | As above but for surnames |
