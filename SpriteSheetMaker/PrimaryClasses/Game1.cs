using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteSheetPipelineReader;


// ToDo rework the sprite sheet class and the content pipely that uses it to accomidate sets in the sheet.

// References...
// Tom Spillman and Andy Dunn
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true
// My post here for how to make a processor importer for this and a little run thru of some of the troubles i had doing it.
// https://community.monogame.net/t/solved-content-importer-processor-how-to-process-seperate-files-into-one-xnb/12040/6
// Newest version of mg supports drag and drop i should update and use it since it would make things quicker.
// https://github.com/learn-monogame/learn-monogame.github.io/discussions/9#discussioncomment-371850
// Apos altas similar to what ive done.
// https://github.com/Apostolique/MonoGamePipelineExtension
//
// https://rexcellentgames.com/imgui-and-boring-ui/

/// This project useed the monogame nuget pcl however i don't think this is neccessary now have to double check.
/// This project required you to change the project appication properties to target,  .Net framework 4.6.1 , to match the nuget pcl ... 
/// I altered this to 4.5 seems to be ok since im using the monogame extensions.
/// 
/// 
/// using SpriteSheetReader; is included.
/// Game1 adds a reference to SpriteSheetReader. 
/// SpriteSheetPipeline adds a reference to SpriteSheetReader.
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
    public class Game1 : Game
    {
        GameModeSelectSprites modeSelectSprites = new GameModeSelectSprites();
        GameModeSelectSets modeSelectSets = new GameModeSelectSets();
        GameModeSelectCutUpSheetImage modeSelectCutUpSpriteSheet = new GameModeSelectCutUpSheetImage();
        GameModeCutUpSpriteSheet modeCutUpSpriteSheet = new GameModeCutUpSpriteSheet();

        string msg = "";

        public Game1()
        {
            Globals.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.TextInput += modeSelectSets.TakeText;
        }

        protected override void Initialize()
        {
            Globals.graphics.PreferredBackBufferWidth = 800;
            Globals.graphics.PreferredBackBufferHeight = 600;
            Globals.graphics.ApplyChanges();
            base.Initialize();   
        }

        protected override void UnloadContent() { }

        protected override void LoadContent()
        {
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.font = Content.Load<SpriteFont>("MgFont");
            Globals.device = Globals.graphics.GraphicsDevice;
            MgDrawExt.Initialize(Globals.device, Globals.spriteBatch);

            var savedir = Path.Combine(Environment.CurrentDirectory, "Output");
            Globals.SetSaveDirectory(savedir);

            //modeSelectSprites.GetSubDirectorysAndFiles(Globals.CurrentDirectory);
            modeSelectSprites.Load();
            modeSelectCutUpSpriteSheet.Load();

            msg = TestLoad();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseHelper.Update();

            switch (Globals.mode)
            {
                
                case "SelectCutUpSpriteSheet":
                    modeSelectCutUpSpriteSheet.Update(gameTime);
                    break;
            case "CutUpSpriteSheet":
                    modeCutUpSpriteSheet.Update(gameTime);
            break;
                case "SelectImages":
                    modeSelectSprites.Update(gameTime);
                    break;
                case "Select Anim Sets":
                    modeSelectSets.Update(gameTime);
                    break;
                default:

                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Globals.device.Clear(Color.CornflowerBlue);

            switch (Globals.mode)
            {

                case "SelectCutUpSpriteSheet":
                    modeSelectCutUpSpriteSheet.Draw(gameTime);
                    break;
                case "CutUpSpriteSheet":
                    modeCutUpSpriteSheet.Draw(gameTime);
                    break;
                case "SelectImages":
                    modeSelectSprites.Draw(gameTime);
                    break;
                case "Select Anim Sets":
                    modeSelectSets.Draw(gameTime);
                    break;
                default:

                    int buttonLength = 200;
                    int lh = Globals.font.LineSpacing;
                    int y = lh * 1;

                    Globals.device.Clear(Color.CadetBlue);
                    Globals.spriteBatch.Begin();

                    var r = new Rectangle(new Point(buttonLength * 0 + 10, y), new Point(buttonLength, Globals.font.LineSpacing));
                    DrawCheckClickSetCommand(r, "Cut Up Sprite Sheet", "SelectCutUpSpriteSheet", Color.White, Color.Blue);

                    y = lh * 2;

                    r = new Rectangle(new Point(buttonLength * 0 + 10, y), new Point(buttonLength, Globals.font.LineSpacing));
                    DrawCheckClickSetCommand(r, "Select Images", "SelectImages", Color.White, Color.Blue);

                    Globals.spriteBatch.DrawString(Globals.font, msg, new Vector2(10, 50), Color.White);

                    Globals.spriteBatch.End();

                    break;
            }

            base.Draw(gameTime);
        }

        public void DrawCheckClickSetCommand(Rectangle r, string label, string commandName, Color textCol, Color outlineColor)
        {
            Globals.spriteBatch.DrawRectangleOutline(r, 1, outlineColor);
            Globals.spriteBatch.DrawString(Globals.font, label, r.Location.ToVector2(), textCol);
            if (r.Contains(MouseHelper.Pos) && MouseHelper.IsLeftJustReleased)
                Globals.mode = commandName;
        }

        public string TestLoad()
        {
            string msg = "";
            string cdir = Content.RootDirectory;
            Content.RootDirectory = Path.Combine(cdir, "ExampleSpriteSheet");
            SpriteSheet ss = Content.Load<SpriteSheet>("NewAnimSpriteSheet");
            Content.RootDirectory = cdir;
            msg += "\n F I L E  \n" + "\n";
            msg += ss.name + " " + ss.textureSheet.Name +"\n";
            msg += ss.sheetWidth + " " + ss.sheetHeight + "\n";
            msg += "\n S P R I T E 'S  \n" + "\n";
            int i = 0;
            foreach (var sprite in ss.sprites)
            {
                msg += $"[{i}]  " + sprite.nameOfSprite + "     " + "Source rectangle: " + sprite.sourceRectangle + "\n";
                i++;
            }
            msg += "\n S E T 'S  \n" + "\n";
            foreach (var set in ss.sets)
            {
                msg += set.nameOfAnimation+"    ";
                msg += "sprite time: " + set.time+"    ";
                foreach (var index in set.spriteIndexs)
                {
                    msg += index + " ";
                }
                msg += "\n ";
            }
            msg += "\n ";
            Console.WriteLine(msg);
            return msg;
        }

        public Texture2D LoadTexture(string FileName) { return Content.Load<Texture2D>(FileName); }

    }
}