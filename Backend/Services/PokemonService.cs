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
                pokemon = await _dbContext
                    .PokemonsCache.Include(p => p.Moves)
                    .FirstOrDefaultAsync(x => x.Id == id.ToString().PadLeft(4, '0'));

                if (pokemon == null)
                {
                    string url = $"https://pokeapi.co/api/v2/pokemon/{id}";
                    var response = await _httpClient.GetFromJsonAsync<JsonElement>(url);
                    var speciesResponse = await _httpClient.GetFromJsonAsync<JsonElement>(
                        response.GetProperty("species").GetProperty("url").GetString()
                    );

                    var generationResponse = await _httpClient.GetFromJsonAsync<JsonElement>(
                        speciesResponse.GetProperty("generation").GetProperty("url").GetString()
                    );

                    var evolutionResponse = await _httpClient.GetFromJsonAsync<JsonElement>(
                        speciesResponse
                            .GetProperty("evolution_chain")
                            .GetProperty("url")
                            .GetString()
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
                        Gewicht = response.GetProperty("weight").GetInt32(),
                        Groesse = response.GetProperty("height").GetInt32(),
                        Geschlecht = speciesResponse.GetProperty("gender_rate").GetInt32(),
                        Beschreibung =
                            speciesResponse
                                .GetProperty("flavor_text_entries")
                                .EnumerateArray()
                                .FirstOrDefault(x =>
                                    x.GetProperty("language").GetProperty("name").GetString()
                                    == "de"
                                )
                                .GetProperty("flavor_text")
                                .GetString()
                                ?.Replace("\n", " ")
                                ?.Replace("\f", " ")
                                ?.CapitalizeFirstLetter() ?? string.Empty,
                        Art =
                            speciesResponse
                                .GetProperty("genera")
                                .EnumerateArray()
                                .FirstOrDefault(x =>
                                    x.GetProperty("language").GetProperty("name").GetString()
                                    == "de"
                                )
                                .GetProperty("genus")
                                .GetString()
                                ?.CapitalizeFirstLetter() ?? string.Empty,
                        Generation =
                            generationResponse
                                .GetProperty("name")
                                .GetString()
                                ?.CapitalizeFirstLetter() ?? string.Empty,

                        Habitat =
                            speciesResponse
                                .GetProperty("habitat")
                                .GetProperty("name")
                                .GetString()
                                ?.CapitalizeFirstLetter() ?? string.Empty,
                    };

                    AddGenerationChain(evolutionResponse.GetProperty("chain"), pokemon.Entwicklung);

                    foreach (var type in response.GetProperty("types").EnumerateArray())
                    {
                        var typeResponse = await _httpClient.GetFromJsonAsync<JsonElement>(
                            type.GetProperty("type").GetProperty("url").GetString()
                        );
                        var damageRelations = typeResponse.GetProperty("damage_relations");

                        pokemon.SchwächeGegen = damageRelations
                            .GetProperty("double_damage_from")
                            .EnumerateArray()
                            .Select(x => x.GetProperty("name").GetString().CapitalizeFirstLetter())
                            .ToHashSet()
                            .ToList();
                        pokemon.ResistenzGegen = damageRelations
                            .GetProperty("half_damage_from")
                            .EnumerateArray()
                            .Select(x => x.GetProperty("name").GetString().CapitalizeFirstLetter())
                            .ToHashSet()
                            .ToList();
                    }

                    foreach (var moveEntry in response.GetProperty("moves").EnumerateArray())
                    {
                        var moveDetailsUrl = moveEntry
                            .GetProperty("move")
                            .GetProperty("url")
                            .GetString();
                        var moveDetails = await _httpClient.GetFromJsonAsync<JsonElement>(
                            moveDetailsUrl
                        );

                        if (
                            moveDetails.TryGetProperty("power", out var powerProperty)
                            && powerProperty.ValueKind != JsonValueKind.Null
                            && powerProperty.GetInt32() > 0
                        )
                        {
                            var damageClass = moveDetails
                                .GetProperty("damage_class")
                                .GetProperty("name")
                                .GetString();
                            if (damageClass == "physical" || damageClass == "special")
                            {
                                var move = new MoveModel
                                {
                                    Name =
                                        moveDetails
                                            .GetProperty("name")
                                            .GetString()
                                            ?.CapitalizeFirstLetter() ?? string.Empty,
                                    Schaden = powerProperty.GetInt32(),
                                    LevelLearnedAt = moveEntry
                                        .GetProperty("version_group_details")[0]
                                        .GetProperty("level_learned_at")
                                        .GetInt32(),
                                    Typ = moveDetails
                                        .GetProperty("type")
                                        .GetProperty("name")
                                        .GetString(),
                                };

                                pokemon.Moves.Add(move);
                            }
                        }
                    }

                    foreach (var stat in response.GetProperty("stats").EnumerateArray())
                    {
                        string statName = stat.GetProperty("stat").GetProperty("name").GetString();
                        int baseStat = stat.GetProperty("base_stat").GetInt32();

                        switch (statName)
                        {
                            case "hp":
                                pokemon.HP = baseStat;
                                break;
                            case "attack":
                                pokemon.Attack = baseStat;
                                break;
                            case "defense":
                                pokemon.Defense = baseStat;
                                break;
                            case "speed":
                                pokemon.Speed = baseStat;
                                break;
                        }
                    }

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

        private static void AddGenerationChain(JsonElement chain, List<string> pokemonNames)
        {
            var species = chain.GetProperty("species");
            pokemonNames.Add(species.GetProperty("name").GetString().CapitalizeFirstLetter());

            var evolvesTo = chain.GetProperty("evolves_to");
            foreach (var evolution in evolvesTo.EnumerateArray())
            {
                AddGenerationChain(evolution, pokemonNames);
            }
        }
    }
}
