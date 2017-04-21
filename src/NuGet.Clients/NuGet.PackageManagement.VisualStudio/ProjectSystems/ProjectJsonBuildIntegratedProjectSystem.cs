// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.ProjectManagement.Projects;
using NuGet.ProjectModel;
using NuGet.VisualStudio;
using EnvDTEProject = EnvDTE.Project;

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
            IVsProjectAdapter vsProjectAdapter,
            string uniqueName)
            : base(jsonConfigPath, msbuildProjectFilePath)
        {
            _vsProjectAdapter = vsProjectAdapter;

            // set project id
            var projectId = _vsProjectAdapter.ProjectId;
            InternalMetadata.Add(NuGetProjectMetadataKeys.ProjectId, projectId);

            InternalMetadata.Add(NuGetProjectMetadataKeys.UniqueName, uniqueName);
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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var resolvedProjects = context.DeferredPackageSpecs.Select(project => project.Name);
            return VSProjectRestoreReferenceUtility.GetDirectProjectReferences(_vsProjectAdapter.DteProject, resolvedProjects, context.Logger);
        }
    }
}
