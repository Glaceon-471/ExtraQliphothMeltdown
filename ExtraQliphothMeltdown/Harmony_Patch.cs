using Harmony;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ExtraQliphothMeltdown
{
    public class Harmony_Patch
    {
        public static string DirectoryPath;
        public static string ConfigPath => $"{DirectoryPath}/Config.xml";
        public static string ImagePath => $"{DirectoryPath}/Image";
        public static string LogPath => $"{DirectoryPath}/Log.txt";

        public static Sprite CustomIsolateAlarm;

        public Harmony_Patch()
        {
            DirectoryPath = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            if (string.IsNullOrEmpty(DirectoryPath))
            {
                File.AppendAllText($"{Application.dataPath}/BaseMods/Error.txt", $"[ExtraQliphothMeltdown] Path could not be found!\n");
                return;
            }
            File.WriteAllText(LogPath, "");

            ConfigManager.Instance.LoadConfig();

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes($"{ImagePath}/CustomIsolateAlarm.png"));
            CustomIsolateAlarm = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            HarmonyInstance instance = HarmonyInstance.Create("Lobotomy.Glaceon471.ExtraQliphothMeltdown");
            new BinahOverloadUIPatch(instance);
            new CreatureModelPatch(instance);
            new CreatureOverloadManagerPatch(instance);
            new IsolateOverloadPatch(instance);
            instance.Patch<GameManager, Harmony_Patch>(nameof(GameManager.StartGame), null, nameof(GameManager_StartGame_Postfix));
        }

        public static void LogWrite(string text) => File.AppendAllText(LogPath, $"{text}\n");

        public static void GameManager_StartGame_Postfix() => ExtraQliphothMeltdownManager.Instance.ResetCount();
    }
}
