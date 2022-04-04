using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Noranokyoju.AutoCreateAsset
{

    [InitializeOnLoad]
    internal static class AutoCreateAsset
    {
        private static SettingObject _settingObj;
        private const string AssetPath = "ProjectSettings/Noranokyoju/AutoCreateAsset";
        
        private static SettingObject SettingObject
        {
            get
            {
                if (_settingObj)
                {
                    return _settingObj;
                }
                if (!File.Exists(AssetPath))
                {
                    _settingObj = ScriptableObject.CreateInstance<SettingObject>();
                    return _settingObj;
                }

                _settingObj = InternalEditorUtility
                        .LoadSerializedFileAndForget(AssetPath)
                        .OfType<SettingObject>()
                        .FirstOrDefault();

                if (_settingObj == null)
                {
                    _settingObj = ScriptableObject.CreateInstance<SettingObject>();
                }
                return _settingObj;
            }
        }
        
        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            var instance = SettingObject;
            var serializedObject = new SerializedObject(instance);
            var keywords = SettingsProvider.GetSearchKeywordsFromSerializedObject(serializedObject);

            var provider = new SettingsProvider("Noranokyoju/", SettingsScope.User)
            {
                label = "AutoCreateAsset",
                guiHandler = GUIHandler,
                keywords = keywords
            };

            return provider;
        }
        
        private static void GUIHandler(string searchContext)
        {
            var instance = SettingObject;
            var editor = Editor.CreateEditor(instance);

            using var scope = new EditorGUI.ChangeCheckScope();
            var serializedObject = editor.serializedObject;

            serializedObject.Update();

            editor.DrawDefaultInspector();


            if (!scope.changed) return;

            serializedObject.ApplyModifiedProperties();

            var directoryPath = Path.GetDirectoryName(AssetPath);

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            InternalEditorUtility.SaveToSerializedFileAndForget
            (
                obj: new[] { editor.target },
                path: AssetPath,
                allowTextSerialization: true
            );
        }

        [Serializable]
        private struct ToParseObject
        {
            public string name;
        }
        
        static AutoCreateAsset()
        {
            LoadAssembly("Assembly-CSharp");
            var asmdefAssets = SettingObject.Assemblies;
            if(asmdefAssets == null || asmdefAssets.Length == 0) return;
            var assemblyNames = asmdefAssets.Select(x => JsonUtility.FromJson<ToParseObject>(x.text).name);
            foreach (var name in assemblyNames)
            {
                LoadAssembly(name);
            }
        }
        
        private static void LoadAssembly(string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            var targetTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(UnityEngine.ScriptableObject)));
            foreach (var scriptableType in targetTypes)
            {
                var attribute = scriptableType.GetCustomAttribute<AutoCreateAssetAttribute>();
                if (attribute == null) continue;

                var path = attribute.Path;
                if (File.Exists(path)) continue;
                
                var obj = ScriptableObject.CreateInstance(scriptableType);
                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.Refresh();
            }
        }
    }
}
