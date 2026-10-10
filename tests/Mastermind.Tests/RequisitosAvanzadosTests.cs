using Mastermind.Core;
using Xunit;

namespace Mastermind.Tests;

public class RequisitosAvanzadosTests
{
    private static Evaluador Partida(int posicion = 1) => new(cheatCodes:
        [new("PISTA", 2), new("OTRA", 1), new("AGOTADO", 0)], logging: _ => { }, seleccionarPosicion: _ => posicion);

    [Theory]
    [InlineData("", "")]
    [InlineData("RGB", "RGB")]
    [InlineData("RGBYY", "RGBYY")]
    [InlineData("RGBY", "RGB")]
    [InlineData("RGBZ", "RGBY")]
    [InlineData("RGBY", "rgbY")]
    public void RechazaEntradasInvalidas(string secreto, string intento) =>
        Assert.Throws<ArgumentException>(() => Partida().EvaluarIntento(secreto, intento));

    [Fact]
    public void RechazaNull()
    {
        Assert.Throws<ArgumentNullException>(() => Partida().EvaluarIntento(null!, "RGBY"));
        Assert.Throws<ArgumentNullException>(() => Partida().EvaluarIntento("RGBY", null!));
    }

    [Fact]
    public void AceptaSeisPosiciones() => Assert.Equal(new Resultado(6, 0), Partida().EvaluarIntento("RGBYCK", "RGBYCK"));

    [Fact]
    public void ColoresDinamicos()
    {
        var evaluador = new Evaluador("12", logging: _ => { });
        Assert.Equal(new Resultado(2, 2), evaluador.EvaluarIntento("1122", "1212"));
        Assert.Throws<ArgumentException>(() => evaluador.EvaluarIntento("RGBY", "RGBY"));
    }

    [Fact]
    public void PistaValidaNoModificaConteo()
    {
        var resultado = Partida().EvaluarIntento("RGBY", "RBYG", "PISTA");
        Assert.Equal(1, resultado.BienPosicionados);
        Assert.Equal(3, resultado.ColorCorrecto);
        Assert.Equal(new Pista(2, 'G', false), resultado.Cheat);
    }

    [Fact]
    public void UnUsoPorPartidaInclusoConOtroCodigo()
    {
        var evaluador = Partida();
        evaluador.EvaluarIntento("RGBY", "RBYG", "PISTA");
        Assert.Throws<InvalidOperationException>(() => evaluador.EvaluarIntento("RGBY", "RBYG", "PISTA"));
        Assert.Throws<InvalidOperationException>(() => evaluador.EvaluarIntento("RGBY", "RBYG", "OTRA"));
        Assert.Equal(new Resultado(4, 0), evaluador.EvaluarIntento("RGBY", "RGBY"));
    }

    [Fact]
    public void SinSuerteConsumeCheat()
    {
        var evaluador = Partida(0);
        Assert.Equal(new Pista(1, null, true), evaluador.EvaluarIntento("RGBY", "RBYG", "PISTA").Cheat);
        Assert.Throws<InvalidOperationException>(() => evaluador.EvaluarIntento("RGBY", "RBYG", "PISTA"));
    }

    [Fact]
    public void RechazaDesconocidoYAgotado()
    {
        var evaluador = Partida();
        Assert.Throws<ArgumentException>(() => evaluador.EvaluarIntento("RGBY", "RBYG", "NO"));
        Assert.Throws<InvalidOperationException>(() => evaluador.EvaluarIntento("RGBY", "RBYG", "AGOTADO"));
        Assert.NotNull(evaluador.EvaluarIntento("RGBY", "RBYG", "PISTA").Cheat);
    }

    [Fact]
    public void IntentoInvalidoNoConsumeCheat()
    {
        var evaluador = Partida();
        Assert.Throws<ArgumentException>(() => evaluador.EvaluarIntento("RGBY", "ZZZZ", "PISTA"));
        Assert.NotNull(evaluador.EvaluarIntento("RGBY", "RBYG", "PISTA").Cheat);
    }

    [Fact]
    public void NuevaPartidaTieneSuPropioUso()
    {
        Partida().EvaluarIntento("RGBY", "RBYG", "PISTA");
        Assert.NotNull(Partida().EvaluarIntento("RGBY", "RBYG", "PISTA").Cheat);
    }

    [Fact]
    public void LoggingConFechaYResultado()
    {
        var mensajes = new List<string>();
        var evaluador = new Evaluador(logging: mensajes.Add, reloj: () => new DateTime(2026, 10, 10, 9, 8, 7));
        evaluador.EvaluarIntento("RGBY", "RBYG");
        Assert.Equal("En la fecha 10-10-2026 09:08:07 se evaluó el intento RBYG contra el patrón RGBY. Resultado: 1 bien posicionados – 3 con color correcto.", Assert.Single(mensajes));
        Assert.Throws<ArgumentException>(() => evaluador.EvaluarIntento("RGBY", "ZZZZ"));
        Assert.Single(mensajes);
    }

    [Fact]
    public void CargaJsonYRechazaConfiguracionInvalida()
    {
        string ruta = Path.GetTempFileName();
        try
        {
            File.WriteAllText(ruta, """{"colores":{"1":"Rojo","2":"Azul"},"cheatCodes":[{"codigo":"PISTA","intentosDisponibles":1}]}""");
            var config = Configuracion.Cargar(ruta);
            Assert.Equal(2, config.Colores.Count);
            Assert.Equal(new CheatCode("PISTA", 1), Assert.Single(config.CheatCodes));
            File.WriteAllText(ruta, """{"colores":{"XX":"Rojo"},"cheatCodes":[]}""");
            Assert.Throws<ArgumentException>(() => Configuracion.Cargar(ruta));
        }
        finally { File.Delete(ruta); }
    }

    [Fact]
    public void RechazaCheatsDuplicadosYNegativos()
    {
        Assert.Throws<ArgumentException>(() => new Evaluador(cheatCodes: [new("X", 1), new("X", 2)]));
        Assert.Throws<ArgumentException>(() => new Evaluador(cheatCodes: [new("X", -1)]));
    }
}
