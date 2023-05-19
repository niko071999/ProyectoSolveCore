const check = document.getElementById('gridCheck');
let dropDown = document.getElementById('inputPeriodos');
let inputCheck = document.getElementById('inputCheck');
inputCheck.value = 1;

check.addEventListener('change', () => {
    if (check.checked) {
        inputCheck.value = 1;
    } else {
        inputCheck.vale = 0;
    }
});

function obtenerPeriodos() {
    $.ajax({
        url: '/Vehiculo/GetPeriodosMantencion',
        type: 'GET',
        success: function (result) {
            dropDown.innerHTML = '';
            for (var i = 0; i < result.length; i++) {
                var option = document.createElement('option');
                option.value = result[i].Value;
                option.innerHTML = result[i].Text;
                dropDown.appendChild(option);
            }
        },
        error: function (xhr, error) {
            console.log(error);
        }
    });
}