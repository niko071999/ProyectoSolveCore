$(document).ready(function () {
    /*$.fn.dataTable.moment('DD-MM-YYYY H:mm:ss');*/
    $('#tablaVehiculos').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
        },
    });
});

const btn_eliminar = document.querySelectorAll('.btn-danger'),
    btn_editar = document.querySelectorAll('.btn-warning');

btn_eliminar.forEach(function (boton) {
    boton.addEventListener('click', () => {
        EliminarVehiculo(boton);
    });
});
btn_editar.forEach(function (boton) {
    boton.addEventListener('click', () => {
        EditarVehiculo(boton);
    });
});

const EditarVehiculo = (boton) => {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Vehiculo/EditarVehiculo?id=' + id,
        type: 'GET',
        success: function (result) {
            abrirModal(result);
        },
        error: function (xhr, error) {
            console.log(error);
        }
    });
}

const EliminarVehiculo = (boton) => {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Vehiculo/EliminarVehiculo?id=' + id,
        type: 'GET',
        success: function (result) {
            abrirModal(result);
        },
        error: function (xhr, error) {
            console.log(error);
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