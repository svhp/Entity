using Play.Inventory.Service.Dtos;

namespace Plau.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<CatalogClient> logger;
        private readonly IConfiguration configuration;

        public CatalogClient(HttpClient httpClient, ILogger<CatalogClient> logger, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemAsync()
        {
            var response = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
            return response;
        }
    }
}