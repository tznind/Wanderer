# Cookbook

This page contains simple recipes for common level building tasks.

## Contents

- [Room Recipes](#room-recipes)
  - [Starting Room](#starting-room)
- [Dialogue Recipes](#dialogue-recipes)
  - [Remark about injury](#remark-about-injury)
## Room Recipes

### Starting room
The [Player] always starts at 0,0,0.  The following recipy creates a unique starting room that will not spawn anywhere else:

```yaml
- Name: Somewhere Cool
  FixedLocation: 0,0,0
  Unique: true
```
_[[View Test]](./Tests/Cookbook/StartingRoom.cs)_

## Dialogue Recipes

### Remark about injury

Each [DialogueNode] is made up of 1 or more blocks of text.  You can apply conditional operations to them.  For example if we want the Npc to remark on the players injured status we could write the following:

```
- Body: 
   - Text: Greetings berk
   - Text: that's a nasty looking cut you got there
     Condition: 
       - return AggressorIfAny:Has('Injured')
```
_[[View Test]](./Tests/Cookbook/RemarkAboutInjury.cs)_


[DialogueNode]: ./src/Dialogues/DialogueNode.cs
[Player]: ./src/Actors/You.cs
 
