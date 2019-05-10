using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.TableManager;
using Steroids.Core.CodeQuality;

namespace SteroidsVS.CodeQuality.Diagnostic
{
    /// <summary>
    /// Provides diagnostic infos by crawling the Sources known to ITableManager.
    /// </summary>
    public class TableManagerDiagnosticsProvider : IDiagnosticProvider, ITableDataSink
    {
        private readonly List<ITableEntry> _knownTableEntries = new List<ITableEntry>();
        private readonly Dictionary<ITableEntriesSnapshotFactory, List<DiagnosticInfo>> _knownDiagnostics = new Dictionary<ITableEntriesSnapshotFactory, List<DiagnosticInfo>>();
        private readonly ITableManager _tableManager;
        private readonly ICollection<ITableDataSource> _subscriptions = new List<ITableDataSource>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableManagerDiagnosticsProvider"/> class.
        /// </summary>
        /// <param name="tableManagerProvider">The <see cref="ITableManagerProvider"/>.</param>
        public TableManagerDiagnosticsProvider(ITableManagerProvider tableManagerProvider)
        {
            _tableManager = tableManagerProvider.GetTableManager(StandardTables.ErrorsTable);
            _tableManager.SourcesChanged += DataSourcesChanged;
            SubscribeToNewSources();
        }

        /// <inheritdoc />
        public event EventHandler<DiagnosticsChangedEventArgs> DiagnosticsChanged;

        /// <inheritdoc />
        public IReadOnlyCollection<DiagnosticInfo> CurrentDiagnostics => _knownDiagnostics.SelectMany(x => x.Value).ToList();

        /// <inheritdoc />
        public bool IsStable { get; set; }

        /// <inheritdoc />
        public void AddEntries(IReadOnlyList<ITableEntry> newEntries, bool removeAllEntries = false)
        {
            if (removeAllEntries)
            {
                RemoveAllEntries();
            }

            _knownTableEntries.AddRange(newEntries);
            RaiseDiagnosticsChanged();
        }

        /// <inheritdoc />
        public void RemoveEntries(IReadOnlyList<ITableEntry> oldEntries)
        {
            foreach (var entry in oldEntries)
            {
                _knownTableEntries.Remove(entry);
            }

            RaiseDiagnosticsChanged();
        }

        /// <inheritdoc />
        public void ReplaceEntries(IReadOnlyList<ITableEntry> oldEntries, IReadOnlyList<ITableEntry> newEntries)
        {
            RemoveEntries(oldEntries);
            AddEntries(newEntries);
        }

        /// <inheritdoc />
        public void RemoveAllEntries()
        {
            _knownTableEntries.Clear();
            RaiseDiagnosticsChanged();
        }

        /// <inheritdoc />
        public void AddSnapshot(ITableEntriesSnapshot newSnapshot, bool removeAllSnapshots = false)
        {
            return;
        }

        /// <inheritdoc />
        public void RemoveSnapshot(ITableEntriesSnapshot oldSnapshot)
        {
            return;
        }

        /// <inheritdoc />
        public void RemoveAllSnapshots()
        {
            return;
        }

        /// <inheritdoc />
        public void ReplaceSnapshot(ITableEntriesSnapshot oldSnapshot, ITableEntriesSnapshot newSnapshot)
        {
            return;
        }

        /// <inheritdoc />
        public void AddFactory(ITableEntriesSnapshotFactory newFactory, bool removeAllFactories = false)
        {
            if (removeAllFactories)
            {
                _knownDiagnostics.Clear();
            }

            if (_knownDiagnostics.ContainsKey(newFactory))
            {
                return;
            }

            _knownDiagnostics.Add(newFactory, new List<DiagnosticInfo>());
        }

        /// <inheritdoc />
        public void RemoveFactory(ITableEntriesSnapshotFactory oldFactory)
        {
            if (!_knownDiagnostics.ContainsKey(oldFactory))
            {
                return;
            }

            _knownDiagnostics.Remove(oldFactory);
        }

        /// <inheritdoc />
        public void ReplaceFactory(ITableEntriesSnapshotFactory oldFactory, ITableEntriesSnapshotFactory newFactory)
        {
            return;
        }

