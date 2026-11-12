using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Expendedora;

public class Program
{
    static readonly CultureInfo ESPANOL = new CultureInfo("es-ES");
    
    const int PrecioZumo = 140;
    const int PrecioGalletas = 90;
    const int PrecioPizza = 310;

    const int Capacidad = 9;
    
    const int Moneda10 = 10;
    const int Moneda20 = 20;
    const int Moneda50 = 50;
    const int Moneda1Euro = 100;
    const int Moneda2Euros = 200;
    
    /// <summary>
    /// Punto de entrada del programa: coordina el menú de la máquina expendedora.
    /// </summary>
    /// <param name="args">Argumentos de la línea de comandos (no se usan).</param>
    static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = ESPANOL;
        Console.OutputEncoding = Encoding.UTF8;

        // TODO: En el Bloque 2, agrupar el stock y las ventas en arrays para simplificar la gestión.
        int stockZumo = Capacidad;
        int stockGalletas = Capacidad;
        int stockPizza = Capacidad;

        int ventasZumo = 0;
        int ventasGalletas = 0;
        int ventasPizza = 0;
        int recaudacion = 0;

        bool apagada = false;

        do
        {
            MostrarMenu();
            string? opcion = Console.ReadLine();

            if (opcion is null)
            {
                Console.Error.WriteLine("Entrada cerrada: se apaga la máquina por falta de teclado.");
                break;
            }

            opcion = opcion.Trim();

            switch (opcion)
            {
                case "1":
                    if (!IntentarElegirProducto(out Producto prodCompra, out bool volverCompra))
                    {
                        Console.Error.WriteLine("Entrada cerrada: se apaga la máquina por falta de teclado.");
                        apagada = true;
                        break;
                    }

                    if (volverCompra)
                    {
                        break;
                    }

                    int stockActual = prodCompra switch
                    {
                        Producto.Zumo => stockZumo,
                        Producto.Galletas => stockGalletas,
                        _ => stockPizza
                    };

                    if (stockActual <= 0)
                    {
                        Console.Error.WriteLine($"No quedan unidades de {prodCompra}.");
                        break;
                    }

                    int precio = Precio(prodCompra);
                    int pagado = PedirMonedas(precio);

                    if (pagado == -2)
                    {
                        Console.Error.WriteLine("Entrada cerrada: se apaga la máquina por falta de teclado.");
                        apagada = true;
                        break;
                    }

                    if (pagado == -1)
                    {
                        break;
                    }

                    try
                    {
                        int cambio = Cambio(pagado, precio);
                        Console.WriteLine($"Retire su {prodCompra}. Cambio: {Formatear(cambio)}");

                        if (cambio > 0)
                        {
                            Console.WriteLine(Desglose(cambio));
                        }

                        recaudacion += precio;
                        if (prodCompra == Producto.Zumo)
                        {
                            stockZumo--;
                            ventasZumo++;
                        }
                        else if (prodCompra == Producto.Galletas)
                        {
                            stockGalletas--;
                            ventasGalletas++;
                        }
                        else
                        {
                            stockPizza--;
                            ventasPizza++;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.Error.WriteLine("No se ha podido completar el cálculo del cambio.");
                    }
                    break;

                case "2":
                    if (!IntentarElegirProducto(out Producto prodReponer, out bool volverReponer))
                    {
                        Console.Error.WriteLine("Entrada cerrada: se apaga la máquina por falta de teclado.");
                        apagada = true;
                        break;
                    }

                    if (volverReponer)
                    {
                        break;
                    }

                    Console.Write("Unidades: ");
                    string? textoUnidades = Console.ReadLine();

                    if (textoUnidades is null)
                    {
                        Console.Error.WriteLine("Entrada cerrada: se apaga la máquina por falta de teclado.");
                        apagada = true;
                        break;
                    }

                    if (!int.TryParse(textoUnidades, out int unidades))
                    {
                        Console.Error.WriteLine("La cantidad de unidades tiene que ser un número entero.");
                        break;
                    }

                    int stockAntes = prodReponer switch
                    {
                        Producto.Zumo => stockZumo,
                        Producto.Galletas => stockGalletas,
                        _ => stockPizza
                    };

                    try
                    {
                        int stockDespues = NuevoStock(stockAntes, unidades, Capacidad);
                        if (prodReponer == Producto.Zumo)
                        {
                            stockZumo = stockDespues;
                        }
                        else if (prodReponer == Producto.Galletas)
                        {
                            stockGalletas = stockDespues;
                        }
                        else
                        {
                            stockPizza = stockDespues;
                        }
                        Console.WriteLine($"Stock de {prodReponer}: {stockDespues} unidades.");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.Error.WriteLine(
                            $"No se puede reponer: la cantidad no puede ser negativa y cada canal admite como máximo {Capacidad} unidades.");
                    }
                    break;

                case "3":
                    MostrarInforme(
                        ventasZumo, stockZumo,
                        ventasGalletas, stockGalletas,
                        ventasPizza, stockPizza,
                        recaudacion);
                    break;

                case "0":
                    Console.Write("¿Seguro que quiere apagar la máquina? (S/N): ");
                    string? confirmacion = Console.ReadLine();
                    if (confirmacion is null)
                    {
                        Console.Error.WriteLine("Entrada cerrada: se apaga la máquina por falta de teclado.");
                        apagada = true;
                        break;
                    }

                    if (confirmacion.Trim().ToUpper() == "S")
                    {
                        apagada = true;
                        Console.WriteLine("Máquina apagada.");
                    }
                    break;

                default:
                    Console.Error.WriteLine("Opción no válida. Elige 1, 2, 3 o 0.");
                    break;
            }
        }
        while (!apagada);
    }
    
