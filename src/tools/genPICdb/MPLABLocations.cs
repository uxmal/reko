#region License
/* 
 *
 * Copyrighted (c) 2017-2025 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.netbeans.org/cddl.html
 * or http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * When distributing Covered Code, include this CDDL Header Notice in each file
 * and include the License file at http://www.netbeans.org/cddl.txt.
 * If applicable, add the following below the CDDL Header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */
#endregion

namespace Reko.Tools.genPICdb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;

#if NETCOREAPP
    using System.Diagnostics.CodeAnalysis;
#else
    using Microsoft.Win32;
#endif

    /// <summary>
    /// An interface providing MPLAB X IDE installation information.
    /// </summary>
    public interface IMPLABLocations
    {
        /// <summary>
        /// Gets a value indicating whether the installation information is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the MPLAB X IDE version as a string.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets a value indicating whether the MPLAB X IDE uses DFP - Device Family Packs - (MPLAB X version &gt;= 4.10) or the older 'crownking.edc.jar' file
        /// </summary>
        bool UsePacks { get; }

        /// <summary>
        /// Gets the pathname where to find the DFPs or the 'jar' file.
        /// </summary>
        string SourceFolder { get; }

    }

    /// <summary>
    /// This factory class provided the installed MPLAB X IDE locations information for Windows, Linux and OSX depending on the run-time environment.
    /// </summary>
    public static class MPLABLocations
    {

        #region Inner classes

#if NETCOREAPP

        /// <summary>
        /// A dummy registry implementation to satisfy the C # compiler until NetCore eventually supports this class.
        /// </summary>
        private static class Registry
        {
            public sealed class RegistryKey
            {
                public int SubKeyCount => 0;

                public RegistryKey OpenSubKey(string _) => null;
                public object GetValue(string _1, object _2) => null;
            }

            public static readonly RegistryKey LocalMachine = new RegistryKey();

        }

#endif


        /// <summary>
        /// This class permits to retrieve the real/actual path of a given file pathname. Any symbol link is followed to get the target absolute path.
        /// </summary>
        private sealed class RealFilePath : IDisposable
        {

#if PLATFORM_UNIX
            /// <summary>
            /// Returns the canonicalized absolute pathname of given pathname.
            /// </summary>
            /// <param name="fname">The pathname to resolve.</param>
            /// <param name="buffer">The optional buffer to receive resolved pathname.</param>
            /// <returns>
            /// A pointer to unmanaged memory containing the resolved pathname or null if any error.
            /// </returns>
            [DllImport("libc", EntryPoint = "realpath", CharSet = CharSet.Ansi, SetLastError = true)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "ANSI for Linux")]
            public static extern IntPtr RealPath(string fname, string buffer);

            /// <summary>
            /// Free the unmanaged allocated memroy being pointed to by <paramref name="ptr"/>.
            /// </summary>
            /// <param name="ptr">The pointer to unmanaged memory..</param>
            [DllImport("libc", EntryPoint = "free", SetLastError = true)]
            public static extern void Free(IntPtr ptr);

            private IntPtr unmanagedFName = IntPtr.Zero;
#else
            public static string RealPath(string fname, string _)
                => fname;

            public static void Free(string _) { }

#endif

            private readonly string origPath;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="FileName">The pathname to the file.</param>
            public RealFilePath(string FileName)
                => origPath = FileName;

            /// <summary>
            /// Gets the absolute path of the original path removing relative path and symbolic links.
            /// </summary>
            public string GetPath
#if PLATFORM_UNIX
            {
                get
                {
                    if (unmanagedFName == IntPtr.Zero)
                    {
                        unmanagedFName = RealPath(origPath, null);
                    }
                    return Marshal.PtrToStringAnsi(unmanagedFName);
                }
            }
#else
                    => origPath;
#endif

            void IDisposable.Dispose()
            {
                cleanup();
                GC.SuppressFinalize(this);
            }

            ~RealFilePath() => cleanup();

            private void cleanup()
            {
#if PLATFORM_UNIX


                if (unmanagedFName != IntPtr.Zero)
                {
                    Free(unmanagedFName);
                    unmanagedFName = IntPtr.Zero;
                }
#endif
            }

        }

        /// <summary>
        /// Implementation for Linux.
        /// </summary>
        private sealed class MPLABLocationsUX : IMPLABLocations
        {
            private const string mplab_IDE_UX = @"/usr/bin/mplab_ide";

            public MPLABLocationsUX()
            {
                string fname = null;
                using (var p = new RealFilePath(mplab_IDE_UX))
                {
                    fname = p.GetPath;
                }
                if (string.IsNullOrWhiteSpace(fname))
                    return;

                // Expected real path on Linux is "<installdir>/vX.YY/mplab_platform/bin/mplab_ide".
                // We need to keep "vX.YY" as the version string and "<installdir>/vX.YY/packs/Microchip/" as the pathname of packs folder.
                // 
                fname = Path.GetDirectoryName(fname); // remove trailing "/mplab_ide"
                if (string.IsNullOrWhiteSpace(fname))
                    return;
                fname = Path.GetDirectoryName(fname); // remove trailing "/bin"
                if (string.IsNullOrWhiteSpace(fname))
                    return;
                fname = Path.GetDirectoryName(fname); // remove trailing "/mplab_platform"
                if (string.IsNullOrWhiteSpace(fname))
                    return;
                Version = Path.GetFileName(fname);
                SourceFolder = Path.Combine(fname, "packs", "Microchip");
                if (Directory.Exists(SourceFolder))
                {
                    IsValid = true;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the Linux installation information is valid.
            /// </summary>
            public bool IsValid { get; } = false;

            /// <summary>
            /// Gets the MPLAB X IDE version as a string for this Linux installation.
            /// </summary>
            public string Version { get; } = null;

            /// <summary>
            /// MPLAB X IDE installations on Linux always use Device Family Packs.
            /// </summary>
            public bool UsePacks => true;

            /// <summary>
            /// Gets the pathname to the Device Family Packs (DFP) folder for this Linux installation.
            /// </summary>
            public string SourceFolder { get; } = null;

        }

        /// <summary>
        /// Implementation for OS/X.
        /// </summary>
        private sealed class MPLABLocationsOSX : IMPLABLocations
        {
            private const string mplab_IDE_OSX = @"/Applications/microchip/mplabx";

            public MPLABLocationsOSX()
            {
                string fname = null;
                using (var p = new RealFilePath(mplab_IDE_OSX))
                {
                    fname = p.GetPath;
                }
                if (string.IsNullOrWhiteSpace(fname))
                    return;
                if (!Directory.Exists(fname))
                    return;

                // Expected real path on MacOS is "/Applications/microchip/mplabx" which is a folder containing a "vX.YY/" subfolder.
                // It is assumed that this is *THE* installation directory which contains one subfolder named after the MPLAB X IDE version number.

                var dirs = new List<string>(Directory.EnumerateDirectories(fname, "v*", SearchOption.TopDirectoryOnly).OrderByDescending(dirname => dirname));
                if (dirs.Count < 1)
                    return;
                var installDir = dirs[0];
                if (!Directory.Exists(installDir))
                    return;
                Version = Path.GetFileName(installDir);
                SourceFolder = Path.Combine(installDir, "packs", "Microchip");
                if (Directory.Exists(SourceFolder))
                    IsValid = true;
            }

            /// <summary>
            /// Gets a value indicating whether the OsX installation information is valid.
            /// </summary>
            public bool IsValid { get; } = false;

            /// <summary>
            /// Gets the MPLAB X IDE version as a string for this OsX installation.
            /// </summary>
            public string Version { get; } = null;

            /// <summary>
            /// MPLAB X IDE installations on OsX always use Device Family Packs.
            /// </summary>
            public bool UsePacks => true;

            /// <summary>
            /// Gets the pathname to the Device Family Packs (DFP) folder for this OsX installation.
            /// </summary>
            public string SourceFolder { get; } = null;

        }

        /// <summary>
        /// Implementation for Windows.
        /// </summary>
        private sealed class MPLABLocationsWIN : IMPLABLocations
        {

            private const string jarFolderPath = @"mplab_ide\mplablibs\modules\ext";
            private const string jarFilename = @"crownking.edc.jar";

            public MPLABLocationsWIN()
            {
                // Starts trying to get post-4.10 version installation information (with DFP).
                if (mplabXInstallationFolder is not null)
                {
                    Version = new DirectoryInfo(mplabXInstallationFolder).Name;
                    var fname = Path.GetDirectoryName(mplabXInstallationFolder); // remove trailing "/vX.YY/"
                    if (string.IsNullOrWhiteSpace(fname))
                        return;
                    SourceFolder = Path.Combine(fname, "packs", "Microchip");
                    if (Directory.Exists(SourceFolder))
                        IsValid = true;
                }
                // If not successful, trying to get information for older MPLAB X (version less then 4.10 - no DFP, but jar file).
                if (!IsValid && mplabXInstallationFolder is not null)
                {
                    UsePacks = false;
                    SourceFolder = Path.Combine(mplabXInstallationFolder, jarFolderPath, jarFilename);
                    if (File.Exists(SourceFolder))
                    {
                        IsValid = true;
                    }
                }

            }

            private string mplabXInstallationFolder
            {
                get
                {
                    if (mplabxInstallationFolder is null)
                    {
                        var MicrochipKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microchip");
                        if (MicrochipKey is null || MicrochipKey.SubKeyCount <= 0)
                            MicrochipKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microchip");
                        mplabxInstallationFolder = (string) (MicrochipKey?.OpenSubKey("MPLAB X")?.GetValue("InstallDir", null));
                    }
                    return mplabxInstallationFolder;
                }
            }
            private string mplabxInstallationFolder = null;

            /// <summary>
            /// Gets a value indicating whether the Windows installation information is valid.
            /// </summary>
            public bool IsValid { get; } = false;

            /// <summary>
            /// Gets the MPLAB X IDE version as a string for this Windows installation.
            /// </summary>
            public string Version { get; } = null;

            /// <summary>
            /// MPLAB X IDE installations on Windows use Device Family Packs (version &gt;= 4.10) or 'JAR' file for legacy MPLAB IDE.
            /// </summary>
            public bool UsePacks { get; } = true;

            /// <summary>
            /// Gets the pathname to the Device Family Packs (DFP) folder or the '.jar' file for this Windows installation.
            /// </summary>
            public string SourceFolder { get; } = null;

        }

        private sealed class DummyLocations : IMPLABLocations
        {
            /// <summary>
            /// Gets a value indicating whether the Windows installation information is valid.
            /// </summary>
            public bool IsValid => false;

            /// <summary>
            /// Gets the MPLAB X IDE version as a string for this Windows installation.
            /// </summary>
            public string Version => null;

            /// <summary>
            /// MPLAB X IDE installations on Windows use Device Family Packs (version &gt;= 4.10) or 'JAR' file for legacy MPLAB IDE.
            /// </summary>
            public bool UsePacks => false;

            /// <summary>
            /// Gets the pathname to the Device Family Packs (DFP) folder or the '.jar' file for this Windows installation.
            /// </summary>
            public string SourceFolder => null;

        }

        #endregion

        /// <summary>
        /// Gets the MPLAB X IDE locations information.
        /// </summary>
        private static IMPLABLocations dummyLoc { get; } = new DummyLocations();

        public static IMPLABLocations Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new MPLABLocationsWIN();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new MPLABLocationsUX();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new MPLABLocationsOSX();
            return dummyLoc;
        }

    }

}
