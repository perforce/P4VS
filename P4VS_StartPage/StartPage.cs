using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.CodeContainerManagement;
using ICodeContainerProvider = Microsoft.VisualStudio.Shell.CodeContainerManagement.ICodeContainerProvider;
using CodeContainer = Microsoft.VisualStudio.Shell.CodeContainerManagement.CodeContainer;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using GelUtilities = Microsoft.Internal.VisualStudio.PlatformUI.Utilities;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using EnvDTE;
using EnvDTE80;
using EnvDTE90;
using EnvDTE100;
//using NLog;

namespace Perforce.P4VS_StartPage
{
    [ProvideBindingPath]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(PackageGuidString)]
    [ProvideCodeContainerProvider("Helix Container",
        PackageGuidString,
        ImagesGuidString,
        1,
        "#113",
        "#114",
        typeof(HelixContainerProvider))]

    public sealed class StartPagePackage : ExtensionPointPackage
    {
        public const string PackageGuidString = "524718f2-8d11-42ae-8f31-d21bf46992aa";

        public const string ImagesGuidString = "26bf1301-2eea-459a-b0c2-165099c6c313";

        static IServiceProvider serviceProvider;
        internal static IServiceProvider ServiceProvider { get { return serviceProvider; } }
        
        public StartPagePackage()
        {
            serviceProvider = this;
        }
    }

   
    public partial class HelixContainerProvider :  ICodeContainerProvider
    {
        bool ExecuteCommand(string command)
        {
            try
            {
                // Get an instance of the currently running Visual Studio IDE.
                DTE2 dte2;
                dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;

                // 2 checks to make sure the command exists in the IDE.
                Commands2 commands = (Commands2)(dte2.Commands);
                if (commands == null)
                    return false;

                Command dte_command = commands.Item(command, 0);
                if (dte_command == null)
                    return false;

                dte2.ExecuteCommand(command);
                return dte_command.IsAvailable;
            }
           catch (Exception ex)
            {
                if (ex.HResult != -2147467259)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
                return false;
            }
        }

        public Task<CodeContainer> AcquireCodeContainerAsync(IProgress<ServiceProgressData> downloadProgress, CancellationToken cancellationToken)
        {
            if(ExecuteCommand("File.Perforce.P4VS.Open_Solution_in_Perforce_Depot"))
            {
                downloadProgress.Report(new ServiceProgressData(string.Empty, string.Empty, 1, 1));
                return null;
            }
            else
            {
                try
                {
                    IVsRegisterScciProvider registerSccProvider =
                        ServiceProvider.GlobalProvider.GetService(typeof(IVsRegisterScciProvider)) as IVsRegisterScciProvider;
                    if (registerSccProvider == null)
                    {
                        return null;
                    }

                    // Try to activate the Scc provider
                    int hr = registerSccProvider.RegisterSourceControlProvider(GuidList.guidP4VsProvider);
                    if (ErrorHandler.Failed(hr))
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }



                if (ExecuteCommand("File.Perforce.P4VS.Open_Solution_in_Perforce_Depot"))
                {
                    downloadProgress.Report(new ServiceProgressData(string.Empty, string.Empty, 1, 1));
                    return null;
                }
            }

            System.Windows.Forms.MessageBox.Show(Resources.StartPagePluginRegError);
            
            return null;
        }

        public Task<CodeContainer> AcquireCodeContainerAsync(CodeContainer onlineCodeContainer, IProgress<ServiceProgressData> downloadProgress, CancellationToken cancellationToken)
        {
            // Implementing this interface member is required,
            // but it will never be used.
            return null;
        }

        public Task<CodeContainer> AcquireCodeContainerAsync(RemoteCodeContainer onlineCodeContainer, IProgress<ServiceProgressData> downloadProgress, CancellationToken cancellationToken)
        {
            // Implementing this interface member is required,
            // but it will never be used.
            return null;
        }

    }
}