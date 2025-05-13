using Backend.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class PokemonUserCollectionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly AuthenticationStateProvider _authenticationStateAsync;

        public PokemonUserCollectionService(IDbContextFactory<ApplicationDbContext> factory, AuthenticationStateProvider authenticationStateAsync)
        {
            _dbFactory = factory;
            _authenticationStateAsync = authenticationStateAsync;
        }

        private async Task<string?> GetCurrentUserName()
        {
            var authstate = await _authenticationStateAsync.GetAuthenticationStateAsync();

            if (authstate == null)
            {
                return null;
            }

            return authstate.User.Identity?.Name ?? string.Empty;
        }

        public async Task<DateOnly> GetCurrentUserLastPick()
        {
            var currentUserName = await GetCurrentUserName();
            if (!string.IsNullOrEmpty(currentUserName))
            {
                using var context = _dbFactory.CreateDbContext();
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == currentUserName.ToUpper());

                return user?.LastPick ?? DateOnly.MinValue;
            }

            return DateOnly.MaxValue;
        }

        public async Task AddPokemonToUserCollection(PokemonModel pokemon)
        {
            var currentUserName = await GetCurrentUserName();
            if (!string.IsNullOrEmpty(currentUserName))
            {
                using var context = _dbFactory.CreateDbContext();
                var user = await context.Users
                    .Include(u => u.UserPokemons)
                    .ThenInclude(up => up.Pokemon)
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == currentUserName.ToUpper());

                if (user != null)
                {
                    var userPokemonExists = user.UserPokemons.Any(up => up.PokemonId == pokemon.Id);

                    if (!userPokemonExists)
                    {
                        var userPokemon = new UserPokemonModel
                        {
                            UserId = user.Id,
                            PokemonId = pokemon.Id
                        };

                        user.UserPokemons.Add(userPokemon);
                        user.LastPick = DateOnly.FromDateTime(DateTime.Now);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<List<PokemonModel>> GetUserCollection()
        {
            var currentUserName = await GetCurrentUserName();
            if (!string.IsNullOrEmpty(currentUserName))
            {
                using var context = _dbFactory.CreateDbContext();
                var user = await context.Users
                    .Include(u => u.UserPokemons)
                    .ThenInclude(up => up.Pokemon)
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == currentUserName.ToUpper());

                return user?.UserPokemons.Select(up => up.Pokemon).ToList() ?? new List<PokemonModel>();
            }

            return new List<PokemonModel>();
        }
    }
}
