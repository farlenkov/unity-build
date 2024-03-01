#if UNITY_EDITOR

using System.Linq;
using UnityEngine;
using UnityUtility;

namespace UnityBuild
{
    public static class BuildMaker
    {
        public static void Build()
        {
            var args = BuildArgs.Read();
            var config = BuildConfig.Get();
            Build(args, config);
        }

        public static void Build(BuildArgs args, BuildConfig config)
        {
            var settingsName = string.IsNullOrEmpty(args.BuildType) ? "Default" : args.BuildType;
            var settingsAll = Resources.LoadAll<BuildSettings>(string.Empty);
            var settings = settingsAll.FirstOrDefault(settings => settings.name == settingsName);

            if (settings == null)
                Log.Error($"[BuildMaker: Build] BuildSettings not found by name: '{settingsName}'");

            settings.Build(args, config);
        }
    }
}

#endif