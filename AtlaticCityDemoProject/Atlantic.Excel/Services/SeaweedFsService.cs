namespace Atlantic.Excel.Services
{
    public class SeaweedFsService : IStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public SeaweedFsService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var filerUrl = _config["SeaweedFS:FilerUrl"];
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var requestUrl = $"{filerUrl}/{fileName}";

            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);

            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error al subir el archivo a SeaweedFS");
            }

            return requestUrl;
        }
    }
}
