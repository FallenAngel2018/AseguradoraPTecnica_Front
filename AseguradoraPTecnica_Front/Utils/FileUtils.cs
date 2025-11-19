using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
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

        public static bool ValidarArchivoOpenXml(Stream stream)
        {
            try
            {
                using (var doc = SpreadsheetDocument.Open(stream, false))
                {
                    // Intentar leer las hojas
                    var sheets = doc.WorkbookPart.Workbook.Sheets;
                    return sheets != null && sheets.Any();
                }
            }
            catch
            {
                return false;
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


        public static (bool esValido, string mensaje) ValidarArchivoXlsx(IFormFile archivo)
        {
            try
            {
                using (var stream = archivo.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.Worksheet(1); // Primera hoja

                    if (worksheet == null)
                        return (false, "El archivo XLSX no contiene hojas de trabajo");

                    var firstRowUsed = worksheet.FirstRowUsed();
                    if (firstRowUsed == null)
                        return (false, "La hoja de trabajo está vacía");

                    // Validar columnas esperadas
                    var columnasEsperadas = new[] { "Cedula", "Nombres", "Apellidos", "Telefono", "Edad" };

                    for (int i = 0; i < columnasEsperadas.Length; i++)
                    {
                        var valorCelda = firstRowUsed.Cell(i + 1).GetString().Trim();

                        if (string.IsNullOrEmpty(valorCelda))
                            return (false, $"La columna {i + 1} en el encabezado está vacía");

                        if (!valorCelda.Equals(columnasEsperadas[i], StringComparison.OrdinalIgnoreCase))
                            return (false, $"La columna {i + 1} debe ser '{columnasEsperadas[i]}', pero se encontró '{valorCelda}'");
                    }

                    // Validar que haya al menos una fila de datos además del encabezado
                    var rowsUsed = worksheet.RowsUsed();
                    int totalFilas = 0;
                    foreach (var r in rowsUsed)
                        totalFilas++;

                    if (totalFilas < 2)
                        return (false, "El archivo debe tener al menos una fila de datos además del encabezado");

                    return (true, "Validación exitosa");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error al validar archivo: {ex.Message}");
            }
        }

        // Cambiar de IFormFile a Stream
        public static async Task<(bool esValido, string mensaje)> ValidarArchivoXlsxAsync(Stream stream)
        {
            // Configurar licencia EPPlus (una vez en la app idealmente)
            ExcelPackage.License.SetNonCommercialPersonal("Isaac OBesso");

            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");

            try
            {
                // Guardar el stream a archivo temporal
                stream.Position = 0;
                using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }

                // Abrir con EPPlus desde archivo físico
                using var package = new ExcelPackage(new FileInfo(tempFile));

                if (package.Workbook.Worksheets.Count == 0)
                    return (false, "El archivo XLSX no contiene hojas de trabajo");

                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet.Dimension == null)
                    return (false, "La hoja de trabajo está vacía");

                // Aquí haces las validaciones que necesites
                // Por ejemplo validar columnas, filas, etc...

                return (true, "Validación exitosa");
            }
            catch (Exception ex)
            {
                return (false, $"Error al validar archivo: {ex.Message}");
            }
            finally
            {
                // Elimina archivo temporal
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
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



