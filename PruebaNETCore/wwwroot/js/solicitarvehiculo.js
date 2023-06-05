$(document).ready(() => {
    //CONFIGURACION INPUT DATETIME
    flatpickr(fecha_salida_input, {
        enableTime: true,
        dateFormat: "d-m-Y H:i:S",
        defaultDate: moment().tz("America/Santiago").toDate(),
        minDate: "today",
        locale: "es",
        firstDayOfWeek: 1,
        onChange: function (selectedDates) {
            if (selectedDates.length > 0) {
                const minDate = selectedDates[0];
                fecha_llegada_input._flatpickr.set("minDate", minDate);
            }
        }
    });
    flatpickr(fecha_llegada_input, {
        enableTime: true,
        dateFormat: "d-m-Y H:i:S",
        minDate: "today",
        locale: "es",
        firstDayOfWeek: 1,
        onChange: function (selectedDates) {
            if (selectedDates.length > 0) {
                const maxDate = selectedDates[0];
                fecha_salida_input._flatpickr.set("maxDate", maxDate);
            }
        }
    });
    document.getElementById('inputFechaLlegada')._flatpickr.set("minDate", fecha_salida_input.value);
})

const btn_getVehiculo = document.getElementById('btn_getVehiculo'),
    btn_solicitar = document.getElementById('btn_solicitar');
const fecha_salida_input = document.getElementById('inputFechaSalida'),
    fecha_llegada_input = document.getElementById('inputFechaLlegada'),
    FechaSolicitado = document.getElementById('FechaSolicitado');
const dropDown = document.getElementById('inputVehiculos');

dropDown.addEventListener('mousedown', getVehiculos);

btn_getVehiculo.addEventListener('click', getVehiculos);
btn_solicitar.addEventListener('click', function () {
    ObtenerFechaHoy();
})


function ObtenerFechaHoy() {
    var fechaHoraActual = new Date();
    // Obtener los componentes de fecha y hora
    var year = fechaHoraActual.getFullYear();
    var mes = ("0" + (fechaHoraActual.getMonth() + 1)).slice(-2);
    var dia = ("0" + fechaHoraActual.getDate()).slice(-2);
    var horas = ("0" + fechaHoraActual.getHours()).slice(-2);
    var minutos = ("0" + fechaHoraActual.getMinutes()).slice(-2);
    // Construir el valor del input datetime-local
    var valorInput = year + "-" + mes + "-" + dia + "T" + horas + ":" + minutos;
    // Establecer el valor en el input
    FechaSolicitado.value = valorInput;

    console.log(valorInput);

    //document.getElementById('form_solicitar').submit();
}

//Inicializacion de los pasajeros
$('[name=Pasajeros]').tagify({
    duplicates: false,
    maxTags: Infinity,
    trim: true,
    backspace: true,
    editTags: {
        clicks: 1,
        keepInvalid: true,
    },
    mixMode: {
        insertAfterTag: ',',
    }
});
$('[name=Pasajeros]').on('change', function (e) {
    var arreglo = e.target.value !== "" ? JSON.parse(e.target.value) : [];
    document.getElementById('inputNumPasajeros').value = arreglo.length;
});

dropDown.addEventListener('change', () => {
    const opcionSeleccionada = dropDown.value;
    document.getElementById('idVehiculo').value = opcionSeleccionada;
});

function obtenerPasajeros() { 
    JSON.parse(document.getElementById('inputPasajeros').value);
    console.log(arreglo.length);
}

function getVehiculos() {
    if (fecha_salida_input.value !== '' || fecha_llegada_input.value !== '') {
        var datos = {
            fecha_salida: fecha_salida_input.value,
            fecha_llegada: fecha_llegada_input.value
        };

        $.ajax({
            url: '/Solicitud/GetVehiculos',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(datos),
            contentType: 'application/json',
            success: function (response) {
                console.log(response);
                if (response.data.length > 0) {
                    dropDown.innerHTML = '';
                    for (var i = 0; i < response.data.length; i++) {
                        var option = document.createElement('option');
                        option.value = response.data[i].Value;
                        option.innerHTML = response.data[i].Text;
                        console.log(response.data[i].Text);
                        dropDown.appendChild(option);
                    }
                    document.getElementById('idVehiculo').value = response.data[0].Value
                } else {
                    alert(response.mensaje);
                }
            }
        });
        return;
    }
    alert("Antes de desplegar vehiculos, se deben seleccionar la fecha de salida como la de llegada");

}