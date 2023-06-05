$(document).ready(() => {
    const fecha_salida_input = document.getElementById('inputFechaSalida'),
        fecha_llegada_input = document.getElementById('inputFechaLlegada'),
        FechaSolicitado = document.getElementById('FechaSolicitado');

    const formato1 = "d-m-Y H:i:S";
    const btn_getVehiculo = document.getElementById('btn_getVehiculo'),
        btn_solicitar = document.getElementById('btn_solicitar');
    const dropDown = document.getElementById('inputVehiculos');

    //CONFIGURACION INPUT DATETIME
    flatpickr(fecha_salida_input, {
        enableTime: true,
        dateFormat: "Y-m-d H:i:S",
        altFormat: formato1, // Formato para enviar al controlador
        altInput: true, // Habilita un input auxiliar para el formato de envío
        altInputClass: "flatpickr-alt-input form-control", // Clase CSS para el input auxiliar
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
        dateFormat: "Y-m-d H:i:S",
        altFormat: formato1, // Formato para enviar al controlador
        altInput: true, // Habilita un input auxiliar para el formato de envío
        altInputClass: "flatpickr-alt-input form-control", // Clase CSS para el input auxiliar
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
    dropDown.addEventListener('mousedown', getVehiculos);

    btn_getVehiculo.addEventListener('click', getVehiculos);

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
    document.getElementById('inputFechaLlegada')._flatpickr.set("minDate", fecha_salida_input.value);
    btn_solicitar.addEventListener('click', function () {
        console.log(fecha_llegada_input.value);

        //ObtenerFechaHoy();
        document.getElementById('form_solicitar').submit();
    });
});