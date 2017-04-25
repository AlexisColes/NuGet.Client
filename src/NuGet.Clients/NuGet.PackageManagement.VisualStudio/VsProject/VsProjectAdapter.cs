// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.ProjectManagement;
using NuGet.ProjectModel;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    internal class VsProjectAdapter : IVsProjectAdapter
    {
        private readonly EnvDTE.Project _dteProject;
        private readonly Func<EnvDTE.Project> _loadDTEProject;
        private readonly IVsProjectAdapterProvider _vsProjectAdapterProvider;
        private readonly IDeferredProjectWorkspaceService _deferredProjectWorkspaceService;

        private string _fullProjectPath;

        public string CustomUniqueName => EnvDTEProjectInfoUtility.GetCustomUniqueName(_dteProject);

        public string FullName
        {
            get
            {
                if (_dteProject != null)
                {
                    return _dteProject.FullName;
                }
                else
                {
                    return _fullProjectPath;
                }
            }
        }

        public string FullPath
        {
            get
            {
                if (_dteProject != null)
                {
                    return EnvDTEProjectInfoUtility.GetFullPath(_dteProject);
                }
                else
                {
                    return _fullProjectPath;
                }
            }
        }

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

        public IVsHierarchy IVsHierarchy
        {
            get
            {
                if (_dteProject != null)
                {
                    return VsHierarchyUtility.ToVsHierarchy(_dteProject);
                }
                else
                {
                    return null;
                }
            }
        }

        public IVsProjectBuildSystem ProjectBuildSystem => EnvDTEProjectUtility.GetVsProjectBuildSystem(_dteProject);

        public string ProjectId => VsHierarchyUtility.GetProjectId(_dteProject);

        public string ProjectName
        {
            get
            {
                if (_dteProject != null)
                {
                    return EnvDTEProjectInfoUtility.GetName(_dteProject);
                }
                else
                {
                    return Path.GetFileNameWithoutExtension(_fullProjectPath);
                }
            }
        }

        public ProjectNames ProjectNames
        {
            get
            {
                if (_dteProject != null)
                {
                    return ProjectNames.FromDTEProject(_dteProject);
                }
                else
                {
                    return ProjectNames.FromFullProjectPath(_fullProjectPath);
                }
            }
        }

        public string[] ProjectTypeGuids
        {
            get
            {
                if (_dteProject != null)
                {
                    return VsHierarchyUtility.GetProjectTypeGuids(_dteProject);
                }
                else
                {
                    return new string[] { };
                }
            }
        }

        public string UniqueName
        {
            get
            {
                if (_dteProject != null)
                {
                    return EnvDTEProjectInfoUtility.GetUniqueName(_dteProject);
                }
                else
                {
                    return _fullProjectPath;
                }
            }
        }

        public EnvDTE.Project DteProject => _dteProject;

        public bool SupportsReference => EnvDTEProjectUtility.SupportsReferences(_dteProject);

        public VsProjectAdapter(
            EnvDTE.Project dteProject,
            IVsProjectAdapterProvider vsProjectAdapterProvider)
        {
            Assumes.Present(dteProject);
            Assumes.Present(vsProjectAdapterProvider);

            _dteProject = dteProject;
            _vsProjectAdapterProvider = vsProjectAdapterProvider;
        }

        public VsProjectAdapter(
            string projectPath, 
            Func<EnvDTE.Project> loadDTEProject,
            IVsProjectAdapterProvider vsProjectAdapterProvider,
            IDeferredProjectWorkspaceService deferredProjectWorkspaceService)
        {
            Assumes.NotNullOrEmpty(projectPath);
            Assumes.Present(loadDTEProject);
            Assumes.Present(vsProjectAdapterProvider);
            Assumes.Present(deferredProjectWorkspaceService);

            _fullProjectPath = projectPath;
            _loadDTEProject = loadDTEProject;
            _vsProjectAdapterProvider = vsProjectAdapterProvider;
            _deferredProjectWorkspaceService = deferredProjectWorkspaceService;
        }

        public IList<IVsProjectAdapter> GetReferencedProjects()
        {
            return EnvDTEProjectUtility.GetReferencedProjects(_dteProject).Select(project => _vsProjectAdapterProvider.CreateVsProject(project)).ToList();
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
                    foreach (var item in items.Cast<EnvDTE.ProjectItem>())
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

        public async Task<IReadOnlyList<ProjectRestoreReference>> GetDirectProjectReferencesAsync(IEnumerable<string> resolvedProjects, ILogger logger)
        {
            if (_deferredProjectWorkspaceService != null)
            {
                var references = await _deferredProjectWorkspaceService.GetProjectReferencesAsync(_fullProjectPath);

                return references
                    .Select(reference => new ProjectRestoreReference
                    {
                        ProjectPath = reference,
                        ProjectUniqueName = reference
                    })
                    .ToList();
            }
            else
            {
                return await VSProjectRestoreReferenceUtility.GetDirectProjectReferencesAsync(_dteProject, resolvedProjects, logger);
            }
        }

        public async Task<EnvDTE.ProjectItem> GetProjectItemAsync(string path)
        {
            return await EnvDTEProjectUtility.GetProjectItemAsync(_dteProject, path);
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

        public void AddImportStatement(string targetsPath, ImportLocation location)
        {
            EnvDTEProjectUtility.AddImportStatement(_dteProject, targetsPath, location);
        }

        public async Task<bool> DeleteProjectItemAsync(string path)
        {
            return await EnvDTEProjectUtility.DeleteProjectItemAsync(_dteProject, path);
        }

        public void RemoveImportStatement(string targetsPath)
        {
            EnvDTEProjectUtility.RemoveImportStatement(_dteProject, targetsPath);
        }

        public void Save()
        {
            EnvDTEProjectUtility.Save(_dteProject);
        }
    }
}
