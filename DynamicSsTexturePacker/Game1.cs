using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using SpriteSheetXnbReader;


// references for additional extras to make this pipelinable...
// Tom Spillman and Andy Dunn
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true
// My post here for how to make a processor importer for this and a little run thru of some of the troubles i had doing it.
// https://community.monogame.net/t/solved-content-importer-processor-how-to-process-seperate-files-into-one-xnb/12040/6

namespace DynamicSsTexturePacker
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        MouseState mouseState;

        // Were we will place textures that will be packed.
        List<Texture2D> textures = new List<Texture2D>();

        // Does the conversion.
        SpriteSheetBuilder ssCreator;

        // A little class that encapsulates things that are related to stuff that are specific to a spritesheet like rectangles in it and stuff.
        SpriteSheet myGeneratedSpriteSheetInstance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
        }

        protected override void Initialize(){   base.Initialize();}

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("MgFont");

            // Well get the textures.
            textures.Add(LoadTexture("MonoGameLogoSpliffedup"));
            textures.Add(LoadTexture("TestOutlineImage"));
            textures.Add(LoadTexture("filterTestImage"));
            textures.Add(LoadTexture("sphereImage"));

            // This class is responsible for converting things to or from images to a spritesheet ect.
            ssCreator = new SpriteSheetBuilder();

            string savepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "spriteSheetTest01.spr");
            ssCreator.MakeSpriteSheet(GraphicsDevice, "mySpriteSheet", 1024, 1024, textures, out myGeneratedSpriteSheetInstance, true, savepath);

            // open the directory were we saved the image and descriptor to.
            //Process.Start(Path.GetDirectoryName(savepath));
        }

        public Texture2D LoadTexture( string FileName)
        {
            return Content.Load<Texture2D>(FileName);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Draw the individual textures.
            for (int i = 0; i < textures.Count; i++)
            {
                spriteBatch.Draw(textures[i], new Rectangle(10 + (i * 100), 10, 100, 100), Color.White);
                spriteBatch.DrawString(font, textures[i].Name, new Vector2(10 + (i * 100), (i * 10) + 10), Color.White);
            }

            // Draw the resulting spritesheet.
            var offset = new Vector2(50, 250);
            spriteBatch.Draw(myGeneratedSpriteSheetInstance.textureSheet, offset, Color.White);

            // Draw the names of the sprites in the sheet at their locations allow color change over sprites.
            for (int i = 0; i < myGeneratedSpriteSheetInstance.sprites.Count; i++)
            {
                var spriteName = myGeneratedSpriteSheetInstance.sprites[i].nameOfSprite;
                var nameoffset = myGeneratedSpriteSheetInstance.sprites[i].sourceRectangle;
                nameoffset.Location = nameoffset.Location + offset.ToPoint();
                var color = Color.White;
                if (nameoffset.Contains(mouseState.Position))
                    color = Color.Red;
                spriteBatch.DrawString(font, spriteName, nameoffset.Center.ToVector2(), color);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}