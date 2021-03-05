using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteSheetPipelineReader;


namespace SpriteSheetCreator
{

    public class GameModeSelectSets
    {
        string command = "none";
        //string command2 = "none";
        int commandIndex = 0;

        int visualSelectedImagesStartIndex = 0;
        int visualSelectedSetStartIndex = 0;

        float animationTime = 0f;

        public void Update(GameTime gameTime)
        {

            if (command == "SelectImages")
            {
                Globals.mode = "SelectImages";
                command = "none";
            }

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



            if (command == "AddNewSet")
            {
                int index = Globals.tempSets.Count;
                currentSetIndex = index;
                SpriteSheet.Set set = new SpriteSheet.Set();
                setName = "set_" + index.ToString();
                set.nameOfAnimation = setName;
                Globals.tempSets.Add(set);
                command = "none";
            }

            if (command == "AddToCurrentSet")
            {
                if (currentSetIndex >= 0)
                {
                    var textureIndex = commandIndex;
                    Globals.tempSets[currentSetIndex].spriteIndexs.Add(textureIndex);
                }
                command = "none";
            }

            if (command == "RemoveFromCurrentSet")
            {
                if (currentSetIndex >= 0)
                {
                    Globals.tempSets[currentSetIndex].spriteIndexs.RemoveAt(commandIndex);
                }
                command = "none";
            }

            if (command == "LastSet")
            {
                if (currentSetIndex > 0)
                {
                    currentSetIndex--;
                    setName = Globals.tempSets[currentSetIndex].nameOfAnimation;
                    setTime = Globals.tempSets[currentSetIndex].time;
                    setTimeString = setTime.ToString();
                }
                command = "none";
            }

            if (command == "NextSet")
            {
                int len = Globals.tempSets.Count;
                if (currentSetIndex < len-1 && len > 0)
                {
                    currentSetIndex++;
                    setName = Globals.tempSets[currentSetIndex].nameOfAnimation;
                    setTime = Globals.tempSets[currentSetIndex].time;
                    setTimeString = setTime.ToString();
                }
                command = "none";
            }

        }

        string newSaveName = Globals.saveFileName;
        string setName = "";
        int currentSetIndex = -1;

        string setTimeString = ".25f";
        float setTime = .25f;

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

            if (command == "NameSet")
            {
                setName += e.Character;

                if (Keys.Enter.IsKeyDown())
                {
                    Globals.tempSets[currentSetIndex].nameOfAnimation = setName;
                    command = "none";
                }
                if (Keys.Back.IsKeyDown())
                {
                    SpriteSheet.Set n = Globals.tempSets[currentSetIndex];
                    setName = "";
                    Globals.tempSets[currentSetIndex].nameOfAnimation = setName;
                }
            }

