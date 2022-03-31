using UnityEditorInternal;
using UnityEngine;

namespace Noranokyoju.AutoCreateAsset
{
    internal sealed class SettingObject : ScriptableObject
    {
        [SerializeField] private AssemblyDefinitionAsset[] _assemblies;
        public AssemblyDefinitionAsset[] Assemblies => _assemblies;

    }
}