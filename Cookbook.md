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
  - [Grenade](#grenade)
  - [Ammo](#ammo)
- [Dialogue Recipes](#dialogue-recipes)
  - [Remark about injury](#remark-about-injury)

## Room Recipes

### Starting room
<sup>[[View Test]](./Tests/Cookbook/StartingRoom.cs)</sup>

The [Player] always starts at 0,0,0.  The following recipy creates a unique starting room that will not spawn anywhere else:

```yaml
- Name: Somewhere Cool
  FixedLocation: 0,0,0
  Unique: true
```
<sup>./rooms.yaml</sup>


### Add same item to many rooms
<sup>[[View Test]](./Tests/Cookbook/SameItemToManyRooms.cs)</sup>

You can use the blueprint inheritence system to create multiple references to the same master blueprint:

```yaml
- Name: Armoury
  MandatoryItems:
   - Ref: e4ff5be4-233a-46b5-bb57-63831376b81d
   - Ref: e4ff5be4-233a-46b5-bb57-63831376b81d
   - Ref: e4ff5be4-233a-46b5-bb57-63831376b81d
```
<sup>./rooms.yaml</sup>

Then create the base item:

```yaml
- Name: Rose
  Identifier: e4ff5be4-233a-46b5-bb57-63831376b81d
```
<sup>./items.yaml</sup>


## Item Recipes

### Equippable Weapon
<sup>[[View Test]](./Tests/Cookbook/EquippableWeapon.cs)</sup>

Items are equippable only if they have defined 'slots'.  To do this we first have to declare some default slots.  Create a file `slots.yaml` in the root of your resources folder:

```
Wrist: 2
```
<sup>./slots.yaml</sup>

This defines that by default all actors have 2 wrists.  Next create the item:

```yaml
- Name: Wrist blade
  Stats:
    Fight: 10
  Slot:
   Name: Wrist
   NumberRequired: 1
```
<sup>./items.yaml</sup>


### Grenade
<sup>[[View Test]](./Tests/Cookbook/Grenade.cs)</sup>

Everyone loves grenades! To create one we will first need an injury system for the damage inflicted (or you can reuse an existing one e.g. tissue injuries)

```yaml
Identifier: 7ccafc68-d51f-4408-861c-f1d7e4e6351a
Name: Blast Damage
FatalThreshold: 100
FatalVerb: injuries

Injuries:
- Name: Wounded
  Severity: 10";
```
<sup>./blast.injury.yaml</sup>

Next we need to create the item.  We will make it SingleUse and give it a custom fight action that damages everyone in the room

```yaml
- Name: Grenade
  InjurySystem: 7ccafc68-d51f-4408-861c-f1d7e4e6351a
  Stack: 1
  MandatoryAdjectives:
   - SingleUse
  Actions:
    - Type: FightAction
      Stats: 
         Fight: 30
      Effect:
        #Injury everyone in the room
        - World:GetSystem('7ccafc68-d51f-4408-861c-f1d7e4e6351a'):ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,20,AggressorIfAny,null,Round))
```
<sup>./items.yaml</sup>

Finally getting hit by a stray grenade blast should probably make other NPCs angry.  Add another effect to the item:

```yaml
        #And make them all angry at you
        - World.Relationships:ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,-10,AggressorIfAny,null,Round))
```
<sup>./items.yaml</sup>

We can make these Effects reusable by moving them to [Main.lua]

```lua
function SplashDamage(injurySystem,amount,affectsRelationships)

	injurySystem:ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,20,AggressorIfAny,null,Round))

    if affectsRelationships then
		World.Relationships:ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,-10,AggressorIfAny,null,Round))
	end
end
```
<sup>./Main.lua</sup>

This lets you call it from any item with any amount of damage with a single Effect e.g.

```yaml
- Name: Grenade
  InjurySystem: 7ccafc68-d51f-4408-861c-f1d7e4e6351a
  Stack: 1
  MandatoryAdjectives:
   - SingleUse
  Actions:
    - Type: FightAction
      Stats: 
         Fight: 30
      Effect:
        - SplashDamage(Action.InjurySystem,20,true)
```
<sup>./items.yaml</sup>


### Ammo
<sup>[[View Test]](./Tests/Cookbook/Ammo.cs)</sup>

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
<sup>./InjurySystems/lasers.injury.yaml</sup>

- Give the player some hands!

```yaml
Hand: 2
```
<sup>./slots.yaml</sup>

- Now we need an adjective that ties the ammo clip to the weapon:

```yaml
- Name: LaserPowered
```
<sup>./adjectives.yaml</sup>

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
 <sup>./items.yaml</sup>

The item (Laser Clip) works by requiring the player to have an item with the "LaserPowered" adjective.  This will only happen when the Laser Pistol is equipped.  The SingleUse and Stack properties ensure that each time FightAction occurs
on the clip it runs down.

## Dialogue Recipes

### Remark about injury
<sup>[[View Test]](./Tests/Cookbook/RemarkAboutInjury.cs)</sup>

Each [DialogueNode] is made up of 1 or more blocks of text.  You can apply conditional operations to them.  For example if we want the Npc to remark on the players injured status we could write the following:

```
- Body: 
   - Text: Greetings berk
   - Text: that's a nasty looking cut you got there
     Condition: 
       - return AggressorIfAny:Has('Injured')
```
<sup>./dialogue.yaml</sup>

If you have multiple injury systems e.g. fire, cold, tissue damage etc and want to restrict your condition to only one of them then you can pass the injury system Guid instead e.g. return `AggressorIfAny:Has('7cc7a784-949b-4c26-9b99-c1ea7834619e')`


[DialogueNode]: ./src/Dialogues/DialogueNode.cs
[Player]: ./src/Actors/You.cs
[Main.lua]: ./src/Resources/Main.lua
