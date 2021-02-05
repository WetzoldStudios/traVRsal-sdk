using System;
using System.IO;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    public class DirectoryWatcher
    {
        public FileSystemWatcher watcher;
        public bool isCalled = false;
        public List<string> affectedFiles = new List<string>();
        public string methodCalled = "";
        public Dictionary<string, Action> matchedMethods = new Dictionary<string, Action>();

        public DirectoryWatcher(FSWParams props = null)
        {
            watcher = new FileSystemWatcher(props.path, props.filter);

            watcher.NotifyFilter = props.notifiers;
            watcher.IncludeSubdirectories = props.includeSubfolders;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }

        public void ClearAffected()
        {
            lock (affectedFiles) affectedFiles.Clear();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            methodCalled = "OnChanged";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            methodCalled = "OnCreated";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            methodCalled = "OnDeleted";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            methodCalled = "OnRenamed";

            lock (affectedFiles)
            {
                if (!isCalled) affectedFiles.Clear();
                affectedFiles.Add(e.FullPath);
            }
            isCalled = true;
        }

        public void StartFSW()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void CancelFSW()
        {
            watcher.EnableRaisingEvents = false;
        }

        public override string ToString()
        {
            return $"Directory Watcher ({watcher.Path})";
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