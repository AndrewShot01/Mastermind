using Mastermind.Core;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
try
{
    var config = Configuracion.Cargar(args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "configuracion.json"));
    Console.WriteLine("Mastermind — adivina el patrón. Escribe salir para terminar.");
    Console.WriteLine(string.Join(", ", config.Colores.Select(c => $"{c.Key}={c.Value}")));
    Console.Write("Tamaño (4 o 6): ");
    var entrada = Console.ReadLine();
    if (entrada is null || entrada.Equals("salir", StringComparison.OrdinalIgnoreCase)) return;
    if (!int.TryParse(entrada, out int largo) || largo is not (4 or 6))
        throw new ArgumentException("El tamaño debe ser 4 o 6.");
    char[] colores = config.Colores.Keys.Select(k => k[0]).ToArray();
    string secreto = new(Enumerable.Range(0, largo).Select(_ => colores[Random.Shared.Next(colores.Length)]).ToArray());
    string log = Path.Combine(Environment.CurrentDirectory, "logs", "evaluaciones.log");
    Directory.CreateDirectory(Path.GetDirectoryName(log)!);
    var evaluador = new Evaluador(colores, config.CheatCodes,
        mensaje => File.AppendAllText(log, mensaje + Environment.NewLine));
    Console.WriteLine("Formato: patrón [cheat]. Ejemplo: RGBY PISTA. Un cheat por partida.");
    int intentos = 0;
    while (true)
    {
        Console.Write("Intento: ");
        string? linea = Console.ReadLine();
        if (linea is null || linea.Equals("salir", StringComparison.OrdinalIgnoreCase)) break;
        string[] partes = linea.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        try
        {
            if (partes.Length is < 1 or > 2) throw new ArgumentException("Usa patrón [cheat].");
            var resultado = evaluador.EvaluarIntento(secreto, partes[0], partes.Length == 2 ? partes[1] : null);
            intentos++;
            Console.WriteLine($"{resultado.BienPosicionados} bien posicionados, {resultado.ColorCorrecto} con color correcto.");
            if (resultado.Cheat is { } pista)
                Console.WriteLine(pista.SinSuerte ? "Sin suerte: la posición ya estaba acertada. Cheat consumido." : $"Pista: posición {pista.Posicion} = {pista.Color}. Cheat consumido.");
            if (resultado.BienPosicionados == largo)
            {
                Console.WriteLine($"¡Ganaste en {intentos} intentos!");
                break;
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
catch (Exception ex) when (ex is ArgumentException or IOException or System.Text.Json.JsonException)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.ExitCode = 1;
}
