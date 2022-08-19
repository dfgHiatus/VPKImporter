using BaseX;
using CodeX;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace VPKImporter
{
    public class VPKImporter : NeosMod
    {
        public override string Name => "VPKImporter";
        public override string Author => "dfgHiatus";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/dfgHiatus/VPKImporter";

        private static ModConfiguration _config;
        private static string _cachePath = Path.Combine(Engine.Current.CachePath, "Cache", "DecompressedVPKs");

        public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder)
        {
            builder
                .Version(new Version(1, 0, 0))
                .AutoSave(true);
        }

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> importAsRawFiles =
            new("importAsRawFiles",
            "Import files directly into Neos. VPKs can be very large, keep this true unless you know what you're doing!",
            () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> importText =
            new("importText", "Import Text", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> importTexture =
            new("importTexture", "Import Textures", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> importDocument =
            new("importDocument", "Import Documents", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> importMesh =
            new("importMesh", "Import Mesh", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> importPointCloud =
            new("importPointCloud", "Import Point Clouds", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> importAudio =
            new("importAudio", "Import Audio", () => true);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> importFont =
            new("importFont", "Import Fonts", () => true);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> importVideo =
            new("importVideo", "Import Videos", () => true);

        public override void OnEngineInit()
        {
            new Harmony("net.dfgHiatus.VPKImporter").PatchAll();
            _config = GetConfiguration();
            Directory.CreateDirectory(_cachePath);
        }

        [HarmonyPatch(typeof(UniversalImporter), "Import", typeof(AssetClass), typeof(IEnumerable<string>),
            typeof(World), typeof(float3), typeof(floatQ), typeof(bool))]
        public class UniversalImporterPatch
        {
            public static bool Prefix(ref IEnumerable<string> files)
            {
                List<string> hasVPK = new();
                List<string> noVPK = new();
                foreach (var file in files)
                {
                    if (Path.GetExtension(file).ToLower().Equals(".vpk"))
                        hasVPK.Add(file);
                    else
                        noVPK.Add(file);
                }

                List<string> allDirectoriesToBatchImport = new();
                foreach (var dir in DecomposeVPKs(hasVPK.ToArray()))
                    allDirectoriesToBatchImport.AddRange(Directory.GetFiles(dir, "*", SearchOption.AllDirectories)
                        .Where(ShouldImportFile).ToArray());

                var slot = Engine.Current.WorldManager.FocusedWorld.AddSlot("VPK Import");
                slot.PositionInFrontOfUser();
                BatchFolderImporter.BatchImport(slot, allDirectoriesToBatchImport, _config.GetValue(importAsRawFiles));

                if (noVPK.Count <= 0) return false;
                files = noVPK.ToArray();
                return true;
            }
        }

        private static string[] DecomposeVPKs(string[] files)
        {
            var fileToHash = files.ToDictionary(file => file, Utils.GenerateMD5);
            HashSet<string> dirsToImport = new();
            HashSet<string> vpksToDecompress = new();
            foreach (var element in fileToHash)
            {
                var dir = Path.Combine(_cachePath, element.Value);
                if (!Directory.Exists(dir))
                    vpksToDecompress.Add(element.Key);
                else
                    dirsToImport.Add(dir);
            }
            foreach (var package in vpksToDecompress)
            {
                var packageName = Path.GetFileNameWithoutExtension(package);
                if (Utils.ContainsUnicodeCharacter(packageName))
                {
                    Error("Imported VPK cannot have unicode characters in its file name.");
                    continue;
                }
                var extractedPath = Path.Combine(_cachePath, fileToHash[package]);
                VPKExtractor.Unpack(package, extractedPath);
                dirsToImport.Add(extractedPath);
            }
            return dirsToImport.ToArray();
        }

        private static bool ShouldImportFile(string file)
        {
            var assetClass = AssetHelper.ClassifyExtension(Path.GetExtension(file));
            return (_config.GetValue(importText) && assetClass == AssetClass.Text) ||
            (_config.GetValue(importTexture) && assetClass == AssetClass.Texture) ||
            (_config.GetValue(importDocument) && assetClass == AssetClass.Document) ||
            (_config.GetValue(importMesh) && assetClass == AssetClass.Model
                && Path.GetExtension(file).ToLower() != ".xml") ||
            (_config.GetValue(importPointCloud) && assetClass == AssetClass.PointCloud) ||
            (_config.GetValue(importAudio) && assetClass == AssetClass.Audio) ||
            (_config.GetValue(importFont) && assetClass == AssetClass.Font) ||
            (_config.GetValue(importVideo) && assetClass == AssetClass.Video);
        }
    }
}