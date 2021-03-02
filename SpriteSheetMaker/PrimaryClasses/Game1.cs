using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteSheetPipelineReader;


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
///   /reference:..\..\SpriteSheetPipelineReader\bin\Debug\SpriteSheetPipelineReader.dll
///   /reference:..\..\SpriteSheetPipeline\bin\Debug\SpriteSheetPipeline.dll
///   
/// or
/// 
///   /reference:..\..\SpriteSheetPipelineReader\bin\Debug\netcoreapp3.1\SpriteSheetPipelineReader.dll
///   /reference:..\..\SpriteSheetPipeline\bin\Debug\netcoreapp3.1\SpriteSheetPipeline.dll
/// 
///
///   #---------------------------------- Content ---------------------------------#
///   


namespace SpriteSheetCreator
{
    public static class Globals
    {
        public static string mode = "SelectImages";
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice device;
        public static SpriteBatch spriteBatch;
        public static SpriteFont font;
        // Does the conversion.
        public static AnimatedSpriteSheetGenerator ssCreator;

        // A little class that encapsulates things that are related to stuff that are specific to a spritesheet like rectangles in it and stuff.
        //public static SpriteSheetAnimated spriteSheetInstance;
        public static SpriteSheet spriteSheetInstance;

        public static List<Texture2D> textures = new List<Texture2D>();
        //public static List<SpriteSheetAnimated.Set> tempSets = new List<SpriteSheetAnimated.Set>();
        public static List<SpriteSheet.Set> tempSets = new List<SpriteSheet.Set>();
        public static string CurrentDirectory = Environment.CurrentDirectory; //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
            //spriteSheetInstance = new SpriteSheetAnimated();
            spriteSheetInstance = new SpriteSheet();
            if (Directory.Exists(Globals.saveDirectory) == false)
                Directory.CreateDirectory(Globals.saveDirectory);
            ssCreator.MakeSpriteSheet(Globals.device, Globals.saveFileName, 2048, 2048, Globals.textures, tempSets , out spriteSheetInstance, true, Globals.savePath);
            if (openDirectory)
                Process.Start(Path.GetDirectoryName(Globals.savePath));
        }

        public static void OpenDirectory(string path)
        {
            //Process.Start(path);
            if (File.Exists(path))
            {
                Process.Start("explorer.exe", "/select, " + path);
            }
        }
    }

    public class Game1 : Game
    {
        GameModeSelectSprites modeSelectSprites = new GameModeSelectSprites();
        GameModeSelectSets modeSelectSets = new GameModeSelectSets();

        //SpriteSheet ss;

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

            TestLoad();

            var savedir = Path.Combine(Environment.CurrentDirectory, "Output");
            Globals.SetSaveDirectory(savedir);

            modeSelectSprites.GetSubDirectorysAndFiles(Globals.CurrentDirectory);
        }

        public void TestLoad()
        {
            string cdir = Content.RootDirectory;
            Content.RootDirectory = Path.Combine(cdir, "ExampleSpriteSheet");
            SpriteSheet ss = Content.Load<SpriteSheet>("NewAnimSpriteSheet");
            Content.RootDirectory = cdir;
            Console.WriteLine("\n F I L E  \n");
            Console.WriteLine(ss.name +" "+ ss.textureSheet.Name);
            Console.WriteLine(ss.sheetWidth + " " + ss.sheetHeight);
            Console.WriteLine("\n S P R I T E 'S  \n");
            int i = 0;
            foreach (var sprite in ss.sprites)
            {
                Console.WriteLine($"[{i}] \n"+sprite.nameOfSprite + "\n" + "Source rectangle: " + sprite.sourceRectangle);
                i++;
            }
            Console.WriteLine("\n S E T 'S  \n");
            foreach (var set in ss.sets)
            {
                Console.WriteLine(set.nameOfAnimation);
                Console.WriteLine("sprite time: "+set.time);
                foreach (var index in set.spriteIndexs)
                {
                    Console.Write(index + " ");
                }
                Console.WriteLine("\n ");
            }
            Console.WriteLine("\n ");
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