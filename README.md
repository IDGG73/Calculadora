# Calculadora | Nancy Ximena Gaytán Guerra

¡Hola! Este es mi proyecto de Matemáticas, una calculadora sencilla pero eficiente. La aplicación está hecha utilizando el motor de videojuegos `Unity`, el lenguaje de programación `C#`,  la librería [`NCalc`](https://github.com/ncalc/ncalc) y la ayuda de mi hermano.

## Características de la Calculadora
Algunas de las características que diferencian a esta calculadora del resto en esta clase son (probablemente):
* **Historial:** Muestra todas las operaciones que se han realizado en la sesión actual.
* **Interfaz:** Las funciones de la calculadora, operadores y números se distinguen por colores, facilitando su lectura.
* **Audio:** Los botones en pantalla hacen ruido de un teclado mécanico. Puede desactivarse.
* **Controles:** La mayoría de funciones básicas no necesitan de un mouse y pueden ejecutarse con el teclado. [Ver más](#controles).
* **Responsivo:** El contenido se ajusta al tamaño y forma de la ventana.
* **Grados y Radianes:** Utiliza la unidad que prefieras para las funciones de Seno, Coseno y Tangente.
* **Botón π:** Otorga acceso rápido al valor de `π (Pi)`, no es necesario escribirlo manualmente.
* **Código Abierto:** El código de esta aplicación está disponible para todo público. Puede descargarse y editarse.
* **Online:** La aplicación puede utilizarse sin descargar nada. Sólo se necesita una conexión a internet.

## ¿Por qué Unity?
He utilizado el motor de videojuegos `Unity` por su facilidad de uso, capacidad de crear las aplicaciones para distintas plataformas y sus herramientas para creación de interfaces rápidas e intuitivas. En general, es mucho más rápido crear algo con `Unity` que con `Visual Studio` y `XAML`.


Claro que, al estar hecha con un motor para videojuegos, esta calculadora técnicamente **ES** un videojuego, por lo que consume más recursos que una aplicación de verdad. Pero para el propósito de este proyecto, no es de mucha importancia.


## ¿Qué es NCalc?
NCalc es una librería desarrollada por [Sébastien Ros](https://github.com/sebastienros) y [Alexey Yakovlev](https://github.com/yallie) capaz de convertir problemas matemáticos de texto (Llamado [`String`](https://learn.microsoft.com/es-mx/dotnet/csharp/programming-guide/strings/) en `C#`) a números (Llamado [`Double`](https://learn.microsoft.com/es-mx/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types) en `C#`).

Esta librería se encarga de interpretar el texto introducido por el usuario y transformarlo en una variable con la que se puede trabajar para **resolver la operación**.

Como dije antes, el texto es un `string` mientras que los números son `double`; al ser tipos de variables diferentes, no son compatibles el uno con el otro.

Los símbolos como `+`, `-`, `x`, `÷` **no son números, sino texto**. Por lo que algo tan simple como `1 + 1` **no puede resolverse**. No sin un algoritmo más eficiente que pueda diferenciar entre signos, números negativos, contenido entre paréntesis, jerarquía de operaciones...

¡Y ahí es donde entra NCalc!

NCalc se encarga de interpretar todo lo que escribe el usuario y lo separa entre números y símbolos. Después resuelve las operaciones básicas como `+`, `-`, `x`, `÷`.

Las operaciones más complejas como `Seno`, `Coseno`, `Tangente` y `Raíz Cuadrada` son un poco más especiales, **porque pueden escribirse como una sola función, pero en realidad tienen qué hacer muchas más operaciones detrás de escena**. Estas operaciones se resuelven usando otra librería llamada `System.Math` que viene incluida por defecto en `C#`.

#### Cambios Hechos Por Mí (Y Mi Hermano)

Hemos tenido que modificar NCalc para que fuese compatible con el entorno de `Unity`.

También tuvimos que agregar soporte para `° (Grados)` en las funciones de `Seno`, `Coseno` y`Tangente`, ya que la librería originalmente sólo maneja `Radianes`.

Y por último cambiamos la sintaxis de los exponentes; originalmente sólo se admitía el uso de `Pow(x, y)` (donde `x` es el número base y `y` es la potencia). Cambiamos eso por algo más entendible: `x^y`.

***

## Controles

Las funciones más básicas (como escribir números y signos) pueden ejecutarse mediante atajos en el teclado. Funciones más complejas o muy específicas (como `Seno`, `Coseno`, `Tangente`, `Raíz Cuadrada` y `π (Pi)`) necesitan que uses el mouse para hacer clic en los botones de la pantalla.

**Todas las teclas de números y signos funcionan como es de esperarse**.

Estos son algunos de los atajos de teclado disponibles para la aplicación:

| Teclas      | Función |
| ----------- | ----------- |
| `TAB`, `Flecha Izquierda`| Abrir y cerrar la Bandeja de Funciones Avanzadas |
| `Flecha Derecha` | Cerrar la Bandeja de Funciones Avanzadas |
| `CTRL`, `Menu Contextual`, `Menu`, `Context Menu` | Abrir y cerrar el historial |
| `S`, `M` | Activar y desactivar ruido de tecleo |
| `Esc`, `Del`  , `Supr`, `Suprimir` | Borrar todo lo que escribiste |
| `Retroceso`, `Backspace` | Borrar el último carácter escrito |
| `Enter`, `Intro`, `Return` | Calcular |
