using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using VSLangProj;

namespace NuGet.PackageManagement.VisualStudio
{
    public static class VsProjectAdapterUtility
    {
        public static References GetReferences(IVsProjectAdapter project)
        {
            return EnvDTEProjectUtility.GetReferences(project.DteProject);
        }

        public static bool SupportsINuGetProjectSystem(IVsProjectAdapter project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var projectKProject = ProjectKNuGetProjectProvider.GetProjectKProject(project);
            return projectKProject != null;
        }
    }
}
