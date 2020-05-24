# Cookbook

This page contains simple recipes for common level building tasks.  To test a recipe for yourself you should create file(s) in your Resources directory (using the names provided under the code blocks).

## Contents

- [Custom Stats](#custom-stats)
- [Room Recipes](#room-recipes)
  - [Starting room](#starting-room)
  - [Add same item to many rooms](#add-same-item-to-many-rooms)
  - [Random room items](#random-room-items)
  - [RoomHas](#roomhas)
- [Item Recipes](#item-recipes)
  - [Equippable weapon](#equippable-weapon)
  - [Grenade](#grenade)
  - [Ammo](#ammo)
- [Dialogue Recipes](#dialogue-recipes)
  - [OnEnter room dialogue](#onenter-room-dialogue)
  - [Remark about injury](#remark-about-injury)
  - [Give Item](#give-item)
- [Script Blocks](#script-blocks)


## Custom Stats

<sup>[[View Test]](./Tests/Cookbook/CustomStats.cs)</sup>

You can define new stats by declaring a stats.yaml file:

```yaml
- Sanity
- Seduction
```
<sup>./stats.yaml</sup>

Stats can immediately be used e.g. on rooms, items etc:

```
- Name: Relaxing Bar
  Stats:
    Seduction: 10
  MandatoryItems:
    - Name: Helmet of Madness
      Require:
        - Stat: Seduction <= 0
      Stats:
        Sanity: -500
```
<sup>./rooms.yaml</sup>

## Room Recipes

### Starting room
<sup>[[View Test]](./Tests/Cookbook/StartingRoom.cs)</sup>

The [Player] always starts at 0,0,0.  The following recipy creates a unique starting room that will not spawn anywhere else:

```yaml
- Name: Somewhere Cool
  FixedLocation: 0,0,0
  Unique: true
  Identifier: b1aa5ce4-213a-46b5-aa57-63831376b81d
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

### Random Room Items
<sup>[[View Test]](./Tests/Cookbook/RandomRoomItems.cs)</sup>

Assuming an empty Resources directory, the first room you create will contain no items:

```yaml
- Name: Chamber of Horrors
```
<sup>./rooms.yaml</sup>

Creating an items.yaml file will result in a random number of items spawning (including the possibility for duplicates)

```yaml
- Name: Rose
- Name: Egg
```
<sup>./items.yaml</sup>

You can control the maximum / minimum number of these items per room with the following settings:


```yaml
- Name: Chamber of Horrors
  OptionalItemsMin: 4
  OptionalItemsMax: 10
```
<sup>./rooms.yaml</sup>

You can create a room with no random items by setting the max to 0:

```yaml
- Name: Chamber of Horrors
  OptionalItemsMax: 0
```
<sup>./rooms.yaml</sup>

### RoomHas
<sup>[[View Test]](./Tests/Cookbook/RoomHas.cs)</sup>

If you want to check for something in the [Room] an [Actor] is in rather than the [Actor] themselves you can use the `Check: Room` condition.  For example we could create a 'Bionic Arm' that requires a medical bay to install.

**Note: RoomHas also includes anything any Actor in the room Has (or items they are carrying)**

Create the adjective Medical:

```
- Name: Medical
```
<sup>./adjectives.yaml</sup>

Define that all Actors have by default 2 arms (including the Player):

```
Arm: 2
```
<sup>./slots.yaml</sup>

Create a couple of rooms, one with the arm and one that is a medical bay:

```
- Name: Warehouse
  FixedLocation: 0,0,0
  MandatoryItems:
    - Name: Bionic Arm
      Stats:
        Fight: 20
      Slot:
        Name: Arm
        NumberRequired: 1
      EquipRequire:
         - Check: Room
           Has: Medical

- Name: Med Bay
  FixedLocation: -1,0,0
  MandatoryAdjectives:
    - Medical
```
<sup>./rooms.yaml</sup>

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
  Severity: 10
```
<sup>./blast.injury.yaml</sup>

Next we need to create the item.  We will make it SingleUse and give it a custom fight action that damages everyone in the room.  For the damage Effect we will get the [InjurySystem] and apply it to everyone in the room.  Note that when calling `ApplyToAll` the Recipient can be `null` because it will be assigned for each of the passed actors.

Getting hit by a stray grenade blast should probably make other NPCs angry.  Add another effect to the item.  This time we use `ApplyAll` on the [RelationshipSystem]

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
        - Lua: World:GetSystem('7ccafc68-d51f-4408-861c-f1d7e4e6351a'):ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,20,AggressorIfAny,null,Round))
        #And make them all angry at you
        - Lua: World.Relationships:ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,-10,AggressorIfAny,null,Round))
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
        - Lua: SplashDamage(Action.InjurySystem,20,true)
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
    - Has: LaserPowered

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
       - Has: Injured
```
<sup>./dialogue.yaml</sup>

If you have multiple injury systems e.g. fire, cold, tissue damage etc and want to restrict your condition to only one of them then you can pass the injury system Guid instead e.g. return `AggressorIfAny:Has('7cc7a784-949b-4c26-9b99-c1ea7834619e')`

### OnEnter room dialogue
<sup>[[View Test]](./Tests/Cookbook/OnEnterRoomDialogue.cs)</sup>

Say we want to run the room dialogue as soon as the player enters the room.  Heres how we can do that.  Create a new behaviour:

```yaml
- Name: DialogueOnEnter
  Identifier: 5ae55edf-36d0-4878-bbfd-dbbb23d42b88
  OnEnter: 
   Condition: 
     - Lua: AggressorIfAny == World.Player
     - Lua: Room == Behaviour.Owner
     - Lua: Room.Dialogue.Next ~= nil
   Effect: 
     - Lua: World.Dialogue:Apply(SystemArgs(World,UserInterface,0,AggressorIfAny,Room,Round))
     - Lua: Room.Dialogue.Next = null
```
<sup>./behaviours.yaml</sup>

The effect applies only when the Player is the one doing the entering (so Npc wandering into the Room won't trigger it).  We also check that the Room being entered is the behaviour owner and that they have Dialogue to run.

To use the new behaviour on a Room we just have to reference it (and set up suitable dialogue)

```yaml
- Name: Dank Cellar
  Behaviours:
    - Ref: 5ae55edf-36d0-4878-bbfd-dbbb23d42b88
  Dialogue:
    Next: 6da41741-dada-4a52-85d5-a019cd9d38f7
```
<sup>./rooms.yaml</sup>

`-Ref:` allows us to reference the behaviour from as many rooms as we want.

```yaml
- Identifier: 6da41741-dada-4a52-85d5-a019cd9d38f7
  Body: 
   - Text: Goblins fill the room from floor to ceiling
```
<sup>./dialogue.yaml</sup>

If we want to only apply the behaviour the first time the Player enters the room we can add a second Effect to the behaviour that clears the room dialogue.


```yaml
   [...]
   Effect: 
     - World.Dialogue:Apply(SystemArgs(World,UserInterface,0,AggressorIfAny,Room,Round))
     - Room.Dialogue.Next = null
```
<sup>./behaviours.yaml</sup>

### Give Item
<sup>[[View Test]](./Tests/Cookbook/SpawnItem.cs)</sup>

We can give or sell an item to the player with the `Spawn:` effect.  First lets create a goblin shopkeeper:

```yaml
- Name: Shop
  MandatoryActors:
    - Name: Goblin Shopkeeper
      # Don't let her wander off
      SkipDefaultActions: true
      Dialogue: 
        Verb: Shopping
        Next: 66e99df7-efd9-46cc-97a1-9fed851e0d8f
      MandatoryItems:
         - Name: Shiny Pebble
```
<sup>./rooms.yaml</sup>

```yaml
- Identifier: 66e99df7-efd9-46cc-97a1-9fed851e0d8f
  Body:
    - Text: Here burk, want to buy this painted rock?
  Options:
     - Text: Yes please, heres 20 gold!
       Condition: 
          - Variable: Gold >= 20
       Effect:
          - Spawn: Shiny Pebble
          - Set: Gold -= 20
     - Text: Lend us some gold will you?
       SingleUse: true
       Effect:
          - Set: Gold += 20
     - Text: No thanks... my days of chasing shine are long behind me
```
<sup>./dialogue.yaml</sup>

## Script Blocks

Most [Conditions] and [Effects] can be expressed using yaml alone e.g.

```yaml
Require:
 - Stat: Fight > 20
   Has: Sword
```
However for some advanced use cases you might need to write a [Lua] script instead:

```yaml
Require:
 - Lua: AggressorIfAny.BaseStats[Fight] > 20 && AggressorIfAny:Has('Sword')
```

When using Lua for a `Condition`, the script expression must evaluate to true or false.

In scripts the following global variables are available:

| Variable        | Description |
| ------------- |-------------|
| [this]      |  Input parameter of Type [SystemArgs] for the condition / effect.  E.g. for dialogue this describes who is talking to who|
| [World]      |  root variable for the game world |
| AggressorIfAny ([Actor]) |  The player or Npc that is triggering the action/event.  This can be null for actions/events that are not instigated by an [Actor]|
| Recipient | [Actor], [Room], [Item] etc which is the target of the action/event (e.g. for Dialogue this would be the person being talked too)|
| [Room] | Where the action/event is tacking place (Can be null for some events e.g. RoundEnding) |
| [UserInterface] | root variable for the graphical user interface (required argument for many methods) |
| Round | Unique identifier for the current round (required argument for many methods) |
| [Action] | Only aplies to action related events e.g. OnPush, references the action being performed |
| [Behaviour] | Only applies to behaviour events (OnPush, OnRoundEnding etc).  References the behaviour object (which will be attached to a specific Owner) |

[InjurySystem]: ./src/Systems/InjurySystem.cs
[RelationshipSystem]: ./src/Systems/RelationshipSystem.cs
[DialogueNode]: ./src/Dialogues/DialogueNode.cs
[Player]: ./src/Actors/You.cs
[Main.lua]: ./src/Resources/Main.lua
[World]: ./src/IWorld.cs
[Actor]: ./src/Actors/IActor.cs
[Room]: ./src/Rooms/IRoom.cs
[Item]: ./src/Items/IItem.cs
[UserInterface]: ./src/IUserInterface.cs
[Action]: ./src/Actions/IAction.cs
[Behaviour]: ./src/Behaviours/IBehaviour.cs
[Lua]: https://www.lua.org/about.html
[yaml]: https://yaml.org/
[Stat]: ./src/Stats/Stat.cs
[StatsCollection]: ./src/Stats/StatsCollection.cs
[Conditions]: ./src/Compilation/ICondition.cs
[Effects]: ./src/Compilation/IEffect.cs
[SystemArgs]: ./src/Systems/SystemArgs.cs
