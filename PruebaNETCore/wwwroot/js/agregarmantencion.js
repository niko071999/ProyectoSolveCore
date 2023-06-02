$(document).ready(function () {
    $('.form-select').select2({
        language: "es"
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
            console.log(result);
            $('#inputConductor').val(result).trigger("change");
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
            $('#inputVehiculo').val(result).trigger("change");
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