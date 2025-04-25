namespace Backend.Models
{
    public class PokemonModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BildUrl { get; set; } = string.Empty;
        public string Primärtyp { get; set; } = string.Empty;
        public string Sekundärtyp { get; set; } = string.Empty;
        public int Gewicht { get; set; } // in gramm * 100 gerechnet
        public int Groesse { get; set; } // in cm * 10 gerechnet
        public int Geschlecht { get; set; }
        public string Generation { get; set; } = string.Empty;
        public string Art { get; set; } = string.Empty;
        public string Beschreibung { get; set; } = string.Empty;
        public List<string> ResistenzGegen { get; set; } = [];
        public List<string> SchwächeGegen { get; set; } = [];

        public string Habitat { get; set; } = string.Empty;
        public string Hauptfähigkeit { get; set; } = string.Empty;
        public string VersteckteFähigkeit { get; set; } = string.Empty;
        public int KP { get; set; }
        public int HP { get; set; }
        public int Angriff { get; set; }
        public int Verteidigung { get; set; }
    }
}
