let ticketActual = null;
let lectorQr = null;

document.getElementById("boton-cerrar-sesion").addEventListener("click", () => {
  cerrarSesion();
});

document.getElementById("boton-escanear").addEventListener("click", () => {
  iniciarEscaneo();
});

document.getElementById("boton-escanear-otro").addEventListener("click", () => {
  reiniciarPantalla();
});

document.getElementById("formulario-despacho").addEventListener("submit", async (evento) => {
  evento.preventDefault();
  await registrarDespacho();
});

function iniciarEscaneo() {
  document.getElementById("boton-escanear").hidden = true;

  lectorQr = new Html5Qrcode("lector-qr");
  lectorQr
    .start(
      { facingMode: "environment" },
      { fps: 10, qrbox: 250 },
      onQrDetectado,
      () => {}
    )
    .catch((error) => {
      alert("No se pudo acceder a la camara: " + error);
      document.getElementById("boton-escanear").hidden = false;
    });
}

async function onQrDetectado(textoDecodificado) {
  await lectorQr.stop();
  document.getElementById("lector-qr").innerHTML = "";

  const token = extraerTokenDeUrl(textoDecodificado);
  await validarTicket(token);
}

function extraerTokenDeUrl(texto) {
  const partes = texto.split("/").filter((parte) => parte.length > 0);
  return partes[partes.length - 1];
}

async function validarTicket(token) {
  const seccionTicket = document.getElementById("seccion-ticket");
  const detalleTicket = document.getElementById("detalle-ticket");
  const mensajeValidacion = document.getElementById("mensaje-validacion");
  const formulario = document.getElementById("formulario-despacho");

  seccionTicket.hidden = false;
  mensajeValidacion.textContent = "";
  formulario.hidden = true;

  try {
    const respuesta = await llamarApi(`/api/Tickets/validar/${token}`);

    if (!respuesta.ok) {
      mensajeValidacion.textContent = "Ticket no encontrado.";
      detalleTicket.innerHTML = "";
      return;
    }

    const ticket = await respuesta.json();
    ticketActual = { ...ticket, token };

    detalleTicket.innerHTML = `
      <p><strong>Ticket:</strong> ${ticket.numeroTicket}</p>
      <p><strong>Empleado:</strong> ${ticket.empleadoNombreSnapshot}</p>
      <p><strong>Vehiculo:</strong> ${ticket.vehiculoPlacaSnapshot}</p>
      <p><strong>Cantidad Autorizada:</strong> ${ticket.cantidadAutorizada} galones</p>
      <p><strong>Estado:</strong> ${ticket.estadoVisual}</p>
    `;

    if (!ticket.hashValido) {
      mensajeValidacion.textContent = "Advertencia: no se pudo verificar la autenticidad de este ticket.";
      return;
    }

    if (["Consumido", "Anulado", "Vencido"].includes(ticket.estadoVisual)) {
      mensajeValidacion.textContent = `Este ticket no se puede despachar (estado: ${ticket.estadoVisual}).`;
      return;
    }

    document.getElementById("galones").max = ticket.cantidadAutorizada;
    document.getElementById("galones").value = ticket.cantidadAutorizada;
    formulario.hidden = false;
  } catch (error) {
    mensajeValidacion.textContent = "No se pudo validar el ticket. Intenta de nuevo.";
  }
}

async function registrarDespacho() {
  const mensajeResultado = document.getElementById("mensaje-resultado");
  mensajeResultado.textContent = "";
  mensajeResultado.className = "";

  const cuerpo = {
    tokenTicket: ticketActual.token,
    galonesDespachados: parseFloat(document.getElementById("galones").value),
    estacion: document.getElementById("estacion").value,
    observaciones: document.getElementById("observaciones").value || null
  };

  try {
    const respuesta = await llamarApi("/api/Despachos", {
      method: "POST",
      body: JSON.stringify(cuerpo)
    });

    if (!respuesta.ok) {
      const textoError = await respuesta.text();
      mensajeResultado.textContent = textoError;
      mensajeResultado.className = "mensaje-error";
      return;
    }

    mensajeResultado.textContent = "Despacho registrado correctamente.";
    mensajeResultado.className = "mensaje-exito";
    document.getElementById("formulario-despacho").hidden = true;
    document.getElementById("boton-escanear-otro").hidden = false;
  } catch (error) {
    mensajeResultado.textContent = "No se pudo registrar el despacho.";
    mensajeResultado.className = "mensaje-error";
  }
}

function reiniciarPantalla() {
  ticketActual = null;
  document.getElementById("seccion-ticket").hidden = true;
  document.getElementById("detalle-ticket").innerHTML = "";
  document.getElementById("mensaje-validacion").textContent = "";
  document.getElementById("mensaje-resultado").textContent = "";
  document.getElementById("boton-escanear-otro").hidden = true;
  document.getElementById("boton-escanear").hidden = false;
}