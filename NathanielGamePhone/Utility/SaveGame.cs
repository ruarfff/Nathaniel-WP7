using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyStorage;

namespace NathanielGame.Utility
{
    public struct SaveGameData
    {
        public int currentLevel;
        public int resources;
        public int lives;
        public bool sound;
    }
    public class SaveGameGlobal
    {
        public static ISaveDevice SaveDevice { get; set; }

        public static string saveFileName = "Nathaniel_Save";
        public static string containerName = "Nathaniel_Container";

        public static SaveGameData saveData;
        public static SaveGameData loadData;
    }


}
