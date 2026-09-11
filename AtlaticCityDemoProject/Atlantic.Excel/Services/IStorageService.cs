namespace Atlantic.Excel.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
