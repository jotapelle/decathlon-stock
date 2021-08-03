var interval;
$('#tableResults').DataTable();

function start() {
    var t = $('#tableResults').DataTable();
    var productUrl = $('#productUrl').val();
    var email = $('#email').val();

    enableDisable("btnStart", true);
    enableDisable("btnStop", false);

    interval = setInterval(function () {

        $.ajax({
            type: "POST",
            url: "/Home/CheckProduct",
            data: { url: productUrl, email: email },
            success: function (result) {
                addRow(t, result.localTimeStr, productUrl, result.resultTxt);
            }
        });
        
    }, 60000);    
}

function stop() {
    $.ajax({
        type: "POST",
        url: "/Home/ResetSendEmail",
        data: { },
        success: function () {
        }
    });

    enableDisable("btnStart", false);
    enableDisable("btnStop", true);

    clearInterval(interval);
}

function addRow(table, time, url, text) {
    table.row.add([
        time,
        url,
        text
    ]).draw(false);
}

function enableDisable(id, enable) {
    $("#" + id).prop('disabled', enable);
}
