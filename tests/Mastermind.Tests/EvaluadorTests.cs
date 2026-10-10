using Mastermind.Core;
using Xunit;

namespace Mastermind.Tests;

public class EvaluadorTests
{
    [Theory]
    [InlineData("RGBY", "RGBY", 4, 0)]
    [InlineData("RGBY", "RBYG", 1, 3)]
    [InlineData("RGBY", "CCCC", 0, 0)]
    [InlineData("RRGG", "RRRR", 2, 0)]
    [InlineData("RRGB", "GRRR", 1, 2)]
    [InlineData("RRGG", "GGRR", 0, 4)]
    public void CuentaSinDuplicar(string secreto, string intento, int exactos, int colores)
    {
        Assert.Equal(new Resultado(exactos, colores), new Evaluador().EvaluarIntento(secreto, intento));
    }
}
