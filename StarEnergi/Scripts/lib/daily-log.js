function setIframeHeight(sel, h) {
    $(sel).height(h);
}

function onSuccessAttachment1(data) {
    var has = data.response.files.split(";;");
    $('#atch1').empty();
    for (var f in has) {
        if (has[f] != "") {
            $('#atch1').append('<a href="' + data.response.path + has[f] + '">' + has[f] + '</a><br/>');
        }
    }
}

function onSuccessAttachment2(data) {
    var has = data.response.files.split(";;");
    $('#atch2').empty();
    for (var f in has) {
        if (has[f] != "") {
            $('#atch2').append('<a href="' + data.response.path + has[f] + '">' + has[f] + '</a><br/>');
        }
    }
}

function onSelectAttachment2(data) {
    var has = data.files;
    for (var f in has) {
        ext = has[f].name.split(".").pop().toLowerCase();
        if (ext == "jpg" || ext == "jpeg" || ext == "png" || ext == "bmp") {

        } else {
            alert("Warning! Only image files that can be shown in report. Another types of file should be downloaded");
        }
    }
}

function onSelectAttachment1(data) {
    var has = data.files;
    for (var f in has) {
        ext = has[f].name.split(".").pop().toLowerCase();
        if (ext == "jpg" || ext == "jpeg" || ext == "png" || ext == "bmp") {

        } else {
            alert("Warning! Only image files that can be shown in report. Another types of file should be downloaded");
        }
    }
}
        
//mengganti masukan user tipe text / input
function changeInput(radio,shift) {
    //kamus
    var val = $(radio).val();
    var td, tdFlow, tdWhp;
    var selector;

    //algoritma
    td = $(radio).parent().next();
    tdFlow = td.next();
    tdWhp = tdFlow.next();
    selector = '.sh' + shift + '_fcv';

    if (val == 1) { //text => hide flow & whp
        td.attr("class", "yellow-bg center");
        td.attr('colspan', '3');

        td.find(selector).attr('style', 'width: 150px;');

        tdFlow.hide();
        tdWhp.hide();
    } else { //input => show flow & whp
        td.attr("class", "gray-bg center number");
        td.removeAttr('colspan');

        td.find(selector).attr('style', 'width: 50px;');

        tdFlow.show();
        tdWhp.show();
    }
}

function validate(shift) {
    var retVal = true;
    retVal = $("#sh" + shift + "_date").val() == "" ? false : true;
    return retVal;
}

function loadContent() {
    if (id2 != '') {
        $('.log-part13').load('DailyLogs/part3?id=' + id2, function () {
            $('.log-part3').load('DailyLogs/part3?id=' + id1);
        });
        $('.log-part14').load('DailyLogs/part4?id=' + id2, function () {
            $('.log-part4').load('DailyLogs/part4?id=' + id1);
        });
        $('.log-part16').load('DailyLogs/part6?id=' + id2, function () {
            $('.log-part6').load('DailyLogs/part6?id=' + id1);
        });
        $('.log-part17').load('DailyLogs/part7?id=' + id2, function () {
            $('.log-part7').load('DailyLogs/part7?id=' + id1);
        });
        $('.log-part18').load('DailyLogs/part8?id=' + id2, function () {
            $('.log-part8').load('DailyLogs/part8?id=' + id1);
        });
    } else {
        $('.log-part3').load('DailyLogs/part3?id=' + id1);
        $('.log-part4').load('DailyLogs/part4?id=' + id1);
        $('.log-part6').load('DailyLogs/part6?id=' + id1);
        $('.log-part7').load('DailyLogs/part7?id=' + id1);
        $('.log-part8').load('DailyLogs/part8?id=' + id1);
    }
}

function bindatch() {
    $.post("DailyLogs/Atch", { id: id1 }, function (data) {
        var has = data.files.split(";;");
        $('#atch1').empty();
        for (var f in has) {
            if (has[f] != "") {
                $('#atch1').append('<a href="' + data.path + has[f] + '">' + has[f] + '</a><br/>');
            }
        }
    });
    if (id2 != '') {
        $.post("DailyLogs/Atch", { id: id2 }, function (data) {
            var has = data.files.split(";;");
            $('#atch2').empty();
            for (var f in has) {
                if (has[f] != "") {
                    $('#atch2').append('<a href="' + data.path + has[f] + '">' + has[f] + '</a><br/>');
                }
            }
        });
    }
}

function back() {
    if (!isSave) {
        $.post("DailyLog/DeleteAllPowerStation", function (data) { });
        $.post("DailyLog/DeleteAllSAGS", function (data) { });
        $.post("DailyLog/DeleteAllPRO", function (data) { });
        $.post("DailyLog/DeleteAllMaintenance", function (data) { });
        $.post("DailyLog/DeleteAllLastPlantStatus", function (data) { });
    }
    //$('#contentEventLog').load('/DailyLogs');
    window.location = "/DailyLogs";
}

function approve(shift) {
    var values = {
        is_approve: 1,
        shift: shift
    };

    var ids = shift == 1 ? id1 : id2;
    values.id = ids;
    $.post("DailyLogs/Approve", values, function (data) {
        console.log(data);
        if (data != "" && data != null) {
            $('#realmod_save').css("display", "none");
            alert('Change shift Daily Log');
            $('#contentEventLog').load('DailyLogs/addDailyLog/' + id1);
        } else {
            $('#realmod_save').css("display", "none");
            alert('Target is not achieving, please submit remark');
        }
    });
}

function loadWell(shift) {
}

function ConvertTimeformat(str) {
    var time = str;
    var hours = Number(time.match(/^(\d+)/)[1]);
    var minutes = Number(time.match(/:(\d+)/)[1]);
    var AMPM = time.match(/\s(.*)$/)[1];
    if (AMPM == "PM" && hours < 12) hours = hours + 12;
    if (AMPM == "AM" && hours == 12) hours = hours - 12;
    var sHours = hours.toString();
    var sMinutes = minutes.toString();
    if (hours < 10) sHours = "0" + sHours;
    if (minutes < 10) sMinutes = "0" + sMinutes;

    //Creating the todays date in the right format
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1;//January is 0!`
    var yyyy = today.getFullYear();
    if (dd < 10) { dd = '0' + dd }
    if (mm < 10) { mm = '0' + mm }
    var todaysdate = dd + '/' + mm + '/' + yyyy + " "; //<--I added an extra space!
    var hoursNminutes = sHours + ":" + sMinutes
    //CREATE THE RIGHT FORMAT FOR DATE.PARSEXACT "dd/MM/yyyy HH:mm"
    //var dateToParse = todaysdate + hoursNminutes
    var dateToParse = hoursNminutes
    return dateToParse;
}