using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//using SpriteSheetXnbReader;

using SpriteSheetAnimationPipelineReader;

// ToDo rework the sprite sheet class and the content pipely that uses it to accomidate sets in the sheet.


// References for additional extras to make this pipelinable...
// Tom Spillman and Andy Dunn
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true
// My post here for how to make a processor importer for this and a little run thru of some of the troubles i had doing it.
// https://community.monogame.net/t/solved-content-importer-processor-how-to-process-seperate-files-into-one-xnb/12040/6
// Newest version of mg supports drag and drop i should update and use it since it would make things quicker.
// https://github.com/learn-monogame/learn-monogame.github.io/discussions/9#discussioncomment-371850
// Apos altas similar to what ive done.
// https://github.com/Apostolique/MonoGamePipelineExtension


/// This project useed the monogame nuget pcl however i don't think this is neccessary now have to double check.
/// This project required you to change the project appication properties to target,  .Net framework 4.6.1 , to match the nuget pcl ... 
/// I altered this to 4.5 seems to be ok since im using the monogame extensions.
/// 
/// 
/// using SpriteSheetXnbReader; is included.
/// Game1 adds a reference to SpriteSheetXnbReader. 
/// SpriteSheetXnbPipelineCreator adds a reference to SpriteSheetXnbReader.
/// 
/// 
/// This project adds the mgcb references for the spritesheet pipline dll to the monogame pipeline tool.
/// #-------------------------------- References --------------------------------#
///   
///   /reference:..\..\SpriteSheetXnbReader\bin\Debug\SpriteSheetXnbReader.dll
///   /reference:..\..\SpriteSheetXnbPipelineCreator\bin\Debug\SpriteSheetXnbPipelineCreator.dll
///   
/// or
/// 
///   /reference:..\..\SpriteSheetAnimationPipelineReader\bin\Debug\netcoreapp3.1\SpriteSheetAnimationPipelineReader.dll
///   /reference:..\..\SpriteSheetAnimationPipeline\bin\Debug\netcoreapp3.1\SpriteSheetAnimationPipeline.dll
/// 
///
///   #---------------------------------- Content ---------------------------------#
///   


namespace SpriteSheetMaker
{

    public static class Globals
    {
        public static string mode = "SelectImages";
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice device;
        public static SpriteBatch spriteBatch;
        public static SpriteFont font;
        // Does the conversion.
        public static AnimatedSpriteSheetCreator ssCreator;

        // A little class that encapsulates things that are related to stuff that are specific to a spritesheet like rectangles in it and stuff.
        public static AnimatedSpriteSheet spriteSheetInstance;

        public static List<Texture2D> textures = new List<Texture2D>();
        public static List<AnimatedSpriteSheet.Set> tempSets = new List<AnimatedSpriteSheet.Set>();
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
            ssCreator = new AnimatedSpriteSheetCreator();
            spriteSheetInstance = new AnimatedSpriteSheet();
            ssCreator.MakeSpriteSheet(Globals.device, Globals.saveFileName, 2048, 2048, Globals.textures, tempSets , out spriteSheetInstance, true, Globals.savePath);
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

        SpriteSheet ss;

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

            string cdir = Content.RootDirectory;
            Content.RootDirectory = Path.Combine(cdir, "ExampleSpriteSheet");
            ss = Content.Load<SpriteSheet>("spriteSheetTest01");
            Content.RootDirectory = cdir;

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