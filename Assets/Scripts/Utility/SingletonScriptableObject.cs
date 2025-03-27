using UnityEngine;

namespace BlastRoot.Utility {

    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject {

        #region Member Variables

        private static T _instance;

        #endregion Member Variables

        #region Properties

        public static T Instance {
            get {
                if (!_instance) {
                    GetInstance(typeof(T).Name);
                }

                return _instance;
            }
        }

        #endregion Properties

        #region Private Methods

        private static void GetInstance(string name) {
            _instance = Resources.Load<T>(name);

            if (_instance) {
                return;
            }

            _instance = CreateInstance<T>();

#if UNITY_EDITOR
            if (Application.isPlaying) return;
            if (!System.IO.Directory.Exists(Application.dataPath + "/Resources")) {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
            }

            UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/Resources/" + name + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        #endregion Private Methods
    }
}
