using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Diagnostics;

namespace Perforce.P4VS.InfoBar
{
    internal class InfoBarManager : IVsInfoBarUIEvents
    {
        private readonly IVsInfoBarUIFactory _factory;
        private readonly IVsInfoBarHost _host;
        private readonly SettingsHandler _settings;
        private IVsInfoBarUIElement _infoBar;

        private const string InfoMessage =
            "P4VS Tip: Use recommended Data Retrieval options based on solution size for better performance.";

        private const string LearnMoreUrl =
            "https://help.perforce.com/helix-core/integrations-plugins/p4vs/current/Content/P4VS/intro.preferences.data.html";

        public InfoBarManager(IVsInfoBarUIFactory factory, IVsInfoBarHost host)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _settings = new SettingsHandler();
        }

        /// <summary>
        /// Show the InfoBar if user settings allow it.
        /// </summary>
        public void ShowInfoBar()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!_settings.ShouldShowInfoBar)
                return;

            var model = CreateInfoBarModel(InfoMessage);
            _infoBar = _factory.CreateInfoBar(model);
            _infoBar.Advise(this, out _);
            _host.AddInfoBar(_infoBar);
        }

        private static InfoBarModel CreateInfoBarModel(string message)
        {
            return new InfoBarModel(
                new[] { new InfoBarTextSpan(message) },
                new IVsInfoBarActionItem[]
                {
                    new InfoBarHyperlink("Learn more"),
                    new InfoBarHyperlink("Change Setting"),
                    new InfoBarButton("Dismiss"),
                    new InfoBarButton("Don't show this again")
                },
                KnownMonikers.StatusInformation,
                isCloseButtonVisible: true);
        }

        /// <summary>
        /// Handles actions from the InfoBar.
        /// </summary>
        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUI, IVsInfoBarActionItem actionItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            switch (actionItem.Text)
            {
                case "Dismiss":
                    _infoBar?.Close();
                    break;

                case "Learn more":
                    LaunchUrl(LearnMoreUrl);
                    break;

                case "Change Setting":
                    LaunchP4DataRetrievalOptionsDialog();
                    break; 

                case "Don't show this again":
                    _settings?.DisableInfoBar();
                    _infoBar?.Close();
                    break;
            }
        }

        private void LaunchP4DataRetrievalOptionsDialog()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Open P4-Data Retrievl options page
            VsShellUtilities.ShowToolsOptionsPage(new Guid("93F744C8-B872-4541-A650-B57A806F2624"));
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
            
        }

        private static void LaunchUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to launch URL: {ex}");
            }
        }

        /// <summary>
        /// Handles storage and retrieval of user settings for the InfoBar.
        /// </summary>
        private class SettingsHandler
        {
            private const string CollectionPath = "P4VS\\InfoBar";
            private const string PropertyName = "ShowP4VSInfoBar";

            private readonly WritableSettingsStore _store;

            public SettingsHandler()
            {
                var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
                _store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

                EnsureDefaults();
            }

            public bool ShouldShowInfoBar
            {
                get
                {
                    return _store.PropertyExists(CollectionPath, PropertyName) &&
                           _store.GetBoolean(CollectionPath, PropertyName);
                }
            }

            public void DisableInfoBar()
            {
                _store.SetBoolean(CollectionPath, PropertyName, false);
            }

            private void EnsureDefaults()
            {
                if (!_store.CollectionExists(CollectionPath))
                    _store.CreateCollection(CollectionPath);

                if (!_store.PropertyExists(CollectionPath, PropertyName))
                    _store.SetBoolean(CollectionPath, PropertyName, true);
            }
        }
    }
}
