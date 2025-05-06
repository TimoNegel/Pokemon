using Backend.Models;
using PokeApiNet;

namespace Backend.Services
{
    public class IPokemonService
    {
        private readonly PokeApiClient _pokeApiClient;

        //private readonly IMemoryCache _cache;
        //private readonly ApplicationDbContext _dbContext;

        public IPokemonService(PokeApiClient pokeApiClient)
        {
            _pokeApiClient = pokeApiClient;
        }

        public async IAsyncEnumerable<PokemonModel> GetPokemonsDetailsAsync(int limit)
        {
            limit = (limit > 100000 || limit < 0) ? 100000 : limit;
            for (int id = 1; id <= limit; id++)
            {
                yield return await GetPokemonBaseDetailsAsyncById(id).ConfigureAwait(false);
            }
        }

        public async Task<PokemonModel> GetPokemonBaseDetailsAsyncById(int id)
        {
            var pokemon = await _pokeApiClient
                .GetResourceAsync<PokeApiNet.Pokemon>(id)
                .ConfigureAwait(false);
            var species = await _pokeApiClient
                .GetResourceAsync<PokemonSpecies>(pokemon.Species.Name)
                .ConfigureAwait(false);
            var generation = await _pokeApiClient
                .GetResourceAsync<Generation>(species.Generation.Name)
                .ConfigureAwait(false);

            PokemonModel pokemonModel = new PokemonModel
            {
                Id = pokemon.Id.ToString().PadLeft(4, '0'),
                Name = pokemon.Name.CapitalizeFirstLetter(),
                BildUrl = pokemon.Sprites.FrontDefault ?? string.Empty,
                Primärtyp =
                    pokemon.Types.FirstOrDefault()?.Type.Name.CapitalizeFirstLetter()
                    ?? string.Empty,
                Sekundärtyp =
                    pokemon.Types.Count > 1
                        ? pokemon.Types[1].Type.Name.CapitalizeFirstLetter()
                        : string.Empty,
                Generation = generation.Name.CapitalizeFirstLetter(),
                Gewicht = pokemon.Weight,
                Groesse = pokemon.Height,
            };

            return pokemonModel;
        }

        public async Task<PokemonModel> GetPokemonExpandedDetailsAsync(PokemonModel pokemon)
        {
            var pokemonResource = await _pokeApiClient
                .GetResourceAsync<PokeApiNet.Pokemon>(int.Parse(pokemon.Id))
                .ConfigureAwait(false);
            var species = await _pokeApiClient
                .GetResourceAsync<PokemonSpecies>(pokemonResource.Species.Name)
                .ConfigureAwait(false);
            //var evolutionChain = await _pokeApiClient
            //    .GetResourceAsync<EvolutionChain>(
            //        int.Parse(species.EvolutionChain.Url.TrimEnd('/').Split('/').Last())
            //    )
            //    .ConfigureAwait(false);

            await SetSpeciesInformation(species, pokemon).ConfigureAwait(false);
            //await SetGenerationChain(evolutionChain.Chain, pokemon.Entwicklung)
            //    .ConfigureAwait(false);
            //await SetPokemonTypeDetailsAsync(pokemonResource, pokemon).ConfigureAwait(false);
            //await SetPokemonMovesAsync(pokemonResource, pokemon).ConfigureAwait(false);
            await SetBaseStats(pokemonResource.Stats, pokemon).ConfigureAwait(false);

            return pokemon;
        }

        private Task SetGenerationChain(ChainLink evolutionChain, List<string> pokemonNames)
        {
            if (!string.IsNullOrEmpty(evolutionChain.Species.Name))
            {
                pokemonNames.Add(evolutionChain.Species.Name.CapitalizeFirstLetter());
            }

            foreach (var evolution in evolutionChain.EvolvesTo)
            {
                SetGenerationChain(evolution, pokemonNames).ConfigureAwait(false);
            }
            return Task.CompletedTask;
        }

        private Task SetBaseStats(List<PokemonStat> stats, PokemonModel pokemon)
        {
            foreach (var stat in stats)
            {
                switch (stat.Stat.Name)
                {
                    case "hp":
                        pokemon.HP = stat.BaseStat;
                        break;
                    case "attack":
                        pokemon.Attack = stat.BaseStat;
                        break;
                    case "defense":
                        pokemon.Defense = stat.BaseStat;
                        break;
                    case "speed":
                        pokemon.Speed = stat.BaseStat;
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private async Task SetPokemonTypeDetailsAsync(
            PokeApiNet.Pokemon pokemon,
            PokemonModel pokemonModel
        )
        {
            foreach (var type in pokemon.Types)
            {
                var typeDetails = await _pokeApiClient
                    .GetResourceAsync<PokeApiNet.Type>(type.Type.Name)
                    .ConfigureAwait(false);

                pokemonModel.SchwächeGegen = typeDetails
                    .DamageRelations.DoubleDamageFrom.Select(x => x.Name.CapitalizeFirstLetter())
                    .ToList();

                pokemonModel.ResistenzGegen = typeDetails
                    .DamageRelations.HalfDamageFrom.Select(x => x.Name.CapitalizeFirstLetter())
                    .ToList();
            }
        }

        private async Task SetPokemonMovesAsync(
            PokeApiNet.Pokemon pokemon,
            PokemonModel pokemonModel
        )
        {
            foreach (var moveEntry in pokemon.Moves)
            {
                var moveDetails = await _pokeApiClient
                    .GetResourceAsync<Move>(moveEntry.Move.Name)
                    .ConfigureAwait(false);

                if (moveDetails.Power.HasValue && moveDetails.Power.Value > 0)
                {
                    if (
                        moveDetails.DamageClass.Name == "physical"
                        || moveDetails.DamageClass.Name == "special"
                    )
                    {
                        var move = new MoveModel
                        {
                            Name = moveDetails.Name.CapitalizeFirstLetter(),
                            Schaden = moveDetails.Power.Value,
                            LevelLearnedAt =
                                moveEntry.VersionGroupDetails.FirstOrDefault()?.LevelLearnedAt ?? 0,
                            Typ = moveDetails.Type.Name,
                        };

                        pokemonModel.Moves.Add(move);
                    }
                }
            }
        }

        private Task SetSpeciesInformation(PokemonSpecies species, PokemonModel pokemon)
        {
            pokemon.Geschlecht = species.GenderRate;
            pokemon.Beschreibung =
                species
                    .FlavorTextEntries.FirstOrDefault(x => x.Language.Name == "de")
                    ?.FlavorText?.Replace("\n", " ")
                    ?.Replace("\f", " ")
                    ?.CapitalizeFirstLetter() ?? string.Empty;

            pokemon.Art =
                species
                    .Genera.FirstOrDefault(x => x.Language.Name == "de")
                    ?.Genus?.CapitalizeFirstLetter() ?? string.Empty;

            pokemon.Habitat = species.Habitat?.Name.CapitalizeFirstLetter() ?? string.Empty;

            return Task.CompletedTask;
        }
    }
}
