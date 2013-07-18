$(document).ready(function () {
    $("#id_area").bind("change", function () {
        $('#detailEquipment').hide();
        if ($('#id_area').val() != '') {

            $('#realmod_checker_area').css("display", "block");
            $.post(
			    "Fracas/GetUnit",
			    {
			        id_area: $('#id_area').val()
			    },
			    function (data) {
			        hasil = eval(data);
			        $("#id_unit").html("");
			        $('<option value="">-- Select Unit --</option>').appendTo("#id_unit");
			        for (var i = 0; i < hasil.length; i++) {
			            $("<option value='" + hasil[i]['id'] + "'>" + hasil[i]['nama'] + "</option>").appendTo("#id_unit");
			        }
			        $('#realmod_checker_area').css("display", "none");
			    }
			);
        } else {
            $('#realmod_checker_area').css("display", "none");
        }
    });

    $("#id_unit").bind("change", function () {
        $('#detailEquipment').hide();
        if ($('#id_unit').val() != '') {
            $('#realmod_checker_unit').css("display", "block");
            $.post(
				    "Fracas/GetSystem",
				    {
				        id_unit: $('#id_unit').val()
				    },
				    function (data) {
				        hasil = eval(data);
				        $("#id_system").html("");
				        $('<option value="">-- Select System --</option>').appendTo("#id_system");
				        for (var i = 0; i < hasil.length; i++) {
				            $("<option value='" + hasil[i]['id'] + "'>" + hasil[i]['nama'] + "</option>").appendTo("#id_system");
				        }
				        $('#realmod_checker_unit').css("display", "none");
				    }
			        );
        } else {
            $('#realmod_checker_unit').css("display", "none");
        }
    });

    $("#id_system").bind("change", function () {
        $('#detailEquipment').hide();
        if ($('#id_system').val() != '') {
            $('#realmod_checker_system').css("display", "block");
            $.post(
				    "Fracas/GetEquipment",
				    {
				        id_system: $('#id_system').val()
				    },
				    function (data) {
				        hasil = eval(data);
				        $("#id_equipment").html("");
				        $('<option value="">-- Select Equipment --</option>').appendTo("#id_equipment");
				        for (var i = 0; i < hasil.length; i++) {
				            $("<option value='" + hasil[i]['id'] + "'>" + hasil[i]['tag_num'] + "</option>").appendTo("#id_equipment");
				        }
				        $('#realmod_checker_system').css("display", "none");
				    }
			        );
        } else {
            $('#realmod_checker_system').css("display", "none");
        }
    });

    $("#id_equipment").bind("change", function () {
        if ($('#id_equipment').val() != '') {
            $('#realmod_checker_equipment').css("display", "block");
            $.post(
				    "Fracas/GetDataEquipment",
				    {
				        id_equipment: $('#id_equipment').val()
				    },
				    function (data) {
				        hasil = eval(data);
				        var test = $('#id_equipment').val();
				        $("#last_operation").val('');
				        $("#id_part").html('');
				        $("#id_failure_mode").html('');

				        $("#last_operation").val(hasil[1]);
				        $('<option value="">-- None --</option>').appendTo("#id_part");
				        for (var i = 0; i < hasil[0].length; i++) {
				            $("<option value='" + hasil[0][i]['id'] + "'>" + hasil[0][i]['tag_number'] + "</option>").appendTo("#id_part");
				        }
				        $('<option value="">-- Select Failure Mode --</option>').appendTo("#id_failure_mode");
				        for (var i = 0; i < hasil[2].length; i++) {
				            $("<option value='" + hasil[2][i]['id'] + "'>" + hasil[2][i]['description'] + "</option>").appendTo("#id_failure_mode");
				        }

//				        $('#detailEquipmentContent').empty().append("<h3>" + hasil[3][0]['nama'] + "</h3><p>PDF:<input type=\"text\" readonly=\"readonly\" value=" + hasil[3][0]['pdf'] + "></input></p><p>MTBF:" + hasil[3][0]['mtbf'] + "</p><p>MTTR:" + hasil[3][0]['mttr'] + "</p>");
				        $('#detailEquipmentContent').empty().append("<h3>" + hasil[3][0]['nama'] + "</h3><table cellspacing=\"0\" border=\"0\" cellpadding=\"0\" width=\"300px\" style=\"margin-top: -15px; border-collapse:collapse; \"><tr><td style=\"font-weight:normal; font-size:11pt;\">PDF</td><td><input type=\"text\" readonly=\"readonly\" value=" + hasil[3][0]['pdf'] + "></input></td></tr><tr><td style=\"font-weight:normal; font-size:11pt;\">MTBF</td><td><input type=\"text\" style=\"width:30px\" readonly=\"readonly\" value=" + hasil[3][0]['mtbf'] + "></input></td></tr><tr><td style=\"font-weight:normal; font-size:11pt;\">MTTR</td><td><input type=\"text\" style=\"width:30px\" readonly=\"readonly\" value=" + hasil[3][0]['mttr'] + "></input></td></tr></table>");
				        $('#realmod_checker_equipment').css("display", "none");
				        $('.hideFracas').css("display", "block");
				        $('#batalFracas').css("display", "none");
				        $('#detailEquipment').show();
				    }
			        );
        } else {
				    $('#realmod_checker_equipment').css("display", "none");
				    $('#detailEquipment').hide();
        }
    });

    $("#id_part").bind("change", function () {
        if ($('#id_part').val() != '') {
            $('#realmod_checker_part').css("display", "block");
            $.post(
				    "Fracas/GetLastOpPart",
				    {
				        id_part: $('#id_part').val(),
				        id_equipment: $('#id_equipment').val()
				    },
				    function (data) {
				        $("#last_operation").val('');
				        $("#last_operation").val(data);

				        $('#realmod_checker_part').css("display", "none");
				    }
			);
        } else {
            $('#realmod_checker_part').css("display", "none");
        }
    });

});


