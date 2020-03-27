# Cookbook

This page contains simple recipes for common level building tasks.

> TODO:

> AggressorIfAny should probably be Initiator

> Move every code block to use SystemArgs if possible? (for consistency of script blocks)

## Contents

- [Room Recipes](#room-recipes)
  - [Starting room](#starting-room)
  - [Add same item to many rooms](#add-same-item-to-many-rooms)
- [Item Recipes](#item-recipes)
  - [Equippable weapon](#equippable-weapon)
  - [Ammo](#ammo)
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
_./Rooms.yaml_

_[[View Test]](./Tests/Cookbook/StartingRoom.cs)_

### Add same item to many rooms

You can use the blueprint inheritence system to create multiple references to the same master blueprint:

```yaml
- Name: Armoury
  MandatoryItems:
   - Ref: e4ff5be4-233a-46b5-bb57-63831376b81d
   - Ref: e4ff5be4-233a-46b5-bb57-63831376b81d
   - Ref: e4ff5be4-233a-46b5-bb57-63831376b81d
```
_./Rooms.yaml_

Then create the base item:

```yaml
- Name: Rose
  Identifier: e4ff5be4-233a-46b5-bb57-63831376b81d
```
_./Items.yaml_

_[[View Test]](./Tests/Cookbook/SameItemToManyRooms.cs)_

## Item Recipes

### Equippable Weapon

Items are equippable only if they have defined 'slots'.  To do this we first have to declare some default slots.  Create a file `slots.yaml` in the root of your resources folder:

```
Wrist: 2
```
_./Slots.yaml_

This defines that by default all actors have 2 wrists.  Next create the item:

```yaml
- Name: Wrist blade
  Stats:
    Fight: 10
  Slot:
   Name: Wrist
   NumberRequired: 1
```
_./Items.yaml_

_[[View Test]](./Tests/Cookbook/EquippableWeapon.cs)_


## Ammo

This will cover creating a laser pistol.  For this recipe we are going to need entries in 4 files.

- An injury system (for laser burns):

```yaml
Identifier: 3bfc44ce-28ba-4fa8-951a-f97ec6dddf0f
Name: Laser Damage
IsDefault: true
FatalThreshold: 100
FatalVerb: injuries

Injuries:
- Name: Laser Burn
  Severity: 10
```
_./InjurySystems/Lasers.yaml_

- Give the player some hands!

```yaml
Hand: 2
```
_Slots.yaml_


- Now we need an adjective that ties the ammo clip to the weapon.  Create `./Adjectives.yaml`

```yaml
- Name: LaserPowered
```

- Finally we can create our items

```yaml
- Name: Laser Clip
  Stack: 2
  MandatoryAdjectives:
    - SingleUse
  Actions: 
    - Type: FightAction
      InjurySystem: 3bfc44ce-28ba-4fa8-951a-f97ec6dddf0f
      Stats: 
         Fight: 20
  Require:
    - return this:Has('LaserPowered')

- Name: Laser Pistol
  Slot:
    Name: Hand
    NumberRequired: 1
  MandatoryAdjectives:
    - LaserPowered
```
 _./Items.yaml_

The item (Laser Clip) works by requiring the player to have an item with the "LaserPowered" adjective.  This will only happen when the Laser Pistol is equipped.  The SingleUse and Stack properties ensure that each time FightAction occurs
on the clip it runs down.

_[[View Test]](./Tests/Cookbook/Ammo.cs)_

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
_./Dialogue.yaml_

_[[View Test]](./Tests/Cookbook/RemarkAboutInjury.cs)_

If you have multiple injury systems e.g. fire, cold, tissue damage etc and want to restrict your condition to only one of them then you can pass the injury system Guid instead e.g. return `AggressorIfAny:Has('7cc7a784-949b-4c26-9b99-c1ea7834619e')`


[DialogueNode]: ./src/Dialogues/DialogueNode.cs
[Player]: ./src/Actors/You.cs
 
