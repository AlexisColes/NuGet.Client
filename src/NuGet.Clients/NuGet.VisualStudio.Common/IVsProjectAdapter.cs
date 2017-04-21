using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.Frameworks;
using NuGet.ProjectManagement;

namespace NuGet.VisualStudio
{
    public interface IVsProjectAdapter
    {
        bool IsSupported { get; }

        string ProjectName { get; }

        IVsHierarchy IVsHierarchy { get; }

        ProjectNames ProjectNames { get; }

        string FullProjectPath { get; }

        string CustomUniqueName { get; }

        string UniqueName { get; }

        string ProjectId { get; }

        string FullPath { get; }

        string FullName { get; }

        string[] ProjectTypeGuids { get; }

        EnvDTE.Project DteProject { get; }

        bool IsSupportsReference { get; }

        IVsProjectBuildSystem ProjectBuildSystem { get; }

        IList<IVsProjectAdapter> GetReferencedProjects();

        UnconfiguredProject GetUnconfiguredProject();

        NuGetFramework GetTargetNuGetFramework();

        void EnsureCheckedOutIfExists(string root, string path);

        Task<EnvDTE.ProjectItems> GetProjectItemsAsync(string folderPath, bool createIfNotExists);

        Task<EnvDTE.ProjectItem> GetProjectItemAsync(string path);

        Task<bool> DeleteProjectItemAsync(string path);

        FrameworkName GetDotNetFrameworkName();

        void AddImportStatement(string targetsPath, ImportLocation location);

        void RemoveImportStatement(string targetsPath);

        Task<bool> ContainsFile(string path);

        dynamic GetPropertyValue(string propertyName);

        IEnumerable<string> GetChildItems(string path, string filter, string desiredKind);

        IEnumerable<string> GetFullPaths(string fileName);

        bool SupportsBindingRedirects();

        HashSet<string> GetAssemblyClosure(IDictionary<string, HashSet<string>> visitedProjects);

        string GetConfigurationFile();

        void Save();
    }
}