function back() {
    window.location = "/Fracas";
    //$('#content').load('Fracas/Index');
}

function calculate_downtime() {
    var valid = false;
    var ops = 0; var stop = 0; var now = 0; last_ops = 0;
    if ($("#datetime_ops").val() != '')
        ops = (new Date($("#datetime_ops").val())).getTime();
    if ($("#datetime_stop").val() != '')
        stop = (new Date($("#datetime_stop").val())).getTime();
    if ($("#last_operation").val() != '')
        last_ops = (new Date($("#last_operation").val())).getTime();
    now = (new Date()).getTime();
    
    if (($("#datetime_stop").val() == '') && ($("#datetime_ops").val() == ''))
        return;

    if (((now - ops) > 0) && ((now - stop) > 0)) {
        if ((stop - last_ops) > 0) {
            valid = true;
            $("#realmod_date_ops").html("");
            $("#realmod_date_stop").html("");
        } else {
            $("#realmod_date_stop").html("<font COLOR=\"red\">Date/Time Stop harus lebih besar dari<br /> Last Operation </font>");
            return;
        }
        if(ops != 0){
            if ((ops - stop) > 0) {
                valid = true;
                $("#realmod_date_ops").html("");
                $("#realmod_date_stop").html("");
            } else {
                $("#realmod_date_ops").html("<font COLOR=\"red\">Date/Time Ops harus lebih besar dari<br /> Date/Time Stop</font>");
                $("#datetime_ops").val("");
                $("#downtime").val("");
                return;
            }
        }
    } else {
        if ((now - ops) < 0) {
            $("#datetime_ops").val(""); $("#realmod_date_ops").html("<font COLOR=\"red\">Date/Time Ops tidak boleh melebihi hari ini</font>"); $("#downtime").val("");
        }
        else {
            $("#datetime_stop").val(""); $("#realmod_date_stop").html("<font COLOR=\"red\">Date/Time Stop tidak boleh melebihi hari ini</font>"); $("#downtime").val("");
        }
    }

    if (valid) {
        if ($("#datetime_stop").val() != '') {
            if ($("#datetime_ops").val() != '') {
                var downtime = (ops - stop) / (60 * 60 * 1000);
                $("#downtime").val(downtime);
                $("#durasi").val(downtime);
            } else {
                $("#downtime").val("");
                $("#durasi").val("");
            }
        }
    }
}

function addValue(idArea, idDropdown) {
    var value = $('#'+idArea).val();
    var html = '';
    if ($('#' + idDropdown + ' :selected').val() != "") {
        if (value == '') {
            html = $('#' + idDropdown + ' :selected').text();
        } else {
            html = ', ';
            html += $('#' + idDropdown + ' :selected').text();
        }
    }
    $('#'+idArea).val(value + html);
}

function addFailure() {
    addValue('area_failure_mode','id_failure_mode');    
}

function addCause() {
    addValue('area_failure_cause', 'id_failure_cause');
}

function addEffect() {
    addValue('area_failure_effect', 'id_failure_effect');
}

function addSecondaryEffect() {
    addValue('area_secondary_effect', 'id_secondary_effect');
}

function addEventDesc() {
    addValue('area_event_desc', 'id_event_desc');
}

function addImmidiate() {
    addValue('area_immidiate', 'id_immidiate');
}