            if (command == "SetTime")
            {
                setTimeString += e.Character;

                if (Keys.Enter.IsKeyDown())
                {
                    float tempResult;
                    if(float.TryParse(setTimeString, out tempResult))
                    {
                        Globals.tempSets[currentSetIndex].time = tempResult;
                        setTime = tempResult;
                        setTimeString = tempResult.ToString();
                    }
                    else
                    {
                        setTimeString = setTime.ToString();
                    }
                    command = "none";
                }
                if (Keys.Back.IsKeyDown())
                {
                    SpriteSheet.Set n = Globals.tempSets[currentSetIndex];
                    setTimeString = "";
                }
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

            Globals.spriteBatch.DrawString(Globals.font, "Set groups of images to be animation sets.", new Vector2(10, y), Color.White);

            x = buttonLength * 0 + 10;
            y = lsp * 2;

            // Go back to sprite select.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawCheckClickSetCommand(r, "Go back to select images", "SelectImages", Color.White, Color.Blue);

            y = lsp * 14;

            // Draw the sheet if possible.
            if (Globals.spriteSheetInstance != null)
                DrawSheetAndShowLabels(new Rectangle(300, y, Globals.spriteSheetInstance.sheetWidth, Globals.spriteSheetInstance.sheetHeight));




            // Animation sets.
            x = buttonLength * 0 + 10;
            y = lsp * 4;

            // New set.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawCheckClickSetCommand(r, "Add new animation set", "AddNewSet", Color.White, Color.Blue);

            y = lsp * 5;

            // Name set.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawTextboxClickSetCommand(r, "Name set", setName, "NameSet", Color.White, Color.Blue);

            y = lsp * 7;

            // Set time.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawTextboxClickSetCommand(r, "Set time", setTimeString , "SetTime", Color.White, Color.Blue);


            y = lsp * 9;

            // calculate and display total duration.
            if (Globals.tempSets.Count > 0)
            {
                var setTotalAnimationDuration = Globals.tempSets[currentSetIndex].time * Globals.tempSets[currentSetIndex].spriteIndexs.Count;
                Globals.spriteBatch.DrawString(Globals.font, "Set's total duration time: " + setTotalAnimationDuration, new Vector2(10, y), Color.White);
            }

            x = buttonLength * 0 + 10;
            y = lsp * 11;

            // Last Set.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawCheckClickSetCommand(r, "Last Set", "LastSet", Color.White, Color.Blue);

            y = lsp * 12;

            // Next Set.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawCheckClickSetCommand(r, "Next Set", "NextSet", Color.White, Color.Blue);

            x = buttonLength * 3 + 30;
            y = lsp * 2;

            // draw animation if it exists.
            r = new Rectangle(x, y, buttonLength, buttonLength);
            DrawSpriteAnimation(r);



            // Sheet stuff.
            x = buttonLength * 1 + 20;
            y = lsp * 4;

            // Draw the create sheet button.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawCheckClickSetCommand(r, "Create sprite sheet", "CreateSheet", Color.White, Color.Blue);

            y = lsp * 5;

            // Name file.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawTextboxClickSetCommand(r, "Name spritesheet", newSaveName, "NameFile", Color.Black, Color.Blue);

            y = lsp * 8;

            // Open the current save path.
            r = new Rectangle(new Point(x, y), new Point(buttonLength, h));
            DrawCheckClickSetCommand(r, "Open save path", "OpenSavePath", Color.White, Color.Blue);

            Globals.spriteBatch.End();


            // Lists.
            x = buttonLength * 0 + 10;
            y = lsp * 14;
            h = lsp * 5;

            // Draw the individual textures when clicked add them to a set that is selected.
            r = new Rectangle(x, y, buttonLength, h);
            int index = DrawVisualClickListDisplay(r.Location.ToVector2(), 200, 50, 5, ref visualSelectedImagesStartIndex, Globals.textures);
            if (index >= 0)
            {
                command = "AddToCurrentSet";
                commandIndex = index;
            }

            x = buttonLength * 1 + 20;

            if (Globals.tempSets.Count > 0)
            {
                var set = Globals.tempSets[currentSetIndex];
                r = new Rectangle(x, y, buttonLength, h);
                index = DrawVisualClickListDisplay(r.Location.ToVector2(), 200, 50, 5, ref visualSelectedSetStartIndex, set);
                if (index >= 0)
                {
                    command = "RemoveFromCurrentSet";
                    commandIndex = index;
                }
            }

        }

        public void DrawCheckClickSetCommand(Rectangle r, string label ,string commandName, Color textCol, Color outlineColor)
        {
            Globals.spriteBatch.DrawRectangleOutline(r, 1, outlineColor);
            Globals.spriteBatch.DrawString(Globals.font, label, r.Location.ToVector2(), textCol);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                command = commandName;
        }

