using FitArmLog.Models;
using FitArmLog.Validation;
using Xunit;

namespace FitArmLog.Tests.Validation;

public class AppValidatorTests
{
    private static readonly Exercise Press = new() { Name = "Press banca" };

    // ---------- Rutinas ----------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateRoutine_NombreVacio_EsInvalido(string? name)
    {
        var result = AppValidator.ValidateRoutine(name, 2);

        Assert.False(result.IsValid);
        Assert.Contains("nombre", result.FirstError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateRoutine_NombreMuyLargo_EsInvalido()
    {
        var result = AppValidator.ValidateRoutine(new string('a', 101), 2);

        Assert.False(result.IsValid);
        Assert.Contains("100", result.FirstError);
    }

    [Fact]
    public void ValidateRoutine_SinEjercicios_EsInvalido()
    {
        var result = AppValidator.ValidateRoutine("Pecho", 0);

        Assert.False(result.IsValid);
        Assert.Contains("ejercicio", result.FirstError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateRoutine_DatosCorrectos_EsValido()
    {
        var result = AppValidator.ValidateRoutine("Pecho", 3);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateRoutine_NombreDeExactamente100Caracteres_EsValido()
    {
        var result = AppValidator.ValidateRoutine(new string('a', 100), 1);

        Assert.True(result.IsValid);
    }

    // ---------- Series ----------

    [Theory]
    [InlineData("0")]
    [InlineData("-3")]
    [InlineData("101")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateSet_RepeticionesInvalidas_EsInvalido(string? reps)
    {
        var result = AppValidator.ValidateSet(Press, reps, "40", out _, out _);

        Assert.False(result.IsValid);
        Assert.Contains("repeticiones", result.FirstError, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("10")]
    [InlineData("100")]
    public void ValidateSet_RepeticionesEnRango_EsValido(string reps)
    {
        var result = AppValidator.ValidateSet(Press, reps, "40", out var parsedReps, out _);

        Assert.True(result.IsValid);
        Assert.Equal(int.Parse(reps), parsedReps);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("501")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateSet_PesoInvalido_EsInvalido(string? weight)
    {
        var result = AppValidator.ValidateSet(Press, "10", weight, out _, out _);

        Assert.False(result.IsValid);
        Assert.Contains("peso", result.FirstError, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("0", 0)]          // peso corporal o calentamiento: regla de negocio
    [InlineData("42.5", 42.5)]
    [InlineData("42,5", 42.5)]    // en Argentina se escribe con coma
    [InlineData(" 20 ", 20)]
    [InlineData("500", 500)]
    public void ValidateSet_PesoValido_ParseaCorrectamente(string text, double expected)
    {
        var result = AppValidator.ValidateSet(Press, "10", text, out _, out var weight);

        Assert.True(result.IsValid);
        Assert.Equal(expected, weight);
    }

    [Fact]
    public void ValidateSet_SinEjercicio_EsInvalido()
    {
        var result = AppValidator.ValidateSet(null, "10", "40", out _, out _);

        Assert.False(result.IsValid);
        Assert.Contains("ejercicio", result.FirstError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateSet_TodoMal_AcumulaTresErrores()
    {
        var result = AppValidator.ValidateSet(null, "0", "-1", out _, out _);

        Assert.Equal(3, result.Errors.Count);
    }

    // ---------- Sesiones ----------

    [Fact]
    public void ValidateSession_SinSeries_EsInvalido()
    {
        var result = AppValidator.ValidateSession(new List<SetEntry>(), null);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidateSession_ConUnaSerie_EsValido()
    {
        var sets = new List<SetEntry> { new() { ExerciseName = "Press", Reps = 10, WeightKg = 40 } };

        var result = AppValidator.ValidateSession(sets, null);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateSession_NotasMuyLargas_EsInvalido()
    {
        var sets = new List<SetEntry> { new() { Reps = 10, WeightKg = 40 } };

        var result = AppValidator.ValidateSession(sets, new string('x', 501));

        Assert.False(result.IsValid);
        Assert.Contains("500", result.FirstError);
    }
}
