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
        public static string CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    public class Game1 : Game
    {

        ModeSelectSprites modeSelectSprites = new ModeSelectSprites();
        ModeSelectSets modeSelectSets = new ModeSelectSets();

        string saveDirectory = "";
        string savePath = "";
        string saveFileName = "NewSpriteSheet.spr";

        // Does the conversion.
        SpriteSheetCreator ssCreator;

        // A little class that encapsulates things that are related to stuff that are specific to a spritesheet like rectangles in it and stuff.
        SpriteSheet myGeneratedSpriteSheetInstance;

        public Game1()
        {
            Globals.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
        }

        protected override void Initialize(){   base.Initialize();}

        protected override void LoadContent()
        {
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.font = Content.Load<SpriteFont>("MgFont");
            Globals.device = Globals.graphics.GraphicsDevice;
            MgDrawExt.Initialize(Globals.device, Globals.spriteBatch);

            saveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileName = "NewSpriteSheet";
            savePath = Path.Combine(saveDirectory, saveFileName);
            savePath = savePath + ".spr";

            modeSelectSprites.GetSubDirectorysAndFiles(Globals.CurrentDirectory);

            CreateAndSave();
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
                //case "DisplaySheet":
                //    DrawSheetAndShowLabels();
                //    break;
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
                case "DisplaySheet":
                    DrawSheetAndShowLabels();
                    break;
            }


            base.Draw(gameTime);
        }

        public void DrawSheetAndShowLabels()
        {
            Globals.spriteBatch.Begin();

            // Draw the resulting spritesheet.
            var offset = new Vector2(50, 250);
            Globals.spriteBatch.Draw(myGeneratedSpriteSheetInstance.textureSheet, offset, Color.White);

            // Draw the names of the sprites in the sheet at their locations allow color change over sprites.
            for (int i = 0; i < myGeneratedSpriteSheetInstance.sprites.Count; i++)
            {
                var spriteName = myGeneratedSpriteSheetInstance.sprites[i].nameOfSprite;
                var nameoffset = myGeneratedSpriteSheetInstance.sprites[i].sourceRectangle;
                nameoffset.Location = nameoffset.Location + offset.ToPoint();
                var color = Color.White;
                if (nameoffset.Contains(MouseHelper.Pos))
                    color = Color.Red;
                Globals.spriteBatch.DrawString(Globals.font, spriteName, nameoffset.Center.ToVector2(), color);
            }
            Globals.spriteBatch.End();
        }

        public Texture2D LoadTexture(string FileName)
        {
            return Content.Load<Texture2D>(FileName);
        }

        public void OpenDirectory(string path)
        {
            Process.Start(Path.GetDirectoryName(path));
        }

        public void ExampleLoad()
        {
            // Well get the textures.
            modeSelectSprites.textures.Add(LoadTexture("MonoGameLogoSpliffedup"));
            modeSelectSprites.textures.Add(LoadTexture("TestOutlineImage"));
            modeSelectSprites.textures.Add(LoadTexture("filterTestImage"));
            modeSelectSprites.textures.Add(LoadTexture("sphereImage"));
        }

        public void CreateAndSave()
        {
            ssCreator = new SpriteSheetCreator();
            ssCreator.MakeSpriteSheet(Globals.device, saveFileName, 1024, 1024, modeSelectSprites.textures, out myGeneratedSpriteSheetInstance, true, savePath);
            OpenDirectory(savePath);
        }

    }
}