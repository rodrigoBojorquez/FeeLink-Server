using FeeLink.Api.Common.Controllers;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Commands.AssignTherapist;
using FeeLink.Application.UseCases.Patients.Commands.AssignTutor;
using FeeLink.Application.UseCases.Patients.Commands.Create;
using FeeLink.Application.UseCases.Patients.Commands.Update;
using FeeLink.Application.UseCases.Patients.Common;
using FeeLink.Application.UseCases.Patients.Queries.ListAvailable;
using FeeLink.Application.UseCases.Patients.Queries.Summary;
using FeeLink.Application.UseCases.Readings.Queries.ActivitySummary;
using FeeLink.Application.UseCases.Readings.Queries.MonthlyPatientActivity;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class PatientsController(
    IMediator mediator,
    IPatientRepository patientRepository,
    IAuthService authService,
    IToyRepository toyRepository,
    ISensorReadingRepository sensorReadingRepository)
    : ApiController
{
    public record CreatePatientRequest(
        string Name,
        string LastName,
        int Age,
        string Gender,
        float Height,
        float Weight,
        List<Guid>? TherapistIds = null,
        List<Guid>? TutorsIds = null);

    public record UpdatePatientRequest(
        Guid Id,
        string Name,
        string LastName,
        int Age,
        string Gender,
        float Height,
        float Weight);

    public record ListPatientsRequest(
        int Page = 1,
        int PageSize = 10,
        string? Search = null,
        Guid? TherapistId = null,
        Guid? TutorId = null);

    public record AssignTherapistsRequest(
        List<Guid> TherapistIds);

    public record AssignTutorsRequest(
        List<Guid> TutorIds);

    public record GetPatientActivitySummaryRequest(
        DateOnly Date,
        bool? Dummy = false);

    public record ListMonthlyPatientActivityRequest(
        int Month,
        bool? Dummy = false);
    
    public record ListAvailablePatientsRequest(
        int? Page,
        int? PageSize);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest req)
    {
        var command = new CreatePatientCommand(req.Name, req.LastName, req.Age, req.Gender, req.Height, req.Weight,
            req.TherapistIds, req.TutorsIds);
        var result = await mediator.Send(command);

        return result.Match(
            v => Ok(),
            Problem);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Find(Guid id)
    {
        var patient = await patientRepository.GetByIdAsync(id);

        return patient is null ? Problem(Errors.Patient.NotFound) : Ok(patient.ToResult());
    }

    [HttpGet("{id:guid}/toy")]
    public async Task<IActionResult> GetPatientByToyId(Guid id)
    {
        var toy = await toyRepository.GetByPatientIdAsync(id);
        return toy is null ? Problem(Errors.Patient.NotFound) : Ok(toy.ToResult());
    }

    [HttpGet("{id:guid}/summary")]
    public async Task<IActionResult> GetPatientSummary(Guid id, [FromQuery] DateOnly? date)
    {
        var query = new GetPatientSummaryQuery(id, date ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var result = await mediator.Send(query);
        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListPatientsRequest req)
    {
        var data = await patientRepository.ListAsync(
            page: req.Page,
            pageSize: req.PageSize,
            search: req.Search,
            therapistId: req.TherapistId,
            tutorId: req.TutorId);

        var result = new ListResult<PatientResult>(data.Items.Select(i => i.ToResult()), data.TotalItems, data.Page,
            data.PageSize, data.TotalPages);

        return Ok(result);
    }

    [HttpPost("{id:guid}/therapists")]
    public async Task<IActionResult> AssignTherapists(Guid id, [FromBody] AssignTherapistsRequest request)
    {
        var command = new AssignTherapistCommand(id, request.TherapistIds);
        var result = await mediator.Send(command);
        return result.Match(
            v => Ok(),
            Problem);
    }

    [HttpPost("{id:guid}/tutors")]
    public async Task<IActionResult> AssignTutors(Guid id, [FromBody] AssignTutorsRequest request)
    {
        var command = new AssignTutorCommand(id, request.TutorIds);
        var result = await mediator.Send(command);
        return result.Match(
            v => Ok(),
            Problem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequest req)
    {
        var command = new UpdatePatientCommand(id, req.Name, req.LastName, req.Age, req.Gender, req.Height, req.Weight);
        var result = await mediator.Send(command);

        return result.Match(
            v => Ok(),
            Problem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await patientRepository.HardDeleteAsync(id);

        return Ok();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetPatientActivitySummary([FromQuery] GetPatientActivitySummaryRequest request)
    {
        if (request.Dummy is true)
        {
            var dummyResult = await sensorReadingRepository.GetPatientActivityCountAsyncDummy(request.Date);
            return Ok(dummyResult);
        }
        
        var query = new GetPatientActivitySummaryQuery(request.Date);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("activity/summary")]
    public async Task<IActionResult> GetMonthlyPatientActivitySummary(
        [FromQuery] ListMonthlyPatientActivityRequest request)
    {
        if (request.Dummy is true)
        {
            var dummyResult = await sensorReadingRepository.GetTherapistPatientActivitySummaryAsyncDummy(
                authService.GetUserId().Value, request.Month);
            return Ok(dummyResult);
        }

        var therapistId = authService.GetUserId();
        if (therapistId.IsError)
            return Problem(therapistId.Errors);
        var query = new ListMonthlyPatientActivityQuery(therapistId.Value, request.Month);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("available")]
    public async Task<IActionResult> ListAvailable([FromQuery] ListPatientsRequest req)
    {
        var query = new ListAvailablePatientsQuery(
            Page: req.Page,
            PageSize: req.PageSize);
        var result = await mediator.Send(query);
        return result.Match(
            Ok,
            Problem);
    }

}