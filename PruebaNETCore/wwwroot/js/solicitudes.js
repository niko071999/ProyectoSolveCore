$(document).ready(function () {
    $.fn.dataTable.moment('DD-MM-YYYY H:mm:ss');
    $('#tableSolicitudes').DataTable({
        pageLength: 8,
        lengthChange: false,
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
        },
        columnDefs: [
            {
                targets: 7,
                searchable: false,
                orderable: false
            }
        ]
    });
});

const btn_info = document.querySelectorAll('.btn-info');

const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]'),
    tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

btn_info.forEach(function (boton) {
    boton.addEventListener('click', () => {
        MostrarFichaSolicitud(boton);
    });
});

const MostrarFichaSolicitud = (boton) => {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Solicitud/MasInformacionSolicitud?id=' + id +'&aprobacion=false',
        type: 'GET',
        success: function (result) {
            abrirModal(result);
        },
        error: function (xhr, error) {
            if (xhr.status === 400) {
                alert(xhr.responseText);
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