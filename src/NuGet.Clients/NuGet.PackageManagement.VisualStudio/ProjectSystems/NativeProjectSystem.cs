// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using NuGet.ProjectManagement;
using NuGet.VisualStudio;
using EnvDTEProject = EnvDTE.Project;
using Task = System.Threading.Tasks.Task;

namespace NuGet.PackageManagement.VisualStudio
{
    public class NativeProjectSystem : CpsProjectSystem
    {
        public NativeProjectSystem(IVsProjectAdapter vsProjectAdapter, INuGetProjectContext nuGetProjectContext)
            : base(vsProjectAdapter, nuGetProjectContext)
        {
        }

        public override void AddReference(string referencePath)
        {
            // We disable assembly reference for native projects
        }

        protected override async Task AddFileToProjectAsync(string path)
        {
            if (ExcludeFile(path))
            {
                return;
            }

            await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Get the project items for the folder path
            var folderPath = Path.GetDirectoryName(path);
            var fullPath = FileSystemUtility.GetFullPath(ProjectFullPath, path);

            VCProjectHelper.AddFileToProject(VsProjectAdapter.DteProject.Object, fullPath, folderPath);

            NuGetProjectContext.Log(ProjectManagement.MessageLevel.Debug, Strings.Debug_AddedFileToProject, path, ProjectName);
        }

        public override bool ReferenceExists(string name)
        {
            // We disable assembly reference for native projects
            return true;
        }

        public override void RemoveReference(string name)
        {
            // We disable assembly reference for native projects
        }

        public override void RemoveFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var folderPath = Path.GetDirectoryName(path);
            var fullPath = FileSystemUtility.GetFullPath(ProjectFullPath, path);

            bool succeeded;
            succeeded = VCProjectHelper.RemoveFileFromProject(VsProjectAdapter.DteProject.Object, fullPath, folderPath);
            if (succeeded)
            {
                // The RemoveFileFromProject() method only removes file from project.
                // We want to delete it from disk too.
                FileSystemUtility.DeleteFileAndParentDirectoriesIfEmpty(ProjectFullPath, path, NuGetProjectContext);

                if (!string.IsNullOrEmpty(folderPath))
                {
                    NuGetProjectContext.Log(ProjectManagement.MessageLevel.Debug, Strings.Debug_RemovedFileFromFolder, Path.GetFileName(path), folderPath);
                }
                else
                {
                    NuGetProjectContext.Log(ProjectManagement.MessageLevel.Debug, Strings.Debug_RemovedFile, Path.GetFileName(path));
                }
            }
        }
    }
}
