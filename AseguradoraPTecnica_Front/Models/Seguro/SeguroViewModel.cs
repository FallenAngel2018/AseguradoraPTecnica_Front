namespace AseguradoraPTecnica_Front.Models.Seguro
{
    public class SeguroViewModel
    {
        public long IdSeguro { get; set; }
        public string? NombreSeguro { get; set; }
        public string? Codigo { get; set; }
        public decimal? SumaAsegurada { get; set; }
        public decimal? Prima { get; set; }
        public int EdadMinima { get; set; }
        public int EdadMaxima { get; set; }
        public int Estado { get; set; }
    }
}
