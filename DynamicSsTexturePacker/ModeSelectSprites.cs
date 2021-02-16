using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace DynamicSsTexturePacker
{

    // file selection for sprites that will be added to the sheet or allows to remove the ones picked.
    public class ModeSelectSprites
    {
        // Listing of directorys in current directory.
        public List<string> directorySubFolders = new List<string>();

        // Listing of files in the current directory.
        public List<string> directoryFiles = new List<string>();

        // Listing of selected files.
        public List<string> selectedImageFiles = new List<string>();

        // Were we will place textures that will be packed.
        public List<Texture2D> textures = new List<Texture2D>();


        public List<string> visualDirectorySubFolders = new List<string>();
        public List<string> visualDirectoryFiles = new List<string>();
        public List<string> visualSelectedImageFiles = new List<string>();
        int visualDirectorySubFolderStartIndex = 0;
        int visualDirectoryFilesStartIndex = 0;
        int visualSelectedImagesStartIndex = 0;

        int visualListItemsAllowed = 20;
        int visualListItemBoxWidth = 250;


        RasterizerState rs_scissors_on = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState rs_scissors_off = new RasterizerState() { ScissorTestEnable = false };

        public void Update(GameTime gameTime)
        {



            if (command == "FolderBack" )
            {
                Globals.CurrentDirectory = PathGetParentDirectory(Globals.CurrentDirectory);
                GetSubDirectorysAndFiles(Globals.CurrentDirectory);
                command = "none";
            }

            // need add entire folder.


            // need create sets to goto the next mode.


            // need create button ... and thats about it.


            if (command == "EnterSubFolder" && commandIndex >= 0)
            {
                Globals.CurrentDirectory = directorySubFolders[commandIndex];
                GetSubDirectorysAndFiles(Globals.CurrentDirectory);
                command = "none";
                commandIndex = -1;
            }

            if (command == "AddFile" && commandIndex >= 0)
            {
                bool isgood = true;
                foreach(var s in selectedImageFiles)
                {
                    if (s == directoryFiles[commandIndex])
                        isgood = false;
                }
                if (isgood)
                {
                    selectedImageFiles.Add(directoryFiles[commandIndex]);
                    string[] brokenpath = directoryFiles[commandIndex].Split('\\');
                    visualSelectedImageFiles.Add(brokenpath.Last());
                }
                command = "none";
                commandIndex = -1;
            }

            if (command == "RemoveFile" && commandIndex >= 0)
            {
                selectedImageFiles.RemoveAt(commandIndex);
                visualSelectedImageFiles.RemoveAt(commandIndex);
                command = "none";
                commandIndex = -1;
            }

            //selectedImageFiles
        }

        string command = "none";
        string command2 = "none";
        int commandIndex = 0;

        public void Draw(GameTime gameTime)
        {
            Globals.spriteBatch.Begin();

            // Draw the individual textures.
            for (int i = 0; i < textures.Count; i++)
            {
                Globals.spriteBatch.Draw(textures[i], new Rectangle(10 + (i * 100), 10, 100, 100), Color.White);
                Globals.spriteBatch.DrawString(Globals.font, textures[i].Name, new Vector2(10 + (i * 100), (i * 10) + 10), Color.White);
            }

            // draw the back button.
            var r = new Rectangle(new Point(10,10), new Point(50, Globals.font.LineSpacing));
            Globals.spriteBatch.DrawRectangleOutline(r, 1, Color.Red);
            Globals.spriteBatch.DrawString(Globals.font, "Back", r.Location.ToVector2(), Color.Red);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
            {
                command = "FolderBack";
            }


            Globals.spriteBatch.End();

            int index = -1;



            index = VisualClickListDisplay(new Vector2(visualListItemBoxWidth * 0 + 10, 50),ref visualDirectorySubFolderStartIndex, directorySubFolders, visualDirectorySubFolders);
            if (index >= 0)
            {
                // enter this sub folder.
                command = "EnterSubFolder";
                commandIndex = index;
                visualDirectorySubFolderStartIndex = 0;
                visualDirectoryFilesStartIndex = 0;
            }

            index = VisualClickListDisplay(new Vector2(visualListItemBoxWidth * 1 + 10, 50), ref visualDirectoryFilesStartIndex, directoryFiles, visualDirectoryFiles);
            if (index >= 0)
            {
                // select this file.
                command = "AddFile";
                commandIndex = index;
            }

            index = VisualClickListDisplay(new Vector2(visualListItemBoxWidth * 2 + 10, 50) ,ref visualSelectedImagesStartIndex, selectedImageFiles, visualSelectedImageFiles);
            if (index >= 0)
            {
                //// un-select this file prolly.
                command = "RemoveFile";
                commandIndex = index;
            }

            Globals.device.RasterizerState = rs_scissors_off;

        }

        public int VisualClickListDisplay(Vector2 position, ref int startIndex, List<string> items, List<string> visualItems)
        {
            Globals.device.RasterizerState = rs_scissors_on;
            Globals.device.ScissorRectangle = new Rectangle(position.ToPoint(), new Point(visualListItemBoxWidth, (visualListItemsAllowed + 2) * Globals.font.LineSpacing));
            Globals.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, rs_scissors_on, null, null);

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

        public void GetSubDirectorysAndFiles(string path)
        {
            directorySubFolders = Directory.GetDirectories(path).ToList();
            directoryFiles = GetFileTypesInFolderAsList(path, new string[] { ".png", ".jpg" });

            visualDirectorySubFolders = new List<string>();
            for (int i = 0; i < directorySubFolders.Count; i++)
            {
                string[] brokenpath = directorySubFolders[i].Split('\\');
                visualDirectorySubFolders.Add(brokenpath.Last());
            }

            visualDirectoryFiles = new List<string>();
            for (int i = 0; i < directoryFiles.Count; i++)
            {
                string[] brokenpath = directoryFiles[i].Split('\\');
                visualDirectoryFiles.Add(brokenpath.Last());
            }
        }

        public static string PathGetParentDirectory(string path)
        {
            DirectoryInfo d = Directory.GetParent(path);
            if (d != null)
                return d.FullName;
            else
                return path;
        }

        public static string[] GetFileTypesInFolderAsArray(string path, string[] filetypes)
        {
            return GetFileTypesInFolderAsList(path, filetypes).ToArray();
        }

        /// <summary>
        /// Gets files of type pass null to filetypes to get all files.
        /// </summary>
        public static List<string> GetFileTypesInFolderAsList(string path, string[] filetypes)
        {
            List<string> matchinglist = new List<string>();
            string[] namearray;
             
            bool anyall_result = false;
            if (filetypes == null)
            {
                anyall_result = true;
                namearray = Directory.GetFiles(path);
                for (int n = 0; n < namearray.Length; n++)
                    matchinglist.Add(namearray[n]);
            }
            if (anyall_result == false)
            {
                for (int index = 0; index < filetypes.Length; index++)
                {
                    filetypes[index] = filetypes[index].Replace(".", ""); // strip period off first
                    namearray = Directory.GetFiles(path, "*." + filetypes[index]);
                    for (int n = 0; n < namearray.Length; n++)
                    {
                        matchinglist.Add(namearray[n]);
                    }
                }
            }
            return matchinglist;
        }


    }
}
