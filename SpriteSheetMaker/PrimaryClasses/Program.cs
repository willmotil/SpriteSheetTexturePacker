using System;

namespace SpriteSheetCreator
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1()) game.Run();
        }
    }
}
