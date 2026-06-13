# ADR-03: Definición del Estilo Arquitectónico de VeryLike

| Campo | Valor |
|-------|-------|
| Autor | Venus Getsemaní Semino Alemán |
| Estado| `Aceptado` |


## Contexto

VeryLike es una plataforma web que unifica y almacena contenido cinematográfico (películas y series) de distintas plataformas de streaming en un solo lugar. Sirve como sistema de seguimiento para que el usuario pueda anotar lo que desea ver y, una vez consumido, otorgarle una calificación. 

Esta plataforma surge como solución a la fragmentación de catálogos, evitando que el usuario tenga que cambiar constantemente de aplicación para gestionar sus listas. Además, busca resolver la falta de interacción social integrando un foro y un sistema de calificaciones más amplio. La plataforma está enfocada en los amantes del cine que buscan centralizar su experiencia y compartirla con una comunidad.

---

## Decisión

Para la fase inicial de este proyecto, decidí utilizar el patrón arquitectónico **MVC (Model-View-Controller)** implementado con **ASP.NET Core**.

### ¿Por qué?

Elegí esta opción porque permite separar de manera eficiente la lógica de negocio, los datos y la interfaz. Conforme el proyecto escale, las acciones del usuario van a cambiar o aumentar. Al usar MVC, puedo modificar la vista del usuario sin interferir en la lógica, y puedo alterar los modelos de datos sin romper las interfaces. 

Esto es fundamental pensando en el futuro del proyecto a larga escala, ya que nos prepara para la **implementación de APIs externas** (como la búsqueda automática de portadas) y la integración de **servicios de Inteligencia Artificial** para la recomendación y búsqueda de plataformas, facilitando una futura transición hacia microservicios o el cambio de base de datos.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Arquitectura monolítica ("Clase Dios")** | Tener todos los procesos en una sola clase rompe con el Principio de Responsabilidad Única (SRP) que analizamos en la práctica del Ahorcado. Causa acoplamiento severo en el código y bloquea la capacidad del programa para escalar. |
| **Patrón MVVM (Model-View-ViewModel)** | Se evaluó para la interfaz, pero se descartó porque requiere una sincronización constante y compleja en el lado del cliente (frontend). Para el flujo actual de VeryLike, el enrutamiento y las peticiones HTTP estándar de MVC son más que suficientes y no sobre-complican el sistema. |
| **Archivos estáticos (HTML/JS sin backend)** | Se descartó porque no permitiría procesar de forma segura la lógica de las calificaciones de la comunidad, ni estructurar las listas dinámicas de los usuarios de forma segura y ordenada en el servidor. |

---

## Consecuencias

**Lo que gano:**

- **Consecuencia técnica:** Puedo corregir o rediseñar las vistas (archivos `.cshtml`) cada vez que sea necesario para mejorar la UI/UX, sin alterar o romper por accidente las reglas de negocio con las que se guardan los datos o se calculan las calificaciones. 
- **Consecuencia sobre el proceso/equipo:** Permite una organización estructurada del trabajo. Al estar las responsabilidades separadas en carpetas (`Models`, `Views`, `Controllers`), es muy rápido identificar qué archivo modificar al momento de agregar nuevas funciones, como el foro de la comunidad, manteniendo el código limpio.

**Lo que sacrifico o asumo:**

- **Limitación técnica:** La separación estricta aumenta la cantidad de archivos y la navegación entre ellos, lo que requiere más tiempo inicial comparado con un script plano. De igual forma, exige invertir tiempo en configurar correctamente la Inyección de Dependencias en el archivo `Program.cs` para enlazar todas las capas.
- **Deuda o riesgo:** En esta etapa inicial, los datos se manejan con persistencia temporal. El riesgo asumido es que la concurrencia masiva (cuando el foro crezca) será un cuello de botella hasta que migremos a un motor de base de datos robusto y dividamos los servicios.

## Diagrama

<img width="511" height="361" alt="Diagrama de Arquitectura MVC" src="https://github.com/user-attachments/assets/9e4038d9-9690-4435-9d45-256b25be16aa" /> 

---
## Uso de IA:

En este proyecto se está implementando el uso de IA para darle una esstructura más profeciomal acercandolo más a un poryecto en el ámbito laboral, se implementó para aclarar duda o corregir ideas mal planteadas.

---
