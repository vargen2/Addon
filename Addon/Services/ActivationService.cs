using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Addon.Helpers;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Addon.Activation;
using Addon.Core.Helpers;
using Addon.Core.Models;

namespace Addon.Services
{
    // For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal class ActivationService
    {
        private readonly App _app;
        private readonly Lazy<UIElement> _shell;
        private readonly Type _defaultNavItem;

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            _app = app;
            _shell = shell;
            _defaultNavItem = defaultNavItem;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize things like registering background task before the app is loaded
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    Window.Current.Content = _shell?.Value ?? new Frame();
                }
            }

            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem);
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }

                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }

            Task.Run(async () =>
            {
                //await Controls.Storage.LoadTask();
                var storeAddons = await ReadFile();
                Singleton<Session>.Instance.StoreAddons = new ObservableCollection<StoreAddon>(storeAddons);
                Debug.WriteLine("Loaded StoreAddons. count=" + Singleton<Session>.Instance.StoreAddons.Count);
            });

        }

        private async Task InitializeAsync()
        {
            await Singleton<LiveTileService>.Instance.EnableQueueAsync();
            await ThemeSelectorService.InitializeAsync();
        }

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();
            Singleton<LiveTileService>.Instance.SampleUpdate();
            await Task.CompletedTask;
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<LiveTileService>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        private static async Task<IList<StoreAddon>> ReadFile()
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var sampleFile = await packageFolder.GetFileAsync(@"Assets\curseaddons.txt");
            var text = await FileIO.ReadTextAsync(sampleFile);
            // TODO fix time
            IList<CurseAddon> curseAddons = await Json.ToObjectAsync<List<CurseAddon>>(@text);
            return curseAddons
                .Select(ca => new StoreAddon(ca.addonURL, ca.title, ca.description, ca.downloads, DateTime.Now, DateTime.Now))
                .ToList();
        }
    }
}
