using System;

namespace Noranokyoju.AutoCreateAsset
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoCreateAssetAttribute : Attribute
    {
        public readonly string Path;

        public AutoCreateAssetAttribute(string path)
        {
            Path = path;
        }
    }
}