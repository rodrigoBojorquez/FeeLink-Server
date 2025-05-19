namespace FeeLink.Application.Common.Files;

public interface IFile
{
    string ContentType { get; }
    long Length { get; }
    string FileName { get; }
    string Name { get; }
    Stream OpenReadStream();
}