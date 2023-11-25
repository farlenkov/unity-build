using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace UnityBuild
{
    public static class BuildVersion
    {
        public static string Get()
        {
            BuildProcess.Run(
                @"git",
                @"describe --tags --long --match ""version/[0-9]*""",
                Application.dataPath,
                out var tag,
                out var error);

            tag = tag.Replace("version/", string.Empty);
            var tagParts = tag.Split('-');
            return string.Join('.', tagParts[0], tagParts[1], tagParts[2].Substring(1));
        }
    }
}
