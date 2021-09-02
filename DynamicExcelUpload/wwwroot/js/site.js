// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var datatable = null;
var datatables = {
    init: function () {
        try {
            addTable();
            var my_columns = [];
            var dataset = [];

            console.log("filename:" + $('#FileName').val());

            $.ajax({
                url: "/Home/GetData?filename=" + $('#FileName').val(),
                type: "POST",
                async: false,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {

                    if (data.length > 0) {
                        dataset = data.slice(0);
                        $.each(dataset[0], function (key, value) {

                            var my_item = {};
                            my_item.data = key;
                            my_item.title = key;
                            my_columns.push(my_item);
                        });
                        $.each(dataset, function (key, value) {
                            Object.keys(value).forEach(function (key) {
                                if (jQuery.isEmptyObject(value[key])) {
                                    value[key] = '';
                                }
                            })

                        });

                        datatable = $('#datatable').DataTable({
                            destroy: true,
                            data: dataset,
                            ordering: false,
                            columns: my_columns
                        });
                    } else {
                        var my_item = {};
                        my_item.data = '';
                        my_item.title = '';
                        my_columns.push(my_item);

                        datatable = $('#datatable').DataTable({
                            destroy: true,
                            ordering: false,
                            columns: my_columns
                        });
                    }

                }

            });

        }
        catch (e) {
            console.log(e);
        }
    }
}

$('#uploadExcel #btnUpload').click(function () {
    processDoc();
});

$('#uploadExcel #btnDownload').click(function () {
    window.open('/Home/DownloadFile?fileName=' + $('#FileName').val())
});



function processDoc() {
    var isValid = true;
    var $dataForm = $('#uploadExcel');
    $dataForm.validate;
    var fileInput = $('#uploadExcel input[name=ExcelFile]');

    var fileCheck = fileInput.get()[0].files;

    if (fileCheck.length == 0) {
        $('#uploadExcel #ExcelFile-error').text('Field ini harus diisi.');
        isValid = false;
    }

    if (!$dataForm.valid()) {
        $('#uploadExcel #Title-error').addClass('text-danger');
        isValid = false;
    }

    if (!isValid) {
        $('#uploadExcel #Title-error').addClass('text-danger');
        return;
    }
    var data = new FormData();

    var file = fileInput.get()[0].files;
    data.append("file", file[0]);
    swal({
        title: "Processing...",
        text: "Please wait",
        imageUrl: "~/images/ajaxloading.gif",
        showConfirmButton: false,
        allowOutsideClick: false
    });
    $.ajax({
        type: 'POST',
        url: '/Home/Upload',
        data: data,
        async: false,
        dataType: 'json',
        contentType: false,
        processData: false,
        error: function (xhr, error, code) {
            console.log(xhr.status);
            console.log(error);
            swal("Error", xhr.responseText, "error");
        },
        statusCode: {
            500: function () {
                console.log('Internal server error 500');
                swal("Error", "Internal server error 500", "error");

            }
        },
        success: function (data) {
            swal("Success", "", "success");
            $('#FileName').val(data.filename)
            $('#datatable').DataTable().destroy();
            datatables.init();
        }
    });
}



$('#uploadExcel #ExcelFile').change(function () {
    $('#uploadExcel #ExcelFile-error').text('');
    checkFileDoc();

});

function checkFileDoc() {
    //File type check
    var fileInput = $('#uploadExcel input[name=ExcelFile]');

    var file = fileInput.get()[0].files;
    if (file) {
        var type = file[0].name.substring(file[0].name.lastIndexOf(".") + 1);

        if (type.toLowerCase() !== 'xls' && type.toLowerCase() !== 'xlsx') {
            fileInput.val('');
            $('#uploadExcel #ExcelFile-error').text('hanya untuk file tipe excel .xls atau .xlsx');
            return false;
        }
    }
    return true;
}

function addTable() {
    $('#tbl table').empty();
    $('#tbl').append('<table id="datatable" class="table table-hover table-striped table-borderless nowrap" style="width:100%"></table>');
}

$(document).ready(function () {
    addTable();
    datatables.init();
    //tblFiles.init();
});