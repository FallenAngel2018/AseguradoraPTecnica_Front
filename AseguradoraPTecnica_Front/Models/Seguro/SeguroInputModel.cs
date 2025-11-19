using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AseguradoraPTecnica_Front.Models.Seguro
{
    public class SeguroInputModel : IValidatableObject
    {
        [Required]
        public string? NombreSeguro { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "La suma asegurada debe ser mayor o igual a 0.")]
        public decimal SumaAsegurada { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "La prima debe ser mayor o igual a 0.")]
        public decimal Prima { get; set; }

        [Required]
        [Range(0, 120, ErrorMessage = "La edad mínima debe estar entre 0 y 120.")]
        public int EdadMinima { get; set; }

        [Required]
        [Range(0, 120, ErrorMessage = "La edad máxima debe estar entre 0 y 120.")]
        public int EdadMaxima { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Prima > SumaAsegurada)
            {
                yield return new ValidationResult(
                    "La prima no puede ser mayor que la suma asegurada.",
                    new[] { nameof(Prima) });
            }

            if (EdadMaxima < EdadMinima)
            {
                yield return new ValidationResult(
                    "La edad máxima no puede ser menor que la edad mínima.",
                    new[] { nameof(EdadMaxima), nameof(EdadMinima) });
            }

            if (EdadMaxima > 120)
            {
                yield return new ValidationResult(
                    "La edad máxima no puede superar los 120 años.",
                    new[] { nameof(EdadMaxima), nameof(EdadMinima) });
            }
        }
    }
}
