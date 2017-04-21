using System;
using EnvDTE;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    public interface IVsProjectAdapterProvider
    {
        IVsProjectAdapter CreateVsProject(Project dteProject);
        IVsProjectAdapter CreateVsProject(string projectPath, Func<Project> loadedDTEProject);
    }
}