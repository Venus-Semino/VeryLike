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
