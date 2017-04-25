// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    [Export(typeof(IVsProjectAdapterProvider))]
    internal class VsProjectAdapterProvider : IVsProjectAdapterProvider
    {
        private readonly IDeferredProjectWorkspaceService _deferredProjectWorkspaceService;

        [ImportingConstructor]
        public VsProjectAdapterProvider(IDeferredProjectWorkspaceService dpws)
        {
            Assumes.Present(dpws);

            _deferredProjectWorkspaceService = dpws;
        }

        public IVsProjectAdapter CreateVsProject(EnvDTE.Project dteProject)
        {
            Assumes.Present(dteProject);

            return new VsProjectAdapter(dteProject, this);
        }

        public IVsProjectAdapter CreateVsProject(IVsHierarchy project, Func<EnvDTE.Project> loadDTEProject)
        {
            Assumes.Present(project);
            Assumes.Present(loadDTEProject);

            return new VsProjectAdapter(GetDeferredProjectPath(project), loadDTEProject, this, _deferredProjectWorkspaceService);
        }

        private string GetDeferredProjectPath(IVsHierarchy project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            project.GetCanonicalName(VSConstants.VSITEMID_ROOT, out string projectPath);
            return projectPath;
        }
    }
}
