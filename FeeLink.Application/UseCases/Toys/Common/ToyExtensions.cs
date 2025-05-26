using FeeLink.Domain.Entities;

namespace FeeLink.Application.UseCases.Toys.Common;

public static class ToyExtensions
{
    public static ToyResult ToResult(this Toy toy)
    {
        return new ToyResult(
            Id: toy.Id,
            Name: toy.Name ?? string.Empty,
            PatientId: toy.PatientId,
            MacAddress: toy.MacAddress);
    }
}