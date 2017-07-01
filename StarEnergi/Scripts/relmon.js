//RENDER CONTEXT MENU
onLoad = function (e) {  
    //PLANT
    $('#TreeSBS').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 5) == 'PLANT'));
        },
        menuItems: [
            {
                text: '+Area',
                onclick: MyTreeView_OnAdd
            }
        ]
    });

    //SUBCOMPONENT
    $('#TreeSBS').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 12) == 'SUBCOMPONENT'));
        },
        menuItems: [
            {
                text: 'Edit',
                onclick: MyTreeView_OnEdit
            },
            {
                text: 'Delete',
                onclick: MyTreeView_OnDelete
            },
        ]
    });

    //AREA
    $('#TreeSBS').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 4) == 'AREA'));
        },
        menuItems: CreateContextMenu('Unit')
    });

    //UNIT
    $('#TreeSBS').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 4) == 'UNIT'));
        },
        menuItems: CreateContextMenu('System')
    });

    //SYSTEM
    $('#TreeSBS').data('tTreeView').addContextMenu({       
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 6) == 'SYSTEM'));
        },
        //menuItems: CreateContextMenu('Equipment-Group')
        menuItems: CreateContextMenu('Equipment')
    });

    //EQUIPMENT GROUP
    //$('#TreeSBS').data('tTreeView').addContextMenu({
    //    evaluateNode: function (treeview, node) {
    //        var nodeValue = treeview.getItemValue(node);
    //        return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 15) == 'EQUIPMENT_GROUP'));
    //    },
    //    menuItems: CreateContextMenu('Equipment')
    //});

    //EQUIPMENT
    $('#TreeSBS').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 10) == 'EQUIPMENTS'));
        },
        //menuItems: CreateContextMenu('Sub-Equipment')
        menuItems: CreateContextMenu('Component')
    });

    //PART
    //$('#TreeSBS').data('tTreeView').addContextMenu({
    //    evaluateNode: function (treeview, node) {
    //        var nodeValue = treeview.getItemValue(node);
    //        return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 4) == 'PART'));
    //    },
    //    menuItems: CreateContextMenu('Component')
    //});

    //COMPONENT
    //$('#TreeSBS').data('tTreeView').addContextMenu({
    //    evaluateNode: function (treeview, node) {
    //        var nodeValue = treeview.getItemValue(node);
    //        return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 9) == 'COMPONENT'));
    //    },
    //    menuItems: CreateContextMenu('Sub-Component')
    //});

    //COMPONENT
    $('#TreeSBS').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 9) == 'COMPONENT'));
        },
        menuItems: [
            {
                text: 'Edit',
                onclick: MyTreeView_OnEdit
            },
            {
                text: 'Delete',
                onclick: MyTreeView_OnDelete
            },
        ]
    });
}

MyTreeView_OnAdd = function (e) {
    var value = e.treeview.getItemValue(e.node);
    DeselectNode();
    SelectNode(value);
    value = value.split(';');
    $('#content').load(ReturnLink(value, 'Create'));
    //alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
}

MyTreeView_OnEdit = function (e) {
    var value = e.treeview.getItemValue(e.node);
    DeselectNode();
    SelectNode(value);
    value = value.split(';');
    $('#content').load(ReturnLink(value,'Edit')+value[1]);
    //alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
}

MyTreeView_OnDelete = function (e) {
    //alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
    var value = e.treeview.getItemValue(e.node);
    DeselectNode();
    SelectNode(value);
    value = value.split(';');
    var values =
    {
        "Id": value[1]
    }
    var answer = confirm("Hapus " + value[0] + " " + e.treeview.getItemText(e.node) + " ?")
    if (answer) {
        $.post(ReturnLink(value, 'Delete'), values, function (data) {
            if (data) {
                RemoveItem(e);
                $('#content').html('<div id="hapus">'+value[0] + ' ' + e.treeview.getItemText(e.node)+' berhasil dihapus</div>');
            } else {
                alert("Data " + value[0] + " gagal dihapus");
            }
        });
    }

}

