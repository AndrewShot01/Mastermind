# Mastermind

Trabajo de BSI-19 — Programación Avanzada. Evaluador en C# con aplicación de consola, configuración JSON y pruebas xUnit.

## Ejecutar

Requiere .NET SDK 10. Desde la raíz:

```powershell
dotnet run --project src/Mastermind.Console
dotnet test Mastermind.slnx
```

Elige tamaño 4 o 6. Escribe un patrón como `RBYG` o `RBYG PISTA`; `salir` termina. Los símbolos son sensibles a mayúsculas. Se permiten colores repetidos. El patrón se genera al azar y los intentos no tienen límite.

La configuración predeterminada está en `src/Mastermind.Console/configuracion.json`. Para usar otra:

```powershell
dotnet run --project src/Mastermind.Console -- C:\ruta\configuracion.json
```

Cada color tiene un símbolo alfanumérico de un carácter y un nombre. `B` representa Brown y `G` Green. Los códigos incluyen `codigo` e `intentosDisponibles`; `PISTA` tiene un uso y `AGOTADO` ninguno.

## API y reglas

`new Evaluador(...).EvaluarIntento(patronSecreto, intentoJugador, cheatCode: null)` devuelve `BienPosicionados`, `ColorCorrecto` y una `Cheat` opcional. Primero cuenta coincidencias exactas; después consume las frecuencias restantes para evitar duplicados. `RGBY` contra `RBYG` devuelve 1 y 3.

Una instancia representa una partida: permite un solo cheat exitosamente validado, incluso si se cambia el código. La pista elige cualquier posición aleatoria, numerada desde 1. Si esa posición ya estaba correcta devuelve `SinSuerte`; el uso se consume igualmente. En otro caso revela el color de esa posición, sin cambiar el resultado del intento. Los códigos desconocidos, agotados y los intentos inválidos se rechazan sin consumir usos. Una nueva instancia inicia otra partida; los usos JSON son disponibilidad inicial por partida, no un inventario persistente entre partidas.

Los registros incluyen el patrón secreto según el enunciado y se escriben en `logs/evaluaciones.log` desde la consola. La API admite un destino de logging, reloj y selector aleatorio inyectables para verificar el comportamiento de forma determinista.

## Estructura

- `src/Mastermind.Core`: algoritmo, validaciones, sesión y carga JSON.
- `src/Mastermind.Console`: juego interactivo y configuración.
- `tests/Mastermind.Tests`: pruebas originales y requisitos avanzados.
- `docs`: informe y evidencia de ejecución.

La solución conserva la llamada con dos argumentos y las seis pruebas básicas sin cambios. El informe señala los apartados que deben completar personalmente el estudiante y la conversación disponible.
