$(document).ready(function () {
    $.fn.dataTable.moment('DD-MM-YYYY H:mm:ss');
    $('#tablaMantencion').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
        },
        order: [[0, 'desc']]
    });
});