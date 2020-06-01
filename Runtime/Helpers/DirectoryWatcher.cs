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

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            methodCalled = "OnChanged";

            if (!isCalled) affectedFiles.Clear();
            affectedFiles.Add(e.FullPath);
            isCalled = true;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            methodCalled = "OnCreated";

            if (!isCalled) affectedFiles.Clear();
            affectedFiles.Add(e.FullPath);
            isCalled = true;
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            methodCalled = "OnDeleted";

            if (!isCalled) affectedFiles.Clear();
            affectedFiles.Add(e.FullPath);
            isCalled = true;
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            methodCalled = "OnRenamed";

            if (!isCalled) affectedFiles.Clear();
            affectedFiles.Add(e.FullPath);
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
    }

    public class FSWParams
    {
        public string path, filter;
        public NotifyFilters notifiers;
        public bool includeSubfolders;

        public FSWParams(string path)
        {
            this.path = path;
            filter = "*.*";
            notifiers = NotifyFilters.CreationTime | NotifyFilters.DirectoryName
                | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                | NotifyFilters.Size;
            includeSubfolders = true;
        }

        public FSWParams(string p, string f, NotifyFilters nf, bool isf)
        {
            path = p;
            filter = f;
            notifiers = nf;
            includeSubfolders = isf;
        }
    }
}