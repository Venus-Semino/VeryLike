// Funciones para la animación de "gota de agua" y cambio de vistas
function cambiarAVistaFormulario() {
    const vistaDetalles = document.getElementById('vistaDetalles');
    const vistaFormulario = document.getElementById('vistaFormulario');

    vistaDetalles.classList.add('d-none');
    vistaFormulario.classList.remove('d-none');

    vistaFormulario.classList.remove('water-drop-enter');
    void vistaFormulario.offsetWidth; // Forzar repintado
    vistaFormulario.classList.add('water-drop-enter');
}

function cambiarAVistaDetalles() {
    const vistaDetalles = document.getElementById('vistaDetalles');
    const vistaFormulario = document.getElementById('vistaFormulario');

    vistaFormulario.classList.add('d-none');
    vistaDetalles.classList.remove('d-none');

    vistaDetalles.classList.remove('water-drop-enter');
    void vistaDetalles.offsetWidth;
    vistaDetalles.classList.add('water-drop-enter');
}

// Inyector dinámico de datos al abrir el modal
document.addEventListener('DOMContentLoaded', function () {
    var modalDetalle = document.getElementById('modalDetalleMedia');

    // Verificamos que el modal exista en la página antes de agregarle el evento
    if (modalDetalle) {
        modalDetalle.addEventListener('show.bs.modal', function (event) {
            var tarjeta = event.relatedTarget;

            var tipo = tarjeta.getAttribute('data-tipo');
            var titulo = tarjeta.getAttribute('data-titulo');
            var sinopsis = tarjeta.getAttribute('data-sinopsis');
            var estudio = tarjeta.getAttribute('data-estudio');
            var plataformas = tarjeta.getAttribute('data-plataformas');

            document.getElementById('modalTitulo').textContent = titulo;
            document.getElementById('modalSinopsis').textContent = sinopsis;
            document.getElementById('modalEstudio').textContent = estudio;

            var infoTecnica = document.getElementById('modalInfoTecnica');
            if (tipo === 'pelicula') {
                var duracion = tarjeta.getAttribute('data-duracion');
                infoTecnica.textContent = '⏱ Duración: ' + duracion;
            } else if (tipo === 'serie') {
                var temporadas = tarjeta.getAttribute('data-temporadas');
                var episodios = tarjeta.getAttribute('data-episodios');
                infoTecnica.textContent = '📺 ' + temporadas + ' Temporadas | ' + episodios + ' Eps';
            }

            var contenedorPlataformas = document.getElementById('modalPlataformas');
            contenedorPlataformas.innerHTML = '';

            if (plataformas) {
                var listaPlataformas = plataformas.split(',');
                listaPlataformas.forEach(function (plat) {
                    var badge = document.createElement('a');
                    badge.href = "#";
                    badge.className = "badge rounded-pill text-decoration-none px-3 py-2 me-1";
                    badge.style.border = "1px solid var(--border-glass)";
                    badge.style.color = "var(--text-main)";
                    badge.textContent = plat.trim();
                    contenedorPlataformas.appendChild(badge);
                });
            }

            cambiarAVistaDetalles();
        });
    }
});
```

### 2. Limpiar tu Vista y conectar el nuevo archivo

Ahora ve a tu archivo **`Views / Pizarron / Index.cshtml`**. 

Ve hasta el fondo del archivo, borra absolutamente todo el bloque de `< script > ... </script > ` que habíamos puesto y **reemplázalo** por esto:

```html
@section Scripts {
    <script src="~/js/pizarron.js" asp-append-version="true"></script>
}