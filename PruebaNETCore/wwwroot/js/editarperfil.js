var popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]')
var popoverList = [...popoverTriggerList].map(popoverTriggerEl => new bootstrap.Popover(popoverTriggerEl))
var contenedores_conductores = document.querySelector("#conductor");
//CHECKBOXS
var admin = document.querySelector("#Administrador"),
    jefe = document.querySelector("#Jefe"),
    vehiculos = document.querySelector("#Vehiculos"),
    usuarios = document.querySelector("#Usuarios"),
    solicitador = document.querySelector("#Solicitador"),
    maquinaria = document.querySelector("#Maquinaria"),
    bitacora = document.querySelector("#Bitacora_c"),
    login = document.querySelector("#LoginAccess");
//INPUTS
var rolAdmin = document.querySelector("#RolAdministrador"),
    rolJefe = document.querySelector("#RolJefe"),
    rolMantenedorUsuarios = document.querySelector("#RolMantendorUsuarios"),
    rolMantenedorVehiculos = document.querySelector("#RolMantenedorVehiculos"),
    rolSolicitador = document.querySelector("#RolSolicitador"),
    rolMaquinaria = document.querySelector("#RolMantenedorVehiculosMaq"),
    rolBitacora = document.querySelector("#RolMantenedorBitacora"),
    inputDepartamento = document.getElementById('inputDepartamento');
var btn_newDepartamento = document.getElementById('btn_newDepartamento');
if (check_poliza.checked) {
    contenedores_conductores.style.display = "block";
}

btn_newDepartamento.addEventListener('click', function () {
    const departamento = prompt('Ingrese el departamento');
    $.ajax({
        url: '/Usuario/SelectAgregarDepartamento?departamento=' + departamento,
        type: 'GET',
        success: function (result) {
            if (result.type === "void") {
                alert(result.mensaje);
            }
            if (result.type === "error") {
                alert(result.mensaje);
            }
            if (result.type === "success") {
                var option = document.createElement('option');
                option.value = result.data.Id;
                option.text = result.data.Departamento1;
                inputDepartamento.appendChild(option);

                alert(result.mensaje);
            }
        },
        error: function (xhr, error) {
            console.log(error);
        }
    });
});

admin.addEventListener("click", function () {
    jefe.checked = admin.checked;
    vehiculos.checked = admin.checked;
    usuarios.checked = admin.checked;
    maquinaria.checked = admin.checked;
    bitacora.checked = admin.checked;
    if (admin.checked) {
        solicitador.checked = admin.checked;
    }
    updateInputs();
});
solicitador.addEventListener("click", () => {
    if (!admin.checked || !jefe.checked && !vehiculos.checked && !usuarios.checked
        && !bitacora.checked && !maquinaria.checked) {
        solicitador.checked = true;
    }
})

jefe.addEventListener("change", updateInputs);
vehiculos.addEventListener("change", updateInputs);
usuarios.addEventListener("change", updateInputs);
solicitador.addEventListener("change", updateInputs);
maquinaria.addEventListener("change", updateInputs);
bitacora.addEventListener("change", updateInputs);

function updateInputs() {
    if (jefe.checked && vehiculos.checked && usuarios.checked && solicitador.checked && maquinaria.checked && bitacora.checked) {
        admin.checked = true;
    } else {
        admin.checked = false;
    }
    rolAdmin.value = admin.checked;
    rolJefe.value = jefe.checked;
    rolMantenedorVehiculos.value = vehiculos.checked;
    rolMantenedorUsuarios.value = usuarios.checked;
    rolSolicitador.value = solicitador.checked;
    rolMaquinaria.value = maquinaria.checked;
    rolBitacora.value = bitacora.checked;
}

//Verificar si el check de la poliza esta activado
check_poliza.addEventListener('change', () => {
    if (check_poliza.checked) {
        contenedores_conductores.style.display = "block";
    } else {
        contenedores_conductores.style.display = "none";
    }
});