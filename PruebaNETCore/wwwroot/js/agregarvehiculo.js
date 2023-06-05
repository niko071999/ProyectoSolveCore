const check = document.getElementById('gridCheck');
const btn_newPeriodo = document.getElementById('btn_newPeriodo'),
    btn_newCategoria = document.getElementById('btn_newCategoria');
let dropDown = document.getElementById('inputPeriodos');
let inputCheck = document.getElementById('inputCheck');
let inputPeriodo = document.getElementById('inputPeriodo'),
    inputCategoria = document.getElementById('inputCategoria');
inputCheck.value = 1;

check.addEventListener('change', () => {
    if (check.checked) {
        inputCheck.value = 1;
    } else {
        inputCheck.vale = 0;
    }
});
btn_newCategoria.addEventListener('click', function () {
    const c = prompt("Ingrese la nueva categoría");
    $.ajax({
        url: '/Vehiculo/SelectAgregarCategoria?categoria=' + c,
        type: 'GET',
        success: function (result) {
            if (result.type === "void") {
                alert(result.mensaje);
                return;
            }
            if (result.type === "error") {
                alert(result.mensaje);
                return;
            }
            if (result.type === "success") {
                var option = document.createElement('option');
                option.value = result.data.Id;
                option.text = result.data.Categoria1;
                inputCategoria.appendChild(option);

                alert(result.mensaje);
                return;
            }
        },
        error: function (xhr, error) {
            console.log(error);
        }
    });
})
btn_newPeriodo.addEventListener('click', function () {
    const p = prompt("Ingrese el nuevo periodo");
    $.ajax({
        url: '/Vehiculo/SelectAgregarPeriodo?periodo=' + p,
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
                option.text = result.data.Periodo;
                inputPeriodo.appendChild(option);

                alert(result.mensaje);
            }
        },
        error: function (xhr, error) {
            console.log(error);
        }
    });

});

function obtenerPeriodos() {
    $.ajax({
        url: '/Vehiculo/GetPeriodosMantencion',
        type: 'GET',
        success: function (result) {
            if (result !== null || result.length > 0) {
                dropDown.innerHTML = '';
                for (var i = 0; i < result.length; i++) {
                    var option = document.createElement('option');
                    option.value = result[i].Value;
                    option.innerHTML = result[i].Text;
                    dropDown.appendChild(option);
                }
            }
            alert("No existen registros de periodos de mantención");
        },
        error: function (xhr, error) {
            alert(error);
        }
    });
}