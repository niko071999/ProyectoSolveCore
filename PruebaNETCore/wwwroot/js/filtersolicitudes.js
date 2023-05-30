$(document).ready(() => {
    //CONFIGURACION INPUT DATETIME
    flatpickr(FechaDesde, {
        enableTime: true,
        dateFormat: "d-m-Y H:i:S",
        defaultDate: moment().tz("America/Santiago").toDate(),
        locale: "es",
        firstDayOfWeek: 1,
        onChange: function (selectedDates) {
            if (selectedDates.length > 0) {
                const minDate = selectedDates[0];
                FechaDesde._flatpickr.set("minDate", minDate);
            }
        }
    });
    flatpickr(FechaHasta, {
        enableTime: true,
        dateFormat: "d-m-Y H:i:S",
        locale: "es",
        firstDayOfWeek: 1,
    });
    document.getElementById('FechaHasta')._flatpickr.set("minDate", FechaDesde.value);
});
const FechaDesde = document.getElementById('FechaDesde'),
    FechaHasta = document.getElementById('FechaHasta');

//const btn_filtrar = document.getElementById('btn_filtrar');
const FormFilter = document.getElementById('FormFilter');

function Filtrar() {
    FormFilter.submit();
}