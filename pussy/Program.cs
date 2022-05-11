using System;
using System.Media;
using System.Windows.Forms;
using Keystroke.API;
using System.IO;
using System.Reflection;

namespace pussy
{
    internal static class Program
    {
        const string SAFEWORD = "pussy";
        static string given_safeword = "";
        [STAThread]
        static void Main()
        {
            using (var keyApi = new KeystrokeAPI())
            {
                SoundPlayer player;
                keyApi.CreateKeyboardHook((key) =>
                {
                    if (key.KeyCode.ToString().Length == 1)
                    {
                        char keyChar = char.Parse(key.KeyCode.ToString().ToLower());
                        if (keyChar == SAFEWORD[given_safeword.Length])
                        {
                            given_safeword += keyChar;
                            if (given_safeword == SAFEWORD)
                            {
                                Environment.Exit(0);
                            }
                        } else
                        {
                            given_safeword = "";
                        }
                        string fileName = $"pussy.Sounds.{keyChar}.wav";
                        try
                        {
                            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                            player = new SoundPlayer(fileStream);
                            player.Play();
                        }
                        catch { }
                    }
                });
                Application.Run();
            }
            while (true) ;
        }

    }
    
}
