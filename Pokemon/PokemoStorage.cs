using Backend.Models;

namespace Pokemon
{
    public static class PokemoStorage
    {
        public static bool firstRender = true;
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
