using System;
using System.IO;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    public class DirectoryWatcher
    {
        public bool isCalled;
        public List<string> affectedFiles = new List<string>();
        public Dictionary<string, Action> matchedMethods = new Dictionary<string, Action>();

        private FileSystemWatcher _watcher;
        private string _methodCalled = "";

        public DirectoryWatcher(FSWParams props = null)
        {
            _watcher = new FileSystemWatcher(props.path, props.filter);

            _watcher.NotifyFilter = props.notifiers;
            _watcher.IncludeSubdirectories = props.includeSubfolders;

            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Created += new FileSystemEventHandler(OnCreated);
            _watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }

        public void ClearAffected()
        {
            lock (affectedFiles) affectedFiles.Clear();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _methodCalled = "OnChanged";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            _methodCalled = "OnCreated";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            _methodCalled = "OnDeleted";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            _methodCalled = "OnRenamed";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        public void StartFSW()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void CancelFSW()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public override string ToString()
        {
            return $"Directory Watcher ({_watcher.Path})";
        }
    }

    public class FSWParams
    {
        public string path;
        public string filter;
        public NotifyFilters notifiers;
        public bool includeSubfolders;

        public FSWParams(string path, string filter = "*.*")
        {
            this.path = path;
            this.filter = filter;
            notifiers = NotifyFilters.CreationTime | NotifyFilters.DirectoryName
                                                   | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                   | NotifyFilters.Size;
            includeSubfolders = true;
        }

        public FSWParams(string path, string filter, NotifyFilters notifiers, bool includeSubfolders)
        {
            this.path = path;
            this.filter = filter;
            this.notifiers = notifiers;
            this.includeSubfolders = includeSubfolders;
        }
    }
}