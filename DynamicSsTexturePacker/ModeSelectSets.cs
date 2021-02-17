using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace DynamicSsTexturePacker
{
    public class ModeSelectSets
    {

        string command = "none";
        string command2 = "none";
        int commandIndex = 0;


        public void Update(GameTime gameTime)
        {

            if (command == "CreateSheet")
            {
                Globals.CreateAndSave(false);
                command = "none";
            }

            if (command == "OpenSavePath")
            {
                Globals.OpenDirectory(Globals.savePath);
                command = "none";
            }

            if (command == "NameFile")
            {

            }

        }

        string newSaveName = Globals.saveFileName;
        public void TakeText(Object sender, TextInputEventArgs e)
        {
            if (command == "NameFile")
            {
                newSaveName += e.Character;

                if (Keys.Enter.IsKeyDown())
                {
                    Globals.saveFileName = newSaveName;
                    command = "none";
                }
                if (Keys.Back.IsKeyDown())
                {
                    newSaveName = "";
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            Rectangle r = new Rectangle();

            Globals.spriteBatch.Begin();

            // Draw the individual textures.
            for (int i = 0; i < Globals.textures.Count; i++)
            {
                Globals.spriteBatch.Draw(Globals.textures[i], new Rectangle(10 + (i * 200), 100, 200, 200), Color.White);
                Globals.spriteBatch.DrawString(Globals.font, Globals.textures[i].Name, new Vector2(10 + (i * 200), (i * 10) + 100), Color.White);
            }
            // Draw the sheet if possible.
            if (Globals.myGeneratedSpriteSheetInstance != null)
                DrawSheetAndShowLabels();



            Globals.spriteBatch.DrawString(Globals.font, "Set groups of images to be animation sets.", new Vector2(10, 0), Color.White);

            // draw the create sheet button.
            r = new Rectangle(new Point(10, 20), new Point(100, Globals.font.LineSpacing));
            DrawCheckClickSetCommand(r, "CreateSheet", "CreateSheet", Color.White, Color.Blue);

            // open the current save path.
            r = new Rectangle(new Point(110, 20), new Point(100, Globals.font.LineSpacing));
            DrawCheckClickSetCommand(r, "OpenSavePath", "OpenSavePath", Color.White, Color.Blue);

            // name file.
            r = new Rectangle(new Point(10, Globals.font.LineSpacing + 20), new Point(100, Globals.font.LineSpacing));
            DrawCheckClickSetCommand(r, newSaveName, "NameFile", Color.White, Color.Blue);


            Globals.spriteBatch.End();
        }

        public void DrawCheckClickSetCommand(Rectangle r, string label ,string commandName, Color textCol, Color outlineColor)
        {
            Globals.spriteBatch.DrawRectangleOutline(r, 1, outlineColor);
            Globals.spriteBatch.DrawString(Globals.font, label, r.Location.ToVector2(), textCol);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                command = commandName;
        }

        public void DrawSheetAndShowLabels()
        {
            // Draw the resulting spritesheet.
            var offset = new Vector2(50, 450);
            Globals.spriteBatch.Draw(Globals.myGeneratedSpriteSheetInstance.textureSheet, offset, Color.White);

            // Draw the names of the sprites in the sheet at their locations allow color change over sprites.
            for (int i = 0; i < Globals.myGeneratedSpriteSheetInstance.sprites.Count; i++)
            {
                var spriteName = Globals.myGeneratedSpriteSheetInstance.sprites[i].nameOfSprite;
                var nameoffset = Globals.myGeneratedSpriteSheetInstance.sprites[i].sourceRectangle;
                nameoffset.Location = nameoffset.Location + offset.ToPoint();
                var color = Color.White;
                if (nameoffset.Contains(MouseHelper.Pos))
                    color = Color.Red;
                Globals.spriteBatch.DrawString(Globals.font, spriteName, nameoffset.Center.ToVector2(), color);
            }
        }

    }
}
