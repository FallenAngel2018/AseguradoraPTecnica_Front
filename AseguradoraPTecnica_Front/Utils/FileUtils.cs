using Microsoft.Extensions.Diagnostics.HealthChecks;
using OfficeOpenXml;

namespace AseguradoraPTecnica_Front.Utils
{
    public class FileUtils
    {
        public static async Task<(bool esValido, string mensaje)> ValidarArchivoTxtAsync(Stream stream)
        {
            stream.Position = 0; // Resetear posición

            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                var contenido = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(contenido))
                    return (false, "El archivo TXT está vacío");

                var lineas = contenido.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                if (lineas.Length < 2)
                    return (false, "El archivo debe tener al menos una línea de encabezado y una línea de datos");

                var encabezado = lineas[0].Trim();
                var columnasEsperadas = new[] { "Cedula", "Nombres", "Apellidos", "Telefono", "Edad" };
                var columnas = encabezado.Split('|');

                if (columnas.Length != 5)
                    return (false, $"El archivo debe tener exactamente 5 columnas separadas por '|'. Se encontraron {columnas.Length}");

                for (int i = 0; i < columnasEsperadas.Length; i++)
                {
                    if (!columnas[i].Trim().Equals(columnasEsperadas[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return (false, $"La columna {i + 1} debe ser '{columnasEsperadas[i]}', pero se encontró '{columnas[i].Trim()}'");
                    }
                }

                for (int i = 1; i < lineas.Length; i++)
                {
                    var lineaActual = lineas[i].Trim();
                    if (string.IsNullOrWhiteSpace(lineaActual))
                        continue;

                    var validacion = ValidarLineaTxt(lineaActual, i + 1);
                    if (!validacion.esValido)
                        return validacion;
                }

                return (true, "Validación exitosa");
            }
        }

        private static (bool esValido, string mensaje) ValidarLineaTxt(string linea, int numeroLinea)
        {
            var campos = linea.Split('|');

            if (campos.Length != 5)
                return (false, $"Línea {numeroLinea}: Debe tener 5 campos separados por '|'. Se encontraron {campos.Length}");

            var cedula = campos[0].Trim();
            if (string.IsNullOrWhiteSpace(cedula))
                return (false, $"Línea {numeroLinea}: La Cédula no puede estar vacía");

            if (cedula.Length < 6 || cedula.Length > 20)
                return (false, $"Línea {numeroLinea}: La Cédula debe tener entre 6 y 20 caracteres");

            var nombres = campos[1].Trim();
            if (string.IsNullOrWhiteSpace(nombres))
                return (false, $"Línea {numeroLinea}: Los Nombres no pueden estar vacíos");

            if (nombres.Length < 2 || nombres.Length > 100)
                return (false, $"Línea {numeroLinea}: Los Nombres deben tener entre 2 y 100 caracteres");

            var apellidos = campos[2].Trim();
            if (string.IsNullOrWhiteSpace(apellidos))
                return (false, $"Línea {numeroLinea}: Los Apellidos no pueden estar vacíos");

            if (apellidos.Length < 2 || apellidos.Length > 100)
                return (false, $"Línea {numeroLinea}: Los Apellidos deben tener entre 2 y 100 caracteres");

            var telefono = campos[3].Trim();
            if (string.IsNullOrWhiteSpace(telefono))
                return (false, $"Línea {numeroLinea}: El Teléfono no puede estar vacío");

            if (!System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^[\d\-\+\(\)\s]+$"))
                return (false, $"Línea {numeroLinea}: El Teléfono tiene un formato inválido");

            var edadStr = campos[4].Trim();
            if (!int.TryParse(edadStr, out int edad))
                return (false, $"Línea {numeroLinea}: La Edad debe ser un número entero válido. Valor encontrado: '{edadStr}'");

            if (edad < 0 || edad > 150)
                return (false, $"Línea {numeroLinea}: La Edad debe estar entre 0 y 150. Valor encontrado: {edad}");

            return (true, string.Empty);
        }

        // Cambiar de IFormFile a Stream
        public static (bool esValido, string mensaje) ValidarArchivoXlsxAsync(Stream stream)
        {
            stream.Position = 0; // Resetear posición

            using (var package = new ExcelPackage(stream))
            {
                if (package.Workbook.Worksheets.Count == 0)
                    return (false, "El archivo XLSX no contiene hojas de trabajo");

                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet.Dimension == null)
                    return (false, "La hoja de trabajo está vacía");

                var filaInicio = worksheet.Dimension.Start.Row;
                var totalFilas = worksheet.Dimension.End.Row;
                var totalColumnas = worksheet.Dimension.End.Column;

                if (totalColumnas < 5)
                    return (false, $"El archivo debe tener al menos 5 columnas. Se encontraron {totalColumnas}");

                var columnasEsperadas = new[] { "Cedula", "Nombres", "Apellidos", "Telefono", "Edad" };

                for (int col = 0; col < columnasEsperadas.Length; col++)
                {
                    var valorCelda = worksheet.Cells[filaInicio, col + 1].Value?.ToString()?.Trim();

                    if (string.IsNullOrWhiteSpace(valorCelda))
                        return (false, $"La columna {col + 1} del encabezado está vacía");

                    if (!valorCelda.Equals(columnasEsperadas[col], StringComparison.OrdinalIgnoreCase))
                    {
                        return (false, $"La columna {col + 1} debe ser '{columnasEsperadas[col]}', pero se encontró '{valorCelda}'");
                    }
                }

                if (totalFilas < 2)
                    return (false, "El archivo debe tener al menos una fila de datos además del encabezado");

                for (int fila = filaInicio + 1; fila <= totalFilas; fila++)
                {
                    var validacion = ValidarFilaXlsx(worksheet, fila);
                    if (!validacion.esValido)
                        return validacion;
                }

                return (true, "Validación exitosa");
            }
        }

