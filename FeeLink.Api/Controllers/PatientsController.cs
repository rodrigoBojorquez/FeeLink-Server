using FeeLink.Api.Common.Controllers;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Commands.AssignTherapist;
using FeeLink.Application.UseCases.Patients.Commands.AssignTutor;
using FeeLink.Application.UseCases.Patients.Commands.Create;
using FeeLink.Application.UseCases.Patients.Commands.Update;
using FeeLink.Application.UseCases.Patients.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class PatientsController(IMediator mediator, IPatientRepository patientRepository) : ApiController
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

    [HttpPost("assign-therapists/{patientId:guid}")]
    public async Task<IActionResult> AssignTherapists(Guid patientId, [FromBody] List<Guid> therapistIds)
    {
        var command = new AssignTherapistCommand(patientId, therapistIds);
        var result = await mediator.Send(command);
        return result.Match(
            v => Ok(),
            Problem);
    }

    [HttpPost("assign-tutors/{patientId:guid}")]
    public async Task<IActionResult> AssignTutors(Guid patientId, [FromBody] List<Guid> tutorIds)
    {
        var command = new AssignTutorCommand(patientId, tutorIds);
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
    
}