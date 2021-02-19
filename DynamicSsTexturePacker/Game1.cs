using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using SpriteSheetXnbReader;


// References for additional extras to make this pipelinable...
// Tom Spillman and Andy Dunn
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true
// My post here for how to make a processor importer for this and a little run thru of some of the troubles i had doing it.
// https://community.monogame.net/t/solved-content-importer-processor-how-to-process-seperate-files-into-one-xnb/12040/6

// https://github.com/learn-monogame/learn-monogame.github.io/discussions/9#discussioncomment-371850
// Newest version of mg supports drag and drop.

namespace DynamicSsTexturePacker
{

    public static class Globals
    {
        public static string mode = "SelectImages";
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice device;
        public static SpriteBatch spriteBatch;
        public static SpriteFont font;
        // Does the conversion.
        public static SpriteSheetCreator ssCreator;
        // A little class that encapsulates things that are related to stuff that are specific to a spritesheet like rectangles in it and stuff.
        public static SpriteSheet myGeneratedSpriteSheetInstance;
        public static List<Texture2D> textures = new List<Texture2D>();
        public static string CurrentDirectory = Environment.CurrentDirectory; //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string saveDirectory = "";
        public static string savePath = "";
        public static string saveFileName = "NewSpriteSheet";

        public static RasterizerState rs_scissors_on = new RasterizerState() { ScissorTestEnable = true };
        public static RasterizerState rs_scissors_off = new RasterizerState() { ScissorTestEnable = false };

        public static void SetSaveDirectory(string directoryPath)
        {
            Globals.saveDirectory = directoryPath;
            Globals.savePath = Path.Combine(Globals.saveDirectory, Globals.saveFileName);
            Globals.savePath = Globals.savePath + ".spr";
        }
        public static void CreateAndSave(bool openDirectory)
        {
            ssCreator = new SpriteSheetCreator();
            myGeneratedSpriteSheetInstance = new SpriteSheet();
            ssCreator.MakeSpriteSheet(Globals.device, Globals.saveFileName, 2048, 2048, Globals.textures, out myGeneratedSpriteSheetInstance, true, Globals.savePath);
            if (openDirectory)
                Process.Start(Path.GetDirectoryName(Globals.savePath));
        }
        public static void OpenDirectory(string path)
        {
            Process.Start(Path.GetDirectoryName(path));
        }
    }

    public class Game1 : Game
    {

        ModeSelectSprites modeSelectSprites = new ModeSelectSprites();
        ModeSelectSets modeSelectSets = new ModeSelectSets();

        public Game1()
        {
            Globals.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            this.IsMouseVisible = true;

            Window.TextInput += modeSelectSets.TakeText;
        }



        protected override void Initialize(){   base.Initialize();}

        protected override void LoadContent()
        {
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.font = Content.Load<SpriteFont>("MgFont");
            Globals.device = Globals.graphics.GraphicsDevice;
            MgDrawExt.Initialize(Globals.device, Globals.spriteBatch);

            Globals.SetSaveDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            modeSelectSprites.GetSubDirectorysAndFiles(Globals.CurrentDirectory);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseHelper.Update();

            switch (Globals.mode)
            {
                case "SelectImages":
                    modeSelectSprites.Update(gameTime);
                    break;
                case "Select Anim Sets":
                    modeSelectSets.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (Globals.mode)
            {
                case "SelectImages":
                    modeSelectSprites.Draw( gameTime );
                    break;
                case "Select Anim Sets":
                    modeSelectSets.Draw(gameTime);
                    break;
            }


            base.Draw(gameTime);
        }

        public Texture2D LoadTexture(string FileName)
        {
            return Content.Load<Texture2D>(FileName);
        }

        public void ExampleLoad()
        {
            // Well get the textures.
            Globals.textures.Add(LoadTexture("MonoGameLogoSpliffedup"));
            Globals.textures.Add(LoadTexture("TestOutlineImage"));
            Globals.textures.Add(LoadTexture("filterTestImage"));
            Globals.textures.Add(LoadTexture("sphereImage"));
        }

    }
}