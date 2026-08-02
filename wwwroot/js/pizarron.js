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

// Genera una cadena de estrellas llenas/mitad/vacías a partir de una calificación 0-5
function generarEstrellas(calificacion) {
    var estrellas = '';
    for (var i = 1; i <= 5; i++) {
        if (calificacion >= i) {
            estrellas += '★';
        } else if (calificacion >= i - 0.5) {
            estrellas += '⯨';
        } else {
            estrellas += '☆';
        }
    }
    return estrellas;
}

// Estado del modal: contenido abierto y puntaje elegido en el formulario
var contenidoActualId = null;
var puntajeElegido = 0;

function pintarEstrellasEditables() {
    document.querySelectorAll('#modalEstrellasEditables .star-rating').forEach(function (estrella) {
        var valor = parseInt(estrella.getAttribute('data-valor'), 10);
        estrella.classList.toggle('active', puntajeElegido >= valor - 0.5);
        estrella.textContent = puntajeElegido >= valor ? '★' : (puntajeElegido >= valor - 0.5 ? '⯨' : '☆');
        estrella.style.opacity = puntajeElegido >= valor - 0.5 ? '1' : '0.35';
    });
    document.getElementById('modalPuntajeElegido').textContent = puntajeElegido.toFixed(1);
}

function establecerPuntaje(valor) {
    puntajeElegido = valor;
    pintarEstrellasEditables();
}

// Carga desde el servidor el resumen de la comunidad, las reseñas públicas y,
// si hay sesión, la calificación propia del usuario.
function cargarCalificaciones(contenidoId) {
    var contenedorResenas = document.getElementById('modalResenas');
    var totalVotos = document.getElementById('modalTotalVotos');
    var resenaPrivada = document.getElementById('modalMiResenaPrivada');

    contenedorResenas.innerHTML = '';
    totalVotos.textContent = '';
    resenaPrivada.classList.add('d-none');
    document.getElementById('modalResenaPublica').value = '';
    document.getElementById('modalResenaPrivada').value = '';
    document.getElementById('modalMensajeCalificacion').textContent = '';
    establecerPuntaje(0);

    if (!contenidoId) {
        return;
    }

    fetch('/Calificaciones/Detalle?contenidoId=' + encodeURIComponent(contenidoId))
        .then(function (respuesta) { return respuesta.json(); })
        .then(function (datos) {
            if (datos.total > 0) {
                document.getElementById('modalCalificacion').textContent = datos.promedio.toFixed(1);
                document.getElementById('modalEstrellas').textContent = generarEstrellas(datos.promedio);
                totalVotos.textContent = datos.total + ' calificación(es) de la comunidad';
            } else {
                totalVotos.textContent = 'Todavía nadie la calificó';
            }

            datos.resenas.forEach(function (resena) {
                var bloque = document.createElement('p');
                bloque.className = 'mb-2';
                bloque.style.fontSize = '0.85rem';
                var autor = document.createElement('span');
                autor.style.color = 'var(--accent-color)';
                autor.textContent = resena.autor + ' (' + resena.puntaje + '/5): ';
                bloque.appendChild(autor);
                bloque.appendChild(document.createTextNode(resena.texto));
                contenedorResenas.appendChild(bloque);
            });

            if (datos.miCalificacion) {
                establecerPuntaje(datos.miCalificacion.puntaje);
                document.getElementById('modalResenaPublica').value = datos.miCalificacion.resenaPublica || '';
                document.getElementById('modalResenaPrivada').value = datos.miCalificacion.resenaPrivada || '';

                if (datos.miCalificacion.resenaPrivada) {
                    resenaPrivada.textContent = 'Tu nota privada: ' + datos.miCalificacion.resenaPrivada;
                    resenaPrivada.classList.remove('d-none');
                }
            }
        })
        .catch(function () {
            totalVotos.textContent = 'No se pudieron cargar las calificaciones';
        });
}

function guardarCalificacion() {
    var mensaje = document.getElementById('modalMensajeCalificacion');

    if (!contenidoActualId) {
        mensaje.textContent = 'Este título todavía no está en el catálogo.';
        return;
    }
    if (puntajeElegido < 0.5) {
        mensaje.textContent = 'Elegí un puntaje antes de guardar.';
        return;
    }

    var token = document.querySelector('input[name="__RequestVerificationToken"]');

    fetch('/Calificaciones/Guardar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token ? token.value : ''
        },
        body: JSON.stringify({
            contenidoId: contenidoActualId,
            puntaje: puntajeElegido,
            resenaPublica: document.getElementById('modalResenaPublica').value,
            resenaPrivada: document.getElementById('modalResenaPrivada').value
        })
    }).then(function (respuesta) {
        if (respuesta.status === 401) {
            mensaje.textContent = 'Iniciá sesión para calificar.';
            return null;
        }
        if (!respuesta.ok) {
            mensaje.textContent = 'No se pudo guardar la calificación.';
            return null;
        }
        return respuesta.json();
    }).then(function (datos) {
        if (!datos) {
            return;
        }
        document.getElementById('modalCalificacion').textContent = datos.promedio.toFixed(1);
        document.getElementById('modalEstrellas').textContent = generarEstrellas(datos.promedio);
        document.getElementById('modalTotalVotos').textContent = datos.total + ' calificación(es) de la comunidad';
        cargarCalificaciones(contenidoActualId);
        cambiarAVistaDetalles();
    });
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

            contenidoActualId = parseInt(tarjeta.getAttribute('data-id'), 10) || null;
            cargarCalificaciones(contenidoActualId);

            cambiarAVistaDetalles();
        });
    }

    // Media estrella al hacer clic en la mitad izquierda, entera en la derecha
    document.querySelectorAll('#modalEstrellasEditables .star-rating').forEach(function (estrella) {
        estrella.addEventListener('click', function (evento) {
            var valor = parseInt(estrella.getAttribute('data-valor'), 10);
            var rect = estrella.getBoundingClientRect();
            var esMitadIzquierda = (evento.clientX - rect.left) < rect.width / 2;
            establecerPuntaje(esMitadIzquierda ? valor - 0.5 : valor);
        });
    });

    var botonGuardar = document.getElementById('modalBotonGuardar');
    if (botonGuardar) {
        botonGuardar.addEventListener('click', guardarCalificacion);
    }
});
