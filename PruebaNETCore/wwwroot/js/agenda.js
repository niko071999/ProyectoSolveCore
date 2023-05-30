$(document).ready(function () {
    function generarColorAzar() {
        var coloresGenerados = [];

        function generarColorHex() {
            var color = Math.floor(Math.random() * 16777216).toString(16);
            return '#' + ('000000' + color).slice(-6);
        }

        function esColorRepetido(color) {
            return coloresGenerados.indexOf(color) !== -1;
        }

        function esBlancoOAdyacente(color) {
            var blanco = "#FFFFFF";
            var tonalidadesCercanas = ["#FEFEFE", "#FDFDFD", "#FCFCFC", "#FBFBFB", "#FAFAFA"];
            return color === blanco || tonalidadesCercanas.includes(color);
        }

        function generarNuevoColor() {
            var color = generarColorHex();
            while (esColorRepetido(color) || esBlancoOAdyacente(color)) {
                color = generarColorHex();
            }
            return color;
        }

        var primerColor = generarNuevoColor();
        coloresGenerados.push(primerColor);

        return generarNuevoColor;
    }

    var obtenerColorAzar = generarColorAzar();
    $.ajax({
        url: '/Solicitud/GetSolicitudes',
        type: 'GET',
        success: function (result) {
            var events = []
            for (var i = 0; i < result.length; i++) {
                var e = {
                    id: result[i].id,
                    name: result[i].vehiculo + ' / ' + result[i].nombreConductor,
                    description: result[i].fechaLongSalidaStr + ' - ' + result[i].fechaLongLlegadaStr + ': ' + result[i].motivo,
                    date: [result[i].fechaSalidaStr, result[i].fechaLlegadaStr],
                    type: 'event',
                    color: obtenerColorAzar()
                };
                events.push(e);
            }
            $("#contendor_calendar").removeClass("d-flex justify-content-center");
            $("#spinner").hide();
            $('#calendar').evoCalendar({
                theme: 'Royal Navy',
                language: 'es',
                todayHighlight: true,
                sidebarToggler: false,
                eventListToggler: false,
                firstDayOfWeek: 1,
                calendarEvents: events
            });
            
        },
        error: function (xhr, error) {
            if (xhr.status === 400) {
                alert(xhr.responseText);
            } else {
                alert('Ocurrio un error inesperado, recargue la pagina');
            }
        }
    });
    
});

$('#calendar').on('selectEvent', function (activeEvent, e) {
    console.log(e);
    var id = e.id;
    $.ajax({
        url: '/Solicitud/MasInformacionSolicitud?id=' + id +'&aprobacion=false',
        type: 'GET',
        success: function (result) {
            abrirModal(result);
        },
        error: function (xhr) {
            if (xhr.status === 400) {
                alert(xhr.responseText);
            } else {
                console.log(xhr);
            }
        }
    });
});


const abrirModal = (data) => {
    try {
        $('#coreModal').html(data);
        $('#coreModal').modal('show');
    } catch (e) {
        alert('No puede ingresar a este modulo o funcion, ya que no tiene los permisos suficientes');
    }
}

