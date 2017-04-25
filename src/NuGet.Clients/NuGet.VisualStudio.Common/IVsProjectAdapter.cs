<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.Frameworks;
using NuGet.ProjectManagement;
=======
﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.ProjectManagement;
using NuGet.ProjectModel;
>>>>>>> lsl-pj

namespace NuGet.VisualStudio
{
    public interface IVsProjectAdapter
    {
<<<<<<< HEAD
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

=======
        string CustomUniqueName { get; }
        string FullName { get; }
        string FullPath { get; }
        string FullProjectPath { get; }
        IVsHierarchy IVsHierarchy { get; }
        bool IsSupported { get; }
        IVsProjectBuildSystem ProjectBuildSystem { get; }
        string ProjectId { get; }
        string ProjectName { get; }
        ProjectNames ProjectNames { get; }
        string[] ProjectTypeGuids { get; }
        string UniqueName { get; }

        EnvDTE.Project DteProject { get; }

        Task<bool> ContainsFile(string path);
        bool SupportsBindingRedirects();
        bool SupportsReference { get; }

        HashSet<string> GetAssemblyClosure(IDictionary<string, HashSet<string>> visitedProjects);
        IEnumerable<string> GetChildItems(string path, string filter, string desiredKind);
        string GetConfigurationFile();
        Task<IReadOnlyList<ProjectRestoreReference>> GetDirectProjectReferencesAsync(IEnumerable<string> resolvedProjects, ILogger log);
        FrameworkName GetDotNetFrameworkName();
        IEnumerable<string> GetFullPaths(string fileName);
        Task<EnvDTE.ProjectItem> GetProjectItemAsync(string path);
        Task<EnvDTE.ProjectItems> GetProjectItemsAsync(string folderPath, bool createIfNotExists);
        dynamic GetPropertyValue(string propertyName);
        IList<IVsProjectAdapter> GetReferencedProjects();
        NuGetFramework GetTargetNuGetFramework();
        UnconfiguredProject GetUnconfiguredProject();

        void AddImportStatement(string targetsPath, ImportLocation location);
        Task<bool> DeleteProjectItemAsync(string path);
        void EnsureCheckedOutIfExists(string root, string path);
        void RemoveImportStatement(string targetsPath);
>>>>>>> lsl-pj
        void Save();
    }
}
