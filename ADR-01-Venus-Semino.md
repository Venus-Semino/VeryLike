# ADR-01: Selección Arquitectónico para la plataforma web VeryLike

| Campo  | Valor |
|--------|-------|
| Autor  | Venus Semino |
| Fecha  | 15/05/2026 |
| Estado | `Propuesto`  |

---

## Contexto

VeryLike es una plataforma web que almacena contenido cinematográfico entre películas y series de distintas plataformas en una sola. Sirve como seguimiento para que el usuario pueda anotar lo que desea ver, y una vez visto darle una calificación y apuntarlo como ya visto.
Esta plataforma surgió como una solución para que el usuario no tenga que cambiarse constantemente entre aplicaciones para ver en cada una su lista de contenidos, como la falta de comunicación entre personas y contar con mejor manejo de calificaciones. Esta página está enfocada a esos amantes cinematográficos que buscan poder compartir con una comunidad sus gustos y experiencias.

---

## Decisión

En este proyecto decidí usar el patrón arquitectónico MVC (Model-View-Controller) con ASP.NET Core.

### ¿Por qué?

Elegí esta opción porque en esta se puede separar de mejor forma la lógica que se debe implementar, que conforme el tiempo esta puede cambiar agregando o eliminando acciones que puede realizar el usuario. Al usar este patrón, la vista del usuario se puede cambiar sin interferir en la lógica o en los datos, y para los datos se podrán cambiar sin la necesidad de interferir en las demás clases. Esto nos ayuda mucho pensando en la futura implementación de las APIs que se deberán de agregar para que la plataforma pueda funcionar (como buscar las portadas de películas) o si es necesario cambiar la base de datos en un futuro.
### Alternativas consideradas

*(Mínimo 3 filas)*

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Arquitectura monolítica ("Clase Dios")         | Tener todos los archivos en una sola clase rompe con el principio de responsabilidad única (SRP) que vimos en la práctica del Ahorcado. Causa mucha confusión en el código y no es adecuado para que el programa pueda escalar.                 |
| Patrón MVVM (Model-View-ViewModel)         | Se evaluó para la interfaz, pero se descartó porque requiere una sincronización constante y compleja en el lado del cliente. Para el flujo de VeryLike, una petición HTTP estándar por cada acción del usuario es más que suficiente y no satura el sistema.                 |
| Archivo estático        | Se descartó porque no permitiría procesar de forma segura la lógica de las calificaciones de la comunidad, ni estructurar las listas dinámicas de los usuarios de forma ordenada en el backend.                 |

---

## Consecuencias

**✅ Lo que gano:**

Menciona al menos:
- Una consecuencia **técnica** — Puedo corregir o rediseñar las vistas (los archivos .cshtml) cada vez que sea necesario, para mejorar la apariencia visual sin alterar o romper por accidente las reglas con las que se guardan los datos o se calculan las calificaciones. 
- Una consecuencia sobre el **proceso o el equipo** — Permite una organización más limpia del trabajo. Al estar las responsabilidades separadas en Carpetas, para identificar de manera rápida el archivo qué archivo ir cuando agregue el foro o modifique las listas sin revolver el código. 

**⚠️ Lo que sacrifico o asumo:**

Menciona al menos:
- Una **limitación técnica** — Como todo está separado, el número de archivos será más extenso y eso puede emplear más tiempo en comparación de un solo archivo plano. De igual forma, tengo que invertir tiempo en darle el formato correcto al archivo program.cs al inicio del proyecto para enlazar todo. 
- Una **deuda o riesgo** — Como en esta etapa inicial de la materia los datos se guardan temporalmente en memoria, si la aplicación se detiene o se reinicia, las listas guardadas por los usuarios se perderán hasta que integremos una base de datos real. 

## Diagrama


![Diagrama del sistema]( <img width="511" height="361" alt="likeeeeee drawio" src="https://github.com/user-attachments/assets/9e4038d9-9690-4435-9d45-256b25be16aa" /> )
