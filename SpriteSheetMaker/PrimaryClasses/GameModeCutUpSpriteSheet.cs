using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteSheetPipelineReader;

namespace SpriteSheetCreator
{
    public class GameModeCutUpSpriteSheet
    {
        public List<Rectangle> sourceRectangles = new List<Rectangle>();
        public int currentIndex = -1;
        public static string selectedImageFile = "";
        public static string visualSelectedImageFile = "";
        public static Texture2D textureToCutUp;
        public static float animationTime = 0f;

        string command = "none";
        //string command2 = "none";
        int commandIndex = 0;

        public static void LoadImage()
        {

            if (textureToCutUp != null)
                textureToCutUp.Dispose();

            Globals.textures = new List<Texture2D>();
            string s = selectedImageFile;
            using (var stream = new FileStream(s, FileMode.Open))
            {
                textureToCutUp = Texture2D.FromStream(Globals.device, stream);
                textureToCutUp.Name = visualSelectedImageFile;
            }

        }

        public void Update(GameTime gameTime)
        {

            if (currentIndex >= 0 && MouseHelper.IsRightDragged) 
            {
                sourceRectangles[currentIndex] = MouseHelper.RightDragRectangle;
            }


            if (command == "AddSpriteRectangle")
            {
                sourceRectangles.Add(new Rectangle());
                command = "none";
                currentIndex = sourceRectangles.Count - 1;
            }

        }

        public void Draw(GameTime gameTime)
        {
            Rectangle r = new Rectangle();
            animationTime += (float)(gameTime.ElapsedGameTime.TotalSeconds);

            int lsp = Globals.font.LineSpacing;
            int buttonLength = 200;
            int x = buttonLength * 0 + 10;
            int y = lsp * 0;
            int h = lsp * 1;
            int w = Globals.device.Viewport.Width - 10;

            Globals.spriteBatch.Begin();

            r = new Rectangle(0, 0, textureToCutUp.Width, textureToCutUp.Height);
            Globals.spriteBatch.Draw(textureToCutUp, r, Color.White);

            Globals.spriteBatch.DrawString(Globals.font, "Add Sprite Rectangle then right drag the mouse to create a source rectangle.", new Vector2(11, 1), Color.Black);
            Globals.spriteBatch.DrawString(Globals.font, "Add Sprite Rectangle then right drag the mouse to create a source rectangle.", new Vector2(10, 0), Color.Aqua);

            x = buttonLength * 0 + 10;
            y = lsp * 1;

            // New set.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawCheckClickSetCommand(r, "Add Sprite Rectangle", "AddSpriteRectangle", Color.White, Color.Blue);

            y = lsp * 2;

            int sx = 10;
            int sy = y;
            foreach(var rect in sourceRectangles)
            {
                var p0 = new Vector2(sx, sy);
                var p1 = new Vector2(sx + 1, sy + 1);
                Globals.spriteBatch.DrawString(Globals.font, rect.ToString(), p0, Color.Black);
                Globals.spriteBatch.DrawString(Globals.font, rect.ToString() , p1, Color.White);
                MgDrawExt.DrawBasicLine(p0, rect.Location.ToVector2(), 1, Color.Aquamarine);
                MgDrawExt.DrawRectangleOutline(rect, 1, Color.Aquamarine);
                var rect2 = rect;
                rect2.X += 1;
                rect2.Y += 1;
                MgDrawExt.DrawBasicLine(p1, rect2.Location.ToVector2(), 1, Color.Black);
                MgDrawExt.DrawRectangleOutline(rect2, 1, Color.Black);
                sy += lsp;
            }

            Globals.spriteBatch.End();

        }

        public void DrawCheckClickSetCommand(Rectangle r, string label, string commandName, Color textCol, Color outlineColor)
        {
            Globals.spriteBatch.DrawRectangleOutline(r, 1, outlineColor);
            var r2 = r;
            r2.X += 1;
            r2.Y += 1;
            Globals.spriteBatch.DrawString(Globals.font, label, r2.Location.ToVector2(), Color.Black);
            Globals.spriteBatch.DrawString(Globals.font, label, r.Location.ToVector2(), textCol);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                command = commandName;
        }

    }
}
