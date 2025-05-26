namespace FeeLink.Application.UseCases.Toys.Common;

public record ToyResult(Guid Id, Guid PatientId, string Name, string MacAddress);