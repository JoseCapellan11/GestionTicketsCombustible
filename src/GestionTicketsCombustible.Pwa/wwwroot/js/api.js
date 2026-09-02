const API_BASE_URL = "https://localhost:7262";

function obtenerToken() {
  return localStorage.getItem("jwt");
}

function guardarToken(token) {
  localStorage.setItem("jwt", token);
}

function cerrarSesion() {
  localStorage.removeItem("jwt");
  window.location.href = "/index.html";
}

async function llamarApi(ruta, opciones = {}) {
  const token = obtenerToken();
  const encabezados = {
    "Content-Type": "application/json",
    ...(opciones.headers || {})
  };

  if (token) {
    encabezados["Authorization"] = `Bearer ${token}`;
  }

  const respuesta = await fetch(`${API_BASE_URL}${ruta}`, {
    ...opciones,
    headers: encabezados
  });

  if (respuesta.status === 401) {
    cerrarSesion();
    throw new Error("Sesion expirada. Inicia sesion de nuevo.");
  }

  return respuesta;
}