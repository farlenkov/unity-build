using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityBuild
{
    public static class BuildMaker
    {
        public static void Build()
        {
            var args = BuildArgs.Read();
        }

        public static void Build(BuildArgs args)
        {
            var settingsName = string.IsNullOrEmpty(args.BuildType) ? "Default" : args.BuildType;
            var settingsAll = Resources.LoadAll<BuildSettings>(string.Empty);
            var settings = settingsAll.FirstOrDefault(settings => settings.name == settingsName);
            settings.Build(args);
        }
    }
}
