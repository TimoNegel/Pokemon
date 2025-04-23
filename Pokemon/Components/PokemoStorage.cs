using Backend.Models;

namespace Pokemon.Components
{
    public static class PokemoStorage
    {
        public static List<PokemonModel> pokemons = [];

        private static List<PokemonModel> searchedPokemons = [];
        public static event Action? OnValueChange;

        public static List<PokemonModel> SearchedPokemons
        {
            get => searchedPokemons;
            set
            {
                searchedPokemons = value;
                OnValueChange?.Invoke();
            }
        }
    }
}
