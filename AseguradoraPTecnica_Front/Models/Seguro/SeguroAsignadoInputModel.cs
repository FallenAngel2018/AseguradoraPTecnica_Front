namespace AseguradoraPTecnica_Front.Models.Seguro
{
    public class SeguroAsignadoInputModel
    {
        public string? Cedula { get; set; }
        public List<string?> CodigosSeguros { get; set; }
        public int Estado { get; set; }
    }
}
