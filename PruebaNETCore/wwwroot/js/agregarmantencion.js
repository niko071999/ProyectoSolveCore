$(document).ready(function () {
    const inputFecha = document.getElementById('inputFecha');
    $('.form-select').select2({
        language: "es"
    });
    flatpickr(inputFecha, {
        dateFormat: "Y-m-d",
        altFormat: "d-m-Y", // Formato para enviar al controlador
        altInput: true, // Habilita un input auxiliar para el formato de envío
        altInputClass: "flatpickr-alt-input form-control", // Clase CSS para el input auxiliar
        defaultDate: moment().tz("America/Santiago").toDate(),
        locale: "es",
        firstDayOfWeek: 1
    });
});

$('#inputConductor').trigger('select2:select');

//const inputVehiculo = document.getElementById('inputVehiculo'),
    //inputConductor = document.getElementById('inputConductor');

$('#inputVehiculo').on('select2:select', function (e) {
    const idvehiculo = e.params.data.id;
    $.ajax({
        url: '/Vehiculo/GetConductorVehiculo?id=' + idvehiculo,
        type: 'GET',
        success: function (result) {
            if (confirm("¿Cambiar conductor?")) {
                $('#inputConductor').val(result).trigger("change");
            }
        },
        error: function (xhr, error) {
            if (xhr.status === 400) {
                alert(xhr.responseText);
            } else {
                console.log(error);
            }
        }
    });
});
$('#inputConductor').on('select2:select', function (e) {
    const idconductor = e.params.data.id;
    $.ajax({
        url: '/Usuario/GetVehiculoConductor?id=' + idconductor,
        type: 'GET',
        success: function (result) {
            if (confirm("¿Cambiar vehículo?")) {
                $('#inputVehiculo').val(result).trigger("change");
            }
        },
        error: function (xhr, error) {
            if (xhr.status === 400) {
                alert(xhr.responseText);
            } else {
                console.log(error);
            }
        }
    });
});