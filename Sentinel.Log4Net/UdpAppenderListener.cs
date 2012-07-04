﻿namespace Sentinel.Log4Net
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Logging;

    using Sentinel.Interfaces;
    using Sentinel.Interfaces.Providers;

    public class UdpAppenderListener : INetworkProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static readonly IProviderRegistrationRecord ProviderRegistrationInformation =
            new ProviderRegistrationInformation(new Log4NetUdpListenerProvider());

        private readonly IUdpAppenderListenerSettings udpSettings;

        private CancellationTokenSource cancellationTokenSource;

        private Task udpListenerTask;

        public UdpAppenderListener(IProviderSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            udpSettings = settings as IUdpAppenderListenerSettings;
            if (udpSettings == null)
            {
                throw new ArgumentException("settings should be assignable to IUdpAppenderListenerSettings", "settings");
            }

            Information = ProviderRegistrationInformation.Info;
        }

        public IProviderInfo Information
        {
            get;
            private set;
        }

        public ILogger Logger { get; set; }

        public string Name { get; set; }

        public bool IsActive
        {
            get
            {
                return udpListenerTask != null && udpListenerTask.Status == TaskStatus.Running;
            }
        }

        public int Port { get; private set; }

        public void Start()
        {
            Log.Debug("Start requested");

            if ( udpListenerTask == null || udpListenerTask.IsCompleted )
            {
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;

                udpListenerTask = Task.Factory.StartNew(Worker, token);
            }
            else
            {
                Log.Warn("UDP listener task is already active and can not be started again.");
            }
        }

        public void Pause()
        {
            Log.Debug("Pause requested");
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                Log.Debug("Cancellation token triggered");
                cancellationTokenSource.Cancel();
            }
        }

        public void Close()
        {
            Log.Debug("Close requested");
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                Log.Debug("Cancellation token triggered");
                cancellationTokenSource.Cancel();
            }
        }

        private void Worker()
        {
            Log.Debug("Worker started");

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                Log.Debug("Ping...");
                Thread.Sleep(1000);
            }

            Log.Debug("Worker completed");
        }
    }
}
