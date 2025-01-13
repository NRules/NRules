using System.Collections.Generic;
using NRules.Diagnostics;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Controls the recording of rule invocations and provides access to the recorded invocations.
/// </summary>
public interface IRuleInvocationRecorder
{
    /// <summary>
    /// Gets all recorded invocations.
    /// </summary>
    IReadOnlyList<IMatch> GetInvocations();

    /// <summary>
    /// Clears all recorded invocations.
    /// </summary>
    void Clear();

    /// <summary>
    /// Pauses recording of rule invocations.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes recording of rule invocations.
    /// </summary>
    void Resume();
}

internal class RuleInvocationRecorder : IRuleInvocationRecorder
{
    private readonly List<IMatch> _invocations = new();
    private bool _paused = false;

    public RuleInvocationRecorder(ISession session)
    {
        session.Events.RuleFiredEvent += OnRuleFired;
    }

    public IReadOnlyList<IMatch> GetInvocations() => _invocations;

    public void Clear()
    {
        _invocations.Clear();
    }

    public void Pause()
    {
        _paused = true;
    }

    public void Resume()
    {
        _paused = false;
    }

    private void OnRuleFired(object sender, AgendaEventArgs e)
    {
        if (_paused) return;

        _invocations.Add(e.Match);
    }
}
