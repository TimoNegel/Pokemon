using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.Services
{
    public class PokemonService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public PokemonService(
            HttpClient httpClient,
            IMemoryCache cache,
            IDbContextFactory<ApplicationDbContext> dbFactory
        )
        {
            _httpClient = httpClient;
            _cache = cache;
            _dbFactory = dbFactory;
        }

        public async Task<List<PokemonModel>> GetPokemonsDetailsAsync(int batchSize, int offSet)
        {
            int id = 0;
            try
            {
                batchSize = (batchSize > 1025 || batchSize < 0) ? 1025 : batchSize;
                var tasks = new List<Task<PokemonModel>>();

                for (id = 1 + offSet; id <= batchSize + offSet; id++)
                {
                    tasks.Add(GetPokemonBaseDetailsAsyncById(id));
                }

                var results = await Task.WhenAll(tasks);
                tasks.Clear();

                return results.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error in GetPokemonsDetailsAsync by Pokemon Id {id}: {ex.Message}"
                );
                throw;
            }
        }

        private async Task<PokemonModel?> CheckIfPokemonExists(int id)
        {
            if (!_cache.TryGetValue($"Pokemon_{id}", out PokemonModel? pokemon))
            {
                using var context = _dbFactory.CreateDbContext();

                pokemon = await context
                    .PokemonsCache.Include(p => p.Moves)
                    .FirstOrDefaultAsync(x => x.Id == id.ToString().PadLeft(4, '0'));
            }
            return pokemon;
        }

        private async Task<MoveModel?> CheckIfMoveExists(string name)
        {
            if (!_cache.TryGetValue($"Move_{name}", out MoveModel? move))
            {
                using var context = _dbFactory.CreateDbContext();

                move = await context.Moves.FirstOrDefaultAsync(x =>
                    x.Name == name.CapitalizeFirstLetter()
                );
            }
            return move;
        }

        public async Task<PokemonModel> GetPokemonBaseDetailsAsyncById(int id)
        {
            try
            {
                PokemonModel? pokemon = await CheckIfPokemonExists(id);
                if (pokemon == null)
                {
                    var response = await FetchBasicDetailsAsync(id);
                    var speciesResponse = await FetchSpeciesDetailsAsync(response);
                    var generationResponse = await FetchGenerationDetailsAsync(speciesResponse);

                    pokemon = SetBasicPokemonModel(response, speciesResponse, generationResponse);
                    SetStats(response, pokemon);

                    _cache.Set($"Pokemon_{id}", pokemon, TimeSpan.FromHours(3));

                    using var context = _dbFactory.CreateDbContext();
                    context.PokemonsCache.Add(pokemon);
                    await context.SaveChangesAsync();
                }

                return pokemon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPokemonBaseDetailsAsyncById: {ex.Message}");
                throw;
            }
        }

        public async Task<PokemonModel> GetPokemonExpandedDetailsAsync(PokemonModel pokemon)
        {
            try
            {
                int id = int.Parse(pokemon.Id);
                pokemon = await CheckIfPokemonExists(id) ?? pokemon;

                if (pokemon.Entwicklung.Count() == 0)
                {
                    var response = await FetchBasicDetailsAsync(id);
                    var speciesResponse = await FetchSpeciesDetailsAsync(response);
                    var evolutionResponse = await FetchEvolutionDetailsAsync(speciesResponse);

                    SetGenerationChain(evolutionResponse.GetProperty("chain"), pokemon.Entwicklung);
                    await SetTypeDetailsAsync(response, pokemon);
                    await SetMoveDetailsAsync(response, pokemon);

                    using var context = _dbFactory.CreateDbContext();
                    context.PokemonsCache.Update(pokemon);
                    await context.SaveChangesAsync();
                }

                return pokemon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPokemonExpandedDetailsAsync: {ex.Message}");
                throw;
            }
        }

        private async Task<T?> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxRetries = 3)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    return await action();
                }
                catch (Exception) when (attempt < maxRetries)
                {
                    attempt++;
                    await Task.Delay(500 * attempt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ExecuteWithRetryAsync: {ex.Message}");
                    throw;
                }
            }
        }

        private async Task<JsonElement> FetchBasicDetailsAsync(int id)
        {
            try
            {
                return await ExecuteWithRetryAsync(() =>
                    _httpClient.GetFromJsonAsync<JsonElement>(
                        $"https://pokeapi.co/api/v2/pokemon/{id}"
                    )
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchBasicDetailsAsync: {ex.Message}");
                throw;
            }
        }

        private async Task<JsonElement> FetchSpeciesDetailsAsync(JsonElement response)
        {
            try
            {
                string speciesUrl =
                    response.GetProperty("species").GetProperty("url").GetString() ?? string.Empty;
                return await ExecuteWithRetryAsync(() =>
                    _httpClient.GetFromJsonAsync<JsonElement>(speciesUrl)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchSpeciesDetailsAsync: {ex.Message}");
                throw;
            }
        }

        private async Task<JsonElement> FetchGenerationDetailsAsync(JsonElement speciesResponse)
        {
            try
            {
                string generationUrl =
                    speciesResponse.GetProperty("generation").GetProperty("url").GetString()
                    ?? string.Empty;
                return await ExecuteWithRetryAsync(() =>
                    _httpClient.GetFromJsonAsync<JsonElement>(generationUrl)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchGenerationDetailsAsync: {ex.Message}");
                throw;
            }
        }

        private async Task<JsonElement> FetchEvolutionDetailsAsync(JsonElement speciesResponse)
        {
            try
            {
                string evolutionUrl =
                    speciesResponse.GetProperty("evolution_chain").GetProperty("url").GetString()
                    ?? string.Empty;
                return await ExecuteWithRetryAsync(() =>
                    _httpClient.GetFromJsonAsync<JsonElement>(evolutionUrl)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchEvolutionDetailsAsync: {ex.Message}");
                throw;
            }
        }

        private PokemonModel SetBasicPokemonModel(
            JsonElement response,
            JsonElement speciesResponse,
            JsonElement generationResponse
        )
        {
            try
            {
                var pokemon = new PokemonModel();
                pokemon.Id = response.GetProperty("id").GetInt32().ToString().PadLeft(4, '0');
                pokemon.Name =
                    response.GetProperty("name").GetString()?.CapitalizeFirstLetter()
                    ?? string.Empty;
                pokemon.BildUrl =
                    response.GetProperty("sprites").GetProperty("front_default").GetString()
                    ?? string.Empty;
                pokemon.Primärtyp =
                    response
                        .GetProperty("types")[0]
                        .GetProperty("type")
                        .GetProperty("name")
                        .GetString()
                        ?.CapitalizeFirstLetter() ?? string.Empty;
                pokemon.Sekundärtyp =
                    response.GetProperty("types").GetArrayLength() > 1
                        ? response
                            .GetProperty("types")[1]
                            .GetProperty("type")
                            .GetProperty("name")
                            .GetString()
                            ?.CapitalizeFirstLetter() ?? string.Empty
                        : string.Empty;
                pokemon.Gewicht = response.GetProperty("weight").GetInt32();
                pokemon.Groesse = response.GetProperty("height").GetInt32();
                pokemon.Geschlecht = speciesResponse.GetProperty("gender_rate").GetInt32();
                pokemon.Beschreibung =
                    speciesResponse
                        .GetProperty("flavor_text_entries")
                        .EnumerateArray()
                        .FirstOrDefault(x =>
                            x.GetProperty("language").GetProperty("name").GetString() == "de"
                        )
                        .GetJsonString("flavor_text")
                        ?.Replace("\n", " ")
                        ?.Replace("\f", " ")
                        ?.CapitalizeFirstLetter() ?? string.Empty;
                pokemon.Art =
                    speciesResponse
                        .GetProperty("genera")
                        .EnumerateArray()
                        .FirstOrDefault(x =>
                            x.GetProperty("language").GetProperty("name").GetString() == "de"
                        )
                        .GetJsonString("genus")
                        ?.CapitalizeFirstLetter() ?? string.Empty;
                pokemon.Generation =
                    generationResponse.GetProperty("name").GetString()?.CapitalizeFirstLetter()
                    ?? string.Empty;
                pokemon.Habitat =
                    speciesResponse.GetJsonString("habitat", "name")?.CapitalizeFirstLetter()
                    ?? string.Empty;

                return pokemon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MapPokemonModel: {ex.Message}");
                throw;
            }
        }

        private async Task SetTypeDetailsAsync(JsonElement response, PokemonModel pokemon)
        {
            try
            {
                foreach (var type in response.GetProperty("types").EnumerateArray())
                {
                    var typeResponse = await ExecuteWithRetryAsync(() =>
                        _httpClient.GetFromJsonAsync<JsonElement>(
                            type.GetProperty("type").GetProperty("url").GetString()
                        )
                    );
                    var damageRelations = typeResponse.GetProperty("damage_relations");

                    pokemon.SchwächeGegen = damageRelations
                        .GetProperty("double_damage_from")
                        .EnumerateArray()
                        .Select(x =>
                            x.GetProperty("name").GetString()?.CapitalizeFirstLetter()
                            ?? string.Empty
                        )
                        .ToHashSet()
                        .ToList();
                    pokemon.ResistenzGegen = damageRelations
                        .GetProperty("half_damage_from")
                        .EnumerateArray()
                        .Select(x =>
                            x.GetProperty("name").GetString()?.CapitalizeFirstLetter()
                            ?? string.Empty
                        )
                        .ToHashSet()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetTypeDetailsAsync: {ex.Message}");
                throw;
            }
        }

        private async Task SetMoveDetailsAsync(JsonElement response, PokemonModel pokemon)
        {
            try
            {
                foreach (var moveEntry in response.GetProperty("moves").EnumerateArray())
                {
                    var move = await CheckIfMoveExists(
                        moveEntry.GetProperty("move").GetProperty("name").GetString()
                    );

                    if (move is null)
                    {
                        var moveDetailsUrl = moveEntry
                            .GetProperty("move")
                            .GetProperty("url")
                            .GetString();
                        var moveDetails = await ExecuteWithRetryAsync(() =>
                            _httpClient.GetFromJsonAsync<JsonElement>(moveDetailsUrl)
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
                                move = new MoveModel
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
                                    Typ =
                                        moveDetails
                                            .GetProperty("type")
                                            .GetProperty("name")
                                            .GetString() ?? string.Empty,
                                };
                                pokemon.Moves.Add(move);
                                _cache.Set(
                                    $"Pokemon_{move.Name.ToLower()}",
                                    move,
                                    TimeSpan.FromHours(3)
                                );
                            }
                        }
                    }
                    else
                    {
                        pokemon.Moves.Add(move);
                        _cache.Set($"Pokemon_{move.Name.ToLower()}", move, TimeSpan.FromHours(3));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetMoveDetailsAsync: {ex.Message}");
                throw;
            }
        }

        private void SetStats(JsonElement response, PokemonModel pokemon)
        {
            try
            {
                foreach (var stat in response.GetProperty("stats").EnumerateArray())
                {
                    string statName =
                        stat.GetProperty("stat").GetProperty("name").GetString() ?? string.Empty;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetStats: {ex.Message}");
                throw;
            }
        }

        private void SetGenerationChain(JsonElement chain, List<string> pokemonNames)
        {
            try
            {
                var species = chain.GetProperty("species");
                pokemonNames.Add(
                    species.GetProperty("name").GetString()?.CapitalizeFirstLetter() ?? string.Empty
                );

                var evolvesTo = chain.GetProperty("evolves_to");
                foreach (var evolution in evolvesTo.EnumerateArray())
                {
                    SetGenerationChain(evolution, pokemonNames);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetGenerationChain: {ex.Message}");
                throw;
            }
        }
    }
}
