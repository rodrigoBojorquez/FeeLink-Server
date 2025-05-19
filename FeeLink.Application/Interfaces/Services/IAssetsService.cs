using ErrorOr;

namespace FeeLink.Application.Interfaces.Services;

public interface IAssetsService
{
    Task<ErrorOr<string>> UploadAsync(string fileName, Stream stream, string? subfolder, CancellationToken? cancellationToken);
    Stream? Get(string fileName);
    void Delete(string fileName);
}