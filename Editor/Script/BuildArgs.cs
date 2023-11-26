#if UNITY_EDITOR

using System;
using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityUtility;

namespace UnityBuild
{
    [CreateAssetMenu(menuName = "Build/Args", fileName = "DefaultArgs")]
    public class BuildArgs : ScriptableObject
    {
        public string[] BuildName;
        public string BuildType = "Default"; // -buildtype
        public string BundleName = "com.company.product"; // -bundle
        public string Product = "MyGame"; // -product

        [NonSerialized] public int VersionCode; // -code
        [NonSerialized] public string Version; // -ver
        [NonSerialized] public string BuildPath; // -buildpath

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
                    case "-ver": result.Version = args[i + 1]; break;
                    case "-code": result.VersionCode = Parse(args[i + 1]); break;
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
        void Build()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(BuildCoroutine());
        }

        IEnumerator BuildCoroutine()
        {
            var config = BuildConfig.Get();
            Version = BuildVersion.Get();
            BuildPath = Path.Combine(config.BuildPath, Version, string.Format(Path.Combine(BuildName), Version));

            Log.Info($"[BuildArgs: Build] {BuildPath}");

            if (config.VersionPass != null)
                yield return BundleVersionCodeRequest.CallCoroutine(this, config.VersionPass);

            BuildMaker.Build(this);
        }
    }
}

#endif