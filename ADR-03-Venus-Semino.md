# ADR-03: Definición del Estilo Arquitectónico de VeryLike

| Campo | Valor |
|-------|-------|
| Autor | Venus Getsemaní Semino Alemán |
| Estado| `Aceptado` |

## Contexto
El proyecto *VeryLike* ha evolucionado en su conceptualización. Originalmente estructurado bajo un patrón MVC simple, la visión actual a larga escala incluye módulos con necesidades técnicas muy distintas: un catálogo transaccional, un motor de búsqueda y redirección impulsado por IA, y una plataforma comunitaria (foro/blog) de alta concurrencia. Es necesario definir un estilo arquitectónico que soporte el crecimiento, la escalabilidad independiente y la mantenibilidad a largo plazo de estos dominios.

---

## Decisión
El estilo arquitectónico elegido para el despliegue a larga escala es la **Arquitectura Basada en Microservicios**, complementada con un estilo **Cliente-Servidor**.

### ¿Por qué?
Este estilo resuelve el problema de la asimetría de recursos. Al separar el sistema en microservicios independientes (Catálogo, Comunidad e IA), garantizamos que una falla o un pico de tráfico en el foro no comprometa la capacidad del usuario para buscar películas. Asimismo, el enfoque Cliente-Servidor nos permite desarrollar un Frontend rico e interactivo de forma aislada, comunicándose con un *API Gateway* que orquesta las peticiones hacia el backend. Esto facilita la futura integración continua y el despliegue escalable en entornos de nube.

### Alternativas consideradas
