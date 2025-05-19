using ErrorOr;
using FeeLink.Application.Interfaces.Services;
using FeeLink.Domain.Common.Errors;
using Microsoft.AspNetCore.StaticFiles;

namespace FeeLink.Infrastructure.Services.Assets;

public class ImageService : IAssetsService
{
    private readonly string _rootPath;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;
    private readonly long _maxSize = 5 * 1024 * 1024; // 5MB

    public ImageService()
    {
        _rootPath = Path.Combine(AppContext.BaseDirectory, "assets/images");
        _contentTypeProvider = new FileExtensionContentTypeProvider();
    }

    public async Task<ErrorOr<string>> UploadAsync(string fileName, Stream stream, string? subfolder,
        CancellationToken? cancellationToken = null)
    {
        List<Error> errors = [];

        if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType) || !contentType.StartsWith("image/"))
            errors.Add(Errors.Asset.InvalidContentType);
        
        if (stream.Length > _maxSize)
            errors.Add(Errors.Asset.InvalidSize);
        
        if (errors.Any())
            return errors;
        
        var filePath = Path.Combine(_rootPath, subfolder ?? string.Empty, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream, cancellationToken ?? CancellationToken.None);
        
        // Ruta relativa
        var relativePath = Path.Combine(subfolder ?? string.Empty, fileName);
        return relativePath;
    }

    public Stream? Get(string fileName)
    {
        var filePath = Path.Combine(_rootPath, fileName);

        if (!File.Exists(filePath))
            return null;

        return File.OpenRead(filePath);
    }

    public void Delete(string fileName)
    {
        var filePath = Path.Combine(_rootPath, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}