//REDIRECT LINK WHEN CONTEXT MENU SELECTED
//function ReturnLink(value, action) {
//    if (action == 'Create') {
//        if (value[0] == 'PLANT') {
//            return 'Foc/' + action + '/' + value[1];
//        } else if (value[0] == 'AREA') {
//            return 'Unit/' + action + '/' + value[1];
//        } else if (value[0] == 'UNIT') {
//            return 'System/' + action + '/' + value[1];
//        } else if (value[0] == 'SYSTEM') {
//            return 'EquipmentGroup/' + action + '/' + value[1];
//        } else if (value[0] == 'EQUIPMENT_GROUP') {
//            return 'Equipment/' + action + '/' + value[1];
//        } else if (value[0] == 'EQUIPMENTS') {
//            return 'SubEquipment/' + action + '/' + value[1];
//        } else if (value[0] == 'PART') {
//            return 'Component/' + action + '/' + value[1];
//        } else if (value[0] == 'COMPONENT') {
//            return 'SubComponent/' + action + '/' + value[1];
//        } 
//        return;
//    } else {
//        if (value[0] == 'PLANT') {
//            return 'Plant/' + action + '/';
//        } else if (value[0] == 'AREA') {
//            return 'Foc/' + action + '/';
//        } else if (value[0] == 'UNIT') {
//            return 'Unit/' + action + '/';
//        } else if (value[0] == 'SYSTEM') {
//            return 'System/' + action + '/';
//        } else if (value[0] == 'EQUIPMENT_GROUP') {
//            return 'EquipmentGroup/' + action + '/';
//        } else if (value[0] == 'EQUIPMENTS') {
//            return 'Equipment/' + action + '/';
//        } else if (value[0] == 'PART') {
//            return 'SubEquipment/' + action + '/';
//        } else if (value[0] == 'COMPONENT') {
//            return 'Component/' + action + '/';
//        } else if (value[0] == 'SUBCOMPONENT') {
//            return 'SubComponent/' + action + '/';
//        } 
//        return '';
//    }
//}

//REDIRECT LINK WHEN CONTEXT MENU SELECTED UPDATED
function ReturnLink(value, action) {
    if (action == 'Create') {
        if (value[0] == 'PLANT') {
            return 'Foc/' + action + '/' + value[1];
        } else if (value[0] == 'AREA') {
            return 'Unit/' + action + '/' + value[1];
        } else if (value[0] == 'UNIT') {
            return 'System/' + action + '/' + value[1];
        } else if (value[0] == 'SYSTEM') {
            return 'Equipment/' + action + '/' + value[1]; // Value system
        } else if (value[0] == 'EQUIPMENTS') {
            return 'Component/' + action + '/' + value[1]; // value equipment
        } 
        return;
    } else {
        if (value[0] == 'PLANT') {
            return 'Plant/' + action + '/';
        } else if (value[0] == 'AREA') {
            return 'Foc/' + action + '/';
        } else if (value[0] == 'UNIT') {
            return 'Unit/' + action + '/';
        } else if (value[0] == 'SYSTEM') {
            return 'System/' + action + '/';
        }  else if (value[0] == 'EQUIPMENTS') {
            return 'Equipment/' + action + '/';
        }  else if (value[0] == 'COMPONENT') {
            return 'Component/' + action + '/';
        } 
        return '';
    }
}

//REMOVE NODE FROM TREE
function RemoveItem(e) {
    var treeView = $("#TreeSBS").data("tTreeView");
    treeView.remove(e.node);
}

//ADD NEW NODE TO TREE
function AppendItem(text,value) {
    var treeView = $("#TreeSBS").data("tTreeView");
    var itemData = { Text: text ,Value:value, ImageUrl :"../Content/image/file.png"};
    var selected = GetNode('#TreeSBS .t-state-selected');
    treeView.append(itemData, selected.length != 0 ? selected : null);
    var select = treeView.findByText(text);
    //getSelectNode().removeClass("t-state-selected");
    //select.next('input').addClass("t-state-selected");
}

//GET NODE SELECTED FROM TREE
function GetNode(tree) {
    return $(tree).closest('li');
}

//CREATE CONTEXT MENU 
function CreateContextMenu(name) {
    return [
            {
                text: 'Edit',
                onclick: MyTreeView_OnEdit
            },
            {
                text: 'Delete',
                onclick: MyTreeView_OnDelete
            },
            {
                text: '+' + name,
                onclick: MyTreeView_OnAdd
            }
        ];
}

