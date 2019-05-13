using Addon.Helpers;
using Addon.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Addon.ViewModels
{
    public class SettingsViewModel : Observable
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            if (_hasInstanceBeenInitialized)
                            {
                                await ThemeSelectorService.SetThemeAsync(param);
                            }
                        });
                }

                return _switchThemeCommand;
            }
        }

        public SettingsViewModel()
        {
        }

        public void Initialize()
        {
            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private bool? _isAutoRefreshVersions;

        public bool? IsAutoRefreshVersions
        {
            get => _isAutoRefreshVersions ?? false;

            set
            {
                if (value != _isAutoRefreshVersions)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(IsAutoRefreshVersions), value ?? false));
                }

                Set(ref _isAutoRefreshVersions, value);
            }
        }

        private bool? _isDeleteOldFilesBeforeUpdate;

        public bool? IsDeleteOldFilesBeforeUpdate
        {
            get => _isDeleteOldFilesBeforeUpdate ?? false;

            set
            {
                if (value != _isDeleteOldFilesBeforeUpdate)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(IsDeleteOldFilesBeforeUpdate), value ?? false));
                }

                Set(ref _isDeleteOldFilesBeforeUpdate, value);
            }
        }

        private bool _hasInstanceBeenInitialized = false;

        public async Task EnsureInstanceInitializedAsync()
        {
            if (!_hasInstanceBeenInitialized)
            {
                IsAutoRefreshVersions = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(IsAutoRefreshVersions));
                IsDeleteOldFilesBeforeUpdate = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(IsDeleteOldFilesBeforeUpdate));
               
                Initialize();
                _hasInstanceBeenInitialized = true;
            }
        }
    }
}
