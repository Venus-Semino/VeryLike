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

// Genera una cadena de estrellas llenas/vacías a partir de una calificación 0-5
function generarEstrellas(calificacion) {
    var llenas = Math.round(calificacion);
    var estrellas = '';
    for (var i = 0; i < 5; i++) {
        estrellas += i < llenas ? '★' : '☆';
    }
    return estrellas;
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
            var poster = tarjeta.getAttribute('data-poster');
            var calificacion = parseFloat(tarjeta.getAttribute('data-calificacion')) || 0;
            var enlace = tarjeta.getAttribute('data-enlace');

            document.getElementById('modalTitulo').textContent = titulo;
            document.getElementById('modalSinopsis').textContent = sinopsis;
            document.getElementById('modalEstudio').textContent = estudio;
            document.getElementById('modalCalificacion').textContent = calificacion.toFixed(1);
            document.getElementById('modalEstrellas').textContent = generarEstrellas(calificacion);

            // Póster real si existe; si no, mantiene el texto de respaldo "PORTADA"
            var imgPortada = document.getElementById('modalPortadaImg');
            var textoPortada = document.getElementById('modalPortadaTexto');
            if (poster) {
                imgPortada.src = poster;
                imgPortada.alt = 'Póster de ' + titulo;
                imgPortada.classList.remove('d-none');
                textoPortada.classList.add('d-none');
            } else {
                imgPortada.classList.add('d-none');
                textoPortada.classList.remove('d-none');
            }

            // Enlace profundo hacia la plataforma de streaming (redirección con un clic)
            var enlaceStreaming = document.getElementById('modalEnlaceStreaming');
            if (enlace) {
                enlaceStreaming.href = enlace;
                enlaceStreaming.classList.remove('d-none');
                enlaceStreaming.textContent = plataformas ? 'Ver en ' + plataformas.split(',')[0].trim() : 'Ver en streaming';
            } else {
                enlaceStreaming.classList.add('d-none');
            }

            var infoTecnica = document.getElementById('modalInfoTecnica');
            if (tipo === 'pelicula') {
                var duracion = tarjeta.getAttribute('data-duracion');
                infoTecnica.textContent = 'Duración: ' + duracion;
            } else if (tipo === 'serie') {
                var temporadas = tarjeta.getAttribute('data-temporadas');
                infoTecnica.textContent = temporadas + ' Temporada(s)';
            }

            var contenedorPlataformas = document.getElementById('modalPlataformas');
            contenedorPlataformas.innerHTML = '';

            if (plataformas) {
                var listaPlataformas = plataformas.split(',');
                listaPlataformas.forEach(function (plat) {
                    var badge = document.createElement('span');
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
