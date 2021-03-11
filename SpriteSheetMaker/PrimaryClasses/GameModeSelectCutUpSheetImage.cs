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
    public class GameModeSelectCutUpSheetImage
    {
        // Listing of directorys in current directory.
        public List<string> directorySubFolders = new List<string>();
        // Listing of files in the current directory.
        public List<string> directoryFiles = new List<string>();
        //// Listing of selected files.
        //public List<string> selectedImageFiles = new List<string>();
        string selectedImageFile = "";

        public List<string> visualDirectorySubFolders = new List<string>();
        public List<string> visualDirectoryFiles = new List<string>();
        public List<string> visualSelectedImageFiles = new List<string>();
        public string visualSelectedImageFile = "";

        int visualDirectorySubFolderStartIndex = 0;
        int visualDirectoryFilesStartIndex = 0;
        int visualSelectedImagesStartIndex = 0;

        int visualListItemsAllowed = 20;
        int visualListItemBoxWidth = 250;

        string command = "none";
        //string command2 = "none";
        int commandIndex = 0;

        public void Load()
        {
            Globals.CurrentDirectory.GetSubDirectorysAndImageFiles(out directorySubFolders, out directoryFiles, out visualDirectorySubFolders, out visualDirectoryFiles);
        }


        public void Update(GameTime gameTime)
        {

            // goto menu.
            if (command == "Menu")
            {
                Globals.mode = "Menu";
                command = "none";
                commandIndex = -1;
            }

            if (command == "FolderBack")
            {
                Globals.CurrentDirectory = Globals.CurrentDirectory.PathGetParentDirectory();
                Globals.CurrentDirectory.GetSubDirectorysAndImageFiles(out directorySubFolders, out directoryFiles, out visualDirectorySubFolders, out visualDirectoryFiles);
                //GetSubDirectorysAndFiles(Globals.CurrentDirectory);
                command = "none";
            }

            if (command == "EnterSubFolder" && commandIndex >= 0)
            {
                Globals.CurrentDirectory = directorySubFolders[commandIndex];
                Globals.CurrentDirectory.GetSubDirectorysAndImageFiles(out directorySubFolders, out directoryFiles, out visualDirectorySubFolders, out visualDirectoryFiles);
                //GetSubDirectorysAndFiles(Globals.CurrentDirectory);
                command = "none";
                commandIndex = -1;
            }

            if (command == "SelectFile" && commandIndex >= 0)
            {
                selectedImageFile = directoryFiles[commandIndex];
                string[] brokenpath = directoryFiles[commandIndex].Split('\\');
                visualSelectedImageFile = brokenpath.Last();

                GameModeCutUpSpriteSheet.selectedImageFile = selectedImageFile;
                GameModeCutUpSpriteSheet.visualSelectedImageFile = visualSelectedImageFile;
                GameModeCutUpSpriteSheet.LoadImage();
                Globals.mode = "CutUpSpriteSheet";
                command = "none";
                commandIndex = -1;
            }

        }

        public void Draw(GameTime gameTime)
        {
            Globals.device.Clear(Color.BurlyWood);

            Rectangle r = new Rectangle();

            int buttonLength = 200;
            int lh = Globals.font.LineSpacing;
            int y = lh * 5;

            int index = -1;
            y = lh * 2;

            Globals.spriteBatch.Begin();

            Globals.spriteBatch.DrawString(Globals.font, "Select spritesheet image to chop up.", new Vector2(10, 0), Color.White);

            r = new Rectangle(new Point(buttonLength * 0 + 10, y), new Point(buttonLength, Globals.font.LineSpacing));
            DrawCheckClickSetCommand(r, "BackToMenu", "Menu", Color.White, Color.Blue);

            y = lh * 4;

            r = new Rectangle(new Point(buttonLength * 0 + 10, y), new Point(buttonLength, Globals.font.LineSpacing));
            DrawCheckClickSetCommand(r, "Directory back", "FolderBack", Color.White, Color.Blue);


            Globals.spriteBatch.End();


            y = lh * 5;

            index = DrawVisualClickListDisplay(new Vector2(visualListItemBoxWidth * 0 + 10, y), ref visualDirectorySubFolderStartIndex, directorySubFolders, visualDirectorySubFolders, Color.Yellow);
            if (index >= 0)
            {
                command = "EnterSubFolder";
                commandIndex = index;
                visualDirectorySubFolderStartIndex = 0;
                visualDirectoryFilesStartIndex = 0;
            }

            index = DrawVisualClickListDisplay(new Vector2(visualListItemBoxWidth * 1 + 10, y), ref visualDirectoryFilesStartIndex, directoryFiles, visualDirectoryFiles, Color.White);
            if (index >= 0)
            {
                command = "SelectFile";
                commandIndex = index;
            }

        }

        public void DrawCheckClickSetCommand(Rectangle r, string label, string commandName, Color textCol, Color outlineColor)
        {
            Globals.spriteBatch.DrawRectangleOutline(r, 1, outlineColor);
            Globals.spriteBatch.DrawString(Globals.font, label, r.Location.ToVector2(), textCol);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                command = commandName;
        }

        /// <summary>
        /// While were drawing them we check for clicks waste not want not. Though this whole thing is realatively not performant as it doesn't need to be.
        /// </summary>
        public int DrawVisualClickListDisplay(Vector2 position, ref int startIndex, List<string> items, List<string> visualItems, Color textColor)
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
                    Globals.spriteBatch.DrawString(Globals.font, visualItems[i], re, textColor);
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

    }
}
