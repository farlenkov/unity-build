#if UNITY_EDITOR

using System;
using UnityEngine;

namespace UnityBuild
{
    [CreateAssetMenu(menuName = "Build/Args", fileName = "DefaultArgs")]
    public class BuildArgs : ScriptableObject
    {
        public string BuildPath = "../../../Builds"; // -buildpath
        public string BuildType = "Default"; // -buildtype
        public string BundleName = "com.company.product"; // -bundle
        public int BundleCode = 1; // -bundlecode
        public string Product = "MyGame"; // -product

        // LOAD

        public static BuildArgs Read()
        {
            var args = Environment.GetCommandLineArgs();
            var result = Resources.LoadAll<BuildArgs>(string.Empty)[0];

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-buildpath": result.BuildPath = args[i + 1]; break;
                    case "-buildtype": result.BuildType = args[i + 1]; break;
                    case "-bundle": result.BundleName = args[i + 1]; break;
                    case "-bundlecode": result.BundleCode = Parse(args[i + 1]); break;
                    case "-product": result.Product = args[i + 1]; break;
                }
            }

            return result;
        }

        static int Parse(string stringValue, int defaultValue = -1)
        {
            if (int.TryParse(stringValue, out var value))
                return value;
            else
                return defaultValue;
        }

        [ContextMenu("Build")]
        public void Build()
        {
            BuildMaker.Build(this);
        }
    }
}

#endif