# ADR-04: Implementación de API REST como mecanismo de comunicación

| Campo | Valor |
|--------|--------|
| **Autor** | Venus Semino |
| **Fecha** | 19 de junio de 2026 |
| **Estado** | `Remplazado por ADR-05` |

## Contexto

El sistema **VeryLike** ha comenzado su transición de una arquitectura monolítica basada en MVC hacia una arquitectura orientada a microservicios.

Actualmente, el proyecto principal (**Frontend**) se encargaba tanto de la interfaz de usuario como de la persistencia de datos mediante archivos `.json` almacenados localmente.

Para lograr un desacoplamiento real y permitir que el catálogo pueda escalar de forma independiente en un entorno de producción, es necesario extraer la lógica de persistencia hacia un servicio de backend separado y establecer un mecanismo de comunicación entre ambos componentes.

---

## Decisión

Se implementará una arquitectura **REST (Representational State Transfer)** utilizando **ASP.NET Core Web API** para desarrollar el servicio **VeryLike.Catalog.API**.

La API será documentada mediante **Swagger (OpenAPI)** para facilitar su exploración, pruebas e integración.

---

## Problema que Resuelve

### Acoplamiento Fuerte

Elimina la dependencia directa del Frontend con el sistema de archivos local, permitiendo que la interfaz gráfica se enfoque únicamente en la presentación de datos.

### Centralización de Datos

Permite que múltiples clientes (como futuras aplicaciones móviles o nuevos microservicios) consuman el mismo catálogo sin duplicar lógica de negocio ni mecanismos de acceso a datos.

---

## ¿Por qué REST?

### Estandarización de la Industria

REST es el estilo arquitectónico más utilizado para la construcción de APIs, garantizando compatibilidad y facilitando futuras integraciones.

### Uso de HTTP

Aprovecha los verbos HTTP estándar para representar operaciones del catálogo:

| Método | Acción |
|----------|----------|
| GET | Consultar información |
| POST | Registrar nuevos elementos |
| PUT | Actualizar información |
| DELETE | Eliminar registros |

### Documentación Interactiva

La integración nativa de ASP.NET Core con Swagger permite generar documentación visual e interactiva de forma automática, facilitando las pruebas y el mantenimiento del servicio.

---

## Alternativas Consideradas

### gRPC

**Descripción:**  
Framework RPC de alto rendimiento desarrollado por Google.

**Razón de Rechazo:**  
Introduce una complejidad innecesaria para los requerimientos actuales del proyecto, ya que requiere definir contratos mediante archivos `.proto` y generar clientes específicos. Para un catálogo simple, la legibilidad y simplicidad de JSON resultan más valiosas que la optimización ofrecida por la comunicación binaria.

### GraphQL

**Descripción:**  
Lenguaje de consulta para APIs que permite a los clientes solicitar únicamente los datos que necesitan.

**Razón de Rechazo:**  
Resulta especialmente útil cuando existen consultas complejas o estructuras altamente anidadas. En VeryLike, el Frontend requiere normalmente el objeto completo, por lo que el sobreconsumo de datos no representa un problema significativo que justifique la adopción de GraphQL y su curva de aprendizaje.

---

## Consecuencias

### Positivas

- El proyecto Frontend se vuelve más ligero y enfocado en la presentación.
- La lógica de persistencia queda centralizada en un único servicio.
- La API puede ser desplegada, escalada y mantenida de forma independiente.
- Se dispone de documentación automática mediante Swagger.
- Facilita la incorporación de nuevos clientes consumidores del servicio.

### Negativas

- Se introduce latencia asociada a la comunicación por red.
- El Frontend debe manejar operaciones asíncronas mediante `async/await`.
- Es necesario contemplar errores de comunicación HTTP y escenarios donde el backend no se encuentre disponible.
- Aumenta la complejidad operativa al existir dos aplicaciones independientes.