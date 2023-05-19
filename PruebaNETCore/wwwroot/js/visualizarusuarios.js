$(document).ready(function () {
    $('#tablaUsuarios').DataTable({

        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-CL.json',
        },
    });
});
const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]'),
    tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
const btn_eliminar = document.querySelectorAll('.btn-danger'),
    btn_editar = document.querySelectorAll('.btn-warning');

btn_eliminar.forEach(function (boton) {
    boton.addEventListener('click', () => {
        EliminarUsuario(boton);
    });
});
btn_editar.forEach(function (boton) {
    boton.addEventListener('click', () => {
        EditarUsuario(boton);
    });
});

function EliminarUsuario(boton) {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Usuario/EliminarUsuario?id=' + id,
        type: 'GET',
        success: function (result) {
            abrirModal(result);
        },
        error: function (xhr, error) {
            console.log(error);
        }
    });
}

function EditarUsuario(boton) {
    const id = boton.dataset.id;
    $.ajax({
        url: '/Usuario/EditarUsuario?id=' + id,
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