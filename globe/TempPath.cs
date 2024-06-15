
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text;
using Globular;

namespace Globular.TempPath
{
    public class Temp
    {
        /**
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
        /**
        * SaveInitialBackground saves the initial background image path to `INITIAL_BACKGROUND_PATH` if the system is Windows.
        */
        public void SaveInitialBackground()
        {   
            // checks the OS. TODO: add support for other OSs.
            if (! RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;
            var key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop"); 
            
            if(key == null)
                return;
            // modified from https://www.whitebyte.info/programming/why-being-a-programmer-ist-great-or-how-to-get-the-current-windows-wallpaper-in-c
            byte[]? path = (byte[]?)key.GetValue("TranscodedImageCache");
            string wallpaper_file = Encoding.Unicode.GetString(GetSlice(path, 24)).TrimEnd("\0".ToCharArray());

            if (File.Exists(wallpaper_file))
            {
                INITIAL_BACKGROUND_PATH = wallpaper_file;
            }
        }
        // GetSlice is a helper function that returns a slice (copy) of a byte array.
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
            // Handles changing the background image with a variety of input types.
            BackgroundSetter backgroundSetter = new BackgroundSetter();

            // If the original bg image is found, replace it.
            if (INITIAL_BACKGROUND_PATH != "")            
                backgroundSetter.SetBackground(INITIAL_BACKGROUND_PATH);
        }

        public void Clear()
        {
            Directory.Delete(ABSOLUTE_PATH, true);
        }


    }
}