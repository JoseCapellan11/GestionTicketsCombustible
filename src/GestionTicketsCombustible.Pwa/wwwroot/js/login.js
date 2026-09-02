document.getElementById("formulario-login").addEventListener("submit", async (evento) => {
  evento.preventDefault();

  const nombreUsuario = document.getElementById("nombreUsuario").value.trim();
  const password = document.getElementById("password").value;
  const mensajeError = document.getElementById("mensaje-error");
  mensajeError.textContent = "";

  try {
    const respuesta = await fetch(`${API_BASE_URL}/api/Auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ nombreUsuario, password })
    });

    if (!respuesta.ok) {
      mensajeError.textContent = "Usuario o contrasena incorrectos.";
      return;
    }

    const datos = await respuesta.json();
    guardarToken(datos.token);
    window.location.href = "/despacho.html";
  } catch (error) {
    mensajeError.textContent = "No se pudo conectar con el servidor.";
  }
});