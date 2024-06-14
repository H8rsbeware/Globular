
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text;
using Globular;

namespace Globular.TempPath
{
    public class Temp
    {
        /*
        * This class is used to create a temporary directory `Temp.ABSOLUTE_PATH`, store it, and provide a method to concatenate a path to it.        
        */
        public string ABSOLUTE_PATH = CreateTempPath();
        public string INITIAL_BACKGROUND_PATH = "";
        // Called on init, this method guarentees a new temporary directory, with a random name in the system's temp directory, and returns its path as a string.
        static string CreateTempPath()
        {
            string tempDir = Path.Combine(
                Path.GetTempPath(),
                Path.GetRandomFileName()   
            );

            if(File.Exists(tempDir))
            {
                return CreateTempPath();
            } else {
                Directory.CreateDirectory(tempDir);
                return tempDir;
            }
            
        }
        // Concatenates the given path with the `ABSOLUTE_PATH` and returns the result.
        public string ConcatWith(string path)
        {
            switch (path[0])
            {
                case '/' :
                    return ABSOLUTE_PATH + path;
                case '\\' :
                    return ABSOLUTE_PATH + path;
                default:
                    return ABSOLUTE_PATH + "\\" + path;
            }
        }

        public void SaveInitialBackground()
        {   
            if (! RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;
            var key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop"); 
            
            if(key == null)
                return;
            
            byte[]? path = (byte[]?)key.GetValue("TranscodedImageCache");
            string wallpaper_file = Encoding.Unicode.GetString(GetSlice(path, 24)).TrimEnd("\0".ToCharArray());

            if (File.Exists(wallpaper_file))
            {
                INITIAL_BACKGROUND_PATH = wallpaper_file;
            }
        }

        // Source: http://stackoverflow.com/a/406576/441907
        static byte[] GetSlice(byte[]? source, int pos)
        {
            if (source == null)
                return new byte[0];

            byte[] destfoo = new byte[source.Length - pos];
            Array.Copy(source, pos, destfoo, 0, destfoo.Length);
            return destfoo;
        }

        public void ReplaceBackground()
        {
            BackgroundSetter backgroundSetter = new BackgroundSetter();

            if (INITIAL_BACKGROUND_PATH != "")
                backgroundSetter.SetBackground(INITIAL_BACKGROUND_PATH);
        }

        public void Clear()
        {
            Directory.Delete(ABSOLUTE_PATH, true);
        }


    }
}