        private static (bool esValido, string mensaje) ValidarFilaXlsx(ExcelWorksheet worksheet, int fila)
        {
            var cedula = worksheet.Cells[fila, 1].Value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(cedula))
                return (false, $"Fila {fila}: La Cédula no puede estar vacía");

            if (cedula.Length < 6 || cedula.Length > 20)
                return (false, $"Fila {fila}: La Cédula debe tener entre 6 y 20 caracteres");

            var nombres = worksheet.Cells[fila, 2].Value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(nombres))
                return (false, $"Fila {fila}: Los Nombres no pueden estar vacíos");

            if (nombres.Length < 2 || nombres.Length > 100)
                return (false, $"Fila {fila}: Los Nombres deben tener entre 2 y 100 caracteres");

            var apellidos = worksheet.Cells[fila, 3].Value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(apellidos))
                return (false, $"Fila {fila}: Los Apellidos no pueden estar vacíos");

            if (apellidos.Length < 2 || apellidos.Length > 100)
                return (false, $"Fila {fila}: Los Apellidos deben tener entre 2 y 100 caracteres");

            var telefono = worksheet.Cells[fila, 4].Value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(telefono))
                return (false, $"Fila {fila}: El Teléfono no puede estar vacío");

            if (!System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^[\d\-\+\(\)\s]+$"))
                return (false, $"Fila {fila}: El Teléfono tiene un formato inválido");

            var edadObj = worksheet.Cells[fila, 5].Value;

            if (edadObj == null)
                return (false, $"Fila {fila}: La Edad no puede estar vacía");

            if (!int.TryParse(edadObj.ToString(), out int edad))
                return (false, $"Fila {fila}: La Edad debe ser un número entero válido");

            if (edad < 0 || edad > 150)
                return (false, $"Fila {fila}: La Edad debe estar entre 0 y 150");

            return (true, string.Empty);
        }


    }

}



