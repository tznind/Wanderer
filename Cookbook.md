# Cookbook

This page contains simple recipes for common level building tasks.

## Contents

- [Room Recipes](#room-recipes)
  - [Starting room](#starting-room)
- [Item Recipes](#item-recipes)
  - [Equippable weapon](#equippable-weapon)
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

## Item Recipes

### Equippable Weapon

Items are equippable only if they have defined 'slots'.  To do this we first have to declare some default slots.  Create a file `slots.yaml` in the root of your resources folder:

```
Wrist: 2
```

This defines that by default all actors have 2 wrists.  Next create `Items.yaml` with the following:

```yaml
- Name: Wrist blade
  Stats:
    Fight: 10
  Slot:
   Name: Wrist
   NumberRequired: 1
```
_[[View Test]](./Tests/Cookbook/EquippableWeapon.cs)_


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

If you have multiple injury systems e.g. fire, cold, tissue damage etc and want to restrict your condition to only one of them then you can pass the injury system Guid instead e.g. return `AggressorIfAny:Has('7cc7a784-949b-4c26-9b99-c1ea7834619e')`



[DialogueNode]: ./src/Dialogues/DialogueNode.cs
[Player]: ./src/Actors/You.cs
 
