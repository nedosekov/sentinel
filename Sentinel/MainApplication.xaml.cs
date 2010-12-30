﻿#region License
//
// © Copyright Ray Hayes
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//
#endregion

#region Using directives

using System.Windows;
using Sentinel.Classification;
using Sentinel.Classification.Interfaces;
using Sentinel.Filters;
using Sentinel.Filters.Interfaces;
using Sentinel.Highlighters;
using Sentinel.Highlighters.Interfaces;
using Sentinel.Interfaces;
using Sentinel.Logger;
using Sentinel.Preferences;
using Sentinel.Properties;
using Sentinel.Services;
using Sentinel.Views.Interfaces;

#endregion

namespace Sentinel
{
    /// <summary>
    /// Interaction logic for MainApplication.xaml
    /// </summary>
    public partial class MainApplication : Application
    {
        /// <summary>
        /// Initializes a new instance of the MainApplication class.
        /// </summary>
        public MainApplication()
        {
            Settings.Default.Upgrade();

            ServiceLocator locator = ServiceLocator.Instance;
            locator.ReportErrors = true;

            locator.Load("settings.xml");
            locator.Register(typeof(IUserPreferences), typeof(UserPreferences), false);
            locator.Register(typeof(IHighlightingService), typeof(HighlightingService), false);
            locator.Register(typeof(IQuickHighlighter), typeof(QuickHighlighter), false);
            locator.Register<LogWriter>(new LogWriter());

            // Do this last so that other services have registered, e.g. the 
            // TypeImageService is called by some classifiers!
            if (!locator.IsRegistered<IClassifierService>())
            {
                locator.Register<IClassifierService>(new Classifiers());
            }

            // Request that the application close on main window close.
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        /// <summary>
        /// Override of the <c>Application.OnExit</c> method.
        /// </summary>
        /// <param name="e">Exit event arguments.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.Instance.Save("settings.xml");
            base.OnExit(e);
        }
    }
}