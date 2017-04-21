using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace NuGet.VisualStudio
{
    public static class VsProjectAdapterInfoUtility
    {
        public static string GetDisplayName(IVsProjectAdapter project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string name = project.CustomUniqueName;
            if (IsWebSite(project))
            {
                name = PathHelper.SmartTruncate(name, 40);
            }
            return name;
        }

        public static bool IsWebSite(IVsProjectAdapter project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return project.DteProject.Kind != null && project.DteProject.Kind.Equals(VsProjectTypes.WebSiteProjectTypeGuid, StringComparison.OrdinalIgnoreCase);
        }
    }
}
