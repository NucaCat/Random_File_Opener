using System.IO;
using Newtonsoft.Json;

namespace Random_File_Opener_Win_Forms.Settings
{
    internal static class SettingsPuller
    {
        public static T Pull<T>(string fileName) where T : class
        {
            var fileExists = File.Exists(fileName);
            if (!fileExists)
                return null;
            
            var settingsJson = new StreamReader(fileName).ReadToEnd();

            var settings = JsonConvert.DeserializeObject<T>(settingsJson);
            return settings;
        }
    }
}