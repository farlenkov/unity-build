using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityBuild
{
    [CreateAssetMenu(menuName = "Build/Settings", fileName = "Default")]
    public class BuildSettings : ScriptableObject
    {
        [field: SerializeField] public BuildTargetGroup BuildTargetGroup { get; private set; }
        [field: SerializeField] public BuildTarget BuildTarget { get; private set; }
        [field: SerializeField] public StandaloneBuildSubtarget StandaloneBuildSubtarget { get; private set; }
        [field: SerializeField] public BuildOptions BuildOptions { get; private set; }
        // [field: SerializeField] public UnityEditor.Build.NamedBuildTarget NamedBuildTarget { get; private set; }
        [field: SerializeField] public UnityEditor.Build.OSArchitecture OSArchitecture { get; private set; }

#if UNITY_STANDALONE_OSX
        [field: SerializeField] public UnityEditor.OSXStandalone.MacOSArchitecture MacOSArchitecture { get; private set; }
#endif

#if UNITY_ANDROID
        [field: SerializeField] public bool BuildAppBundle { get; private set; }
        [field: SerializeField] public bool UseAPKExpansionFiles { get; private set; }
#endif

        [field: SerializeField] public GraphicsDeviceType[] GraphicsDeviceTypes { get; private set; }

        [ContextMenu("Build")]
        public void Build()
        {
            var args = BuildArgs.Load();
            Build(args);
        }

        public virtual void Build(BuildArgs args)
        {
            // LOAD CONFIGS

            var defaultVersion = PlayerSettings.bundleVersion;
            var buildPath = args.BuildPath;
            var version = BuildVersion.Get();

            // SETTINGS

            PlayerSettings.bundleVersion = version;
            PlayerSettings.productName = args.Product;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup, args.BundleName);
            // PlayerSettings.SetIl2CppCodeGeneration(settings.NamedBuildTarget, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);

            EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup, BuildTarget);

#if UNITY_ANDROID

            EditorUserBuildSettings.buildAppBundle = BuildAppBundle;
            PlayerSettings.Android.useAPKExpansionFiles = UseAPKExpansionFiles;
            PlayerSettings.Android.bundleVersionCode = args.BundleCode;

            // if (!config.AndroidKeyStoreName.IsNullOrEmpty())
            // {
            //     PlayerSettings.Android.useCustomKeystore = true;
            //     PlayerSettings.Android.keystoreName = config.AndroidKeyStoreName;
            //     PlayerSettings.Android.keystorePass = config.AndroidKeyStorePass;
            //     PlayerSettings.Android.keyaliasName = config.AndroidKeyAliasName;
            //     PlayerSettings.Android.keyaliasPass = config.AndroidKeyAliasPass;
            // }
#endif

            var options = new BuildPlayerOptions()
            {
                // scenes = BuildScenes.GetPaths(),
                locationPathName = buildPath,
                target = BuildTarget,
                targetGroup = BuildTargetGroup,
                subtarget = (int)StandaloneBuildSubtarget,
                options = BuildOptions
            };

#if UNITY_STANDALONE_OSX   
            UnityEditor.OSXStandalone.UserBuildSettings.architecture = settings.OSArchitecture;
#endif

            if (GraphicsDeviceTypes != null &&
                GraphicsDeviceTypes.Length > 0)
            {
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget, GraphicsDeviceTypes);
            }
            else
            {
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget, true);
            }

            // BUILD

            var report = BuildPipeline.BuildPlayer(options);

            // LOG

            var str = new StringBuilder();
            str.AppendFormat("Build Result: {0} - {1}\n", report.summary.result, version);

            foreach (var step in report.steps)
                str.AppendFormat("Build Step: {0}s - {1}\n", step.duration.TotalSeconds, step.name);

            Debug.Log(str.ToString());
            PlayerSettings.bundleVersion = defaultVersion;
        }
    }
}
