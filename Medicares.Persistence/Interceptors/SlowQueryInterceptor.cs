using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Medicares.Persistence.Interceptors;

public class SlowQueryInterceptor : DbCommandInterceptor
{
    private readonly ILogger<SlowQueryInterceptor> _logger;
    private readonly TimeSpan _threshold;

    public SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger, TimeSpan threshold)
    {
        _logger = logger;
        _threshold = threshold;
    }

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        if (eventData.Duration > _threshold)
        {
            LogSlowQuery(command, eventData.Duration);
        }

        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Duration > _threshold)
        {
            LogSlowQuery(command, eventData.Duration);
        }

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void LogSlowQuery(DbCommand command, TimeSpan duration)
    {
        _logger.LogWarning(
            "Slow query ({Duration}ms): {CommandText}",
            duration.TotalMilliseconds,
            command.CommandText);
    }
}
