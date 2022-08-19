using BaseX;
using System;
using System.IO;
using System.Diagnostics;

namespace VPKImporter
{
    public class VPKExtractor
    {
        private static readonly string executablePath = Path.Combine("nml_mods", "vpk_extractor");
        private static readonly string windowsExecutable = "Decompiler.exe";
        private static readonly string windowsArgs = "-i {0} -o {1} -d --gltf_export_materials"; // We can also add --threads X
        private static readonly string macOSXCommand;
        private static readonly string unixCommand;
        public static void Unpack(string inputFile, string outputPath)
        {
            var platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                // According to https://docs.microsoft.com/en-us/dotnet/api/system.platformid?view=netframework-4.7.2
                // some platform enums are no longer in use. I've left them here:
                // - case PlatformID.Win32S
                // - case PlatformID.Win32Windows
                // - case PlatformID.WinCE
                // - case PlatformID.Xbox:

                case PlatformID.Win32NT:
                    PerformWindowsUnpack(inputFile, outputPath);
                    break;
                case PlatformID.Unix:
                    throw new PlatformNotSupportedException("Unix support has not been added yet! Scream at dfg if you see this!");
                case PlatformID.MacOSX:
                    throw new PlatformNotSupportedException("MacOS support has not been added yet! Scream at dfg if you see this!");
                default:
                    throw new PlatformNotSupportedException("You've managed to run this on a platform .NET doesn't recognize, how are you playing Neos?");
            }
        }

        private static void PerformWindowsUnpack (string inputFile, string outputPath)
        {
            var windowsExecutablePath = Path.GetFullPath(Path.Combine(executablePath, windowsExecutable));
            if (!File.Exists(windowsExecutablePath))
                throw new FileNotFoundException("Could not find ValveResourceFormat decompiler. Is it present under nml_mods/vpk_extractor/Decompiler.exe?");

            var formattedWindowsArgs = string.Format(windowsArgs, inputFile, outputPath);
            var process = new Process();
            process.StartInfo.FileName = windowsExecutablePath;
            process.StartInfo.Arguments = formattedWindowsArgs;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.OutputDataReceived += OnWindowsOutput;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        private static void OnWindowsOutput(object sender, DataReceivedEventArgs e)
        {
            UniLog.Log(e.Data);
        }

        private static void PerformMacOSXUnpack(string inputFile, string outputPath)
        {

        }

        private static void PerformUniXUnpack(string inputFile, string outputPath)
        {

        }
    }
}
