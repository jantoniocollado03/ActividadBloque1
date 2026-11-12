# Máquina Expendedora del Instituto — Actividad Bloque 1

Este proyecto es una aplicación de consola en C# para el módulo de **Programación (1.º DAM)**. El programa simula el funcionamiento de la máquina expendedora de la cafetería del instituto: permite comprar productos metiendo monedas, devuelve el cambio con el menor número de monedas posible, controla el stock disponible y muestra un informe de las ventas.

## 1. Mi variante (Variante F)

El programa está configurado con los datos de la **Variante F**:

| Producto | Precio | Capacidad máxima |
| :--- | :--- | :--- |
| **1. Zumo** | 1,40 € (140 céntimos) | 9 unidades |
| **2. Galletas** | 0,90 € (90 céntimos) | 9 unidades |
| **3. Pizza** | 3,10 € (310 céntimos) | 9 unidades |

* **Monedas que acepta la máquina:** 10, 20 y 50 céntimos, y 1 € (100 céntimos) y 2 € (200 céntimos).
* **Estadística extra en el informe:** Producto menos vendido (si hay empate, muestra el primero de la lista).

## 2. Estructura de carpetas y archivos

La solución `ActividadBloque1.sln` tiene dos proyectos (el programa principal y el de las pruebas unitarias) además de las capturas pedidas:

```text
ActividadBloque1/
├── .gitignore
├── .dockerignore
├── Dockerfile
├── ActividadBloque1.sln
├── README.md
├── respuestas.md
├── evidencias/
│   ├── 01-ejecucion.png
│   ├── 02-pruebas.png
│   ├── 03-depuracion.png
│   ├── 04-tareas.png
│   └── 05-contenedor.png
├── Expendedora/
│   ├── Expendedora.csproj
│   └── Program.cs
└── Expendedora.Pruebas/
    ├── Expendedora.Pruebas.csproj
    └── PruebasExpendedora.cs
```

## 3. Cómo ejecutar el programa

### Desde Visual Studio o Rider
1. Abre el archivo de la solución `ActividadBloque1.sln`.
2. Asegúrate de que el proyecto **`Expendedora`** está seleccionado para ejecutarse.
3. Dale al botón de ejecutar (**Run** o `Ctrl + F5`).
4. También se puede arrancar desde la terminal en la carpeta de la solución con:
   ```bash
   dotnet run --project Expendedora/Expendedora.csproj
   ```

### Desde Docker
La imagen del programa está subida a Docker Hub. Con **Docker Desktop** abierto, pega este comando en la terminal para descargarla y usar la máquina:

```bash
docker run -it --rm usuario/expendedora-bloque1:1.0
```
*(Recuerda cambiar `usuario` por tu nombre de usuario de Docker Hub).*

Si quieres crear la imagen en tu propio ordenador usando el `Dockerfile` del proyecto:
```bash
docker build -t usuario/expendedora-bloque1:1.0 .
```

> **Nota sobre `-it`:** Como el programa lee lo que escribes por teclado, hay que poner `-it`. Si se ejecuta sin eso, `ReadLine` devuelve `null` y el programa avisa y se cierra solo para no quedarse repitiendo el menú sin parar.

## 4. Cómo se usa

Nada más arrancar, la máquina empieza llena (9 unidades de cada producto) y sale el menú principal, que se va repitiendo hasta que eliges apagarla:

```text
1. Comprar · 2. Reponer · 3. Informe · 0. Apagar
Opción:
```

* **`1. Comprar`**:
  * Sale la lista de productos con su precio (`1. Zumo 1,40 € · 2. Galletas 0,90 € · 3. Pizza 3,10 € · 0. Volver`).
  * Si quedan unidades del producto elegido, te va pidiendo monedas en céntimos una a una y te dice cuánto dinero falta.
  * Si metes una moneda que no existe (como `30`) o escribes letras, te avisa del error y te vuelve a pedir moneda sin perder lo que ya habías echado.
  * Si escribes `0`, se cancela la compra y te devuelve el dinero que habías metido.
  * Cuando llegas al precio o te pasas, te da el producto, resta 1 al stock, suma la venta y, si sobra dinero, te dice el cambio y qué monedas te devuelve.

* **`2. Reponer`**:
  * Eliges el producto que quieres rellenar y escribes cuántas unidades vas a meter.
  * Si pones un número negativo, letras, o al sumar las unidades se pasa del máximo de 9, no repone nada y te muestra un aviso.

* **`3. Informe`**:
  * Muestra una tabla con los tres productos, su precio, cuántas ventas lleva cada uno y cuánto stock queda.
  * Debajo enseña el dinero total recaudado, cuál es el producto más vendido y cuál es el menos vendido.

* **`0. Apagar`**:
  * Pregunta si estás seguro de apagar la máquina (`S/N`).
  * Si escribes `S` o `s`, el programa termina. Si escribes cualquier otra cosa, vuelve al menú.

## 5. Cómo ejecutar las pruebas

Las pruebas unitarias están en el proyecto **`Expendedora.Pruebas`** hechas con **xUnit**. Prueban los métodos de cálculo (`Precio`, `EsMonedaValida`, `Cambio`, `Desglose` y `NuevoStock`), incluyendo casos límite y errores.

* **En Visual Studio / Rider:** Abre el panel de pruebas (**Explorador de pruebas** o **Unit Tests**) y dale al botón de ejecutar todas (**Run All**).
* **Por consola:** Abre una terminal en la carpeta principal y escribe:
  ```bash
  dotnet test
  ```