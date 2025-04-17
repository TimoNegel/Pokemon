using System.Net.Http.Json;
using System.Text.Json;
using Backend.Models;

namespace Backend.Services
{
    public class PokemonService
    {
        private readonly HttpClient _httpClient;

        public PokemonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PokemonModel> GetPokemonDetailsAsyncById(int id)
        {
            string url = $"https://pokeapi.co/api/v2/pokemon/{id}";
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(url);
            var speciesResponse = await _httpClient.GetFromJsonAsync<JsonElement>(
                response.GetProperty("species").GetProperty("url").GetString()
            );
            return new PokemonModel
            {
                Name = response.GetProperty("name").GetString(),
                BildUrl = response.GetProperty("sprites").GetProperty("front_default").GetString(),
                Primärtyp = response
                    .GetProperty("types")[0]
                    .GetProperty("type")
                    .GetProperty("name")
                    .GetString(),
                Sekundärtyp =
                    response.GetProperty("types").GetArrayLength() > 1
                        ? response
                            .GetProperty("types")[1]
                            .GetProperty("type")
                            .GetProperty("name")
                            .GetString()
                        : string.Empty,
                Habitat = speciesResponse.GetProperty("habitat").GetProperty("name").GetString(),
                Hauptfähigkeit = response
                    .GetProperty("abilities")[0]
                    .GetProperty("ability")
                    .GetProperty("name")
                    .GetString(),
                VersteckteFähigkeit =
                    response.GetProperty("abilities").GetArrayLength() > 1
                        ? response
                            .GetProperty("abilities")[1]
                            .GetProperty("ability")
                            .GetProperty("name")
                            .GetString()
                        : string.Empty,
                Angriff = response.GetProperty("stats")[1].GetProperty("base_stat").GetInt32(),
                Verteidigung = response.GetProperty("stats")[2].GetProperty("base_stat").GetInt32(),
                KP = response.GetProperty("stats")[0].GetProperty("base_stat").GetInt32(),
                HP = response.GetProperty("stats")[3].GetProperty("base_stat").GetInt32(),
            };
        }

        public async Task<List<PokemonModel>> GetPokemonsDetailsAsync(int limit)
        {
            List<PokemonModel>? result = [];
            limit = (limit > 100000 || limit < 0) ? 100000 : limit;
            for (int id = 1; id <= limit; id++)
            {
                result.Add(await GetPokemonDetailsAsyncById(id));
            }
            return result;
        }
    }
}
