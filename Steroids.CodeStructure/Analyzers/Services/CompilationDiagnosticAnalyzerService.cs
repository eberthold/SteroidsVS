namespace Steroids.CodeStructure.Analyzers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Steroids.Common.Helpers;
    using Steroids.Contracts;
    using DTE = EnvDTE;
    using Shell = Microsoft.VisualStudio.Shell;

    public class CompilationDiagnosticAnalyzerService : IDiagnosticAnalyzerService
    {
        private readonly Dictionary<ProjectId, CompilationWithAnalyzers> _projectCompilations = new Dictionary<ProjectId, CompilationWithAnalyzers>();
        private readonly Dictionary<ProjectId, IEnumerable<Diagnostic>> _projectDiagnostics = new Dictionary<ProjectId, IEnumerable<Diagnostic>>();
        private readonly Debouncer _compilationDebouncer;
        private readonly IWorkspaceManager _workspaceManager;
        private readonly DTE.DocumentEvents _docEvents;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public CompilationDiagnosticAnalyzerService(IWorkspaceManager workspaceManager)
        {
            _workspaceManager = workspaceManager ?? throw new ArgumentNullException(nameof(workspaceManager));

            _compilationDebouncer = new Debouncer(DoCompilation, TimeSpan.FromSeconds(0.5));
            _compilationDebouncer.Start();

            var dte = (DTE.DTE)Shell.Package.GetGlobalService(typeof(DTE.DTE));
            _docEvents = dte.Events.DocumentEvents;
            _docEvents.DocumentSaved += OnDocumentSaved;
        }

        public event EventHandler CompilationFinished;

        /// <inheritdoc />
        public void DoCompilation()
        {
            var token = CreateNewToken();

            var tasks = new List<Task>();
            foreach (var project in _workspaceManager.VsWorkspace.CurrentSolution.Projects)
            {
                if (!_projectCompilations.ContainsKey(project.Id))
                {
                    _projectCompilations.Add(project.Id, null);
                    _projectDiagnostics.Add(project.Id, new List<Diagnostic>());
                }

                tasks.Add(Task.Run(async () => await UpdateProjectCache(project, token).ConfigureAwait(false)));
            }

            Task.WhenAll(tasks)
                .ContinueWith(t => RaiseCompilationFinished(), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public CompilationWithAnalyzers GetProjectCompilationWithAnalyzers(Project project)
        {
            if (!_projectCompilations.ContainsKey(project.Id) || _projectCompilations[project.Id] == null)
            {
                _compilationDebouncer.Start();
                return null;
            }

            return _projectCompilations[project.Id];
        }

        public IEnumerable<Diagnostic> GetProjectDiagnostics(Project project)
        {
            if (!_projectDiagnostics.ContainsKey(project.Id))
            {
                return new List<Diagnostic>();
            }

            if (_projectDiagnostics[project.Id] == null)
            {
                _compilationDebouncer.Start();
                return new List<Diagnostic>();
            }

            return _projectDiagnostics[project.Id];
        }

        private void OnDocumentSaved(DTE.Document document)
        {
            _compilationDebouncer.Start();
        }

        private void RaiseCompilationFinished()
        {
            CompilationFinished?.Invoke(this, EventArgs.Empty);
        }

        private CancellationToken CreateNewToken()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            return _cts.Token;
        }

        private async Task UpdateProjectCache(Project project, CancellationToken token)
        {
            // we want to remain responsive
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            var compilationWithAnalyzers = await GetProjectCompilationWithAnalyzers(project, token);
            if (compilationWithAnalyzers == null)
            {
                return;
            }

            _projectCompilations[project.Id] = compilationWithAnalyzers;
            _projectDiagnostics[project.Id] = await _projectCompilations[project.Id].GetAllDiagnosticsAsync(token);

            // reset thread priority to normal
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
        }

        private async Task<CompilationWithAnalyzers> GetProjectCompilationWithAnalyzers(Project project, CancellationToken token)
        {
            // as far as I know, the GetCompilationAsync is incremental, whereas the analysis is not
            // so we check if the compilation has changed, to only analyze when changes are present
            var compilation = await project.GetCompilationAsync(token);
            if (ProjectIsAnalyzed(project, compilation))
            {
                return _projectCompilations[project.Id];
            }

            var analyzerList = project.AnalyzerReferences.SelectMany(x => x.GetAnalyzers(project.Language));
            if (!analyzerList.Any())
            {
                return null;
            }

            var analyzers = ImmutableArray<DiagnosticAnalyzer>.Empty.AddRange(analyzerList);
            return compilation.WithAnalyzers(analyzers, project.AnalyzerOptions, token);
        }

        private bool ProjectIsAnalyzed(Project project, Compilation compilation)
        {
            if (!_projectCompilations.ContainsKey(project.Id))
            {
                return false;
            }

            if (_projectCompilations[project.Id] == null)
            {
                return false;
            }

            if (_projectCompilations[project.Id].Compilation != compilation)
            {
                return false;
            }

            return true;
        }
    }
}
