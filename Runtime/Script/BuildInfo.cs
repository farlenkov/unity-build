using UnityEngine;

namespace UnityBuild
{
    [CreateAssetMenu(menuName = "Build/Info", fileName = "BuildInfo")]
    public class BuildInfo : ScriptableObject
    {
        public string BundleName;
        public string Version;

#if UNITY_2017_1_OR_NEWER

        public static bool TryLoad(out BuildInfo result)
        {
            var all = Resources.LoadAll<BuildInfo>(string.Empty);

            if (all.Length == 0)
            {
                result = null;
                return false;
            }
            else
            {
                result = all[0];
                return true;
            }
        }

#endif
    }
}
