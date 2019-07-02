using AddonManager.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.UI.Xaml;

namespace AddonManager
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private ExtendedExecutionSession _session;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            Debug.WriteLine("App constructor start");
            InitializeComponent();

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            Debug.WriteLine("App constructor end");
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Debug.WriteLine("App OnLaunched start");
            await Logic.Storage.LoadTask();
            if (!args.PrelaunchActivated)
            {
                Debug.WriteLine("App await ActivationService.ActivateAsync(args); start");
                await ActivationService.ActivateAsync(args);
                Debug.WriteLine("App await ActivationService.ActivateAsync(args); end");
            }
                                 
            if (_session == null)
                await PreventFromSuspending();
            Debug.WriteLine("App OnLaunched end");

        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            Debug.WriteLine("App OnActivated start");
            await ActivationService.ActivateAsync(args);
            Debug.WriteLine("App OnActivated end");
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MasterDetailPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        

        //
        // https://blogs.msdn.microsoft.com/mvpawardprogram/2018/01/30/non-suspending-uwp-desktop-apps/#comment-130715
        //
        private async Task PreventFromSuspending()
        {
            var newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.Unspecified;
            newSession.Revoked += SessionRevoked;

            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    _session = newSession;
                    break;
                default:
                case ExtendedExecutionResult.Denied:
                    newSession.Dispose();
                    break;
            }
            
        }

        private void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }
    }
}
