using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UnityBuild
{
    public static class BuildProcess
    {
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
                    UseShellExecute = false,
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
