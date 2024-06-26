#if UNITY_EDITOR

using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityUtility;

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

        public void Build(BuildArgs args, BuildConfig config)
        {
            // CACHE PROJECT SETTINGS

            var defaultVersion = PlayerSettings.bundleVersion;
            var defaultProduct = PlayerSettings.productName;
            var defaultIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup);

            // LOAD CONFIGS

            var tempBuildInfo = ScriptableObject.CreateInstance<BuildInfo>();
            var buildPath = string.Format(args.BuildPath, args.Version);
            Log.Info($"[BuildSettings: Build] '{name}', '{args.BundleName}', '{args.Product}', '{args.Version}', '{buildPath}'");

            // SETTINGS

            PlayerSettings.bundleVersion = args.Version;
            PlayerSettings.productName = args.Product;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup, args.BundleName);
            // PlayerSettings.SetIl2CppCodeGeneration(settings.NamedBuildTarget, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);

            EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup, BuildTarget);

            if (BuildInfo.TryLoad(out var buildInfo))
            {
                tempBuildInfo.BundleName = buildInfo.BundleName;
                tempBuildInfo.ConfigName = buildInfo.ConfigName;
                tempBuildInfo.Version = buildInfo.Version;

                buildInfo.BundleName = args.BundleName;
                buildInfo.ConfigName = args.ConfigName;
                buildInfo.Version = args.Version;

                EditorUtility.SetDirty(buildInfo);
                AssetDatabase.SaveAssets();
            }

#if UNITY_ANDROID

            EditorUserBuildSettings.buildAppBundle = BuildAppBundle;
            PlayerSettings.Android.useAPKExpansionFiles = UseAPKExpansionFiles;
            PlayerSettings.Android.bundleVersionCode = args.VersionCode;

            // yield return BundleVersionCodeRequest.CallCoroutine(args.BundleName, version, args.VersionPass);
            // PlayerSettings.Android.bundleVersionCode = BundleVersionCodeRequest.Call(args.BundleName, version, args.VersionPass); //  args.BundleCode;

            if (!config.AndroidKeyStoreName.IsNullOrEmpty())
            {
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = config.AndroidKeyStoreName;
                PlayerSettings.Android.keystorePass = config.AndroidKeyStorePass;
                PlayerSettings.Android.keyaliasName = config.AndroidKeyAliasName;
                PlayerSettings.Android.keyaliasPass = config.AndroidKeyAliasPass;
            }
#endif

            var options = new BuildPlayerOptions()
            {
                // scenes = BuildScenes.GetPaths(),
                scenes = GetScenePaths(EditorBuildSettings.scenes),
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

            PreBuild(options);
            var report = BuildPipeline.BuildPlayer(options);
            PostBuild();

            // LOG

            var str = new StringBuilder();
            str.AppendFormat("Build Result: {0} - {1}\n", report.summary.result, args.Version);

            foreach (var step in report.steps)
                str.AppendFormat("Build Step: {0}s - {1}\n", step.duration.TotalSeconds, step.name);

            Debug.Log(str.ToString());

            // RESTORE PROJECT SETTINGS

            PlayerSettings.bundleVersion = defaultVersion;
            PlayerSettings.productName = defaultProduct;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup, defaultIdentifier);

            // UPDATE BuildInfo

            if (BuildInfo.TryLoad(out buildInfo))
            {
                buildInfo.BundleName = tempBuildInfo.BundleName;
                buildInfo.ConfigName = tempBuildInfo.ConfigName;
                buildInfo.Version = tempBuildInfo.Version;

                EditorUtility.SetDirty(buildInfo);
                AssetDatabase.SaveAssets();
            }
        }

        protected virtual void PreBuild(BuildPlayerOptions options)
        {

        }

        protected virtual void PostBuild()
        {

        }

        private string[] GetScenePaths(EditorBuildSettingsScene[] scenes)
        {
            var paths = new string[scenes.Length];

            for (var i = 0; i < scenes.Length; i++)
                paths[i] = scenes[i].path;

            return paths;
        }

    }
}

#endif