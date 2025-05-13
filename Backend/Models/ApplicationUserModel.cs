using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class ApplicationUserModel : IdentityUser
    {
        public DateOnly LastPick { get; set; } = DateOnly.MinValue;
        public virtual List<UserPokemonModel> UserPokemons { get; set; } = [];
    }
}
