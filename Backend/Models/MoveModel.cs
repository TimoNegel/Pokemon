namespace Backend.Models
{
    public class MoveModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Schaden { get; set; }
        public int LevelLearnedAt { get; set; }
        public string Typ { get; set; } = string.Empty;

        public virtual List<PokemonModel> PokemonWithThisMove { get; set; } =
            new List<PokemonModel>();
    }
}
