$(document).ready(function () {
    $.fn.dataTable.moment('DD-MM-YYYY H:mm:ss');
    $('#tablaSolicitud').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
        },
    });
});
const btn_info = document.querySelectorAll('.btn-info'),
    btn_bitacora = document.querySelectorAll('.btn-primary');

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