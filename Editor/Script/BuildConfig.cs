using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityUtility;

namespace UnityBuild
{
    public class BuildConfig
    {
        public string BuildPath;
        public string UploadPath;
        public string UnityPath;
        public string ZipPath;
        public string VersionPass;
        public List<string> Builders;

        public static BuildConfig Get()
        {
            FindFileInParent.Exec("BuildConfig.json", out var configPath);
            var configJson = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<BuildConfig>(configJson);
            return config;
        }
    }
}
