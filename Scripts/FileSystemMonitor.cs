using System;
using System.IO;

namespace NnUtils.Scripts
{
    /// Wrapper for the <see cref="FileSystemWatcher"/> with qol changes <br/>
    public class FileSystemMonitor : IDisposable
    {
        private const NotifyFilters DefaultFilters = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
        
        /// Used to detect file changes
        public FileSystemWatcher Watcher { get; private set; }
        
        /// Full path to the directory to be watched
        public readonly string Directory;
        
        /// Name of a file located in the <see cref="Directory"/> to be watched
        public readonly string File;

        /// <see cref="Directory"/> and <see cref="File"/> combined
        public readonly string Path;
        
        /// Notify Filters
        public readonly NotifyFilters NotifyFilter;

        /// Gets executed when fs changes
        public event Action OnChanged;

        /// <summary>
        /// Creates a new <see cref="FileSystemMonitor"/>
        /// </summary>
        /// <param name="directory">Full path to the directory you want to monitor</param>
        /// <param name="file">Full path to the file you want to monitor</param>
        /// <param name="onChanged">Gets executed when the change is detected</param>
        /// <param name="notifyFilter">Notify filters</param>
        public FileSystemMonitor(string directory, string file = "*.*", Action onChanged = null, NotifyFilters notifyFilter = DefaultFilters)
        {
            // Store values
            Directory    = directory;
            File         = file;
            Path         = System.IO.Path.Combine(Directory, File);
            OnChanged    = onChanged;
            NotifyFilter = notifyFilter;
            
            // Initializer and monitor
            InitializeWatcher();
            MonitorChanges();
        }

        /// <summary>
        /// Creates a new <see cref="FileSystemMonitor"/>
        /// </summary>
        /// <param name="path">Full path to the directory or file you want to monitor</param>
        /// <param name="onChanged">Gets executed when the change is detected</param>
        /// <param name="notifyFilter">Notify filters</param>
        public FileSystemMonitor(string path, Action onChanged = null, NotifyFilters notifyFilter = DefaultFilters)
        {
            // Store values
            Directory    = System.IO.Path.GetDirectoryName(path);
            File         = System.IO.File.Exists(path) ? System.IO.Path.GetFileName(path) : "*.*";
            Path         = path;
            OnChanged    = onChanged;
            NotifyFilter = notifyFilter;

            // Initializer and monitor
            InitializeWatcher();
            MonitorChanges();
        }
        
        private void InitializeWatcher()
        {
            Watcher = new()
            {
                Path         = Directory,
                Filter       = File,
                NotifyFilter = NotifyFilter
            };
            Watcher.IncludeSubdirectories = true;
        }

        /// Sets up the FileSystemWatcher to monitor changes to the config file
        private void MonitorChanges()
        {
            Watcher.Created             += HandleChange;
            Watcher.Changed             += HandleChange;
            Watcher.Renamed             += HandleChange;
            Watcher.Deleted             += HandleChange;
            Watcher.EnableRaisingEvents =  true;
        }

        /// Invokes the <see cref="OnChanged"/> event
        private void HandleChange(object sender, FileSystemEventArgs args)
        {
            if (args.Name.EndsWith(".tmp") || args.Name.StartsWith("~")) return;
            OnChanged?.Invoke();
        }

        #region IDisposable
        
        private bool _disposed;
        
        public void Dispose()
        {
            // Return if already disposed
            if (_disposed) return;
            
            // Dispose components
            Watcher?.Dispose();
            
            // Set _disposed to true
            _disposed = true;
        }
        
        #endregion
    }
}