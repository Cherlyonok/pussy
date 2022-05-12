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
        static string givenSafeword = "";
        static bool isStarted = false;
        static string wallpaperPath;
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
                        if (!isStarted)
                        {
                            wallpaperPath = DesktopManagement.GetCurrentDesktopWallpaper();
                            isStarted = true;
                        }
                        char keyChar = char.Parse(key.KeyCode.ToString().ToLower());

                        if (keyChar == SAFEWORD[givenSafeword.Length])
                        {
                            givenSafeword += keyChar;
                            if (givenSafeword == SAFEWORD)
                            {
                                Wallpaper.Set(wallpaperPath, Wallpaper.Style.Fill);
                                Environment.Exit(0);
                            }
                        } else
                        {
                            givenSafeword = "";
                        }
                        string fileName = $"pussy.Sounds.{keyChar}.wav";
                        try
                        {
                            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                            player = new SoundPlayer(fileStream);
                            string wallpaperName = $"pussy.Wallpapers.{keyChar}.jpg";
                            Wallpaper.Set(wallpaperName, Wallpaper.Style.Fill);
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
        static extern int SystemParametersInfo(int uiAction, int uiParam, string pvParam, int fWinIni);

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
                key.SetValue("WallpaperStyle", "10");
                key.SetValue("TileWallpaper", "0");
            }
            if (style == Style.Fit)
            {
                key.SetValue("WallpaperStyle", "6");
                key.SetValue("TileWallpaper", "0");
            }
            if (style == Style.Span) 
            {
                key.SetValue("WallpaperStyle", "22");
                key.SetValue("TileWallpaper", "0");
            }
            if (style == Style.Stretch)
            {
                key.SetValue("WallpaperStyle", "2");
                key.SetValue("TileWallpaper", "0");
            }
            if (style == Style.Tile)
            {
                key.SetValue("WallpaperStyle", "0");
                key.SetValue("TileWallpaper", "1");
            }
            if (style == Style.Center)
            {
                key.SetValue("WallpaperStyle", "0");
                key.SetValue("TileWallpaper", "0");
            }

            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
    public sealed class DesktopManagement
    {
        private const UInt32 SPI_GETDESKWALLPAPER = 0x73;
        private const int MAX_PATH = 260;
 
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(UInt32 uiAction, int uiParam, string pvParam, int fWinIni);
 
        public static string GetCurrentDesktopWallpaper()
        {
            string currentWallpaper = new string('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, currentWallpaper.Length, currentWallpaper, 0);
            return currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0'));
        }
    }

}
