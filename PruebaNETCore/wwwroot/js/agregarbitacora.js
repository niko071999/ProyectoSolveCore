const btn_addbitacora = document.getElementById('btn_addBitacora');

btn_addbitacora.addEventListener('click', () => {
    var form = document.getElementById("form_AgregarBitacora");
    form.action = "/Bitacora/AgregarEntradaBitacora";
    form.submit();
})