# Cookbook

This page contains simple recipes for common level building tasks.

- [Starting Room](#starting-room)

## Starting Room
The [Player] always starts at 0,0,0.  The following recipy creates a unique starting room that will not spawn anywhere else:

```yaml
- Name: Somewhere Cool
  FixedLocation: 0,0,0
  Unique: true
```

[Test](./Tests/Cookbook/StartingRoom.cs)

[Player]: ./src/Actors/You.cs
 
