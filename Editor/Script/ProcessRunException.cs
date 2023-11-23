using System;

namespace UnityBuild
{
    public class ProcessRunException : InvalidOperationException
    {
        public readonly int ExitCode;

        public ProcessRunException(int exitCode, string errors) : base(errors)
        {
            ExitCode = exitCode;
        }
    }
}
