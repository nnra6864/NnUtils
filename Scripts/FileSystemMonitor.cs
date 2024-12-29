using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NnUtils.Scripts
{
    /// Wrapper for the <see cref="FileSystemWatcher"/> with qol changes
    public class FileSystemMonitor : IDisposable
    {
        private const float DefaultReloadDelay = 0.1f;
        private const NotifyFilters DefaultFilters = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
        
        /// Used to detect file changes
        public FileSystemWatcher Watcher { get; private set; }
        
        /// Full path to the directory to be watched
        public readonly string Directory;
        
        /// Name of a file located in the <see cref="Directory"/> to be watched
        public readonly string File;

        /// <see cref="Directory"/> and <see cref="File"/> combined
        public readonly string Path;

        /// Used to avoid multiple detections at once
        public readonly int ReloadDelay;
        
        /// Notify Filters
        public readonly NotifyFilters NotifyFilter;

        /// Gets executed when fs changes
        public event Action OnChanged;

        /// Used to execute OnChanged on the main thread to avoid weird behaviour
        private readonly SynchronizationContext _sync;

        /// <summary>
        /// Creates a new <see cref="FileSystemMonitor"/>
        /// </summary>
        /// <param name="directory">Full path to the directory you want to monitor</param>
        /// <param name="file">Full path to the file you want to monitor</param>
        /// <param name="onChanged">Gets executed when the change is detected</param>
        /// <param name="notifyFilter">Notify filters</param>
        /// <param name="reloadDelay">Delay before the change is acknowledged</param>
        public FileSystemMonitor(string directory, string file = "*.*", Action onChanged = null, float reloadDelay = DefaultReloadDelay, NotifyFilters notifyFilter = DefaultFilters)
        {
            // Store values
            Directory    = directory;
            File         = file;
            Path         = System.IO.Path.Combine(Directory, File);
            OnChanged    = onChanged;
            NotifyFilter = notifyFilter;
            ReloadDelay  = (int)(reloadDelay * 1000);

            // Get sync context
            _sync = SynchronizationContext.Current;
            
            // Initialize watcher
            InitializeWatcher();

            // Start watching or changes
            MonitorChanges();
        }

        /// <summary>
        /// Creates a new <see cref="FileSystemMonitor"/>
        /// </summary>
        /// <param name="path">Full path to the directory or file you want to monitor</param>
        /// <param name="onChanged">Gets executed when the change is detected</param>
        /// <param name="notifyFilter">Notify filters</param>
        /// <param name="reloadDelay">Delay before the change is acknowledged</param>
        public FileSystemMonitor(string path, Action onChanged = null, float reloadDelay = DefaultReloadDelay, NotifyFilters notifyFilter = DefaultFilters)
        : this(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path), onChanged, reloadDelay, notifyFilter) { }
        
        private void InitializeWatcher() => 
            Watcher = new()
            {
                Path = Directory,
                Filter = File,
                NotifyFilter = NotifyFilter
            };
        
        /// Sets up the FileSystemWatcher to monitor changes to the config file
        private void MonitorChanges()
        {
            Watcher.Created             += HandleChange;
            Watcher.Changed             += HandleChange;
            Watcher.Renamed             += HandleChange;
            Watcher.Deleted             += HandleChange;
            Watcher.EnableRaisingEvents =  true;
        }

        /// Used to cancel the invocation of <see cref="OnChanged"/> in case the config reloaded again
        private CancellationTokenSource _cancellationTokenSource;
        
        /// Executes <see cref="HandleChange()"/> on the main thread to avoid weird behaviour
        private void HandleChange(object sender, FileSystemEventArgs args) => _sync.Post(_ => HandleChange(), null);

        /// Handles the change being changed
        private void HandleChange()
        {
            // Cancel the previous task if it hasn't completed yet
            _cancellationTokenSource?.Cancel();

            // Assign a new cancellation token
            _cancellationTokenSource = new();

            // Trigger the delayed event
            ChangeTask(_cancellationTokenSource.Token).ConfigureAwait(false);
        }
        
        /// Waits for <see cref="ReloadDelay"/> seconds and triggers the <see cref="OnChanged"/> event
        private async Task ChangeTask(CancellationToken cancellationToken)
        {
            await Task.Delay(ReloadDelay, cancellationToken);
            if (!cancellationToken.IsCancellationRequested) OnChanged?.Invoke();
        }

        #region IDisposable
        
        private bool _disposed;
        
        public void Dispose()
        {
            // Return if already disposed
            if (_disposed) return;
            
            // Dispose components
            Watcher?.Dispose();
            _cancellationTokenSource?.Dispose();
            
            // Set _disposed to true
            _disposed = true;
        }
        
        #endregion
    }
}