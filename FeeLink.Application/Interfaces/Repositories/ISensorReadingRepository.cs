using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Readings.Common;
using FeeLink.Domain.Entities;

namespace FeeLink.Application.Interfaces.Repositories;

public interface ISensorReadingRepository : IRepository<SensorReading>
{
    Task<ListResult<ReadingResult>> ListAsync(
        string macAddress,
        DateTime? from = null,
        DateTime? to = null,
        Metric? metric = null,
        float? minValue = null,
        float? maxValue = null,
        int page = 1,
        int pageSize = 50,
        string? sortBy = null);

    Task<StatusReportResult> GetDailyStatusReportAsync(string macAddress, DateOnly from, DateOnly to);
    Task<StatusReportResult> GetDailyStatusReportAsyncDummy(string macAddress, DateOnly from, DateOnly to);
    
    Task<PatientActivitySummaryResult> GetPatientActivityCountAsync(Guid therapistId, DateOnly date);
    Task<PatientActivitySummaryResult> GetPatientActivityCountAsyncDummy(DateOnly date);
    Task<ListResult<MonthlyPatientActivityResult>> GetTherapistPatientActivitySummaryAsync(Guid therapistId, int month);
    Task<ListResult<MonthlyPatientActivityResult>> GetTherapistPatientActivitySummaryAsyncDummy(Guid therapistId, int month);
}