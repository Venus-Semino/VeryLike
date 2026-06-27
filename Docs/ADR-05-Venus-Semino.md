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

## Decisión

Se incorporan dos patrones de diseño **GOF (Gang of Four)** al proyecto:

- **Factory Method**, en `VeryLike.Domain.Factories.ContenidoFactory`, para centralizar la construcción de objetos `ContenidoAudiovisual` a partir del JSON del catálogo.
- **Strategy**, en `PizarronController` (clases `IEstrategiaRecomendacion`, `OrdenarPorCalificacionStrategy` y `MotorDeRecomendacion`), para desacoplar el algoritmo de recomendación del controlador que lo consume.

---

## Patrón 1: Factory Method

### Problema que Resuelve

`CatalogoRepository` necesita convertir cada objeto plano del JSON (`CatalogoItemDto`) en una instancia de `Pelicula` o `Serie`. Sin este patrón, ese `if/switch` viviría dentro del repositorio (o se duplicaría en cualquier otra clase que leyera el catálogo), mezclando responsabilidades de *acceso a datos* con responsabilidades de *construcción de objetos*.

### Funcionamiento

`ContenidoFactory.Crear(CatalogoItemDto dto)` lee el campo discriminador `Tipo` del DTO y decide qué subclase de `ContenidoAudiovisual` instanciar:

```csharp
ContenidoAudiovisual contenido = dto.Tipo.Trim().ToLowerInvariant() switch
{
    "serie"     => new Serie     { Temporadas = dto.Temporadas ?? 0 },
    "pelicula"  => new Pelicula  { Duracion   = dto.Duracion   ?? string.Empty },
    _ => throw new InvalidOperationException($"Tipo de contenido desconocido: '{dto.Tipo}'.")
};
```

`CatalogoRepository` (la clase cliente) solo invoca `ContenidoFactory.CrearTodos(dtos)` y trabaja siempre contra el tipo base `ContenidoAudiovisual`; nunca necesita saber cómo se decide entre `Pelicula` y `Serie`.