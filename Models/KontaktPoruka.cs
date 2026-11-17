namespace PrijavaIspita.Models
{
    public class KontaktPoruka
    {
        public string Ime { get; set; }
        public string Email { get; set; }
        public string Poruka { get; set; }
        public DateTime DatumSlanja { get; set; } = DateTime.Now;
    }

}
