window.reinitSelect2 = (id, dotnetRef, placeholderText) => {
    const el = $('#' + id);

    if (!el.length)
        return;

    // remove old wrapper safely
    el.next('.select2').remove();

    if (el.hasClass("select2-hidden-accessible")) {
        el.select2('destroy');
    }

    el.select2({
        width: '100%',
        //dropdownParent: $('.page-wrapper'),
        placeholder: placeholderText || ''
    });

    el.off('change.select2bind');

    el.on('change.select2bind', function () {
        dotnetRef.invokeMethodAsync('NotifyChange', $(this).val());
    });
};





window.setSelect2Value = (id, value) => {
    const el = $('#' + id);
    if (el.hasClass("select2-hidden-accessible")) {
        el.val(value).trigger('change.select2');
    }
};

function printSection(id) {

    var content = document.getElementById(id).innerHTML;

    var win = window.open('', '', 'width=900,height=650');

    win.document.write(content);

    win.document.close();

    win.print();

}