//SELECT NODE IN TREE
function SelectNode(value) {
    $("#TreeSBS").find('input.t-input[name="itemValue"][value="' + value + '"]').prev().addClass("t-state-selected");
    //$("#TreeSBSEmployee").find('input.t-input[name="itemValue"][value="' + value + '"]').prev().addClass("t-state-selected");
}

//DESELECT NODE IN TREE
function DeselectNode() {
    $('#TreeSBS .t-state-selected').removeClass("t-state-selected");
    //$('#TreeSBSEmployee .t-state-selected').removeClass("t-state-selected");
}

//onSelect menu admin 2
function onSelectMenuAdmin(e) {
    var item = $(e.item);
    if(item.find('> .t-link').text() == 'Readines Navigator'){
        $('#content').load('EquipmentReadNav/Index');
    } else if (item.find('> .t-link').text() == 'MA Area') {
        $('#content').load('Ma/Index');
    } else if (item.find('> .t-link').text() == 'Plant Availibility') {
        $('#content').load('PlantAvailibility/Index');
    } else if (item.find('> .t-link').text() == 'Fracas') {
        $('#content').load('Fracas/Index');
    } else if (item.find('> .t-link').text() == 'Ma Unit') {
        $('#content').load('MaUnit/Index');
    } else if (item.find('> .t-link').text() == 'Incident Report') {
        $('#content').load('Incident/Index');
    } else if (item.find('> .t-link').text() == 'Incident Investigation Report') {
        $('#content').load('Investigation/Index');
    } else if (item.find('> .t-link').text() == 'Daily Log') {
        $('#content').load('DailyLog/Index');
    } else if (item.find('> .t-link').text() == 'Illness Report') {
        $('#content').load('IllnessReport/Index');
    } else if (item.find('> .t-link').text() == 'SHE Observation') {
        $('#content').load('SheObservation/Index');
    } else if (item.find('> .t-link').text() == 'Equipment Daily Report') {
        $('#content').load('EquipmentDailyReport/Index');
    }
}

onLoadEmployee = function (e) {
    //SBS Employee
    //PLANT
    $('#TreeSBSEmployee').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 5) == 'PLANT'));
        },
        menuItems: [
            {
                text: '+Dept',
                onclick: MyTreeViewEmployee_OnAdd
            }
        ]
    });

    //EMPLOYEE DEPT
    $('#TreeSBSEmployee').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 10) == 'DEPARTMENT'));
        },
        menuItems: CreateContextMenuEmployee('Employee')
    });

    //EMPLOYEE
    $('#TreeSBSEmployee').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 12) == 'EMPLOYEEBOSS'));
        },
        menuItems: CreateContextMenuEmployee('Employee')
    });

    //EMPLOYEE
    $('#TreeSBSEmployee').data('tTreeView').addContextMenu({
        evaluateNode: function (treeview, node) {
            var nodeValue = treeview.getItemValue(node);
            return ((node.find('ul').length >= 0) && (nodeValue.substring(0, 8) == 'EMPLOYEE'));
        },
        menuItems: CreateContextMenuEmployee('Employee')
    });
}

MyTreeViewEmployee_OnAdd = function (e) {
    var value = e.treeview.getItemValue(e.node);
    DeselectNodeEmployee();
    SelectNodeEmployee(value);
    value = value.split(';');
    $('#content').load(ReturnLinkEmployee(value, 'Create'));
    //alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
}

MyTreeViewEmployee_OnEdit = function (e) {
    var value = e.treeview.getItemValue(e.node);
    DeselectNodeEmployee();
    SelectNodeEmployee(value);
    value = value.split(';');
    $('#content').load(ReturnLinkEmployee(value, 'Edit') + value[1]);
    //alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
}

MyTreeViewEmployee_OnDelete = function (e) {
    //alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
    var value = e.treeview.getItemValue(e.node);
    DeselectNodeEmployee();
    SelectNodeEmployee(value);
    value = value.split(';');
    var values =
    {
        "Id": value[1]
    }
    var answer = confirm("Hapus " + value[0] + " " + e.treeview.getItemText(e.node) + " ?")
    if (answer) {
        $.post(ReturnLinkEmployee(value, 'Delete'), values, function (data) {
            if (data) {
                RemoveItemEmployee(e);
                $('#content').html('<div id="hapus">' + value[0] + ' ' + e.treeview.getItemText(e.node) + ' berhasil dihapus</div>');
            } else {
                alert("Data " + value[0] + " gagal dihapus");
            }
        });
    }

}



