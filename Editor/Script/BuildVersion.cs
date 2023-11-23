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
            var tag = Run(@"describe --tags --long --match ""version/[0-9]*""");
            tag = tag.Replace("version/", string.Empty);

            var tagParts = tag.Split('-');
            return string.Join('.', tagParts[0], tagParts[1], tagParts[2].Substring(1));
        }

        public static string Run(string arguments)
        {
            using (var process = new System.Diagnostics.Process())
            {
                var exitCode = process.Run(
                    @"git",
                    arguments,
                    Application.dataPath,
                    out var output,
                    out var errors);

                if (exitCode == 0)
                    return output;
                else
                    throw new ProcessRunException(exitCode, errors);
            }
        }

        public static int Run(
            this Process process,
            string application,
            string arguments,
            string workingDirectory,
            out string output,
            out string errors)
        {
            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = application,
                Arguments = arguments,
                WorkingDirectory = workingDirectory
            };

            var outputBuilder = new StringBuilder();
            var errorsBuilder = new StringBuilder();
            process.OutputDataReceived += (_, args) => outputBuilder.AppendLine(args.Data);
            process.ErrorDataReceived += (_, args) => errorsBuilder.AppendLine(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            output = outputBuilder.ToString().TrimEnd();
            errors = errorsBuilder.ToString().TrimEnd();
            return process.ExitCode;
        }
    }
}