    /// <summary>
    /// Muestra los productos y lee la elección del usuario.
    /// </summary>
    /// <param name="producto">Producto elegido cuando es válido.</param>
    /// <param name="volver">Vale true si el usuario elige 0 o escribe un producto no válido para volver al menú.</param>
    /// <returns>false si la entrada estándar devuelve null; true en cualquier otro caso.</returns>
    static bool IntentarElegirProducto(out Producto producto, out bool volver)
    {
        producto = Producto.Zumo;
        volver = false;

        MostrarProductos();
        string? entrada = Console.ReadLine();
        if (entrada is null)
        {
            return false;
        }

        switch (entrada.Trim())
        {
            case "1":
                producto = Producto.Zumo;
                return true;
            case "2":
                producto = Producto.Galletas;
                return true;
            case "3":
                producto = Producto.Pizza;
                return true;
            case "0":
                volver = true;
                return true;
            default:
                Console.Error.WriteLine("Producto no válido.");
                volver = true;
                return true;
        }
    }

    /// <summary>
    /// Devuelve el precio de un producto de la máquina, en céntimos.
    /// </summary>
    /// <param name="producto">El producto del que se quiere saber el precio.</param>
    /// <returns>El precio del producto en céntimos (por ejemplo, 140 para el Zumo).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Si el producto no existe en la máquina.</exception>
    public static int Precio(Producto producto)
    {
        return producto switch
        {
            Producto.Zumo => PrecioZumo,
            Producto.Galletas => PrecioGalletas,
            Producto.Pizza => PrecioPizza,
            _ => throw new ArgumentOutOfRangeException(nameof(producto))
        };
    }

    /// <summary>
    /// Comprueba si la máquina admite una moneda.
    /// </summary>
    /// <param name="centimos">El valor de la moneda en céntimos.</param>
    /// <returns>
    /// true si la moneda es de 10, 20, 50, 100 o 200 céntimos; false en cualquier otro caso.
    /// </returns>
    public static bool EsMonedaValida(int centimos)
    {
        return centimos is Moneda10 or Moneda20 or Moneda50 or Moneda1Euro or Moneda2Euros;
    }
    
    /// <summary>
    /// Calcula el cambio que hay que devolver al cliente, en céntimos.
    /// </summary>
    /// <param name="pagado">El dinero introducido por el cliente, en céntimos.</param>
    /// <param name="precio">El precio del producto, en céntimos.</param>
    /// <returns>La diferencia entre lo pagado y el precio (0 si se paga justo).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Si lo pagado no llega al precio.</exception>
    public static int Cambio(int pagado, int precio)
    {
        if (pagado < precio)
        {
            throw new ArgumentOutOfRangeException(nameof(pagado), "Lo pagado no llega al precio.");
        }
        // TODO: Se deja pendiente comprobar si la máquina dispone de monedas suficientes, porque el enunciado actual asume cambio infinito.
        return pagado - precio;
    }
    
