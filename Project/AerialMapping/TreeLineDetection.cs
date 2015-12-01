//-----------------------------------------------------------------------
// <copyright file="TreeLineDetection.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------
namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Class to run tree detection on a kml folder
    /// </summary>
    public class TreeLineDetection
    {
        private int fileCount;
        private MainWindow primaryWindow;

        /// <summary>
        /// Takes a folder as an arguement and creates a copy that has the detection run 
        /// on it.
        /// </summary>
        /// <param name="path">Path to the folder</param>
        /// <param name="window">Reference to window for which we update the progressbar</param>
        public void ConvertFolder(string path, MainWindow window)
        {
            this.primaryWindow = window;
            this.fileCount = 0;
            if (Directory.Exists(path))
            {
                DirectoryInfo source = new DirectoryInfo(path),
                              target = new DirectoryInfo(path + "_treedetection");
                target.Create();
                this.CopyFilesRecursively(source, target);
                this.RunImageProcessingRecursively(target);
                ////Need ability to add layer once done here
            }
        }

        [DllImport("TreeDetection.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Entry(string filePath, string outputfilePath);

        /// <summary>
        /// Runs image processing on all files in a folder
        /// </summary>
        /// <param name="target">Folder path to recursively traverse</param>
        private void RunImageProcessingRecursively(DirectoryInfo target)
        {
            this.primaryWindow.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    primaryWindow.ProgBarDetection.Visibility = System.Windows.Visibility.Visible;
                    primaryWindow.ProgBarDetection.Value = 0;
                    primaryWindow.ProgBarDetection.Maximum = fileCount;
                }));
            foreach (DirectoryInfo dir in target.GetDirectories())
            {
                this.RunImageProcessingRecursively(dir);
            }

            foreach (FileInfo file in target.GetFiles("*.jpg"))
            {
                this.primaryWindow.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    primaryWindow.ProgBarDetection.Value++;
                }));
                Entry(file.FullName, file.FullName);
            }

            foreach (FileInfo file in target.GetFiles("*.png"))
            {
                this.primaryWindow.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    primaryWindow.ProgBarDetection.Value++;
                }));
                Entry(file.FullName, file.FullName);
            }

            this.primaryWindow.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    primaryWindow.ProgBarDetection.Visibility = System.Windows.Visibility.Hidden;
                }));
        }

        /// <summary>
        /// Copies the source folder to the target folder.
        /// </summary>
        /// <param name="source">The folder to be copied.</param>
        /// <param name="target">The folder to copy to.</param>
        private void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                this.CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                if (!File.Exists(Path.Combine(target.FullName, file.Name)))
                {
                    file.CopyTo(Path.Combine(target.FullName, file.Name));
                }
            }

            foreach (FileInfo file in target.GetFiles("*.jpg"))
            {
                this.fileCount++;
            }

            foreach (FileInfo file in target.GetFiles("*.png"))
            {
                this.fileCount++;
            }
        }
    }
}
