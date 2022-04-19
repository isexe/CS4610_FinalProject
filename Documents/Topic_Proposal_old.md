# <center>Topic Proposal:  3D World Game <br>"name" <br><br>Isaac Sexe, <br>Zach Summers, <br><br>4/19/2022 <br>--- <br> CMP_SC 4610 <br> --- <br>Professor Ye Duan

## Overview

The project we choose to do was a 3D World Game called "name".  This game will be a rogue-like that has the player going deeper into the dungeon until defeated.  We will procedurally generate a dungeon for the player to traverse.  This dungeon will include enemies for the player to defeat, loot to collect, and a boss that must be located and defeated.  After defeating the boss, the player will have the option to proceed to the next floor.  These are the bare minimum features intended for the game and may be expanded upon in the final version of the project.

<br>

## Methods

The dungeon will be procedurally generated using an algorithm covered by vazgriz on his [website][1].  Their algorithm itself was based on a different algorithm featured in the game TinyKeep, which is explained by A. Andonaac's in his [blog][2].  The algorithm functions by creating different aspects of the dungeon and storing its relative information in data structures for later use.  The first data structure is a 2D grid of the types of rooms and the other data structure that stores information about individual rooms. This algorithm features four main processes: generating the rooms, locating the hallways, generating the hallways.

### Generating Rooms

This algorithm generates rooms by selecting a random position and size with parameters given by the user.  It will then create a room object in the scene and a room object in the script.  The positions and size of this room are converted into (x,y) coordinates and changed to rooms in the grid.

### Locating Hallways

The algorithm our's is based on uses something called Delaunay Triangulation to form a graph with the room centers as vertices.  Currently, we haven't decided what the exact method we will use for this step, but if we do decided to change it the end result will still be a graph.  A minimul spanning tree is then calculated from this graph to form short logical paths from one vertex to another.  A few of the removed edges are added back to the graph to create more loops in the graph to make the game fell less linear and like there are more options.

### Generating Hallways

Now that the hallways have been found a pathfinding algorithm is used to 

<br>

## References

1. [https://vazgriz.com/119/procedurally-generated-dungeons/][1]
2. [gamedeveloper.com/programming/procedural-dungeon-generation-alogrithm][2]

<References>

[1]: https://vazgriz.com/119/procedurally-generated-dungeons/
[2]: gamedeveloper.com/programming/procedural-dungeon-generation-alogrithm
