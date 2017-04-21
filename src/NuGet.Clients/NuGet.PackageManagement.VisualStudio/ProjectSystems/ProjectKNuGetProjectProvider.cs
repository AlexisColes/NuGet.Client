// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ProjectSystem.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Utilities;
using NuGet.ProjectManagement;
using NuGet.VisualStudio;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace NuGet.PackageManagement.VisualStudio
{
    [Export(typeof(IProjectSystemProvider))]
    [Name(nameof(ProjectKNuGetProjectProvider))]
    public class ProjectKNuGetProjectProvider : IProjectSystemProvider
    {
        public bool TryCreateNuGetProject(IVsProjectAdapter project, ProjectSystemProviderContext context, out NuGetProject result)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            result = null;

            var projectK = GetProjectKProject(project);
            if (projectK == null)
            {
                return false;
            }

            result = new ProjectKNuGetProject(
                projectK,
                project.ProjectName,
                project.CustomUniqueName,
                project.ProjectId);

            return true;
        }

        public static INuGetPackageManager GetProjectKProject(IVsProjectAdapter project)
        {
            return GetProjectKProject(project.DteProject);
        }

        public static INuGetPackageManager GetProjectKProject(EnvDTE.Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var vsProject = project as IVsProject;
            if (vsProject == null)
            {
                return null;
            }

            IServiceProvider serviceProvider = null;
            vsProject.GetItemContext(
                (uint)VSConstants.VSITEMID.Root,
                out serviceProvider);
            if (serviceProvider == null)
            {
                return null;
            }

            using (var sp = new ServiceProvider(serviceProvider))
            {
                var retValue = sp.GetService(typeof(INuGetPackageManager));
                if (retValue == null)
                {
                    return null;
                }

                if (!(retValue is INuGetPackageManager))
                {
                    // Workaround a bug in Dev14 prereleases where Lazy<INuGetPackageManager> was returned.
                    var properties = retValue.GetType().GetProperties().Where(p => p.Name == "Value");
                    if (properties.Count() == 1)
                    {
                        retValue = properties.First().GetValue(retValue);
                    }
                }

                return retValue as INuGetPackageManager;
            }
        }
    }
}