    /// <summary>
    /// Convierte un importe en céntimos a texto con formato de moneda española.
    /// </summary>
    /// <param name="centimos">El importe en céntimos.</param>
    /// <returns>El importe como texto, por ejemplo "1,40 €".</returns>
    public static string Formatear(int centimos)
    {
        return (centimos / 100m).ToString("C", ESPANOL);
    }
    
    /// <summary>
    /// Reparte un importe en el menor número de monedas, de mayor a menor valor.
    /// </summary>
    /// <param name="centimos">El importe a repartir, en céntimos. Múltiplo de 10.</param>
    /// <returns>Un texto como "1 x 0,20 € · 1 x 0,10 €".</returns>
    /// <exception cref="ArgumentOutOfRangeException">Si es negativo o no es múltiplo de 10.</exception>
    public static string Desglose(int centimos)
    {
        if (centimos < 0 || centimos % Moneda10 != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(centimos), "Importe no válido.");
        }

        string texto = "";
        int resto = centimos;

        // TODO: Se deja pendiente para el Bloque 2 sustituir el switch de monedas por un array constante, ya que en el Bloque 1 aún no se emplean colecciones.
        for (int paso = 0; paso < 5; paso++)
        {
            int moneda = paso switch
            {
                0 => Moneda2Euros,
                1 => Moneda1Euro,
                2 => Moneda50,
                3 => Moneda20,
                _ => Moneda10
            };

            int cuantas = resto / moneda;
            resto %= moneda;

            Debug.Assert(resto >= 0 && resto % Moneda10 == 0, "El resto debe seguir siendo múltiplo de 10.");

            if (cuantas > 0)
            {
                if (texto != "")
                {
                    texto += " · ";
                }
                texto += $"{cuantas} x {Formatear(moneda)}";
            }
        }

        Debug.Assert(resto == 0, "Todo el cambio debe haberse repartido en monedas.");