        public void DrawTextboxClickSetCommand(Rectangle r, string textBoxName , string textValue , string commandName, Color textCol, Color outlineColor)
        {
            Globals.spriteBatch.DrawString(Globals.font, textBoxName, r.Location.ToVector2(), outlineColor);
            Rectangle r2 = r;
            r2.Y += Globals.font.LineSpacing;
            Globals.spriteBatch.DrawRectangleOutline(r2, 1, outlineColor);
            Globals.spriteBatch.DrawString(Globals.font, textValue, r2.Location.ToVector2(), textCol);
            if (r2.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                command = commandName;
        }

        public void DrawSheetAndShowLabels(Rectangle r)
        {
            // Draw the resulting spritesheet.
            Globals.spriteBatch.Draw(Globals.spriteSheetInstance.textureSheet, r, Color.White);
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Red);

            // Draw the names of the sprites in the sheet at their locations allow color change over sprites.
            for (int i = 0; i < Globals.spriteSheetInstance.sprites.Count; i++)
            {
                var spriteName = Globals.spriteSheetInstance.sprites[i].nameOfSprite;
                var nameoffset = Globals.spriteSheetInstance.sprites[i].sourceRectangle;
                nameoffset.Location = nameoffset.Location + r.Location;
                var color = Color.White;
                if (nameoffset.Contains(MouseHelper.Pos))
                    color = Color.Red;
                Globals.spriteBatch.DrawString(Globals.font, spriteName, nameoffset.Center.ToVector2(), color);
                Globals.spriteBatch.DrawRectangleOutline(nameoffset, 1, Color.Red);
            }
        }

        /// <summary>
        /// While were drawing them we check for clicks waste not want not. Though this whole thing is realatively not performant as it doesn't need to be.
        /// </summary>
        public int DrawVisualClickListDisplay(Vector2 position, ref int startIndex, int visualListItemBoxWidth, int visualListItemsAllowed,  List<string> items, List<string> visualItems)
        {
            Globals.device.RasterizerState = Globals.rs_scissors_on;
            Globals.device.ScissorRectangle = new Rectangle(position.ToPoint(), new Point(visualListItemBoxWidth, (visualListItemsAllowed + 2) * Globals.font.LineSpacing));
            Globals.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, Globals.rs_scissors_on, null, null);

            int clickedResult = -1;
            int visualDrawIndex = 0;

