using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    [Export(typeof(IVsProjectAdapterProvider))]
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
        }
    }
}
