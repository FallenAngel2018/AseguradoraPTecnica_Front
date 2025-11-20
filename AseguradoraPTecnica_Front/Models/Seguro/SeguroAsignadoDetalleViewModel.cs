namespace AseguradoraPTecnica_Front.Models.Seguro
{
    public class SeguroAsignadoDetalleViewModel
    {
        public long IdSeguroAsignado { get; set; }
        public string? Cedula { get; set; }
        public string? CodSeguro { get; set; }
        public string? NombreSeguro { get; set; }
        public int Estado { get; set; }
    }
}
