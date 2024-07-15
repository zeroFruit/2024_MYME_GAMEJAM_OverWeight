using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class Resolution
    {
        [MenuItem("Edit/Reset Playerprefs")]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}