        /// <inheritdoc />
        public void FactorySnapshotChanged(ITableEntriesSnapshotFactory factory)
        {
            List<ITableEntry> entries = CreateTabelEntriesFromSnapshot(factory);

            if (!_knownDiagnostics.ContainsKey(factory))
            {
                AddFactory(factory);
            }

            var diagnostics = entries.Select(x => x.ToDiagnosticInfo());
            var knownFactoryDiagnostics = _knownDiagnostics.FirstOrDefault(x => x.Key == factory).Value ?? new List<DiagnosticInfo>();
            MergeDiagnostics(knownFactoryDiagnostics, diagnostics);

            RaiseDiagnosticsChanged();
        }

        /// <inheritdoc />
        public void RemoveAllFactories()
        {
            _knownDiagnostics.Clear();
        }

        /// <summary>
        /// Merges the two lists of <see cref="DiagnosticInfo"/>.
        /// </summary>
        /// <param name="currentDiagnostics">The currently known <see cref="DiagnosticInfo"/>.</param>
        /// <param name="snapshotDiagnostics">The list to merge into the known diagnostics.</param>
        private static void MergeDiagnostics(List<DiagnosticInfo> currentDiagnostics, IEnumerable<DiagnosticInfo> snapshotDiagnostics)
        {
            // remove diagnostics which do not exist anymore
            foreach (var oldEntry in currentDiagnostics.Where(x => !snapshotDiagnostics.Contains(x)).ToList())
            {
                currentDiagnostics.Remove(oldEntry);
            }

            // add diagnostics which aren't known already
            foreach (var newEntry in snapshotDiagnostics.Where(x => !currentDiagnostics.Contains(x)).ToList())
            {
                currentDiagnostics.Add(newEntry);
            }
        }

        /// <summary>
        /// Creates aList of <see cref="ITableEntry"/> out of the <see cref="ITableEntriesSnapshotFactory"/> by getting the latest <see cref="ITableEntriesSnapshot"/>.
        /// </summary>
        /// <param name="factory">The <see cref="ITableEntriesSnapshotFactory"/>.</param>
        /// <returns>The created <see cref="List{ITableEntry}"/>.</returns>
        private static List<ITableEntry> CreateTabelEntriesFromSnapshot(ITableEntriesSnapshotFactory factory)
        {
            var snapshot = factory.GetCurrentSnapshot();
            var entries = new List<ITableEntry>();
            for (var i = 0; i < snapshot.Count; i++)
            {
                entries.Add(new SnapshotEntryWrapper(snapshot, i));
            }

            return entries;
        }

        /// <summary>
        /// Handler for <see cref="ITableManager.SourcesChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        private void DataSourcesChanged(object sender, EventArgs e)
        {
            SubscribeToNewSources();
        }

        /// <summary>
        /// Subscribe to all sources which aren't subscribed yet.
        /// </summary>
        private void SubscribeToNewSources()
        {
            foreach (var source in _tableManager.Sources.Where(x => !_subscriptions.Contains(x)).ToList())
            {
                var disposable = source.Subscribe(this);
                _subscriptions.Add(source);
            }
        }

        /// <summary>
        /// Raises the <see cref="DiagnosticsChanged"/> event.
        /// </summary>
        private void RaiseDiagnosticsChanged()
        {
            DiagnosticsChanged?.Invoke(this, new DiagnosticsChangedEventArgs(CurrentDiagnostics));
        }

        /// <summary>
        /// Wrapper for crawling the <see cref="ITableEntriesSnapshot"/> data by providing a single index and tread it like a <see cref="ITableEntry"/>.
        /// </summary>
        private class SnapshotEntryWrapper : ITableEntry
        {
            private readonly ITableEntriesSnapshot _snapshot;
            private readonly int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="SnapshotEntryWrapper"/> class.
            /// </summary>
            /// <param name="snapshot">The <see cref="ITableEntriesSnapshot"/>.</param>
            /// <param name="index">The index of the <see cref="ITableEntry"/> in the <see cref="ITableEntriesSnapshot"/>.</param>
            public SnapshotEntryWrapper(ITableEntriesSnapshot snapshot, int index)
            {
                _snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
                _index = index;
            }

            /// <inheritdoc />
            public object Identity => _snapshot;

            /// <inheritdoc />
            public bool CanSetValue(string keyName)
            {
                return false;
            }

            /// <inheritdoc />
            public bool TryGetValue(string keyName, out object content)
            {
                return _snapshot.TryGetValue(_index, keyName, out content);
            }

            /// <inheritdoc />
            public bool TrySetValue(string keyName, object content)
            {
                content = null;
                return false;
            }
        }
    }
}
