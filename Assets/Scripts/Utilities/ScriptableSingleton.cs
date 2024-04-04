using System.Linq;
using UnityEngine;

namespace MiningTown.Utilities
{
    /// <summary>
    /// Warning: Any ScriptableSingleton should be in Resources folder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                    instance = instance ?? Resources.Load<T>(typeof(T).Name);
                    if (instance == null)
                    {
                        Debug.LogError("Cannot find scriptable singleton of type " + typeof(T).Name + " " + "in resource folder");
                    }
                }
                return instance;
            }
        }
    }
}