//REDIRECT LINK WHEN CONTEXT MENU SELECTED
function ReturnLinkEmployee(value, action) {
    if (action == 'Create') {
        if (value[0] == 'PLANT') {
            return 'EmployeeDept/' + action + '/' + value[1];
        } else if (value[0] == 'DEPARTMENT') {
            return 'EmployeeBoss/' + action + '/' + value[1];
        } else if (value[0] == 'EMPLOYEEBOSS') {
            return 'Employee/' + action + '/' + value[1];
        } else if (value[0] == 'EMPLOYEE') {
            return 'Employee/' + action + '/' + value[1];
        }
        return;
    } else {
        if (value[0] == 'DEPARTMENT') {
            return 'EmployeeDept/' + action + '/';
        } else if (value[0] == 'EMPLOYEEBOSS') {
            return 'EmployeeBoss/' + action + '/';
        } else if (value[0] == 'EMPLOYEE') {
            return 'Employee/' + action + '/';
        }
        return '';
    }
    return '';
}

//REMOVE NODE FROM TREE
function RemoveItemEmployee(e) {
    var treeView = $("#TreeSBSEmployee").data("tTreeView");
    var parent_item = $(e.node).parent().closest(".t-item");
    console.log(parent_item);
    treeView.remove(e.node);
    treeView.ajaxRequest(parent_item);
    treeView.expand(parent_item);
}

//ADD NEW NODE TO TREE
function AppendItemEmployee(text, value) {
    var treeView = $("#TreeSBSEmployee").data("tTreeView");
    var itemData = { Text: text, Value: value, ImageUrl: "../Content/image/file.png" };
    var selected = GetNode('#TreeSBSEmployee .t-state-selected');
    treeView.append(itemData, selected.length != 0 ? selected : null);
    var select = treeView.findByText(text);
    //getSelectNode().removeClass("t-state-selected");
    //select.next('input').addClass("t-state-selected");
}

//GET NODE SELECTED FROM TREE
function GetNodeEmployee(tree) {
    return $(tree).closest('li');
}

//CREATE CONTEXT MENU 
function CreateContextMenuEmployee(name) {
    return [
            {
                text: 'Edit',
                onclick: MyTreeViewEmployee_OnEdit
            },
            {
                text: 'Delete',
                onclick: MyTreeViewEmployee_OnDelete
            },
            {
                text: '+' + name,
                onclick: MyTreeViewEmployee_OnAdd
            }
    ];
}

//SELECT NODE IN TREE
function SelectNodeEmployee(value) {
    $("#TreeSBSEmployee").find('input.t-input[name="itemValue"][value="' + value + '"]').prev().addClass("t-state-selected");
}

//DESELECT NODE IN TREE
function DeselectNodeEmployee() {
    $('#TreeSBSEmployee .t-state-selected').removeClass("t-state-selected");
}

// Limiting file size for upload
function onSelectUpload(e) {
    if (e.files[0].size > 307200) {
        alert("The file size is too large for upload (max. 300 kB).");
        e.preventDefault();
        return false;
    }
    return true;
}

function getSubClass(idClass, loading, option , selected) {
    if (idClass != '') {

        $(loading).css("display", "inline-block");
        $.post(
                "Equipment/GetSubClass",
                {
                    id_class : idClass
                },
                function (data) {
                    console.log(data);
                    hasil = eval(data);
                    
                    $(option).html("");
                    $('<option value="">-- Select Class First --</option>').appendTo(option);
                    for (var i = 0; i < hasil.length; i++) {
                        if (hasil[i]['id'] == selected) {
                            $("<option value='" + hasil[i]['id'] + "' selected >" + hasil[i]['title'] + "</option>").appendTo(option);
                        } else {
                            $("<option value='" + hasil[i]['id'] + "'>" + hasil[i]['title'] + "</option>").appendTo(option);
                        }
                        
                    }
                    $(loading).css("display", "none");
                }
                );
    } else {
        $(loading).css("display", "none");
    }
}
