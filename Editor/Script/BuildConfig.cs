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

        public string AndroidKeyStoreName;
	    public string AndroidKeyStorePass;
	    public string AndroidKeyAliasName;
	    public string AndroidKeyAliasPass;

        public static BuildConfig Get()
        {
            return Get<BuildConfig>();
        }
        
        public static T Get<T>()
        {
            FindFileInParent.Exec("BuildConfig.json", out var configPath);
            var configJson = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<T>(configJson);
            return config;
        }
    }
}
