using System.Text.Json;

namespace Mastermind.Core;

public sealed record CheatCode(string Codigo, int IntentosDisponibles);

public sealed class Configuracion
{
    public Dictionary<string, string> Colores { get; init; } = new();
    public List<CheatCode> CheatCodes { get; init; } = [];

    public static Configuracion Cargar(string ruta)
    {
        var configuracion = JsonSerializer.Deserialize<Configuracion>(File.ReadAllText(ruta),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new ArgumentException("La configuración está vacía.");
        if (configuracion.Colores is null || configuracion.CheatCodes is null ||
            configuracion.Colores.Count < 2 || configuracion.Colores.Any(c =>
                c.Key.Length != 1 || !char.IsLetterOrDigit(c.Key[0]) || string.IsNullOrWhiteSpace(c.Value)))
            throw new ArgumentException("Cada color debe tener un símbolo alfanumérico y un nombre.");
        _ = new Evaluador(configuracion.Colores.Keys.Select(k => k[0]), configuracion.CheatCodes);
        return configuracion;
    }
}
