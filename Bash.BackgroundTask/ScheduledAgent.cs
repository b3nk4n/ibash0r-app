using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System;
using Bash.Common.Data;
using System.Threading.Tasks;
using Bash.Common;
using Bash.App.Helpers;

namespace Bash.BackgroundTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static IBashClient bashClient;

        /// <remarks>
        /// ScheduledAgent-Konstruktor, initialisiert den UnhandledException-Handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Handler für verwaltete Ausnahmen abonnieren
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });

            bashClient = new CachedBashClient(new BashClient());
        }

        /// Code, der bei nicht behandelten Ausnahmen ausgeführt wird
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Eine nicht behandelte Ausnahme ist aufgetreten. Unterbrechen und Debugger öffnen
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent zum Ausführen einer geplanten Aufgabe
        /// </summary>
        /// <param name="task">
        /// Die aufgerufene Aufgabe
        /// </param>
        /// <remarks>
        /// Diese Methode wird aufgerufen, wenn eine regelmäßige oder ressourcenintensive Aufgabe aufgerufen wird
        /// </remarks>
        protected async override void OnInvoke(ScheduledTask task)
        {
            await UpdateLockScreenAsync();

#if DEBUG
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

            NotifyComplete();
        }

        private async Task UpdateLockScreenAsync()
        {
            var data = await bashClient.GetQuotesAsync(AppConstants.ORDER_VALUE_RANDOM, AppConstants.QUOTES_COUNT, 0);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                BashLockscreenHelper.UpdateAsync(data);
            });
            return;
        }
    }
}