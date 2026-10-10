namespace Mastermind.Core;

public sealed record Pista(int Posicion, char? Color, bool SinSuerte);
public sealed record Resultado(int BienPosicionados, int ColorCorrecto)
{
    public Pista? Cheat { get; init; }
}

public sealed class Evaluador
{
    private readonly HashSet<char> colores;
    private readonly Dictionary<string, int> cheats;
    private readonly Action<string> logging;
    private readonly Func<DateTime> reloj;
    private readonly Func<int, int> seleccionarPosicion;
    private bool cheatUsado;

    // Una instancia representa una partida. Una nueva partida necesita otra instancia.
    public Evaluador(IEnumerable<char>? colores = null,
        IEnumerable<CheatCode>? cheatCodes = null, Action<string>? logging = null,
        Func<DateTime>? reloj = null, Func<int, int>? seleccionarPosicion = null)
    {
        this.colores = new HashSet<char>(colores ?? "YCKPWIRBG");
        if (this.colores.Count < 2 || this.colores.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException("Configure al menos dos símbolos alfanuméricos.");
        cheats = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var cheat in cheatCodes ?? [])
        {
            if (string.IsNullOrWhiteSpace(cheat.Codigo) || cheat.IntentosDisponibles < 0 ||
                !cheats.TryAdd(cheat.Codigo, cheat.IntentosDisponibles))
                throw new ArgumentException("Configuración de cheat codes inválida.");
        }
        this.logging = logging ?? Console.WriteLine;
        this.reloj = reloj ?? (() => DateTime.Now);
        this.seleccionarPosicion = seleccionarPosicion ?? Random.Shared.Next;
    }

    public Resultado EvaluarIntento(string patronSecreto, string intentoJugador, string? cheatCode = null)
    {
        ArgumentNullException.ThrowIfNull(patronSecreto);
        ArgumentNullException.ThrowIfNull(intentoJugador);
        if (patronSecreto.Length is not (4 or 6))
            throw new ArgumentException("El tamaño debe ser 4 o 6.");
        if (patronSecreto.Length != intentoJugador.Length)
            throw new ArgumentException("El patrón y el intento deben tener el mismo tamaño.");
        if (patronSecreto.Any(c => !colores.Contains(c)) || intentoJugador.Any(c => !colores.Contains(c)))
            throw new ArgumentException("El patrón y el intento deben usar los colores configurados.");
        if (cheatCode is not null)
        {
            if (!cheats.TryGetValue(cheatCode, out int disponibles))
                throw new ArgumentException("Cheat code desconocido.");
            if (cheatUsado || disponibles == 0)
                throw new InvalidOperationException("El cheat code no tiene usos disponibles en esta partida.");
        }
        int exactos = 0;
        var restantes = new Dictionary<char, int>();
        for (int i = 0; i < patronSecreto.Length; i++)
        {
            if (patronSecreto[i] == intentoJugador[i]) exactos++;
            else restantes[patronSecreto[i]] = restantes.GetValueOrDefault(patronSecreto[i]) + 1;
        }
        int colores = 0;
        for (int i = 0; i < intentoJugador.Length; i++)
        {
            char color = intentoJugador[i];
            if (patronSecreto[i] != color && restantes.GetValueOrDefault(color) > 0)
            {
                colores++;
                restantes[color]--;
            }
        }
        Pista? pista = null;
        if (cheatCode is not null)
        {
            int posicion = seleccionarPosicion(patronSecreto.Length);
            if (posicion < 0 || posicion >= patronSecreto.Length)
                throw new InvalidOperationException("La posición aleatoria está fuera del patrón.");
            cheatUsado = true;
            cheats[cheatCode]--;
            bool sinSuerte = patronSecreto[posicion] == intentoJugador[posicion];
            pista = new Pista(posicion + 1, sinSuerte ? null : patronSecreto[posicion], sinSuerte);
        }
        logging($"En la fecha {reloj():dd-MM-yyyy HH:mm:ss} se evaluó el intento {intentoJugador} contra el patrón {patronSecreto}. Resultado: {exactos} bien posicionados – {colores} con color correcto.");
        return new Resultado(exactos, colores) { Cheat = pista };
    }
}
