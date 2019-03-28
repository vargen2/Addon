using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Addon.Helpers;
using Addon.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Addon.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
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


        private bool? _isAutoSaveWTFEnabled;

        public bool? IsAutoSaveWTFEnabled
        {
            get => _isAutoSaveWTFEnabled ?? false;

            set
            {
                if (value != _isAutoSaveWTFEnabled)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(IsAutoSaveWTFEnabled), value ?? false));
                }

                Set(ref _isAutoSaveWTFEnabled, value);
            }
        }

        private bool _hasInstanceBeenInitialized = false;

        public async Task EnsureInstanceInitializedAsync()
        {
            if (!_hasInstanceBeenInitialized)
            {
                IsAutoSaveWTFEnabled =
                await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(IsAutoSaveWTFEnabled));

                Initialize();

                _hasInstanceBeenInitialized = true;
            }
        }
    }
}
