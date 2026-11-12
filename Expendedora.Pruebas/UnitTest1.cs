using System;
using Xunit;
using Expendedora;

namespace Expendedora.Pruebas;

public class PruebasExpendedora
{
    
    [Theory]
    [InlineData(Producto.Zumo, 140)]
    [InlineData(Producto.Galletas, 90)]
    [InlineData(Producto.Pizza, 310)]
    public void Precio_ProductoValido_DevuelvePrecioEnCentimos(Producto producto, int esperado)
    {
        Assert.Equal(esperado, Program.Precio(producto));
    }

    [Fact]
    public void Precio_ProductoFueraDelEnum_LanzaArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.Precio((Producto)99));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(200)]
    public void EsMonedaValida_MonedaAdmitida_DevuelveTrue(int centimos)
    {
        Assert.True(Program.EsMonedaValida(centimos));
    }

    [Theory]
    [InlineData(0)]   
    [InlineData(-10)] 
    [InlineData(9)]    
    [InlineData(11)]   
    [InlineData(199)]  
    [InlineData(201)]  
    public void EsMonedaValida_MonedaNoAdmitidaOLimite_DevuelveFalse(int centimos)
    {
        Assert.False(Program.EsMonedaValida(centimos));
    }

    
    [Fact]
    public void Cambio_PagoExacto_DevuelveCero()
    {
        Assert.Equal(0, Program.Cambio(140, 140));
    }

    [Theory]
    [InlineData(100, 90, 10)] 
    [InlineData(200, 140, 60)]
    public void Cambio_PagadoSuficiente_DevuelveDiferencia(int pagado, int precio, int esperado)
    {
        Assert.Equal(esperado, Program.Cambio(pagado, precio));
    }

    [Theory]
    [InlineData(139, 140)]
    [InlineData(0, 90)]
    public void Cambio_PagadoInsuficiente_LanzaArgumentOutOfRangeException(int pagado, int precio)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.Cambio(pagado, precio));
    }

    
    [Fact]
    public void Desglose_CeroCentimos_DevuelveCadenaVacia()
    {
        Assert.Equal("", Program.Desglose(0));
    }

    [Theory]
    [InlineData(30, "1 x 0,20 € · 1 x 0,10 €")]
    [InlineData(380, "1 x 2,00 € · 1 x 1,00 € · 1 x 0,50 € · 1 x 0,20 € · 1 x 0,10 €")]
    public void Desglose_ImporteMultiploDeDiez_DevuelveMenorNumeroDeMonedas(int centimos, string esperado)
    {
        Assert.Equal(esperado, Program.Desglose(centimos));
    }

    [Theory]
    [InlineData(-10)] 
    [InlineData(5)]   
    public void Desglose_NegativoONoMultiploDeDiez_LanzaArgumentOutOfRangeException(int centimos)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.Desglose(centimos));
    }

    
    [Theory]
    [InlineData(5, 0, 9, 5)] 
    [InlineData(5, 4, 9, 9)] 
    public void NuevoStock_ReposicionValida_DevuelveSumaCorrecta(int actual, int cantidad, int maximo, int esperado)
    {
        Assert.Equal(esperado, Program.NuevoStock(actual, cantidad, maximo));
    }

    [Theory]
    [InlineData(5, -1, 9)] 
    [InlineData(5, 5, 9)]  
    public void NuevoStock_CantidadNegativaOSuperaMaximo_LanzaArgumentOutOfRangeException(int actual, int cantidad, int maximo)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.NuevoStock(actual, cantidad, maximo));
    }
}