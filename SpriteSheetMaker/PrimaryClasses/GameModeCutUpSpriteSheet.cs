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
    public class GameModeCutUpSpriteSheet
    {

        public static string selectedImageFile = "";
        public static string visualSelectedImageFile = "";
        public static Texture2D textureToCutUp;

        public static void LoadImage()
        {
            if (textureToCutUp != null)
                textureToCutUp.Dispose();

            Globals.textures = new List<Texture2D>();
            string s = selectedImageFile;
            using (var stream = new FileStream(s, FileMode.Open))
            {
                textureToCutUp = Texture2D.FromStream(Globals.device, stream);
                textureToCutUp.Name = visualSelectedImageFile;
            }
            //Globals.mode = "Select Anim Sets";
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            Globals.spriteBatch.Begin();

            var r = new Rectangle(0, 0, textureToCutUp.Width, textureToCutUp.Height);
            Globals.spriteBatch.Draw(textureToCutUp, r, Color.White);

            Globals.spriteBatch.End();

        }

    }
}
