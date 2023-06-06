$(document).ready(function () {
    //$.fn.dataTable.moment('DD-MM-YYYY H:mm:ss');
    if (document.getElementById('tablaCVF')) {
        $('#tablaCVF').DataTable({
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'pdf',
                    text: '<i class="fa-solid fa-file-pdf"></i> <b>PDF</b>'
                },
                {
                    extend: 'print',
                    text: '<i class="fa-solid fa-print"></i> <b>IMPRIMIR</b>',
                },
            ],
            language: {
                url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
            },
        });
    }
    if (document.getElementById('tablaCVC')) {
        $('#tablaCVC').DataTable({
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'pdf',
                    text: '<i class="fa-solid fa-file-pdf"></i> <b>PDF</b>'
                },
                {
                    extend: 'print',
                    text: '<i class="fa-solid fa-print"></i> <b>IMPRIMIR</b>'
                },
            ],
            language: {
                url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
            },
        });
    }
});
//columnDefs: [
//    {
//        targets: [4, 10],
//        searchable: false,
//        orderable: false
//    }
//],
