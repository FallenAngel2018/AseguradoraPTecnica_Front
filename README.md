# AseguradoraPTecnica Frontend

Este repositorio contiene el código fuente del frontend de la aplicación AseguradoraPTecnica.

## Requisitos previos

- Asegúrate de tener la API backend corriendo antes de iniciar el frontend.
- Visual Studio 2022 instalado.
- Archivo ZIP del proyecto frontend descargado y descomprimido.

## Configuración y ejecución

1. Descarga el archivo ZIP del frontend y descomprímelo en tu equipo.
2. Abre la solución `AseguradoraPTecnica_Front.sln` con Visual Studio 2022.
3. En el proyecto `AseguradoraPTecnica_Front`, abre el archivo `appsettings.json`.
4. Revisa que la propiedad `BaseUrl` dentro de `ApiSettings` coincida con la URL donde está corriendo la API backend. Por defecto, debe estar así:
"ApiSettings": {
  "BaseUrl": "https://localhost:7001/api/"
}

Si no coincide, modifica esta URL para que apunte correctamente a tu API.

5. Ejecuta la aplicación desde Visual Studio.

