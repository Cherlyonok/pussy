using System;
using System.Media;
using System.Windows.Forms;
using Keystroke.API;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Permissions;
using Microsoft.Win32;

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
                            Wallpaper.Set($"pussy.Wallpapers.{keyChar}.jpg", Wallpaper.Style.Fill);
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
    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }

        public static void Set(String wpaper, Style style)
        {
            
            System.Drawing.Image img = System.Drawing.Image.FromFile(Path.GetFullPath(wpaper));
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Span) 
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Stretch)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Tile)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.Center)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
