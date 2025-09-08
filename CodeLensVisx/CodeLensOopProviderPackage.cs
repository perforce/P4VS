using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using IServiceProvider = System.IServiceProvider;
using Task = System.Threading.Tasks.Task;
using Perforce.P4VS;
using Perforce.P4Scm;

namespace CodeLensOopProviderVsix
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidAndCmdID.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideBindingPath]
    public sealed class CodeLensOopProviderPackage : AsyncPackage, IOleCommandTarget
    {
        private IOleCommandTarget pkgCommandTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeLensOopProviderPackage"/> class.
        /// </summary>
        public CodeLensOopProviderPackage()
        {
            //System.Windows.Forms.MessageBox.Show("CodeLensOopProviderPackage");
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            //System.Windows.Forms.MessageBox.Show("InitializeAsync");

            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            this.pkgCommandTarget = await this.GetServiceAsync(typeof(IOleCommandTarget)) as IOleCommandTarget;

            if (cancellationToken.IsCancellationRequested)
                return;
        }

        #endregion

        #region IOleCommandTarget

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            //System.Windows.Forms.MessageBox.Show("IOleCommandTarget.QueryStatus");

            if (pguidCmdGroup == GuidAndCmdID.guidCmdSet)
            {
                switch (prgCmds[0].cmdID)
                {
                    case GuidAndCmdID.cmdidNavigateToChangelist:
                        prgCmds[0].cmdf |= (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_INVISIBLE);
                        return VSConstants.S_OK;
                    case GuidAndCmdID.cmdidShowHistory:
                        prgCmds[0].cmdf |= (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_INVISIBLE);
                        return VSConstants.S_OK;
                }
            }

            return this.pkgCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            //System.Windows.Forms.MessageBox.Show("IOleCommandTarget.Exec");

            if (pguidCmdGroup == GuidAndCmdID.guidCmdSet)
            {
                switch (nCmdID)
                {
                    case GuidAndCmdID.cmdidNavigateToChangelist:
                        if (IsQueryParameterList(pvaIn, pvaOut, nCmdexecopt))
                        {
                            Marshal.GetNativeVariantForObject("p", pvaOut);
                            return VSConstants.S_OK;
                        }
                        else
                        {
                            // no args
                            if (pvaIn == IntPtr.Zero)
                                return VSConstants.S_FALSE;

                            object vaInObject = Marshal.GetObjectForNativeVariant(pvaIn);
                            if (vaInObject == null || vaInObject.GetType() != typeof(string))
                                return VSConstants.E_INVALIDARG;

                            if ((vaInObject is string commitId) && !string.IsNullOrEmpty(commitId))
                            {
                                NavigateToCommit(commitId, this as IServiceProvider);
                            }
                        }
                        return VSConstants.S_OK;

                    case GuidAndCmdID.cmdidShowHistory:
                        if (IsQueryParameterList(pvaIn, pvaOut, nCmdexecopt))
                        {
                            Marshal.GetNativeVariantForObject("p", pvaOut);
                            return VSConstants.S_OK;
                        }
                        else
                        {
                            ShowHistory(this as IServiceProvider);
                        }
                        return VSConstants.S_OK;

                    case GuidAndCmdID.cmdidTimeLapseView:
                        if (IsQueryParameterList(pvaIn, pvaOut, nCmdexecopt))
                        {
                            Marshal.GetNativeVariantForObject("p", pvaOut);
                            return VSConstants.S_OK;
                        }
                        else
                        {
                            // no args
                            if (pvaIn == IntPtr.Zero)
                                return VSConstants.S_FALSE;

                            object vaInObject = Marshal.GetObjectForNativeVariant(pvaIn);
                            if (vaInObject == null || vaInObject.GetType() != typeof(string))
                                return VSConstants.E_INVALIDARG;

                            if ((vaInObject is string filePath) && !string.IsNullOrEmpty(filePath))
                            {
                                TimeLapseView(filePath, this as IServiceProvider);
                            }
                        }
                        return VSConstants.S_OK;
                }
            }

            return this.pkgCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        #endregion

        private static void NavigateToCommit(string commitId, IServiceProvider serviceProvider)
        {
            bool test = P4VsProvider.CurrentScm.CheckConnection();
            P4ScmProvider scm = P4VsProvider.CurrentScm;

            SubmittedChangelistDlg submitted = new SubmittedChangelistDlg(scm, true);
            int.TryParse(commitId, out int ID);
            submitted.ChangeListId = ID;

            Perforce.P4.ServerMetaData smd = scm.GetServerMetaData();
            submitted.Text = string.Format(Resources.SubmittedChangelistsToolWindowControl_SubmittedChangelistDlgCaption,
                                     commitId, smd.Address.Uri, scm.Connection.Repository.Connection.UserName);

            submitted.ShowDialog();
        }

        private static void ShowHistory(IServiceProvider serviceProvider)
        {
            P4ScmProvider scm = P4VsProvider.CurrentScm;
            if (scm != null)
            {
            scm.LaunchHistoryWindow();
        }
        }

        private static void TimeLapseView(string filePath, IServiceProvider serviceProvider)
        {
            P4ScmProvider scm = P4VsProvider.CurrentScm;
            if (scm != null)
            {
            scm.LaunchTimeLapseView(filePath);
        }
        }

        private static bool IsQueryParameterList(IntPtr pvaIn, IntPtr pvaOut, uint nCmdexecopt)
        {
            ushort lo = (ushort)(nCmdexecopt & (uint)0xffff);
            ushort hi = (ushort)(nCmdexecopt >> 16);
            if (lo == (ushort)OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP)
            {
                if (hi == VsMenus.VSCmdOptQueryParameterList)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
