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
    public static class Globals
    {
        public static string mode = "Menu";   // SelectImages
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice device;
        public static SpriteBatch spriteBatch;
        public static SpriteFont font;
        // Does the conversion.
        public static AnimatedSpriteSheetGenerator ssCreator;
        public static GameModeSelectSprites modeSelectSprites = new GameModeSelectSprites();
        public static GameModeSelectSets modeSelectSets = new GameModeSelectSets();
        public static GameModeSelectCutUpSheetImage modeSelectCutUpSpriteSheet = new GameModeSelectCutUpSheetImage();
        public static GameModeCutUpSpriteSheet modeCutUpSpriteSheet = new GameModeCutUpSpriteSheet();

        // A little class that encapsulates things that are related to stuff that are specific to a spritesheet like rectangles in it and stuff.
        public static SpriteSheet spriteSheetInstance;

        public static List<Texture2D> textures = new List<Texture2D>();
        public static List<SpriteSheet.Set> tempSets = new List<SpriteSheet.Set>();
        public static string CurrentDirectory = Environment.CurrentDirectory;
        //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string saveDirectory = "";
        public static string savePath = "";
        public static string saveFileName = "NewAnimSpriteSheet";

        public static RasterizerState rs_scissors_on = new RasterizerState() { ScissorTestEnable = true };
        public static RasterizerState rs_scissors_off = new RasterizerState() { ScissorTestEnable = false };

        public static void SetSaveDirectory(string directoryPath)
        {
            Globals.saveDirectory = directoryPath;
            Globals.savePath = Path.Combine(Globals.saveDirectory, Globals.saveFileName);
            Globals.savePath = Globals.savePath + ".ssa";
        }

        public static void CreateAndSave(bool openDirectory)
        {
            ssCreator = new AnimatedSpriteSheetGenerator();
            spriteSheetInstance = new SpriteSheet();
            if (Directory.Exists(Globals.saveDirectory) == false)
                Directory.CreateDirectory(Globals.saveDirectory);
            ssCreator.MakeSpriteSheet(Globals.device, Globals.saveFileName, 2048, 2048, Globals.textures, tempSets, out spriteSheetInstance, true, Globals.savePath);
            if (openDirectory)
                Process.Start(Path.GetDirectoryName(Globals.savePath));
        }

        public static void OpenDirectory(string path)
        {
            //Process.Start(path);
            if (File.Exists(path))
                Process.Start("explorer.exe", "/select, " + path);
        }
    }
}
