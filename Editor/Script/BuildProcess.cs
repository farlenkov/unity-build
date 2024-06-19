using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityUtility;

namespace UnityBuild
{
    public static class BuildProcess
    {
        public struct UnityArgs
        {
            public string FileName;
            public string BuildName;
            public string BuildType;
            public string Project;
            public string BundleName;
            public string ConfigName;
            public string ProductName;
            public string Version;
            public int VersionCode;
        }
            
        public static string BuildUnity(
            string outPath,
            string logPath,
            BuildConfig config,
            UnityArgs args)
        {
            var buildType = args.BuildType ?? "Default";
            var projectPath = $"./{args.Project}";
            //var outPath = Path.Combine(rootPath, args.BuildName);
            //var logPath = Path.Combine(rootPath, args.BuildName + "-log.txt");
            var filePath = args.FileName == null ? outPath : Path.Combine(outPath, args.FileName);
            var baseArgs = "-batchmode -nographics -quit -executeMethod UnityBuild.BuildMaker.Build";
            var cmd = $"{baseArgs} -projectPath \"{projectPath}\" -buildpath \"{filePath}\" -buildtype \"{buildType}\" -logFile \"{logPath}\" -bundle \"{args.BundleName}\" -config \"{args.ConfigName}\" -product \"{args.ProductName}\" -ver \"{args.Version}\"";

            if (args.VersionCode > 0)
                cmd += $" -code {args.VersionCode}";

            Log.Info($"\n[BuildUnity] {config.UnityPath} {cmd}");

            if (args.FileName == null)
            {
                if (Directory.Exists(outPath))
                    Directory.Delete(outPath, true);
            }
            else
            {
                File.Delete(filePath);
            }

            //var exitCode = Run(config.UnityPath, cmd, null, log => Console.WriteLine(log));
            var exitCode = Run(config.UnityPath, cmd);
            Log.Info($"[BuildUnity] ExitCode: {exitCode} ({buildType}, {args.BundleName})");

            // DELETE DEBUG FOLDER

            return outPath;
        }

        public static int Run(
            string application,
            string arguments = null,
            string workingDirectory = null)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false,
                    FileName = application,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory
                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode;
            }
        }

        public static int Run(
            string application,
            string arguments,
            string workingDirectory,
            out string output,
            out string errors)
        {
            using (var process = new Process())
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

        public static int Run(
            string application,
            string arguments,
            string workingDirectory,
            Action<string> output,
            Action<string> errors = null)
        {
            using (var process = new Process())
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

                if (output != null)
                    process.OutputDataReceived += (_, args) => output(args.Data);

                if (errors != null)
                    process.ErrorDataReceived += (_, args) => errors(args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                return process.ExitCode;
            }
        }
    }
}
