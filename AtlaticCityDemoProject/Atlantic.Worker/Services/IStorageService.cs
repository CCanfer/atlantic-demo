namespace Atlantic.Worker.Services
{
    public interface IStorageService
    {
        Task<Stream> DownloadFileAsync(string url);
    }
}
