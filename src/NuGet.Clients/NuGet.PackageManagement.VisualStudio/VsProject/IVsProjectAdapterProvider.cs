<<<<<<< HEAD
﻿using System;
using EnvDTE;
=======
﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Shell.Interop;
>>>>>>> lsl-pj
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    public interface IVsProjectAdapterProvider
    {
<<<<<<< HEAD
        IVsProjectAdapter CreateVsProject(Project dteProject);
        IVsProjectAdapter CreateVsProject(string projectPath, Func<Project> loadedDTEProject);
=======
        IVsProjectAdapter CreateVsProject(EnvDTE.Project dteProject);
        IVsProjectAdapter CreateVsProject(IVsHierarchy project, Func<EnvDTE.Project> loadDTEProject);
>>>>>>> lsl-pj
    }
}