<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
=======
﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
>>>>>>> lsl-pj
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    [Export(typeof(IVsProjectAdapterProvider))]
<<<<<<< HEAD
    public sealed class VsProjectAdapterProvider : IVsProjectAdapterProvider
    {
        public IVsProjectAdapter CreateVsProject(EnvDTE.Project dteProject)
        {
            if (dteProject == null)
            {
                throw new ArgumentNullException(nameof(dteProject));
            }

            return new VsProjectAdapter(dteProject);
        }

        public IVsProjectAdapter CreateVsProject(string projectPath, Func<EnvDTE.Project> loadedDTEProject)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                throw new ArgumentNullException(nameof(projectPath));
            }

            return new VsProjectAdapter(projectPath, loadedDTEProject);
=======
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
>>>>>>> lsl-pj
        }
    }
}
