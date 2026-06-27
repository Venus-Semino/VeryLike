# ADR-05: Adopción de Patrones de Diseño GOF (Factory Method y Strategy)

| Campo | Valor |
|--------|--------|
| **Autor** | Venus Semino |
| **Fecha** | 30 de junio de 2026 |
| **Estado** | Aceptado |

## Contexto

Tras la separación del catálogo en **VeryLike.Catalog.API** (ADR-04), la lógica de dominio quedó concentrada en **VeryLike.Domain**. Sin embargo, dos problemas de diseño seguían latentes:

1. **Instanciación rígida de contenido audiovisual.** El catálogo mezcla dos tipos de contenido — `Pelicula` y `Serie` — que comparten la mayoría de sus atributos (`Nombre`, `Género`, `Año`, `Sinopsis`, `Studio`, `Calificación`) pero difieren en uno específico (`Duracion` vs. `Temporadas`). Antes de esta iteración, cualquier clase que leyera `catalogo.json` necesitaba conocer ambos tipos concretos y decidir manualmente cuál instanciar, repitiendo esa lógica de decisión en cada lugar que tocara el archivo.

2. **Lógica de recomendación atada al controlador.** El `PizarronController` necesitaba mostrar una lista de contenido "recomendado", pero el criterio de recomendación (mejor calificado, por género, más reciente, y eventualmente un criterio basado en IA) es una regla de negocio que cambia con el tiempo. Si ese criterio se escribe directamente dentro del controlador, cada nuevo criterio implica modificar una clase que no debería conocer el detalle del algoritmo.