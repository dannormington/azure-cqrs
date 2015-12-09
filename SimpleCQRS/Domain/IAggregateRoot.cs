using SimpleCQRS.Infrastructure;
using System;
using System.Collections.Generic;

/// <summary>
/// Interface to an aggregate root
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Aggregate Id
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Get all changes
    /// </summary>
    /// <returns></returns>
    IEnumerable<IEvent> GetUncommittedChanges();

    /// <summary>
    /// Mark the changes a committed
    /// </summary>
    void MarkChangesAsCommitted();

    /// <summary>
    /// Hydrate the aggregate
    /// </summary>
    /// <param name="history"></param>
    void LoadFromHistory(IEnumerable<IEvent> history);

    /// <summary>
    /// Get the current version of the aggregate
    /// </summary>
    int CurrentVersion { get; }
}