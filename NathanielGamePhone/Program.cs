namespace NathanielGame
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (NathanielGame game = new NathanielGame())
            {
                game.Run();
            }
        }
    }
#endif

#if WINDOWS_PHONE
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (var game = new NathanielGame())
            {
                game.Run();
            }
        }
    }
#endif


}

