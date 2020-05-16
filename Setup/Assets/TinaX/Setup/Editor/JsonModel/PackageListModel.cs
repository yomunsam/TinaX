using System.Collections.Generic;

namespace TinaXEditor.Setup.Internal
{
    [System.Serializable]
    public class PackageListModel
    {
        public List<ListItem> packages;

        [System.Serializable]
        public class ListItem
        {
            public string packageName;
            public List<string> versionTags;
            public string displayName;
            public string description;
            public string gitUrl;
            public string repoUrl;
            public string npmUrl;
            public string downloadUrl;

            public List<string> keywords;

            public List<PackageDependency> dependencies;

            public bool thirdparty = false;
        }
        

        [System.Serializable]
        public struct PackageDependency
        {
            public string packageName;
            public List<PackageDependencyVersion> version;
        }

        [System.Serializable]
        public struct PackageDependencyVersion
        {
            public string packageVersionTag;
            public string dependencyVersionTag;
        }

    }

}
