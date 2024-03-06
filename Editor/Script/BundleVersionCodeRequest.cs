using System;
using System.Collections;
using System.Threading.Tasks;
using UnityUtility;

namespace UnityBuild
{
    public static class BundleVersionCodeRequest
    {
        const string URL_TEMPLATE = "https://script.google.com/macros/s/AKfycbxUT_-xdhL30iqM2F6PETuB6dKdyv8bUPnsDvP61OeqzqL_SjC6U6eyF9ZwVHGrXaao/exec?name={0}&version={1}&pass={2}";

#if UNITY_EDITOR

        public static IEnumerator CallCoroutine(BuildArgs args, string pass)
        {
            var url = string.Format(
                URL_TEMPLATE,
                args.BundleName,
                args.Version,
                pass);

            Log.Info($"[BundleVersionCodeRequest] Request: '{url}");

            using (var req = UnityEngine.Networking.UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();

                if (req.error != null)
                {
                    Log.Error($"[BundleVersionCodeRequest] Error: '{req.error}'");
                    yield break;
                }

                var json = req.downloadHandler.text;
                Log.Info($"[BundleVersionCodeRequest] Response: '{json}");
                var resp = UnityEngine.JsonUtility.FromJson<BundleVersionCodeResponse>(json);

                if (resp.IsError)
                {
                    Log.Error($"[BundleVersionCodeRequest] Error: '{resp.error}'");
                    yield break;
                }

                Log.Info($"[BundleVersionCodeRequest] Bundle: '{args.BundleName}' Version: '{args.Version}' Code: {resp.code}");
                yield return null;

                args.VersionCode = resp.code;
            }
        }

#endif

        public static async Task<int> CallAsync(
            string name,
            string version,
            string pass)
        {
            var url = string.Format(
                URL_TEMPLATE,
                name,
                version,
                pass);

            Log.Info($"[BundleVersionCodeRequest] GET: {url}");

            try
            {
                var client = new System.Net.Http.HttpClient();
                var httpResp = await client.GetAsync(url);
                var json = await httpResp.Content.ReadAsStringAsync();
                client.Dispose();

                //var json = WebRequest.Get(url);

                if (!httpResp.IsSuccessStatusCode)
                {
                    Log.Error($"[BundleVersionCodeRequest] Error: {(int)httpResp.StatusCode} - {httpResp.ReasonPhrase}");
                    return 1;
                }

                var resp = Newtonsoft.Json.JsonConvert.DeserializeObject<BundleVersionCodeResponse>(json);

                if (resp.error != null)
                {
                    Log.Error($"[BundleVersionCodeRequest] Error: {resp.error}");
                    return 1;
                }

                Log.Info($"[BundleVersionCodeRequest] Bundle: '{name}' Version: '{version}' Code: {resp.code}");
                return resp.code;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return 1;
            }
        }
    }

    struct BundleVersionCodeResponse
    {
        public string error;
        public string version;
        public int code;
        public bool isNew;

        public bool IsError => error != null;
    }
}
