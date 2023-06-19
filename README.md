# Calculadora | Nancy Ximena Gaytán Guerra

> **NOTA 1:** Esta calculadora ha sido diseñada para pantallas verticales. Para una mejor experiencia, ajuste la ventana de su navegador para que simule la forma de una pantalla de teléfono.

> **NOTA 2:** Para acceder a las fórmulas, presione el **botón verde** que dice **"Fórmulas"** dentro de la aplicación.

¡Hola! Este es mi proyecto de Matemáticas, una calculadora sencilla pero eficiente. La aplicación está hecha utilizando el motor de videojuegos `Unity`, el lenguaje de programación `C#`,  la librería [`NCalc`](https://github.com/ncalc/ncalc) y la ayuda de mi hermano.

### Índice
* [Características de la Calculadora](#características-de-la-calculadora)
* [¿Por qué Unity?](#por-qué-unity)
* [¿Qué es NCalc?](#qué-es-ncalc)
* [Funcionamiento de las Fórmulas](#funcionamiento-de-las-fórmulas)
* [Atajos de Teclado](#atajos-de-teclado)

## Características de la Calculadora
Algunas de las características de esta calculadora:
* **Historial:** Muestra todas las operaciones que se han realizado en la sesión actual.
* **Interfaz:** Las funciones de la calculadora, operadores y números se distinguen por colores, facilitando su lectura.
* **Audio:** Los botones en pantalla hacen ruido de un teclado mécanico. Puede desactivarse.
* **Controles:** La mayoría de funciones básicas no necesitan de un mouse y pueden ejecutarse con el teclado. [Ver más](#atajos-de-teclado).
* **Responsivo:** El contenido se ajusta al tamaño y forma de la ventana.
* **Grados y Radianes:** Utiliza la unidad que prefieras para las funciones de Seno, Coseno y Tangente.
* **Botón π:** Otorga acceso rápido al valor de `π (Pi)`, no es necesario escribirlo manualmente.
* **Código Abierto:** El código de esta aplicación está disponible para todo público. Puede descargarse y editarse.
* **Online:** La aplicación puede utilizarse sin descargar nada. Sólo se necesita una conexión a internet.
* **Offline:** Si lo prefieres (y si tu navegador te lo permite) puedes *instalar* la aplicación y usarla sin conexión a internet. En teléfono también debería ser posible, aunque no lo hemos probado.

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

## Funcionamiento de las Fórmulas

Aunque NCalc es muy útil para operaciones, no puede resolver las fórmulas por sí mismo, por lo que eso tenemos que manejarlo nosotros.

Cada fórmula cuenta con variables a las que debes asignarles un valor, algunas necesitan 2 variables, otras sólo 3 y algunas piden hasta 6.

En el menú `Parámetros` de la aplicación es donde el usuario introduce el valor de cada variable. Dependiendo de qué formula escogiste en el menú anterior, se mostrará en pantalla la cantidad de variables que necesita y sus nombres.

Una vez asignados los valores, se crea una lista de ellos y se envía al método `Calculator.CalculateFormula(Formulas, FormulaArgumentBinder[])`. Dentro de este método se llevan acabo los siguientes pasos:

> **Nota:** Dentro del código, las variables de las fórmulas son referidas por el nombre `Argumentos`

1. **[Empty Arguments](https://github.com/IDGG73/Calculadora/blob/af338753cfc3a748eb3bb92b5d1fa9f2d27c0862/Codigo%20Fuente/Codigo%20de%20la%20App/Scripts/Calculator.cs#LL320C32-L320C32) (Argumentos Vacíos):** Revisa que los argumentos dados por el usuario NO estén vacíos.
2. **[Arguments Count](https://github.com/IDGG73/Calculadora/blob/af338753cfc3a748eb3bb92b5d1fa9f2d27c0862/Codigo%20Fuente/Codigo%20de%20la%20App/Scripts/Calculator.cs#LL330C9-L330C9) (Conteo de Argumentos):** Revisa que la cantidad de argumentos dados por el sistema sea la misma cantidad de argumentos que pide la fórmula.
3. **[Arguments Evaluation](https://github.com/IDGG73/Calculadora/blob/af338753cfc3a748eb3bb92b5d1fa9f2d27c0862/Codigo%20Fuente/Codigo%20de%20la%20App/Scripts/Calculator.cs#LL346C9-L346C9) (Evaluación de Argumentos):** Ya que el usuario puede introducir una operación matemática como argumento, necesitamos resolverla *antes* de pasarla a la fórmula.
	* Tomamos la operación matemática de la lista de Argumentos con `args[i].Argument`
	* Creamos una lista de tipo `CalculatorResult` llamada `argsResults` a donde enviaremos los resultados de las operaciones de cada argumento.
	* Ahora, con el método `Calculator.Evaluate()` podemos resolver la operación y agregar su resultado a la lista `argsResults`. Creamos un nuevo `CalculatorResult` llamado `cR` y usamos `cR = Evaluate(args[i].argument);`
	* El resultado ahora está guardado en la variable `cR`, si `cR.success` es `verdadero` significa que la operación se completó exitosamente, así que ya podemos añadir el resultado a la lista `argsResults` y repetir el proceso por cada argumento que necesita la fórmula.
4. **[Evaluate Formula](https://github.com/IDGG73/Calculadora/blob/af338753cfc3a748eb3bb92b5d1fa9f2d27c0862/Codigo%20Fuente/Codigo%20de%20la%20App/Scripts/Calculator.cs#LL369C9-L369C9) (Evaluar Formula):** Si los pasos anteriores son exitosos, ahora sí podemos calcular la fórmula con los argumentos asignados.
	* Usamos un `switch()` para dividir el código en distintas posibilidades. Necesitamos hacer esto porque cada fórmula debe llevarnos a un algoritmo distinto.
	* Una vez en la fórmula que corresponde, creamos un `CalculatorResult` de nombre `cR`.
	> **Nota:** Estamos en otra parte del código, este `cR` no es el mismo `cR` que usamos en el paso `Arguments Evaluation`. El `cR` del paso anterior ya fue desechado automáticamente en el momento que salimos de ese paso.
	* Y usamos `cR = Evaluate();`. Dentro de los paréntesis de `Evaluate()` ponemos el algoritmo de la fórmula correspondiente junto con sus argumentos. Los argumentos ya resueltos se acceden con `argsResults[i]` donde `i` es el número del argumento que queremos obtener.
	* En el caso de la *Fórmula General* no podemos usar el punto anterior, porque `Evaluate()` sólo devuelve un resultado de tipo `CalculatorResult`, mientras que la *Fórmula General* puede devolver dos resultados y los manda en un tipo `string`. Para esta fórmula reemplazamos `cR = Evaluate();` por `string result = QuadraticFormula();` dentro de los paréntesis introducimos los argumentos `a, b y c`

> La `Fórmula General` y la `Fórmula de Semejanza` funcionan de forma distinta a las demás y necesitaron de métodos completamente distintos. Puede revisar el código correspondiente a través de los siguientes enlaces:
> * [Quadratic Formula](https://github.com/IDGG73/Calculadora/blob/af338753cfc3a748eb3bb92b5d1fa9f2d27c0862/Codigo%20Fuente/Codigo%20de%20la%20App/Scripts/Calculator.cs#LL451C9-L451C9)
> * [Similarity Formula](https://github.com/IDGG73/Calculadora/blob/af338753cfc3a748eb3bb92b5d1fa9f2d27c0862/Codigo%20Fuente/Codigo%20de%20la%20App/Scripts/Calculator.cs#LL486C9-L486C9)
> * [Decimal To Fraction](https://github.com/IDGG73/Calculadora/blob/af338753cfc3a748eb3bb92b5d1fa9f2d27c0862/Codigo%20Fuente/Codigo%20de%20la%20App/Scripts/Calculator.cs#LL669C5-L669C5)

## Atajos de Teclado

Las funciones más básicas (como escribir números y signos) pueden ejecutarse mediante atajos en el teclado. Funciones más complejas o muy específicas (como `Seno`, `Coseno`, `Tangente`, `Raíz Cuadrada` y `π (Pi)`) necesitan que uses el mouse para hacer clic en los botones de la pantalla.

**Todas las teclas de números y signos funcionan como es de esperarse**.

Estos son algunos de los atajos de teclado disponibles para la aplicación:

| Teclas      | Función |
| ----------- | ----------- |
| `TAB`, `Flecha Izquierda`| Abrir y cerrar la Bandeja de Funciones Avanzadas |
| `Flecha Derecha` | Cerrar la Bandeja de Funciones Avanzadas |
| `CTRL`, `Menu Contextual`, `Menu`, `Context Menu` | Abrir y cerrar el historial |
| `F` | Abrir Menú de Fórmulas |
| `S`, `M` | Activar y desactivar ruido de tecleo |
| `ESC`, `Escape` | Volver al menú anterior |
| `C`, `Del`  , `Supr`, `Suprimir` | Borrar todo lo que escribiste |
| `Retroceso`, `Backspace`, `Clic Derecho` | Borrar el último carácter escrito |
| `Enter`, `Intro`, `Return` | Calcular |
