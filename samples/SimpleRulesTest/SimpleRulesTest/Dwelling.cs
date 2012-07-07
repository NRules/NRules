namespace SimpleRulesTest
{
    public enum DwellingTypes
    {
        Condominium = 0,
        TownHouse = 1,
        SingleHouse = 2,
        DuplexHouse = 3,
    }

    public class Dwelling
    {
        public string Address { get; set; }
        public DwellingTypes Type { get; set; }
    }
}