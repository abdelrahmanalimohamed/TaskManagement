﻿namespace TaskManagement.Application.UnitOfWork;
public interface IUnitOfWork : IDisposable
{
	Task<int> CommitAsync(CancellationToken cancellationToken = default);
}