function addLongTerm() {
    addValue('area_long_term', 'id_long_term');
}

function saveFracas() {

    if (validate()) {
        var values = {
            idEquipment: $('#id_equipment').val(),
            idEquipmentPart: $('#id_part').val(),
            dateTimeStop: $('#datetime_stop').val(),
            dateTimeStart: $('#datetime_ops').val(),
            durasi: $('#durasi').val(),
            downtime: $('#downtime').val(),
            failureMode: $('#area_failure_mode').val(),
            cause: $('#area_failure_cause').val(),
            failureEffect: $('#area_failure_effect').val(),
            secondaryEffect: $('#area_secondary_effect').val(),
            failureSeverity: $('#id_severity').val(),
            failureClss: $('#id_clsss').val(),
            immediateAction: $('#area_immidiate').val(),
            longTermAction: $('#area_long_term').val(),
            financialCost: $('#financial_cost').val(),
            engineer: $('#engineer').val(),
            repairCost: $('#repair_cost').val(),
            eventDesc: $('#area_event_desc').val()
        }
        $('#realmod_save').css("display", "block");
        $.post("Fracas/Save", values, function (data) {
            if (data != undefined) {
                $('#realmod_save').css("display", "none");
                alert('Event data successfully saved');
                back();
            } else {
                $('#realmod_save').css("display", "none");
                alert('Fail to add event data');
            }
        });
    }

}

function saveFracasEdit(id, id_equipment) {

    if (validate()) {
        var values = {
            id : id,
            idEquipment : id_equipment,
            idEquipmentPart: '',
            dateTimeStop: $('#datetime_stop').val(),
            dateTimeStart: $('#datetime_ops').val(),
            durasi: $('#durasi').val(),
            downtime: $('#downtime').val(),
            failureMode: $('#area_failure_mode').val(),
            cause: $('#area_failure_cause').val(),
            failureEffect: $('#area_failure_effect').val(),
            secondaryEffect: $('#area_secondary_effect').val(),
            failureSeverity: $('#id_severity').val(),
            failureClss: $('#id_clsss').val(),
            immediateAction: $('#area_immidiate').val(),
            longTermAction: $('#area_long_term').val(),
            financialCost: $('#financial_cost').val(),
            engineer: $('#engineer').val(),
            repairCost: $('#repair_cost').val(),
            eventDesc: $('#area_event_desc').val()
        }
        $('#realmod_save').css("display", "block");
        $.post("Fracas/SaveEdit", values, function (data) {
            if (data == true) {
                $('#realmod_save').css("display", "none");
                alert('Event data successfully updated');
                back();
            } else {
                $('#realmod_save').css("display", "none");
                alert('Inputted date is not valid');
            }
        });
    }

}

function saveFracasEditP(id, id_equipment_part) {

    if (validate()) {
        var values = {
            id: id,
            idEquipment: '',
            idEquipmentPart: id_equipment_part,
            dateTimeStop: $('#datetime_stop').val(),
            dateTimeStart: $('#datetime_ops').val(),
            durasi: $('#durasi').val(),
            downtime: $('#downtime').val(),
            failureMode: $('#area_failure_mode').val(),
            cause: $('#area_failure_cause').val(),
            failureEffect: $('#area_failure_effect').val(),
            secondaryEffect: $('#area_secondary_effect').val(),
            failureSeverity: $('#id_severity').val(),
            failureClss: $('#id_clsss').val(),
            immediateAction: $('#area_immidiate').val(),
            longTermAction: $('#area_long_term').val(),
            financialCost: $('#financial_cost').val(),
            engineer: $('#engineer').val(),
            repairCost: $('#repair_cost').val(),
            eventDesc: $('#area_event_desc').val()
        }
        $('#realmod_save').css("display", "block");
        $.post("Fracas/SaveEditP", values, function (data) {
            if (data == true) {
                $('#realmod_save').css("display", "none");
                alert('Event data successfully updated');
                back();
            } else {
                $('#realmod_save').css("display", "none");
                alert('Inputted date is not valid');
            }
        });
    }

}

function validate() {
    if ($('#id_equipment').val() == '') {
        alert('Please select equipment first');
        return false;
    }

    if ($('#datetime_stop').val() == '') {
        alert(' Fill Date/Time Stop');
        return false;
    }

    if (($("#realmod_date_stop").html() != '') || ($("#realmod_date_ops").html() != '')) {
        alert('Error(s) still found');
        return false;
    }

    if ($('#area_event_desc').val() == '') {
        alert('Fill Event Description');
        return false;
    }
    return true;    
}	