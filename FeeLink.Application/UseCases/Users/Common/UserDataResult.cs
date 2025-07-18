namespace FeeLink.Application.UseCases.Users.Common;

public record UserDataResult(
    Guid Id, 
    string CompanyName, 
    string CompanyAddress, 
    string? TherapistName = null, 
    string? PatientName = null, 
    int? PatientAge = null,
    Guid? PatientId = null);