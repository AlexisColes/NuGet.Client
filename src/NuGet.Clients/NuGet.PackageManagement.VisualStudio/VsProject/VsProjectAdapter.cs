using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.Frameworks;
using NuGet.ProjectManagement;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    internal class VsProjectAdapter : IVsProjectAdapter
    {
        private EnvDTE.Project _dteProject;
        private string _fullProjectPath;
        private Func<EnvDTE.Project> _loadDTEProject;

        [Import]
        public IVsProjectAdapterProvider VsProjectAdapterProvider { get; set; }

        [Import]
        public IDeferredProjectWorkspaceService deferredProjectWorkspaceService { get; set; }

        bool IVsProjectAdapter.IsSupported
        {
            get
            {
                if (_dteProject != null)
                {
                    return EnvDTEProjectUtility.IsSupported(_dteProject);
                }

                return true;
            }
        }

        public string ProjectName => EnvDTEProjectInfoUtility.GetName(_dteProject);

        public IVsHierarchy IVsHierarchy => VsHierarchyUtility.ToVsHierarchy(_dteProject);

        public ProjectNames ProjectNames => ProjectNames.FromDTEProject(_dteProject);

        public string FullProjectPath
        {
            get
            {
                if (string.IsNullOrEmpty(_fullProjectPath))
                {
                    _fullProjectPath = EnvDTEProjectInfoUtility.GetFullProjectPath(_dteProject);
                }
                return _fullProjectPath;
            }
        }

        public string CustomUniqueName => EnvDTEProjectInfoUtility.GetCustomUniqueName(_dteProject);

        public string ProjectId => VsHierarchyUtility.GetProjectId(_dteProject);

        public EnvDTE.Project DteProject => _dteProject;

        public bool IsSupportsReference => EnvDTEProjectUtility.SupportsReferences(_dteProject);

        public string UniqueName => EnvDTEProjectInfoUtility.GetUniqueName(_dteProject);

        public string FullPath => EnvDTEProjectInfoUtility.GetFullPath(_dteProject);

        public string FullName => _dteProject.FullName;

        public string[] ProjectTypeGuids => VsHierarchyUtility.GetProjectTypeGuids(_dteProject);

        public IVsProjectBuildSystem ProjectBuildSystem => EnvDTEProjectUtility.GetVsProjectBuildSystem(_dteProject);

        public VsProjectAdapter(EnvDTE.Project dteProject)
        {
            _dteProject = dteProject;
        }

        public VsProjectAdapter(string projectPath, Func<EnvDTE.Project> loadDTEProject)
        {
            _fullProjectPath = projectPath;
            _loadDTEProject = loadDTEProject;
        }

        public UnconfiguredProject GetUnconfiguredProject()
        {
            var context = _dteProject as IVsBrowseObjectContext;
            if (context == null && _dteProject != null)
            { // VC implements this on their DTE.Project.Object
                context = _dteProject.Object as IVsBrowseObjectContext;
            }
            return context != null ? context.UnconfiguredProject : null;
        }

        public IList<IVsProjectAdapter> GetReferencedProjects()
        {
            return EnvDTEProjectUtility.GetReferencedProjects(_dteProject).Select(project => VsProjectAdapterProvider.CreateVsProject(project)).ToList();
        }

        public NuGetFramework GetTargetNuGetFramework()
        {
            return EnvDTEProjectInfoUtility.GetTargetNuGetFramework(_dteProject);
        }

        public void EnsureCheckedOutIfExists(string root, string path)
        {
            EnvDTEProjectUtility.EnsureCheckedOutIfExists(_dteProject, root, path);
        }

        public async Task<EnvDTE.ProjectItems> GetProjectItemsAsync(string folderPath, bool createIfNotExists)
        {
            return await EnvDTEProjectUtility.GetProjectItemsAsync(_dteProject, folderPath, createIfNotExists);
        }

        public FrameworkName GetDotNetFrameworkName()
        {
            return EnvDTEProjectInfoUtility.GetDotNetFrameworkName(_dteProject);
        }

        public void AddImportStatement(string targetsPath, ImportLocation location)
        {
            EnvDTEProjectUtility.AddImportStatement(_dteProject, targetsPath, location);
        }

        public void Save()
        {
            EnvDTEProjectUtility.Save(_dteProject);
        }

        public async Task<bool> DeleteProjectItemAsync(string path)
        {
            return await EnvDTEProjectUtility.DeleteProjectItemAsync(_dteProject, path);
        }

        public void RemoveImportStatement(string targetsPath)
        {
            EnvDTEProjectUtility.RemoveImportStatement(_dteProject, targetsPath);
        }

        public async Task<bool> ContainsFile(string path)
        {
            return await EnvDTEProjectUtility.ContainsFile(_dteProject, path);
        }

        public dynamic GetPropertyValue(string propertyName)
        {
            try
            {
                var envDTEProperty = _dteProject.Properties.Item(propertyName);
                if (envDTEProperty != null)
                {
                    return envDTEProperty.Value;
                }
            }
            catch (ArgumentException)
            {
                // If the property doesn't exist this will throw an argument exception
            }
            return null;
        }

        public IEnumerable<string> GetChildItems(string path, string filter, string desiredKind)
        {
            return NuGetUIThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var childItems = await EnvDTEProjectUtility.GetChildItems(_dteProject, path, filter, VsProjectTypes.VsProjectItemKindPhysicalFile);
                // Get all physical files
                return from p in childItems
                       select p.Name;
            });
        }

        public IEnumerable<string> GetFullPaths(string fileName)
        {
            return NuGetUIThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var paths = new List<string>();
                var projectItemsQueue = new Queue<EnvDTE.ProjectItems>();
                projectItemsQueue.Enqueue(_dteProject.ProjectItems);
                while (projectItemsQueue.Count > 0)
                {
                    var items = projectItemsQueue.Dequeue();
                    foreach (EnvDTE.ProjectItem item in items)
                    {
                        if (item.Kind == VsProjectTypes.VsProjectItemKindPhysicalFile)
                        {
                            if (StringComparer.OrdinalIgnoreCase.Equals(item.Name, fileName))
                            {
                                paths.Add(item.FileNames[1]);
                            }
                        }
                        else if (item.Kind == VsProjectTypes.VsProjectItemKindPhysicalFolder)
                        {
                            projectItemsQueue.Enqueue(item.ProjectItems);
                        }
                    }
                }

                return paths;
            });
        }

        public bool SupportsBindingRedirects()
        {
            return EnvDTEProjectUtility.SupportsBindingRedirects(_dteProject);
        }

        public HashSet<string> GetAssemblyClosure(IDictionary<string, HashSet<string>> visitedProjects)
        {
            return EnvDTEProjectUtility.GetAssemblyClosure(_dteProject, visitedProjects);
        }

        public string GetConfigurationFile()
        {
            return EnvDTEProjectInfoUtility.GetConfigurationFile(_dteProject);
        }

        public async Task<ProjectItem> GetProjectItemAsync(string path)
        {
            return await EnvDTEProjectUtility.GetProjectItemAsync(_dteProject, path);
        }
    }
}