        return texto;
    }
    
    /// <summary>
    /// Calcula el nuevo stock de un producto después de una reposición.
    /// </summary>
    /// <param name="actual">Las unidades que hay actualmente.</param>
    /// <param name="cantidad">Las unidades que queremos añadir.</param>
    /// <param name="maximo">La capacidad máxima del producto.</param>
    /// <returns>El nuevo número de unidades.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Si la cantidad es negativa o se supera la capacidad máxima.
    /// </exception>
    public static int NuevoStock(int actual, int cantidad, int maximo)
    {
        if (cantidad < 0 || actual + cantidad > maximo)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidad), "Reposición no válida.");
        }

        return actual + cantidad;
    }
    
    /// <summary>
    /// Muestra en la consola las opciones del menú principal de la máquina.
    /// </summary>
    static void MostrarMenu()
    {
        Console.WriteLine("1. Comprar · 2. Reponer · 3. Informe · 0. Apagar");
        Console.Write("Opción: ");
    }

    /// <summary>
    /// Muestra la lista de productos con sus precios formateados y la opción de volver.
    /// </summary>
    static void MostrarProductos()
    {
        Console.WriteLine(
            $"1. Zumo {Formatear(PrecioZumo)} · " +
            $"2. Galletas {Formatear(PrecioGalletas)} · " +
            $"3. Pizza {Formatear(PrecioPizza)} · " +
            "0. Volver");
        Console.Write("Producto: ");
    }

    /// <summary>
    /// Pide monedas por consola una a una hasta alcanzar o superar el precio del producto, o cancelar.
    /// </summary>
    /// <param name="precio">El precio del producto a cobrar, en céntimos. Debe ser mayor que 0.</param>
    /// <returns>
    /// El total introducido en céntimos si se completa el pago; -1 si el usuario cancela con 0;
    /// o -2 si la entrada estándar se cierra (devuelve null).
    /// </returns>
    static int PedirMonedas(int precio)
    {
        Debug.Assert(precio > 0, "El precio a cobrar debe ser mayor que cero.");

        int pagado = 0;

        while (pagado < precio)
        {
            int falta = precio - pagado;
            Console.Write($"Faltan {Formatear(falta)}. Moneda en céntimos (0 cancela): ");
            string? entrada = Console.ReadLine();

            if (entrada is null)
            {
                return -2;
            }

            if (!int.TryParse(entrada, out int moneda))
            {
                Console.Error.WriteLine("Entrada no válida: escribe un número entero en céntimos.");
                continue;
            }

            if (moneda == 0)
            {
                Console.WriteLine($"Compra cancelada. Se devuelven {Formatear(pagado)}.");
                return -1;
            }

            if (!EsMonedaValida(moneda))
            {
                Console.Error.WriteLine(
                    $"Moneda no admitida: {Moneda10}, {Moneda20}, {Moneda50}, {Moneda1Euro} o {Moneda2Euros}.");
                continue; 
            }

            pagado += moneda;
        }

        return pagado;
    }
    
    /// <summary>
    /// Devuelve el producto con mayor número de ventas. En caso de empate, devuelve el primero de la lista.
    /// </summary>
    /// <param name="ventasZumo">Unidades vendidas de Zumo.</param>
    /// <param name="ventasGalletas">Unidades vendidas de Galletas.</param>
    /// <param name="ventasPizza">Unidades vendidas de Pizza.</param>
    /// <returns>El producto más vendido.</returns>
    public static Producto MasVendido(int ventasZumo, int ventasGalletas, int ventasPizza)
    {
        // Producto más vendido (en caso de empate, el primero de la lista)
        // TODO: Se deja pendiente recibir un array de ventas en lugar de tres parámetros separados.
        Producto mayor = Producto.Zumo;
        int maxVentas = ventasZumo;

        if (ventasGalletas > maxVentas)
        {
            mayor = Producto.Galletas;
            maxVentas = ventasGalletas;
        }

        if (ventasPizza > maxVentas)
        {
            mayor = Producto.Pizza;
        }

        return mayor;
    }

    /// <summary>
    /// Devuelve el producto con menor número de ventas. En caso de empate, devuelve el primero de la lista.
    /// </summary>
    /// <param name="ventasZumo">Unidades vendidas de Zumo.</param>
    /// <param name="ventasGalletas">Unidades vendidas de Galletas.</param>
    /// <param name="ventasPizza">Unidades vendidas de Pizza.</param>
    /// <returns>El producto menos vendido.</returns>
    public static Producto MenosVendido(int ventasZumo, int ventasGalletas, int ventasPizza)
    {
        // Estadística extra de la variante F (producto menos vendido)
        Producto menor = Producto.Zumo;
        int minVentas = ventasZumo;

        if (ventasGalletas < minVentas)
        {
            menor = Producto.Galletas;
            minVentas = ventasGalletas;
        }

        if (ventasPizza < minVentas)
        {
            menor = Producto.Pizza;
        }

        return menor;
    }

    /// <summary>
    /// Muestra en la consola el informe de ventas, stock, recaudación y estadísticas en columnas alineadas.
    /// </summary>
    /// <param name="ventasZumo">Unidades vendidas de Zumo.</param>
    /// <param name="stockZumo">Unidades disponibles de Zumo.</param>
    /// <param name="ventasGalletas">Unidades vendidas de Galletas.</param>
    /// <param name="stockGalletas">Unidades disponibles de Galletas.</param>
    /// <param name="ventasPizza">Unidades vendidas de Pizza.</param>
    /// <param name="stockPizza">Unidades disponibles de Pizza.</param>
    /// <param name="recaudacion">Recaudación total acumulada, en céntimos.</param>
    static void MostrarInforme(
        int ventasZumo, int stockZumo,
        int ventasGalletas, int stockGalletas,
        int ventasPizza, int stockPizza,
        int recaudacion)
    {
        // TODO: Permitir exportar el informe de ventas y recaudación a un fichero de texto externo.
        Console.WriteLine($"{"Producto",-10}{"Precio",8}{"Ventas",7}{"Stock",6}");
        Console.WriteLine($"{Producto.Zumo,-10}{Formatear(PrecioZumo),8}{ventasZumo,7}{stockZumo,6}");
        Console.WriteLine($"{Producto.Galletas,-10}{Formatear(PrecioGalletas),8}{ventasGalletas,7}{stockGalletas,6}");
        Console.WriteLine($"{Producto.Pizza,-10}{Formatear(PrecioPizza),8}{ventasPizza,7}{stockPizza,6}");
        Console.WriteLine($"Recaudación: {Formatear(recaudacion)}");
        Console.WriteLine($"Más vendido: {MasVendido(ventasZumo, ventasGalletas, ventasPizza)}");
        // R9: estadística extra de la variante F
        Console.WriteLine($"Menos vendido: {MenosVendido(ventasZumo, ventasGalletas, ventasPizza)}");
    }
}

public enum Producto
{
    Zumo,
    Galletas,
    Pizza
}