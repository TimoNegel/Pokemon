namespace Backend.Models
{
    public class UserPokemonModel
    {
        public string UserId { get; set; }
        public virtual ApplicationUserModel User { get; set; }

        public string PokemonId { get; set; }
        public virtual PokemonModel Pokemon { get; set; }
    }

}
