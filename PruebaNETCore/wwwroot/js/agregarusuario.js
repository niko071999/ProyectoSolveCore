const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]')
const popoverList = [...popoverTriggerList].map(popoverTriggerEl => new bootstrap.Popover(popoverTriggerEl))
const contenedores_conductores = document.querySelector("#conductor");
//CHECKBOXS
const admin = document.querySelector("#Administrador"),
    jefe = document.querySelector("#Jefe"),
    vehiculos = document.querySelector("#Vehiculos"),
    usuarios = document.querySelector("#Usuarios"),
    solicitador = document.querySelector("#Solicitador"),
    maquinaria = document.querySelector("#Maquinaria"),
    bitacora = document.querySelector("#Bitacora_c"),
    login = document.querySelector("#LoginAccess"),
    check_poliza = document.getElementById('check_poliza');
//INPUTS
const rolAdmin = document.querySelector("#RolAdministrador"),
    rolJefe = document.querySelector("#RolJefe"),
    rolMantenedorUsuarios = document.querySelector("#RolMantendorUsuarios"),
    rolMantenedorVehiculos = document.querySelector("#RolMantenedorVehiculos"),
    rolSolicitador = document.querySelector("#RolSolicitador"),
    rolMaquinaria = document.querySelector("#RolMantenedorVehiculosMaq"),
    rolBitacora = document.querySelector("#RolMantenedorBitacora"),
    inp_login = document.querySelector("#Login");
var inputDepartamento = document.getElementById('inputDepartamento');
const form = document.querySelector("#form_AgregarUsuario");
const btn_form = document.getElementById('btn_form'),
    btn_newDepartamento = document.getElementById('btn_newDepartamento');
btn_form.addEventListener('click', () => {
    if (document.getElementById('inputClave').value.lenght < 6) {
        alert("La clave de acceso debe tener como minimo 6 caracteres");
        return;
    }
    if (document.getElementById('inputClave').value !== document.getElementById('inputReClave').value) {
        alert("Las claves ingresadas no son las mismas");
        return;
    }

    if (check_poliza) {
        if (!check_poliza.checked) {
            document.getElementById('inputNumPoliza').value = null;
            document.getElementById('inputFechaInicio').value = null;
            document.getElementById('inputFechaFin').value = null;
        }
    }
    form.submit();
});
btn_newDepartamento.addEventListener('click', function () {
    const departamento = prompt('Ingrese el departamento');
    if (departamento !== null) {
        if (departamento.trim() !== "") {
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
        }
    }
});

iniciarlizarCheckInputs();

solicitador.addEventListener("click", () => {
    if (!solicitador.checked) {
        solicitador.checked = true;
    }
});

jefe.addEventListener("change", updateInputs);
vehiculos.addEventListener("change", updateInputs);
usuarios.addEventListener("change", updateInputs);
solicitador.addEventListener("change", updateInputs);
maquinaria.addEventListener("change", updateInputs);
bitacora.addEventListener("change", updateInputs);
login.addEventListener("change", function () {
    inp_login.value = login.checked;
});

function iniciarlizarCheckInputs() {
    solicitador.checked = true;
    rolSolicitador.value = true;

    login.checked = true;
    inp_login.value = login.checked;
}

function updateInputs() {
    rolAdmin.value = admin.checked;
    rolJefe.value = jefe.checked;
    rolMantenedorVehiculos.value = vehiculos.checked;
    rolMantenedorUsuarios.value = usuarios.checked;
    rolSolicitador.value = solicitador.checked;
    rolMaquinaria.value = maquinaria.checked;
    rolBitacora.value = bitacora.checked;
}

//Verificar si el check de la poliza esta activado

if (check_poliza.checked) {
    contenedores_conductores.style.display = "block";
}

check_poliza.addEventListener('change', () => {
    if (check_poliza.checked) {
        contenedores_conductores.style.display = "block";
    } else {
        contenedores_conductores.style.display = "none";
    }
});

let dropDown = document.getElementById('inputDepartamento');
obtenerDepartamentos();

dropDown.addEventListener('change', () => {
    const opcionSeleccionada = dropDown.value;
    document.getElementById('idDepartamento').value = opcionSeleccionada;
});

function obtenerDepartamentos() {
    $.ajax({
        url: '/Departamento/GetDepartamentos',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            if (result !== null || result.data.lenght > 0) {
                dropDown.innerHTML = '';
                result.data.forEach((d) => {
                    var option = document.createElement('option');
                    option.value = d.Id;
                    option.innerHTML = d.Departamento1;
                    dropDown.appendChild(option);
                });
                document.getElementById('idDepartamento').value = result.data[0].Id;
            } else {
                alert("No existen registros de departamentos para mostrar");
            }
        },
        error: function (xhr, error) {
            alert(error);
        }
    });
}