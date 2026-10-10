# Trabajo en Clase — Mastermind con pruebas unitarias

Nombre del estudiante: PENDIENTE DE COMPLETAR.

Repositorio: https://github.com/AndrewShot01/Mastermind

Asignatura: BSI-19 – Programación Avanzada - Sábado.

Profesor: MSc. Héctor Egérico Fernández Méndez.

Título: Evaluador Mastermind con Cheat Code Integrado.

Fecha: 10-10-2026.

## Cumplimiento de requisitos

Se implementaron el conteo de coincidencias exactas y colores fuera de posición sin doble conteo; validaciones de entradas nulas, tamaño 4 o 6, igualdad de tamaños y colores permitidos; logging con fecha; colores configurables por JSON; parámetro opcional de cheat; validación de código y disponibilidad; un uso por partida; selección aleatoria de posición y consumo del cheat incluso cuando la posición ya estaba acertada.

La aplicación de consola genera patrones aleatorios, permite intentos sucesivos y registra las evaluaciones en archivo. Se usa una instancia del evaluador por partida. La disponibilidad de códigos se carga al comenzar una partida; no se guarda un inventario global. El enunciado no define un esquema JSON obligatorio ni persistencia entre partidas, por lo que estas decisiones se documentan en README.md.

Requisitos técnicos parcialmente realizados: ninguno identificado. Apartados personales pendientes: nombre, reflexión del estudiante y transcripción completa de la conversación. Este informe no inventa esos datos.

## Dificultades y soluciones

El repositorio inicialmente solo contenía .gitignore. Se creó una solución con biblioteca, consola y proyecto de pruebas. El equipo tenía runtime pero carecía de SDK; se instaló .NET SDK 10.0.401 en el perfil del usuario para compilar y ejecutar pruebas.

Para evitar doble conteo se excluyen primero las coincidencias exactas y después se consumen frecuencias de los colores restantes. Para verificar el azar y las fechas sin pruebas inestables se inyectan un selector de posición y un reloj. La compilación detectó un nombre local que ocultaba el campo de colores; se corrigió calificando el campo explícitamente.

## Flujo de Git y evidencia

Se clonó el repositorio y se trabajó en main. No se crearon ramas adicionales ni pull requests. Los cambios se organizaron en las cuatro etapas indicadas: algoritmo básico, pruebas básicas, requisitos avanzados y ampliación de pruebas. La documentación se agregó en un commit separado. El listado verificable de commits está en docs/commits.txt.

## Código fuente y pruebas

src/Mastermind.Core contiene el evaluador y el cargador de configuración. src/Mastermind.Console contiene el juego y su JSON. tests/Mastermind.Tests contiene las pruebas xUnit. README.md explica ejecución y reglas.

Verificación: 24 pruebas aprobadas, 0 fallidas y 0 omitidas. Compilación Release con 0 advertencias y 0 errores. La salida real de dotnet test está en docs/pruebas.txt. Se ejecutó también la consola con entradas inválidas, código desconocido, pista válida y segundo uso rechazado.

La retrocompatibilidad se verifica conservando sin cambios las seis pruebas básicas y la llamada EvaluarIntento con dos argumentos. La ampliación agrega la pista como propiedad opcional sin alterar el conteo original para entradas válidas.

## Conversación con el agente

Solicitud disponible del estudiante: “Look, I'd like us to do this task i put you a file with the instructions, the project folder would be D:\temp\git\Mastermind, and the repository URL would be: https://github.com/AndrewShot01/Mastermind.git”.

El agente leyó el documento, inspeccionó y clonó el repositorio, explicó las decisiones sobre pistas y configuración, implementó el código y ejecutó las verificaciones descritas. Esto es un resumen, no una transcripción íntegra. PENDIENTE: pegar o exportar toda la conversación desde el chat al preparar la entrega.

## Conclusiones y recomendaciones

El ejercicio demuestra cómo separar el algoritmo, el estado de una partida y la interacción de consola. Las pruebas de duplicados y del consumo de cheats comprueban casos que un ejemplo de coincidencia completa no cubre. Para ampliar el proyecto se puede agregar una interfaz gráfica y decidir explícitamente si los usos deben persistir entre partidas.

PENDIENTE: reflexión personal del estudiante sobre el aprendizaje obtenido.
