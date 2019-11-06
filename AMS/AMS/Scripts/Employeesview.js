
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imgPreview').attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
$(document).ready(function () {
    IndexEventFunction();
});

function createmp() {
    $.ajax({
        type: "GET",
        url: "@Url.Action("Create","Employees")",
    }).done(function (result) {
        $("#mmmmmm").html(result);
        $("#empFile").bind("change", function () {
            readURL(this);
        });
        $("#btn1").bind("click", function () {
            savemp();
        });
        $("#btnback").bind("click", BackToIndexFunction);
    }).fail(function (e) { alert(e.responseText) });
}

function savemp() {
    var data = new FormData();
    data.append("EmployeeID", EmployeeID.value);
    data.append("EmployeeName", EmployeeName.value);
    data.append("Email", Email.value);
    data.append("IDNumber", IDNumber.value);
    data.append("DeputyPhone", DeputyPhone.value);
    data.append("Deputy", Deputy.value);
    data.append("Marital", Marital.value);
    data.append("Birthday", Birthday.value);
    data.append("Hireday", Hireday.value);
    data.append("Address", Address.value);
    data.append("Phone", Phone.value);
    data.append("DepartmentName", DepartmentName.value);
    data.append("JobTitle", JobTitle.value);
    data.append("EnglishName", EnglishName.value);
    data.append("gender", gender.value);
    data.append("Notes", Notes.value);
    data.append("Education", Education.value);

    var files = $("#empFile").get(0).files;
    if (files.length > 0) {
        data.append("Photo", files[0]);
    }
    $.ajax({
        url: "@Url.Action("Create","Employees")",
        type: "POST",
        contentType: false,         // 告诉jQuery不要去這置Content-Type
        processData: false,         // 告诉jQuery不要去處理發送的數據
        dataType: "html",
        data: data,
        //data: {"EmployeeID": EmployeeID.value,},
    }).done(function (result) {
        $("#mmmmmm").html(result);
        IndexEventFunction();
        alert('XXXX');

    }).fail(function (e) { alert(e.responseText) });
}



function Updateddl() {
    $.ajax({

        type: "GET",
        url: "@Url.Action("GetDdlandListemp","Employees")/" + Department.value,
    }).done(function (result) {
        $("#ddlemp").html(result);
        $("#Employees").bind("change", function () {
            UpdateEmployeesByEmployee();

        });
    }).fail(function (e) { alert(e.responseText) });
}


function UpdateEmployeesByEmployee() {
    $.ajax({

        type: "GET",
        url: "@Url.RouteUrl("Default1", new { controller = "Employees", action = "Listemp" })/" + Employees.value + "/" + Department.value,
    }).done(function (result) {
        $("#Employeestable").html(result);
        $('#basic-datatables').DataTable({
        });
        $("#basic-datatables_filter").hide();
        $("input[name*='MSIT']").bind("click", function () {
            if (event.target.name.substr(0, 2) == "De") {
                funbtndetails(event.target.name.substr(2));
            }
            else {
                funbtnedit(event.target.name.substr(2));
            }

        });
    }).fail(function (e) { alert(e.responseText) });
}


function UpdateEmployeesByDepartment() {
    $.ajax({
        type: "GET",
        url: "@Url.RouteUrl("Default1", new { controller = "Employees", action = "Listemp" })/" + "null/" + Department.value,
    }).done(function (result) {
        $("#Employeestable").html(result);
        $('#basic-datatables').DataTable({
        });
        $("#basic-datatables_filter").hide();
        $("input[name*='MSIT']").bind("click", function () {

            if (event.target.name.substr(0, 2) == "De") {
                funbtndetails(event.target.name.substr(2));
            }
            else {
                funbtnedit(event.target.name.substr(2));
            }


        });
    }).fail(function (e) { alert(e.responseText) });
}

function funbtnedit(s) {
    $.ajax({
        type: "GET",
        url: "@Url.Action("Edit", "Employees")/" + s,
        dataType: "html",
    }).done(function (result) {
        $("#mmmmmm").html(result);
        $("#btnback").bind("click", BackToIndexFunction);
    }).fail(function (e) { alert(e.responseText) });
}



function funbtndetails(s) {
    $.ajax({
        type: "GET",
        url: "@Url.Action("Details", "Employees")/" + s,
        dataType: "html",
    }).done(function (result) {
        $("#mmmmmm").html(result);
        $("#btnback").bind("click", BackToIndexFunction);
    }).fail(function (e) { alert(e.responseText) });
}

function BackToIndexFunction() {
    $.ajax({
        type: "GET",
        url: "@Url.Action("backIndex", "Employees")",
    }).done(function (result) {
        $("#mmmmmm").html(result);
        IndexEventFunction();
    }).fail(function (e) { alert(e.responseText) });
}



function IndexEventFunction() {
    $('#basic-datatables').DataTable({
    });
    $("#basic-datatables_filter").hide();
    $("#Employees").bind("change", UpdateEmployeesByEmployee);
    $("#Department").bind("change", function () {
        UpdateEmployeesByDepartment();
        Updateddl();
    });
    $("#Button1").bind("click", createmp);
    $("input[name*='MSIT']").bind("click", function () {
        funbtndetails(event.target.name.substr(2));
        if (event.target.name.substr(0, 2) == "De") {
            funbtndetails(event.target.name.substr(2));
        }
        else {
            funbtnedit(event.target.name.substr(2));
        }

    });

}