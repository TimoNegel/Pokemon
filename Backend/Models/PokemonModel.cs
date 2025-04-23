namespace Backend.Models
{
    public class PokemonModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BildUrl { get; set; } = string.Empty;
        public string Primärtyp { get; set; } = string.Empty;
        public string Sekundärtyp { get; set; } = string.Empty;
        public string Habitat { get; set; } = string.Empty;
        public string Hauptfähigkeit { get; set; } = string.Empty;
        public string VersteckteFähigkeit { get; set; } = string.Empty;
        public int KP { get; set; }
        public int HP { get; set; }
        public int Angriff { get; set; }
        public int Verteidigung { get; set; }
    }
}
