using FeeLink.Domain.Entities;

namespace FeeLink.Application.UseCases.Readings.Common;

public record ReadingResult(Guid Id, Guid ToyId, DateTime CreateDate, float Value, Metric Metric);

public record DailyStatusItem(DateOnly Date, string Status);

public record StatusSummary(int Estable, int Ansioso, int Crisis);

public record StatusReportResult(
    StatusSummary Summary,
    List<DailyStatusItem> Items,
    int TotalItems);
    
    
public record PatientActivitySummaryResult(int PatientsWithActivity, int PatientsWithoutActivity);

public record MonthlyPatientActivityResult(Guid Id, int DaysWithActivity, double AverageStress);