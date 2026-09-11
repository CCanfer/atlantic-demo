namespace Atlantic.Worker.Services
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

        public async Task<Stream> DownloadFileAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"No se pudo descargar el archivo desde SeaweedFS. Código: {response.StatusCode}");
            }

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
