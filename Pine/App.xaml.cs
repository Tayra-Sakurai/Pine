// SPDX-FileCopyrighText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Cedar.Contexts;
using Cedar.Interceptors;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pine
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Ioc.Default.ConfigureServices(ConfigureService());
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Ioc.Default.GetRequiredService<IDbContextFactory<CedarContext>>().CreateDbContext().Database.Migrate();

            _window = new MainWindow();
            _window.Activate();
        }

        private static IServiceProvider ConfigureService()
        {
            ServiceCollection services = new();

            services.AddSqliteVectorStore(_ => $"Data Source={Path.Join(Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path, "history.sqlite3")}");
            services.AddDbContextFactory<CedarContext>(
                optionsBuilder => optionsBuilder.UseSqlite(
                    $"Data Source={Path.Join(Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path, "Cedar.sqlite3")}").AddInterceptors(
                    new CedarContextInterceptor()));

            return services.BuildServiceProvider();
        }
    }
}
