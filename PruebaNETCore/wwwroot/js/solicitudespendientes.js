let ids = []
$(document).ready(function () {
    $.fn.dataTable.moment('DD-MM-YYYY H:mm:ss');
    $('#tableSolPendiente').DataTable({
        columnDefs: [
            {
                target: 0,
                visible: false,
                searchable: false,
            },
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
        },
    });
});

const btn_aprobar = document.querySelectorAll('.btn-success'),
    btn_rechazado = document.querySelectorAll('.btn-danger'),
    btn_info = document.querySelectorAll('.btn-info');

btn_info.forEach(function (boton) {
    boton.addEventListener('click', () => {
        MostrarFichaSolicitud(boton);
    });
});

btn_aprobar.forEach(function (boton) {
    boton.addEventListener('click', () => {
        ConfirmacionAprobacion(boton);
    });
});

btn_rechazado.forEach(function (boton) {
    boton.addEventListener('click', () => {
        ConfirmacionRechazar(boton);
    });
});

const ConfirmacionAprobacion = (boton) => {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Solicitud/AsignarConductor?id=' + id,
        type: 'GET',
        success: function (result) {
            abrirModal(result);
        },
        error: function (xhr, error) {
            console.log(error);
        }
    });
}

const ConfirmacionRechazar = (boton) => {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Solicitud/ConfirmacionRechazar?id=' + id,
        type: 'GET',
        success: function (result) {
            if (result !== null) {
                abrirModal(result);
            }
        },
        error: function (status, error) {
            console.log(status, error);
        }
    });
}

const MostrarFichaSolicitud = (boton) => {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Solicitud/MasInformacionSolicitud?id=' + id,
        type: 'GET',
        success: function (result) {
            abrirModal(result);
        },
        error: function (xhr, error) {
            if (xhr.status === 400) {
                $.notify(xhr.responseText, { type: 'error' });
            } else {
                console.log(error);
            }
        }
    });
}

const abrirModal = (data) => {
    try {
        $('#coreModal').html(data);
        $('#coreModal').modal('show');
    } catch (e) {
        alert('No puede ingresar a este modulo o funcion, ya que no tiene los permisos suficientes');
    }
}