using Perforce.P4;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.CodeLens;
using Microsoft.VisualStudio.Language.CodeLens.Remoting;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace CodeLensOopProvider
{
    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(Id)]
    [ContentType("code")]
    [LocalizedName(typeof(Resources), "HelixSubmittedChangelistCodeLensProvider")]
    [Priority(200)]
    internal class HelixSubmittedChangelistDataPointProvider : IAsyncCodeLensDataPointProvider
    {
        internal const string Id = "HelixSubmittedChangelist";

        public Task<bool> CanCreateDataPointAsync(CodeLensDescriptor descriptor, CodeLensDescriptorContext context, CancellationToken token)
        {
            Debug.Assert(descriptor != null);
            var Repo = HelixUtility.GetRepository(descriptor.FilePath, out string repoRoot,
                out Changelist latest);
            return Task.FromResult<bool>(Repo != null && latest !=null);
        }

        public Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(CodeLensDescriptor descriptor, CodeLensDescriptorContext context, CancellationToken token)
        {
            return Task.FromResult<IAsyncCodeLensDataPoint>(new HelixSubmittedChangelistDataPoint(descriptor));
        }

        private class HelixSubmittedChangelistDataPoint : IAsyncCodeLensDataPoint
        {
            private readonly CodeLensDescriptor descriptor;
            private readonly Repository rep;
            private readonly string ws_root;

            public HelixSubmittedChangelistDataPoint(CodeLensDescriptor descriptor)
            {
                this.descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
                this.rep = HelixUtility.GetRepository(descriptor.FilePath, out this.ws_root,
                    out Changelist latest);
            }

            public event AsyncEventHandler InvalidatedAsync;

            public CodeLensDescriptor Descriptor => this.descriptor;

            public Task<CodeLensDataPointDescriptor> GetDataAsync(CodeLensDescriptorContext context, CancellationToken token)
            {
                try
                {
                    Changelist commit = HelixUtility.GetLastCommit(rep, descriptor.FilePath);
                    if (commit == null)
                    {
                        return Task.FromResult<CodeLensDataPointDescriptor>(null);
                    }
                    CodeLensDataPointDescriptor response = new CodeLensDataPointDescriptor()
                    {
                        Description = commit.OwnerName,//commit.Author.Name,
                        TooltipText = $"Last change committed by {commit.OwnerName} at {commit.ModifiedDate.ToString(CultureInfo.CurrentCulture)}",
                        IntValue = null,    // no int value
                        ImageId = GetCommitTypeIcon(commit),
                    };

                    return Task.FromResult(response);
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(
                        "file: " + descriptor.FilePath + "\r\n" +
                        "port: " + this.rep.Server.Address + "\r\n" +
                        "user: " + this.rep.Connection.UserName + "\r\n" +
                        "client: " +  this.rep.Connection.Client.Name + "\r\n" +
                        "Exception:" + "\r\n" +ex.ToString());
                    return Task.FromResult<CodeLensDataPointDescriptor>(null);
                }
            }

            public Task<CodeLensDetailsDescriptor> GetDetailsAsync(CodeLensDescriptorContext context, CancellationToken token)
            {
                // get the most recent 5 commits
                var commits = HelixUtility.GetCommits(this.rep, this.descriptor.FilePath, 5).AsEnumerable();
                if (commits == null || commits.Count() == 0)
                {
                    return Task.FromResult<CodeLensDetailsDescriptor>(null);
                }

                var headers = new List<CodeLensDetailHeaderDescriptor>()
                {
                    new CodeLensDetailHeaderDescriptor()
                    {
                        UniqueName = "CommitType",
                        Width = 22,
                    },
                    new CodeLensDetailHeaderDescriptor()
                    {
                        UniqueName = "Changelist",
                        DisplayName = "Changelist",
                        Width = 100, // fixed width
                    },
                    new CodeLensDetailHeaderDescriptor()
                    {
                        UniqueName = "CommitDescription",
                        DisplayName = "Description",
                        Width = 0.66666, // use 2/3 of the remaining width
                    },
                    new CodeLensDetailHeaderDescriptor()
                    {
                        UniqueName = "CommitAuthor",
                        DisplayName = "Author",
                        Width = 0.33333, // use 1/3 of the remaining width
                    },
                    new CodeLensDetailHeaderDescriptor()
                    {
                        UniqueName = "CommitDate",
                        DisplayName = "Date",
                        Width = 85, // fixed width
                    }
                };

                var entries = commits.Select(
                    commit => new CodeLensDetailEntryDescriptor()
                    {
                        Fields = new List<CodeLensDetailEntryField>()
                        {
                            new CodeLensDetailEntryField()
                            {
                                ImageId = GetCommitTypeIcon(commit),
                            },
                            new CodeLensDetailEntryField()
                            {
                                Text = commit.Id.ToString(),//commit.Id.Sha.Substring(0, 8),
                            },
                            new CodeLensDetailEntryField()
                            {
                                Text = commit.Description.Length > 50 ? 
                                commit.Description.Replace(System.Environment.NewLine," ").Substring(0,50)+"..." :
                                commit.Description.Replace(System.Environment.NewLine," "),//commit.MessageShort,
                            },
                            new CodeLensDetailEntryField()
                            {
                                Text = commit.OwnerName,//commit.Author.Name,
                            },
                            new CodeLensDetailEntryField()
                            {
                                Text = commit.ModifiedDate.ToString(@"MM\/dd\/yyyy", CultureInfo.CurrentCulture),//commit.Author.When.ToString(@"MM\/dd\/yyyy", CultureInfo.CurrentCulture),
                            },
                        },
                        Tooltip = commit.Description,//commit.Message,
                        NavigationCommand = new CodeLensDetailEntryCommand()
                        {
                            CommandSet = new Guid("f3cb9f10-281b-444f-a14e-de5de36177cd"),
                            CommandId = 0x0100,
                            CommandName = "Helix.NavigateToChangelist",
                        },
                        //NavigationCommandArgs = new List<object>() { commit.Id.Sha },
                        NavigationCommandArgs = new List<object>() { commit.Id.ToString() },

                    });

                var result = new CodeLensDetailsDescriptor();
                result.Headers = headers;
                result.Entries = entries;
                List<CodeLensDetailPaneCommand> PaneNavigationCommands = new List<CodeLensDetailPaneCommand>();

                CodeLensDetailPaneCommand historyCmd = new CodeLensDetailPaneCommand();
                historyCmd.CommandId = new CodeLensDetailEntryCommand();
                historyCmd.CommandId.CommandSet = new Guid("f3cb9f10-281b-444f-a14e-de5de36177cd");
                historyCmd.CommandId.CommandId = 0x1005;
                historyCmd.CommandId.CommandName = "Helix.ShowHistory";
                historyCmd.CommandDisplayName = "Show History";
                PaneNavigationCommands.Add(historyCmd);
                if (timelapseExists())
                {
                    CodeLensDetailPaneCommand timeLapseViewCmd = new CodeLensDetailPaneCommand();
                    timeLapseViewCmd.CommandId = new CodeLensDetailEntryCommand();
                    timeLapseViewCmd.CommandId.CommandSet = new Guid("f3cb9f10-281b-444f-a14e-de5de36177cd");
                    timeLapseViewCmd.CommandId.CommandId = 0x1010;
                    timeLapseViewCmd.CommandId.CommandName = "Helix.TimeLapseView";
                    timeLapseViewCmd.CommandDisplayName = "Time-lapse View";
                    timeLapseViewCmd.CommandArgs = new List<object>() { this.descriptor.FilePath };
                    PaneNavigationCommands.Add(timeLapseViewCmd);
                }

                result.PaneNavigationCommands = PaneNavigationCommands;
                return Task.FromResult(result);
            }

            /// <summary>
            /// Raises <see cref="IAsyncCodeLensDataPoint.Invalidated"/> event.
            /// </summary>
            /// <remarks>
            ///  This is not part of the IAsyncCodeLensDataPoint interface.
            ///  The data point source can call this method to notify the client proxy that data for this data point has changed.
            /// </remarks>
            public void Invalidate()
            {
                this.InvalidatedAsync?.Invoke(this, EventArgs.Empty).ConfigureAwait(false);
            }

            private static ImageId GetCommitTypeIcon(Changelist commit)
            {
                var helixCatalog = new Guid("{c030418b-0259-44d7-bbcf-4e8c03ef4d95}");
                int submitted = 0;
                return new ImageId(helixCatalog, submitted);
            }

            private bool timelapseExists()
            {
                try
                {
                    string installRoot = P4InstallLocation();
                    if (installRoot != null && System.IO.File.Exists(installRoot + "p4v.exe"))
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            private string P4InstallLocation()
            {
                const string x64subkey = "SOFTWARE\\WOW6432Node\\Perforce\\Environment";
                const string x86subkey = "SOFTWARE\\Perforce\\Environment";

                RegistryKey key;
                object item = null;

                // x64 local machine
                key = Registry.LocalMachine.OpenSubKey(x64subkey);
                if (key != null)
                {
                    item = key.GetValue("P4INSTROOT");
                    if (item != null) { return item.ToString(); }
                }

                // x64 local user
                key = Registry.CurrentUser.OpenSubKey(x64subkey);
                if (key != null)
                {
                    item = key.GetValue("P4INSTROOT");
                    if (item != null) { return item.ToString(); }
                }

                // x86 local machine
                key = Registry.LocalMachine.OpenSubKey(x86subkey);
                if (key != null)
                {
                    item = key.GetValue("P4INSTROOT");
                    if (item != null) { return item.ToString(); }
                }

                // x86 local user
                key = Registry.CurrentUser.OpenSubKey(x86subkey);
                if (key != null)
                {
                    item = key.GetValue("P4INSTROOT");
                    if (item != null) { return item.ToString(); }
                }

                return null;
            }
        }
    }
}
