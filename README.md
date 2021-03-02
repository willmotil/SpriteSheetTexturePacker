# SpriteSheetTexturePacker

This vs 2019 windows solution designed for monogame. 
Includes a simple project to allow the user to generate a spritesheet with animation set information.
You can then take that generated spritesheet and accompanying .spr description file place it into the content directory.
The included pipeline will generate a xnb and the associated reader can read the xnb in when the application runs.

In a new game1 project one can then load it using the familiar. 
var spritesheet = Content.Load<SpriteSheet>("MySpriteSheet");
In the .mgcb file you must include a reference to the pipeline reader.dll to read in the spritesheet.
  
This project also used the monogame frame work and content nuget packages.

