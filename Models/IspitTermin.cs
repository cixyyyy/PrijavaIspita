namespace PrijavaIspita.Models
{
    public class IspitTermin
    {
        public int Id { get; set; }

        public int PredmetId { get; set; }
        public Predmet Predmet { get; set; }

        public string Rok { get; set; }

        public DateTime Datum { get; set; }
    }
}