using System.Net.Http.Json;
using System.Text.Json;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.Services
{
    public class PokemonService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _dbContext;

        public PokemonService(
            HttpClient httpClient,
            IMemoryCache cache,
            ApplicationDbContext dbContext
        )
        {
            _httpClient = httpClient;
            _cache = cache;
            _dbContext = dbContext;
            _dbContext = dbContext;
        }

        public async Task<PokemonModel> GetPokemonDetailsAsyncById(int id)
        {
            string cacheKey = $"Pokemon_{id}";

            if (!_cache.TryGetValue(cacheKey, out PokemonModel pokemon))
            {
                pokemon = await _dbContext.PokemonsCache?.FirstOrDefaultAsync(x =>
                    x.Id == id.ToString().PadLeft(4, '0')
                );

                if (pokemon == null)
                {
                    string url = $"https://pokeapi.co/api/v2/pokemon/{id}";
                    var response = await _httpClient.GetFromJsonAsync<JsonElement>(url);
                    var speciesResponse = await _httpClient.GetFromJsonAsync<JsonElement>(
                        response.GetProperty("species").GetProperty("url").GetString()
                    );

                    pokemon = new PokemonModel
                    {
                        Id = response.GetProperty("id").GetInt32().ToString().PadLeft(4, '0'),
                        Name =
                            response.GetProperty("name").GetString()?.CapitalizeFirstLetter()
                            ?? string.Empty,
                        BildUrl =
                            response.GetProperty("sprites").GetProperty("front_default").GetString()
                            ?? string.Empty,
                        Primärtyp =
                            response
                                .GetProperty("types")[0]
                                .GetProperty("type")
                                .GetProperty("name")
                                .GetString()
                                ?.CapitalizeFirstLetter() ?? string.Empty,
                        Sekundärtyp =
                            response.GetProperty("types").GetArrayLength() > 1
                                ? response
                                    .GetProperty("types")[1]
                                    .GetProperty("type")
                                    .GetProperty("name")
                                    .GetString()
                                    ?.CapitalizeFirstLetter() ?? string.Empty
                                : string.Empty,
                        Habitat =
                            speciesResponse
                                .GetProperty("habitat")
                                .GetProperty("name")
                                .GetString()
                                ?.CapitalizeFirstLetter() ?? string.Empty,
                        Hauptfähigkeit =
                            response
                                .GetProperty("abilities")[0]
                                .GetProperty("ability")
                                .GetProperty("name")
                                .GetString()
                                ?.CapitalizeFirstLetter() ?? string.Empty,
                        VersteckteFähigkeit =
                            response.GetProperty("abilities").GetArrayLength() > 1
                                ? response
                                    .GetProperty("abilities")[1]
                                    .GetProperty("ability")
                                    .GetProperty("name")
                                    .GetString()
                                    ?.CapitalizeFirstLetter() ?? string.Empty
                                : string.Empty,
                        Angriff = response
                            .GetProperty("stats")[1]
                            .GetProperty("base_stat")
                            .GetInt32(),
                        Verteidigung = response
                            .GetProperty("stats")[2]
                            .GetProperty("base_stat")
                            .GetInt32(),
                        KP = response.GetProperty("stats")[0].GetProperty("base_stat").GetInt32(),
                        HP = response.GetProperty("stats")[3].GetProperty("base_stat").GetInt32(),
                    };
                    _dbContext.PokemonsCache.Add(pokemon);
                    await _dbContext.SaveChangesAsync();
                }

                _cache.Set(cacheKey, pokemon, TimeSpan.FromHours(3));
            }

            return pokemon;
        }

        public async IAsyncEnumerable<PokemonModel> GetPokemonsDetailsAsync(int limit)
        {
            limit = (limit > 100000 || limit < 0) ? 100000 : limit;
            for (int id = 1; id <= limit; id++)
            {
                yield return await GetPokemonDetailsAsyncById(id);
            }
        }
    }
}
