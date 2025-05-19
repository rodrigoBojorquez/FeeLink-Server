namespace FeeLink.Application.Common.Errors;

public interface IServiceException
{
    public int StatusCode { get; }
    public string ErrorMessage { get; }
}