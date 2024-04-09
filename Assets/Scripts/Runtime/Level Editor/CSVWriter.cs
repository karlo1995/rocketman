using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

namespace SayDesign.LevelEditor
{
    public class CSVWriter : SerializedMonoBehaviour
    {
        private const string PATH = "Assets/Level Editor/Data.csv";

        [ContextMenu("Create CSV File")]
        private void CreateCSVFile()
        {
            if (!File.Exists(PATH)) return;
            TextWriter tw = new StreamWriter(PATH, false);
            tw.WriteLine("World, Level, Platform, X, Y, Platform Movement, Platform Name");
            tw.Close();

            Debug.Log("CSV File Has Been Successfully Created");
        }

        public static void WriteCSV(int world, int level, int platform, Vector2 position, PlatformMovement platformMovement, Sprite platformName)
        {
            if (!File.Exists(PATH)) return;
            using (TextWriter tw = new StreamWriter(PATH, true))
            {
                tw.WriteLine($"{world}, {level}, {platform}, {position.x}, {position.y}, {platformMovement}, {platformName.name}");
            }
            Debug.Log($"Data Entry Has Been Successfully Created: \nWorld: {world} | Level: {level} | Platform: {platform} | Position: ({position.x}, {position.y}) | Platform Movement: {platformMovement} | Platform Name: {platformName.name}");
        }
    }
}
