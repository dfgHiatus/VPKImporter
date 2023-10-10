using Elements.Core;
using Elements.Assets;
using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace VPKImporter;

public class VPKImporter : ResoniteMod
{
    public override string Name => "VPKImporter";
    public override string Author => "dfgHiatus";
    public override string Version => "2.0.0";
    public override string Link => "https://github.com/dfgHiatus/VPKImporter";

    private static ModConfiguration Config;
    private readonly static string CachePath = Path.Combine(Engine.Current.CachePath, "Cache", "DecompressedVPKs");
    private const string VPK_FILE_EXTENSION = ".vpk";

    public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder)
    {
        builder
            .Version(new Version(1, 0, 0))
            .AutoSave(true);
    }

    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<bool> importAsRawFiles =
        new("importAsRawFiles",
        "Import files directly into Resonite. VPKs can be very large, keep this true unless you know what you're doing!",
        () => true);
    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<byte> optionalThreadCount =
        new("optionalThreadCount",
        "Additional thread count. This may speed up imports in some circumstances, large numbers may be unstable (~8<). Leave at 0 for no multithreading.",
        () => 0);
    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<bool> importText =
        new("importText", "Import Text", () => true);
    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<bool> importTexture =
        new("importTexture", "Import Textures", () => true);
    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<bool> importDocument =
        new("importDocument", "Import Documents", () => true);
    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<bool> importMesh =
        new("importMesh", "Import Mesh", () => true);
    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<bool> importPointCloud =
        new("importPointCloud", "Import Point Clouds", () => true);
    [AutoRegisterConfigKey]
    private readonly static ModConfigurationKey<bool> importAudio =
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
        Config = GetConfiguration();
        Directory.CreateDirectory(CachePath);
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
                if (Path.GetExtension(file).ToLower() == VPK_FILE_EXTENSION)
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
            BatchFolderImporter.BatchImport(slot, allDirectoriesToBatchImport, Config.GetValue(importAsRawFiles));

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
            var dir = Path.Combine(CachePath, element.Value);
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
                Error($"Imported VPK {packageName} cannot have unicode characters in its file name.");
                continue;
            }
            var extractedPath = Path.Combine(CachePath, fileToHash[package]);
            Directory.CreateDirectory(extractedPath);
            VPKExtractor.Unpack(package, extractedPath, Config.GetValue(optionalThreadCount));
            dirsToImport.Add(extractedPath);
        }
        return dirsToImport.ToArray();
    }

    private static bool ShouldImportFile(string file)
    {
        var assetClass = AssetHelper.ClassifyExtension(Path.GetExtension(file));
        return (Config.GetValue(importText) && assetClass == AssetClass.Text) ||
        (Config.GetValue(importTexture) && assetClass == AssetClass.Texture) ||
        (Config.GetValue(importDocument) && assetClass == AssetClass.Document) ||
        (Config.GetValue(importMesh) && assetClass == AssetClass.Model
            && Path.GetExtension(file).ToLower() != ".xml") ||
        (Config.GetValue(importPointCloud) && assetClass == AssetClass.PointCloud) ||
        (Config.GetValue(importAudio) && assetClass == AssetClass.Audio) ||
        (Config.GetValue(importFont) && assetClass == AssetClass.Font) ||
        (Config.GetValue(importVideo) && assetClass == AssetClass.Video) ||
            Path.GetExtension(file).ToLower() == VPK_FILE_EXTENSION;
    }
}