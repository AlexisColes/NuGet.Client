// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.ProjectManagement.Projects;
using NuGet.ProjectModel;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    /// <summary>
    /// A nuget aware project system containing a .json file instead of a packages.config file
    /// </summary>
    public class ProjectJsonBuildIntegratedProjectSystem : ProjectJsonBuildIntegratedNuGetProject
    {
        private readonly IVsProjectAdapter _vsProjectAdapter;
        private IScriptExecutor _scriptExecutor;

        public ProjectJsonBuildIntegratedProjectSystem(
            string jsonConfigPath,
            string msbuildProjectFilePath,
            IVsProjectAdapter vsProjectAdapter)
            : base(jsonConfigPath, msbuildProjectFilePath)
        {
            Assumes.Present(vsProjectAdapter);

            _vsProjectAdapter = vsProjectAdapter;

            InternalMetadata.Add(NuGetProjectMetadataKeys.ProjectId, _vsProjectAdapter.ProjectId);
            InternalMetadata.Add(NuGetProjectMetadataKeys.UniqueName, _vsProjectAdapter.CustomUniqueName);
        }

        private IScriptExecutor ScriptExecutor
        {
            get
            {
                if (_scriptExecutor == null)
                {
                    _scriptExecutor = ServiceLocator.GetInstanceSafe<IScriptExecutor>();
                }

                return _scriptExecutor;
            }
        }

        public override async Task<bool> ExecuteInitScriptAsync(
            PackageIdentity identity,
            string packageInstallPath,
            INuGetProjectContext projectContext,
            bool throwOnFailure)
        {
            return
                await
                    ScriptExecutorUtil.ExecuteScriptAsync(identity, packageInstallPath, projectContext, ScriptExecutor,
                        _vsProjectAdapter.DteProject, throwOnFailure);
        }

        public override Task<IReadOnlyList<ProjectRestoreReference>> GetDirectProjectReferencesAsync(DependencyGraphCacheContext context)
        {
            Assumes.Present(context);

            var resolvedProjects = context.DeferredPackageSpecs.Select(project => project.Name);
            return _vsProjectAdapter.GetDirectProjectReferencesAsync(resolvedProjects, context.Logger);
        }
    }
}
