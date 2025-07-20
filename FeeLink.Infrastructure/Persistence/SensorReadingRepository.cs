using FeeLink.Application.Common.Extensions;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Common;
using FeeLink.Application.UseCases.Readings.Common;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Persistence;

public class SensorReadingRepository(FeeLinkDbContext context)
    : GenericRepository<SensorReading>(context), ISensorReadingRepository
{
    private readonly FeeLinkDbContext _context = context;
    private readonly Random _random = new Random();

    public async Task<ListResult<ReadingResult>> ListAsync(
        string? macAddress = null,
        DateTime? from = null,
        DateTime? to = null,
        Metric? metric = null,
        float? minValue = null,
        float? maxValue = null,
        int page = 1,
        int pageSize = 50,
        string? sortBy = null)
    {
        var query = _context.SensorReadings.AsQueryable();

        query = query.OrderByDescending(r => r.CreateDate);

        if (macAddress is not null)
            query = query.Where(r => r.Toy.MacAddress == macAddress);

        if (from is not null)
            query = query.Where(r => r.CreateDate >= from);

        if (to is not null)
            query = query.Where(r => r.CreateDate <= to);

        if (metric is not null)
            query = query.Where(r => r.Metric == metric);

        if (minValue is not null)
            query = query.Where(r => r.Value >= minValue);

        if (maxValue is not null)
            query = query.Where(r => r.Value <= maxValue);

        query = sortBy?.ToLower() switch
        {
            "value" => query.OrderByDescending(r => r.Value),
            "date" or _ => query.OrderByDescending(r => r.CreateDate)
        };

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReadingResult(r.Id, r.ToyId, r.CreateDate, r.Value, r.Metric.ToString()))
            .ToListAsync();

        return new ListResult<ReadingResult>(
            Items: items,
            TotalItems: totalItems,
            Page: page,
            PageSize: pageSize,
            TotalPages: totalItems.GetTotalPages(pageSize)
        );
    }

    public async Task<StatusReportResult> GetDailyStatusReportAsync(
        string macAddress,
        DateOnly from,
        DateOnly to)
    {
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt = to.ToDateTime(TimeOnly.MaxValue);

        var readings = await _context.SensorReadings
            .Where(r =>
                r.Toy.MacAddress == macAddress &&
                r.Metric == Metric.PressurePercent &&
                r.CreateDate >= fromDt &&
                r.CreateDate <= toDt)
            .Select(r => new { r.CreateDate, r.Value })
            .ToListAsync();

        var items = new List<DailyStatusItem>();
        int cEstable = 0, cAnsioso = 0, cCrisis = 0;

        for (var day = from; day <= to; day = day.AddDays(1))
        {
            var vals = readings
                .Where(r => DateOnly.FromDateTime(r.CreateDate) == day)
                .Select(r => r.Value)
                .ToList();

            string status;
            if (!vals.Any())
                status = "estable";
            else
            {
                var avg = vals.Average();
                status = avg >= 80f ? "crisis"
                    : avg >= 50f ? "ansioso"
                    : "estable";
            }

            items.Add(new DailyStatusItem(day, status));
            switch (status)
            {
                case "crisis":
                    cCrisis++;
                    break;
                case "ansioso":
                    cAnsioso++;
                    break;
                default:
                    cEstable++;
                    break;
            }
        }

        return new StatusReportResult(
            new StatusSummary(cEstable, cAnsioso, cCrisis),
            items,
            items.Count);
    }

    public Task<StatusReportResult> GetDailyStatusReportAsyncDummy(
        string macAddress,
        DateOnly from,
        DateOnly to)
    {
        var items = new List<DailyStatusItem>();
        int cEstable = 0, cAnsioso = 0, cCrisis = 0;

        for (var day = from; day <= to; day = day.AddDays(1))
        {
            // Genera entre 1 y 5 lecturas aleatorias de PressurePercent (0–100)
            var vals = Enumerable.Range(0, _random.Next(1, 6))
                .Select(_ => (float)(_random.NextDouble() * 100))
                .ToList();

            var avg = vals.Average();
            var status = avg >= 80f ? "crisis"
                : avg >= 50f ? "ansioso"
                : "estable";

            items.Add(new DailyStatusItem(day, status));
            switch (status)
            {
                case "crisis":
                    cCrisis++;
                    break;
                case "ansioso":
                    cAnsioso++;
                    break;
                default:
                    cEstable++;
                    break;
            }
        }

        var summary = new StatusSummary(cEstable, cAnsioso, cCrisis);
        var result = new StatusReportResult(summary, items, items.Count);
        return Task.FromResult(result);
    }

    public async Task<PatientActivitySummaryResult> GetPatientActivityCountAsync(DateOnly date)
    {
        // 1) Definir inicio y fin del día
        var start = date.ToDateTime(TimeOnly.MinValue);   // 00:00:00
        var end   = date.ToDateTime(TimeOnly.MaxValue);   // 23:59:59.999...

        // 2) Número total de pacientes
        var totalPatients = await _context.Patients
            .CountAsync();

        // 3) Número de pacientes con al menos una lectura de presión > 0.3
        var patientsWithActivity = await _context.SensorReadings
            .Where(r =>
                r.Metric == Metric.PressurePercent &&
                r.Value   > 0.3f &&
                r.CreateDate >= start &&
                r.CreateDate <= end)
            // Asumimos que Toy tiene la FK PatientId o bien navegación a Patient
            .Select(r => r.Toy.Patient.Id)   // o r.Toy.PatientId
            .Distinct()
            .CountAsync();

        // 4) Construir el resultado
        return new PatientActivitySummaryResult(
            PatientsWithActivity:     patientsWithActivity,
            PatientsWithoutActivity:  totalPatients - patientsWithActivity
        );
    }


    public Task<PatientActivitySummaryResult> GetPatientActivityCountAsyncDummy(DateOnly date)
    {
        // Genera un número aleatorio de pacientes con actividad (0–10)
        var patientsWithActivity = _random.Next(0, 11);
        var totalPatients = 20; // Total de pacientes fijos para el dummy

        return Task.FromResult(new PatientActivitySummaryResult(
            PatientsWithActivity: patientsWithActivity,
            PatientsWithoutActivity: totalPatients - patientsWithActivity
        ));
    }

    public async Task<ListResult<MonthlyPatientActivityResult>> GetTherapistPatientActivitySummaryAsync(
        Guid therapistId, int month)
    {
        var year = DateTime.UtcNow.Year;
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

        // pre‐filtrar lecturas por fecha y métrica
        var filteredReadings = _context.SensorReadings
            .Where(r =>
                r.Metric == Metric.PressurePercent &&
                r.CreateDate >= monthStart &&
                r.CreateDate <= monthEnd);

        // unir assignments → toys → lecturas
        var query = from ta in _context.TherapistAssignments
            where ta.UserId == therapistId
            join toy in _context.Toys on ta.PatientId equals toy.PatientId
            join r in filteredReadings on toy.Id equals r.ToyId into rg
            select new MonthlyPatientActivityResult(
                ta.PatientId,
                rg.Select(x => x.CreateDate.Date).Distinct().Count(),
                rg.Any()
                    ? rg
                        .GroupBy(x => x.CreateDate.Date)
                        .Select(g => g.Average(x => x.Value))
                        .Average() / 100.0
                    : 0.0
            );

        var items = await query.ToListAsync();

        return new ListResult<MonthlyPatientActivityResult>(
            Items: items,
            TotalItems: items.Count,
            Page: 1,
            PageSize: items.Count,
            TotalPages: 1
        );
    }

    public Task<ListResult<MonthlyPatientActivityResult>> GetTherapistPatientActivitySummaryAsyncDummy(Guid therapistId,
        int month)
    {
        // Genera datos aleatorios para el dummy
        var year = DateTime.UtcNow.Year;
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

        var random = new Random();
        var totalPatients = random.Next(5, 15); // Número aleatorio de pacientes

        var items = Enumerable.Range(1, totalPatients).Select(i => new MonthlyPatientActivityResult(
            Id: Guid.NewGuid(),
            DaysWithActivity: random.Next(0, 30), // Días con actividad aleatorios
            AverageStress: random.NextDouble() * 100 // Promedio de estrés aleatorio (0–100)
        )).ToList();

        return Task.FromResult(new ListResult<MonthlyPatientActivityResult>(
            Items: items,
            TotalItems: items.Count,
            Page: 1,
            PageSize: items.Count,
            TotalPages: 1
        ));
    }

    public async Task<PatientSummaryResult?> GetPatientSummaryAsync(Guid patientId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var nextDay = date.AddDays(1);
        
        var readings = await _context.SensorReadings
            .Where(r => r.Toy.PatientId == patientId &&
                        r.CreateDate >= date.ToDateTime(TimeOnly.MinValue) &&
                        r.CreateDate < nextDay.ToDateTime(TimeOnly.MinValue))
            .ToListAsync(cancellationToken: cancellationToken);

        if (!readings.Any())
            return null;
        
        var total = readings.Count;
        var stable = readings.Count(r => r.Value <= 0.60f);
        var anxious = readings.Count(r => r.Value > 0.60f && r.Value <= 0.87f);
        var crisis = readings.Count(r => r.Value > 0.87f);

        return new PatientSummaryResult(
            patientId,
            StablePercentage: Math.Round((decimal)stable / total * 100, 2),
            AnxiousPercentage: Math.Round((decimal)anxious / total * 100, 2),
            CrisisPercentage: Math.Round((decimal)crisis / total * 100, 2));
    }
}