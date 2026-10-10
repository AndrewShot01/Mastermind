namespace Mastermind.Core;

public sealed record Resultado(int BienPosicionados, int ColorCorrecto);

public sealed class Evaluador
{
    public Resultado EvaluarIntento(string patronSecreto, string intentoJugador)
    {
        ArgumentNullException.ThrowIfNull(patronSecreto);
        ArgumentNullException.ThrowIfNull(intentoJugador);
        if (patronSecreto.Length != intentoJugador.Length)
            throw new ArgumentException("El patrón y el intento deben tener el mismo tamaño.");
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
        return new Resultado(exactos, colores);
    }
}
