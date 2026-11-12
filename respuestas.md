
1. ¿Por qué los importes se guardan en céntimos como int y no en euros como double? Pon un ejemplo
   con tus precios de lo que podría salir mal con double. ¿En qué líneas conviertes de céntimos a euros y qué
   tipo de conversión es?

Double da errores de redondeo en binario: pagar las Galletas (0,90 €) con tres monedas de 0,20 y tres de 0,10 daría 0.8999999999999999 y faltaría dinero.
Paso a euros con una conversión implícita de int a decimal.

2. Explica paso a paso cómo calcula tu método Desglose el cambio de 3,80 €. ¿Qué operadores usas y por
   qué?

Recibe 380 y en el for divide sucesivamente entre 200, 100, 50, 20 y 10, sacando 1 moneda de cada tipo (sobrando 180, 80, 30, 10 y 0 en cada paso).
Uso / para calcular cuántas monedas caben y %= para actualizar el resto pendiente.

3. ¿Qué bucle usas para pedir las monedas y por qué ese y no otro? ¿Dónde usas continue y qué pasaría si
   lo quitaras?

Uso while (pagado < precio) porque de entrada no sé cuántas monedas meterá el usuario hasta cubrir el precio.
Los continue saltan la vuelta si mete letras o monedas falsas; sin ellos, llegarían a pagado += moneda y se sumarían al total.

4. En tu menú, ¿de qué sale cada break? ¿Qué hace que el programa termine?

El break de la línea 53 sale del bucle do-while si la entrada es null, y todos los demás break salen solo del switch para volver al menú.
El programa termina cuando apagada es true, al confirmar "S", saliendo del while (!apagada).

5. ¿Por qué Cambio y NuevoStock lanzan excepciones en lugar de mostrar un mensaje? ¿Dónde las
   capturas?

Lanzan excepciones para separar la lógica de la consola y poder probar los fallos con tests unitarios sin que el programa siga con datos erróneos.
Las capturo en Main con dos try-catch (ArgumentOutOfRangeException): (Cambio) y (NuevoStock).

6. Señala tus tres aserciones y di cuál es la precondición, cuál la postcondición y cuál el invariante. ¿Por qué
   no son excepciones?

La precondición es Debug.Assert(precio > 0), el invariante resto >= 0 && resto % Moneda10 == 0 y la postcondición resto == 0.
No son excepciones porque vigilan posibles bugs míos al programar, no datos inválidos del exterior.

7. ¿Qué casos límite has probado en NuevoStock y en EsMonedaValida? ¿Por qué esos?

En NuevoStock probé añadir 0, llenar exacto al tope 9, pasarse a 10 y meter -1 para cubrir los bordes de la condición del if.
En EsMonedaValida probé las 5 monedas válidas (10, 20, 50, 100, 200) y valores límite inválidos como 0, 5, 25 y -10.

8. Cuenta un error que hayas tenido mientras hacías la actividad y cómo lo encontraste: con una prueba, con
   el depurador o con una aserción.

Al cerrar la entrada en Docker, Console.ReadLine() devolvía null y el menú entraba en bucle infinito.
Lo vi con el depurador y lo arreglé comprobando if (opcion is null) y devolviendo -2 en PedirMonedas para activar apagada = true.

9. ¿Qué ocurre con tus aserciones dentro del contenedor, que se compila en Release? ¿Es un problema?
   ¿Por qué?

En modo Release el compilador ignora y elimina los Debug.Assert, por lo que no se ejecutan en el contenedor.
No es un problema porque solo sirven para detectar bugs durante el desarrollo; los errores reales siguen protegidos con if y excepciones.

10. ¿Por qué hace falta fijar la cultura en Main y en Formatear? Describe qué verías en el contenedor y qué
    pasaría con tus pruebas si no lo hicieras.

El contenedor usa cultura en inglés/invariante por defecto y los tests llaman a Formatear directamente sin pasar por Main.
Sin fijar ESPANOL en ambos sitios, saldrían puntos o símbolos como ¤1.40 en vez de "1,40 €" y fallarían los tests.