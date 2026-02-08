# Off-Mind

Prototipo avanzado estilo Spotify para gestionar música local con un modo desarrollador. Incluye un frontend WPF (C#) y una herramienta de apoyo en Python.

## Características
- Menú lateral inspirado en Spotify y experiencia visual fluida con animaciones de presión.
- Modo desarrollador para registrar canciones con metadatos (artista, álbum, imagen y MP3).
- Estadísticas locales: contador de reproducciones y tiempo total escuchado.
- Creación de álbumes y playlists.
- Logo vectorial incluido.

## Ejecutar en Windows
1. Requisitos: .NET 6 SDK y Windows.
2. Compilar y ejecutar:
   ```bash
   cd src/OffMindApp
   dotnet build
   dotnet run
   ```

## Empaquetado a exe
```bash
cd src/OffMindApp
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true
```
El ejecutable quedará en `src/OffMindApp/bin/Release/net6.0-windows/win-x64/publish`.

## Herramienta de desarrollador (Python)
Permite agregar datos a la biblioteca local de manera rápida.

```bash
python tools/offmind_devtool.py --library "C:\Users\<usuario>\AppData\Local\OffMind\library.json" add-song \
  --id "<guid>" --title "Song" --artist "Artista" --album "Álbum" --file-path "C:\musica\tema.mp3"
```