            // draw a box for moving up.
            var r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * Globals.font.LineSpacing), new Point(50, Globals.font.LineSpacing));
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Green);
            Globals.spriteBatch.DrawString(Globals.font, "Up", r.Location.ToVector2(), Color.Green);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased && startIndex > 0)
                startIndex--;
            visualDrawIndex++;

            // draw the items.
            for (int i = startIndex; i < startIndex + visualListItemsAllowed; i++)
            {
                r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * Globals.font.LineSpacing), new Point(20, Globals.font.LineSpacing));
                var re = new Vector2(r.Right, r.Top);
                if (i < items.Count)
                {
                    Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.White);
                    Globals.spriteBatch.DrawString(Globals.font, i.ToString(), r.Location.ToVector2(), Color.White);
                    Globals.spriteBatch.DrawString(Globals.font, visualItems[i], re, Color.White);
                    if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                    {
                        clickedResult = i;
                    }
                }
                visualDrawIndex++;
            }

            // draw the move down box.
            r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * Globals.font.LineSpacing), new Point(50, Globals.font.LineSpacing));
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Green);
            Globals.spriteBatch.DrawString(Globals.font, "Down", r.Location.ToVector2(), Color.Green);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased && startIndex < items.Count)
                startIndex++;

            Globals.spriteBatch.End();
            return clickedResult;
        }

        /// <summary>
        /// While were drawing them we check for clicks waste not want not. Though this whole thing is realatively not performant as it doesn't need to be.
        /// </summary>
        public int DrawVisualClickListDisplay(Vector2 position, int visualListItemBoxWidth, int visualItemHeight, int visualListItemsAllowed, ref int startIndex, List<Texture2D> items)
        {
            int downSpacing = visualItemHeight;
            Globals.device.RasterizerState = Globals.rs_scissors_on;
            Globals.device.ScissorRectangle = new Rectangle(position.ToPoint(), new Point(visualListItemBoxWidth, (visualListItemsAllowed + 2) * downSpacing));
            Globals.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, Globals.rs_scissors_on, null, null);

            int clickedResult = -1;
            int visualDrawIndex = 0;

            // draw a box for moving up.
            var r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * downSpacing), new Point(50, downSpacing));
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Green);
            Globals.spriteBatch.DrawString(Globals.font, "Up", r.Location.ToVector2(), Color.Green);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased && startIndex > 0)
                startIndex--;
            visualDrawIndex++;

            // draw the items.
            for (int i = startIndex; i < startIndex + visualListItemsAllowed; i++)
            {
                r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * downSpacing), new Point(visualListItemBoxWidth, downSpacing));
                var re = new Vector2(r.Left + 25, r.Top);
                if (i < items.Count)
                {
                    Globals.spriteBatch.Draw(items[i], r, Color.White);
                    Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.White);
                    Globals.spriteBatch.DrawString(Globals.font, i.ToString(), r.Location.ToVector2(), Color.White);
                    Globals.spriteBatch.DrawString(Globals.font, items[i].Name, re, Color.White);
                    if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                    {
                        clickedResult = i;
                    }
                }
                visualDrawIndex++;
            }
            // draw the move down box.
            r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * downSpacing), new Point(50, downSpacing));
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Green);
            Globals.spriteBatch.DrawString(Globals.font, "Down", r.Location.ToVector2(), Color.Green);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased && startIndex < items.Count)
                startIndex++;

            Globals.spriteBatch.End();
            return clickedResult;
        }


        /// <summary>
        /// While were drawing them we check for clicks waste not want not. Though this whole thing is realatively not performant as it doesn't need to be.
        /// </summary>
        public int DrawVisualClickListDisplay(Vector2 position, int visualListItemBoxWidth, int visualItemHeight, int visualListItemsAllowed, ref int startIndex, SpriteSheet.Set set)
        {
            List<int> items = set.spriteIndexs;
            int downSpacing = visualItemHeight;
            Globals.device.RasterizerState = Globals.rs_scissors_on;
            Globals.device.ScissorRectangle = new Rectangle(position.ToPoint(), new Point(visualListItemBoxWidth, (visualListItemsAllowed + 2) * downSpacing));
            Globals.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, Globals.rs_scissors_on, null, null);

            int clickedResult = -1;
            int visualDrawIndex = 0;

            // draw a box for moving up.
            var r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * downSpacing), new Point(50, downSpacing));
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Green);
            Globals.spriteBatch.DrawString(Globals.font, "Up", r.Location.ToVector2(), Color.Green);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased && startIndex > 0)
                startIndex--;
            visualDrawIndex++;

            // draw the items.
            for (int i = startIndex; i < startIndex + visualListItemsAllowed; i++)
            {
                r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * downSpacing), new Point(visualListItemBoxWidth, downSpacing));
                var re = new Vector2(r.Left + 25, r.Top);
                if (i < items.Count)
                {
                    var t = Globals.textures[items[i]];
                    Globals.spriteBatch.Draw(t, r, Color.White);
                    Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.White);
                    Globals.spriteBatch.DrawString(Globals.font, i.ToString(), r.Location.ToVector2(), Color.White);
                    if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                    {
                        clickedResult = i;
                    }
                }
                visualDrawIndex++;
            }
            // draw the move down box.
            r = new Rectangle(position.ToPoint() + new Point(0, visualDrawIndex * downSpacing), new Point(50, downSpacing));
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Green);
            Globals.spriteBatch.DrawString(Globals.font, "Down", r.Location.ToVector2(), Color.Green);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased && startIndex < items.Count)
                startIndex++;

            Globals.spriteBatch.End();
            return clickedResult;
        }

        public void DrawSpriteAnimation(Rectangle r)
        {
            int len = Globals.tempSets.Count;
            if (currentSetIndex > -1 && currentSetIndex < len && len > 0)
            {
                int count = Globals.tempSets[currentSetIndex].spriteIndexs.Count;
                setTime = Globals.tempSets[currentSetIndex].time;
                float totalTime = count * setTime;
                if (animationTime >= totalTime)
                    animationTime = 0; // animationTime - totalTime;
                int initialIndex = (int)(animationTime / setTime);

                if (count > 0)
                {
                    var textureIndex = Globals.tempSets[currentSetIndex].spriteIndexs[initialIndex];
                    Globals.spriteBatch.Draw(Globals.textures[textureIndex], r, Color.White);
                }
            }
        }

    }
}
