////圖檔顯示
//function readURL(input) {
//    if (input.files && input.files[0]) {
//        var reader = new FileReader();
//        reader.onload = function (e) {
//            $('#imgPreview').attr('src', e.target.result);
//        };
//        reader.readAsDataURL(input.files[0]);
//    }
//}

//function Datechange() {
//    SweetAlert("無法提交隔日後之申請");
//}
//function timechange1() {
//    SweetAlert("上班時間請在06:00-11:59之間設定");
//}
//function timechange() {
//    SweetAlert("起始時間不可大於最後時間!");
//}
//function UpdateSuccess() {
//    SweetAlert("審核完成");
//}
//function SweetAlert(alertstring) {


//    swal(alertstring, "", {
//        icon: "info",
//        buttons: {
//            confirm: {
//                className: 'btn btn-info'
//            }
//        },
//    });
//    //$.confirm({

//    //        title: '警告',
//    //    content: `<h2 style="font-family:微軟正黑體;">${alertstring}</h2>`,

//    //        icon: 'fa fa-question',
//    //        theme: 'bootstrap',
//    //        closeIcon: true,
//    //        animation: 'scale',
//    //        type: 'orange',
//    //        buttons: {
//    //            '確認': {
//    //                function() {
//    //                    $.alert('您好!');
//    //                },
//    //                btnClass: 'btn btn-primary'
//    //            },
//    //        '取消': function () {
//    //            $.alert('您不好!');
//    //        }
//    //    }
//    //    });

//}

//function zeroPadded(val) {
//    if (val >= 10)
//        return val;
//    else
//        return '0' + val;
//}
////time1:初始,time2:結束
//function TimeErrorProof(time1, time2) {
//    time1.bind("change", () => {
//        if (time1.val() > time2.val()) {
//            timechange();
//            time1.val(time2.val());
//        }
//    });
//    time2.bind("change", () => {
//        if (time1.val() > time2.val()) {
//            timechange();
//            time2.val(time1.val());
//        }
//    });
//}
////設定TimePicker起始值X/1~X/30
//function SetTimePicker(time1, time2) {
//    d = new Date();
//    time1.val(d.getFullYear() + "-" + zeroPadded(d.getMonth() + 1) + "-01");
//    time2.val(d.getFullYear() + "-" + zeroPadded(d.getMonth() + 1) + "-" + zeroPadded(d.getDate()));
//    TimeErrorProof(time1, time2)
//}

//function PageAjax(PageUrl, sta) {
//    $.ajax({
//        type: "GET",
//        url: PageUrl,
//        dataType: "html",
//    }).done(function (result) {
//        $("#mainpanel").html(result);

//        switch (sta) {
//            case '員工管理':
//                IndexEventFunction();
//                break;
//            case '審核':
//                {
//                    changeT();
//                    $("#Customers").bind("change", Switch);
//                    $("#ddlemp").bind("change", Switch);
//                    $("#CheckAll").click(cAll);
//                    $("input[name*='Le']").bind("click", function () {
//                        Reviewfunbtndetails(event.target.name.substr(2));
//                    });

//                    break;
//                }
//            default:
//                changeT();
//                SetTimePicker($("#time1"), $("#time2"));
//                break;
//        }
//    }).fail(function (e) { alert(e.responseText) });
//}
//////換Table樣式
//function changeT() {

//    if (window.innerWidth >= 600) {

//        $('#basic-datatables').DataTable({
//            destroy: true
//        });
//        $("#basic-datatables_filter").hide();
//    }
//    else {

//        $('#basic-datatables').basictable("destroy");
//        $('#basic-datatables').basictable({ breakpoint: 600 });
//    }
//}

//window.addEventListener('resize', changeT);
///////////////////////正///////////////////////////////////
//function IndexEventFunction() {
//    changeT();


//    $("#Employees").bind("change", UpdateEmployeesByEmployee);
//    $("#Department").bind("change", function () {
//        UpdateEmployeesByDepartment();
//        Updateddl();
//    });
//    $("#Button1").bind("click", createmp);

//}

//function EmployeesPageAjax(PageUrl, sta) {
//    $.ajax({
//        type: "GET",
//        url: PageUrl,
//        dataType: "html",
//    }).done(function (result) {
//        $("#mmmmmm").html(result);

//        switch (sta) {
//            case 'creat':
//                $("#empFile").bind("change", function () {
//                    readURL(this);
//                });
//                break;
//            case 'edit':
//                {
//                    $("#empFile").bind("change", function () {
//                        readURL(this);
//                    });
//                    break;
//                }
//            case 'details':
//                {
//                    break;
//                }
//            default:
//                break;
//        }
//    }).fail(function (e) { alert(e.responseText) });
//}

//function SaveFormDataUseAjaxPost(AjaxUrl) {
//    let myForm = document.querySelector("#FormData");
//    let data = new FormData(myForm);

//    let files = $("#empFile").get(0).files;
//    if (files.length > 0) {
//        data.append("Photo", files[0]);
//    }
//    $.ajax({
//        url: AjaxUrl,
//        type: "POST",
//        contentType: false,         // 告诉jQuery不要去這置Content-Type
//        processData: false,         // 告诉jQuery不要去處理發送的數據
//        dataType: "html",
//        data: data,
//        //data: {"EmployeeID": EmployeeID.value,},
//    }).done(function (result) {
//        $("#mmmmmm").html(result);
//        IndexEventFunction();
//        alert('XXXX');

//    }).fail(function (e) { alert(e.responseText) });
//}
//    /////////////////////正///////////////////////////////////