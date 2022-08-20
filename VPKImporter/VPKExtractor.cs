using System;
using System.IO;
using System.Diagnostics;
using BaseX;

namespace VPKImporter
{
    public class VPKExtractor
    {
        private static readonly string executablePath = Path.Combine("nml_mods", "vpk_extractor");
        private static readonly string windowsExecutable = "Decompiler.exe";
        private static readonly string windowsArgs = "-i {0} -o {1} -d --gltf_export_materials";
        private static readonly string windowsArgsThreaded = "-i {0} -o {1} -d --gltf_export_materials --threads {2}";
        private static readonly string macOSXCommand;
        private static readonly string unixCommand;

        public static void Unpack(string inputFile, string outputPath, byte optionalThreads)
        {
            var platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                // According to https://docs.microsoft.com/en-us/dotnet/api/system.platformid?view=netframework-4.7a.2
                // some platform enums are no longer in use. I've left them here:
                // - case PlatformID.Win32S
                // - case PlatformID.Win32Windows
                // - case PlatformID.WinCE
                // - case PlatformID.Xbox:

                case PlatformID.Win32NT:
                    PerformWindowsUnpack(inputFile, outputPath, optionalThreads);
                    break;
                case PlatformID.Unix:
                    throw new PlatformNotSupportedException("Unix support has not been added yet! Scream at dfg if you see this!");
                case PlatformID.MacOSX:
                    throw new PlatformNotSupportedException("MacOS support has not been added yet! Scream at dfg if you see this!");
                default:
                    throw new PlatformNotSupportedException("You've managed to run this on a platform .NET doesn't recognize, how are you playing Neos?");
            }
        }

        private static void PerformWindowsUnpack (string inputFile, string outputPath, byte optionalThreads)
        {
            var windowsExecutablePath = Path.GetFullPath(Path.Combine(executablePath, windowsExecutable));
            if (!File.Exists(windowsExecutablePath))
                throw new FileNotFoundException("Could not find ValveResourceFormat decompiler. Is it present under nml_mods/vpk_extractor/Decompiler.exe?");

            string formattedWindowsArgs;
            if (optionalThreads > 0)
                formattedWindowsArgs = string.Format(windowsArgsThreaded, inputFile, outputPath, optionalThreads); // 1-255 Threads ;)
            else
                formattedWindowsArgs = string.Format(windowsArgs, inputFile, outputPath);

            UniLog.Log($"Starting VPK extraction with the following command: {windowsExecutablePath} {formattedWindowsArgs}");

            var process = new Process();
            process.StartInfo.FileName = windowsExecutablePath;
            process.StartInfo.Arguments = formattedWindowsArgs;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
            process.WaitForExit();

            UniLog.Log("VPK extraction complete!");

            // Writing to logs en masse can actually hang the application. Don't spam!
            // https://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
        }

        private static void PerformMacOSXUnpack (string inputFile, string outputPath, int? optionalThreads)
        {
            throw new NotImplementedException();
        }

        private static void PerformUniXUnpack (string inputFile, string outputPath, int? optionalThreads)
        {
            throw new NotImplementedException();
        }
    }
}
