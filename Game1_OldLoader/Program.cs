using System;

namespace Game1_OldLoader
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
