# Using Resource Files

Wanderer is almost completely driven by config files.  This page covers how to write/replace these files.  The format for the files is yaml which is a 'human readable' format.  If you haven't used yaml before it is a good idea to read a [tutorial on the format](https://github.com/Animosity/CraftIRC/wiki/Complete-idiot's-introduction-to-yaml) (whitespace / line alignment is very important)

## No Resource Files

Create a new folder `myworld` (anywhere) and start Game with the `-r` option e.g.

```bash
mkdir /home/thomas/myworld
Game/bin/Debug/netcoreapp3.1/Game -r /home/thomas/myworld
```

Start a new game.  You should be in a room called `Empty Room`

## Creating the first room

Create a file `Rooms.yaml` in the root of `myworld` with the following text (_make sure to include the starting hyphen!_):

```yaml
- Name: Magical Princess Ballroom
```

Start the game again and this time you should be in a room called `Magical Princess Ballroom`.  Now that we know that works we should give it an Identifier.  All root objects support the Identifier property and it lets you refer to them in scripts, link them together etc.  Identifiers are 'Guids' so search the internet for Guid Generator and create a bunch.


```yaml
- Name: Magical Princess Ballroom
  Identifier: 9159abe1-ef26-459d-af36-c56d0ffc6d46
```

You can validate your resources with the --validate (or -v) switch e.g.:

```bash
Game/bin/Debug/netcoreapp3.1/Game -r /home/thomas/myworld --validate
```

If you get an error like below then make sure that there are 2 spaces before the `I` in `Identifier` (so that it is vertically aligned with the `N` of `Name`).

```
Error Creating World
(Line: 2, Col: 1, Idx: 34) - (Line: 2, Col: 1, Idx: 34): While parsing a block collection, did not find expected '-' indicator.
```
_Example yaml error message from failing world validation_

Next lets create an occupant for the room.  Update `Rooms.yaml`


```yaml
- Name: Magical Princess Ballroom
  Identifier: 9159abe1-ef26-459d-af36-c56d0ffc6d46
  MandatoryActors:
    - Name: Fae Prince
```

Now when you run the game you should see the `Fae Prince`.  Lets give him something to say.  To do this add the `Dialogue` element:


```yaml
- Name: Magical Princess Ballroom
  Identifier: 9159abe1-ef26-459d-af36-c56d0ffc6d46
  MandatoryActors:
    - Name: Fae Prince
      Dialogue:
        Next: a218081b-6d32-4101-8f2f-a0621fec50be
```

Now when we validate the world we should get an error:

```
Could not find Dialogue 'a218081b-6d32-4101-8f2f-a0621fec50be'
```

Create a new folder `Dialogue` with a file in it called `SomeTalking.yaml`

```bash
 mkdir /home/thomas/myworld/Dialogue
 nano /home/thomas/myworld/Dialogue/SomeTalking.yaml
```

In `SomeTalking.yaml` add the following:

```yaml
- Identifier: a218081b-6d32-4101-8f2f-a0621fec50be
  Body: 
    - Text: My princess, you are looking radiant tonight!
```

Running the game should now let you talk to the Prince.  Lets add some options.

```yaml
- Identifier: a218081b-6d32-4101-8f2f-a0621fec50be
  Body:
    - Text: My princess, you are looking radiant tonight!
  Options:
    - Text: You look equally radiant your... Grace?
    - Text: Alas that my radiance illuminates your wretched ugliness
```

Once you have tested these options we can give them some weight in the world.  Lets add an effect.

```yaml
- Identifier: a218081b-6d32-4101-8f2f-a0621fec50be
  Body:
    - Text: My princess, you are looking radiant tonight!
  Options:
    - Text: You look equally radiant your... Grace?
    - Text: Alas that my radiance illuminates your wretched ugliness
      Effect:
       - |
           tired = Tired(Recipient)
           tired.Name = 'Crippling Insecurity'
           Recipient.Adjectives:Add(tired)
```
_Adding an effect to dialogue.  Make sure to align the hyphens under the above lines correctly_

Now talk to the Fae Prince and be mean then inspect him.  He should have -10 Fight and suffer from "Crippling Insecurity".

Effects (and Conditions) are coded in Lua.  This uses [NLua scripting](https://github.com/NLua/NLua) library.  This lets you do pretty much anything you want.  In this case we create a new `IAdjective` by constructing the `Tired` class and rebranding it by setting it's `Name` property.

We can link dialogue together by adding a `Destination`:


```yaml
- Identifier: a218081b-6d32-4101-8f2f-a0621fec50be
  Body:
    - Text: My princess, you are looking radiant tonight!
  Options:
    - Text: You look equally radiant your... Grace?
      Destination: 0acf5870-93a1-441a-bfa7-3bd3f182dcdc
    - Text: Alas that my radiance illuminates your wretched ugliness
      Effect:
       - |
           tired = Tired(Recipient)
           tired.Name = 'Crippling Insecurity'
           Recipient.Adjectives:Add(tired)
      Destination: bc9492f6-5205-44da-bad7-17097801b9e9

- Identifier: 0acf5870-93a1-441a-bfa7-3bd3f182dcdc
  Body:
    - Text: Yes that is indeed the proper title, quite the musical choice they are playing today isn't it.  Quite Baroque
- Identifier: bc9492f6-5205-44da-bad7-17097801b9e9
  Body:
    - Text: I... well... <The prince turns away to hide his tears>
```
