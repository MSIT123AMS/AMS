//圖檔顯示
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imgPreview').attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function Datechange() {
    SweetAlert("無法提交隔日後之申請");
}
function timechange1() {
    SweetAlert("上班時間請在06:00-11:59之間設定");
}
function timechange() {
    SweetAlert("起始時間不可大於最後時間!");
}
function UpdateSuccess() {
    SweetAlert("審核完成");
}
function SweetAlert(alertstring) {
    swal(alertstring, "", {
        icon: "info",
        buttons: {
            confirm: {
                className: 'btn btn-info'
            }
        },
    });

}
function SuccessSweetAlert(alertstring) {
    swal(alertstring, "", {
        icon: "success",
        buttons: {
            confirm: {
                className: 'btn btn-info'
            }
        },
    });

}
function ConfirmSweetAlert(funcB) {
    swal({
        title: '確定要送出這筆資料嗎?',
        //text: "You won't be able to revert this!",
        type: 'warning',
        buttons: {
            confirm: {
                text: '確定',
                className: 'btn btn-success'
            },
            cancel: {
                text: '取消',
                visible: true,
                className: 'btn btn-danger'
            }
        }
    }).then((Delete) => {
        if (Delete) {
            var funcA = funcB;
            funcA();
        } else {
            swal.close();
        }
    });
}

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    }
};


function zeroPadded(val) {
    if (val >= 10)
        return val;
    else
        return '0' + val;
}
//time1:初始,time2:結束
function TimeErrorProof(time1, time2) {
    time1.bind("change", () => {
        if (time1.val() > time2.val()) {
            timechange();
            time1.val(time2.val());
        }
    });
    time2.bind("change", () => {
        if (time1.val() > time2.val()) {
            timechange();
            time2.val(time1.val());
        }
    });
}
//設定TimePicker起始值X/1~X/30
function SetTimePicker(time1, time2) {
    d = new Date();
    time1.val(d.getFullYear() + "-" + zeroPadded(d.getMonth() + 1) + "-01");
    time2.val(d.getFullYear() + "-" + zeroPadded(d.getMonth() + 1) + "-" + zeroPadded(d.getDate()));
    TimeErrorProof(time1, time2)
}

function PageAjax(PageUrl, sta) {
    $.ajax({
        type: "GET",
        url: PageUrl,
        dataType: "html",
    }).done(function (result) {
        $("#mainpanel").html(result);

        switch (sta) {
            case '員工管理':
                IndexEventFunction();
                break;
            case '未審核':
            case '已審核':
                {
                    changeT();
                    //$("#Customers").bind("change", Switch);
                    //$("#ddlemp").bind("change", Switch);
                    //$("input[name*='Le']").bind("click", function () {
                    //    Reviewfunbtndetails(event.target.name.substr(2));
                    //});

                    break;
                }
            case '部門管理':
                {

                    changeT();
                    break;
                }
            case '日期員工出勤統計':
                {
                    $('#time1').val("2019-12-17");
                    $("#dchat").hide();
                    //SetTimePicker($("#time1"), $("#time2"));
                    GetDailyStatistics();
                    break;
                }
            case '月出勤統計':
                {
                    changeT();
                    //SetTimePicker($("#time1"), $("#time2"));

                    break;
                }
            default:
                changeT();
                SetTimePicker($("#time1"), $("#time2"));
                break;
        }
    }).fail(function (e) { alert(e.responseText) });
}
////換Table樣式

function changeT() {
    $("#basic-datatables").addClass("compact");
    //if (window.innerWidth >= 600) {

    $('#basic-datatables').DataTable({
        destroy: true,
        responsive: true
    });
    $("#basic-datatables_filter").hide();
    //}
    //else {

    //    $('#basic-datatables').basictable("destroy");
    //    $('#basic-datatables').basictable({ breakpoint: 600 });
    //}
}

//window.addEventListener('resize', changeT);
/////////////////////正///////////////////////////////////
function DepartmentCreatDemo() {
    $('#DepartmentName').val('國際部');
    $('#Manager').val('蔡志銘');

}
function EmpCreatDemo() {
    $('#EmployeeID').val('MSIT1230040');
    $('#Password').val('MSIT123a@@');
    $('#EmployeeName').val('陳毅正');
    $('#EnglishName').val('YiCheng');
    $('#Birthday').val("1990-12-01");
    $('#IDNumber').val('A129574428');
    $('#gender')[0].selectedIndex = 1;
    $('#Marital').val('未婚');
    $('#Email').val('SSSSS@gmail.com');
    $('#Address').val('台北市大安區復興南路一段390號');
    $('#Phone').val('0968775997');
    $('#Education').val('勤益科技大學');
    $('#JobTitle').val('員工');
    $('#DepartmentName').val('人資部');
    $('#Manager').val('楊毅賢');
    $('#Hireday').val("2017-11-01");
    $('#Notes').val('無');


}



function EmployeeDetailsToEdit() {
    $(":input").attr("readonly", false)//去除input元素的readonly屬性
}

function EmployeePrint() {
    $('#basic-datatables').DataTable({
        destroy: true,
        paging: false,
        ordering: false,
        info: false,

    });
    $("#basic-datatables_filter").hide();
    $('#basic-datatables').removeClass("table");
    window.print();
    changeT();
    $('#basic-datatables').addClass("table");

}
function IndexEventFunction() {
    changeT();


    $("#Employees").bind("change", UpdateEmployeesByEmployee);
    $("#Department").bind("change", function () {
        UpdateEmployeesByDepartment();
        Updateddl();
    });
}


function EmployeesPageAjax(PageUrl, sta) {
    $.ajax({
        type: "GET",
        url: PageUrl,
        dataType: "html",
    }).done(function (result) {
        $("#mmmmmm").html(result);

        switch (sta) {
            case 'creat':
                //$("#empFile").bind("change", function () {
                //    readURL(this);
                //});
                break;
            case 'edit':
                {
                    //$("#empFile").bind("change", function () {
                    //    readURL(this);
                    //});
                    break;
                }
            case 'details':
                {
                    break;
                }
            default:
                break;
        }
    }).fail(function (e) { alert(e.responseText) });
}
///post
function SaveFormDataUseAjaxPost(AjaxUrl) {
    swal({
        title: '確定要送出這筆資料嗎?',
        //text: "You won't be able to revert this!",
        type: 'warning',
        buttons: {
            confirm: {
                text: '確定',
                className: 'btn btn-success'
            },
            cancel: {
                text: '取消',
                visible: true,
                className: 'btn btn-danger'
            }
        }
    }).then((Delete) => {
        if (Delete) {
            let myForm = document.querySelector("#FormData");
            var validator = $("#FormData").validate({
                debug: true
            }
            );
            console.log(validator);
            let data = new FormData(myForm);
            //data.append("DepartmentID","2");
            let files = $("#empFile").get(0).files;
            if (files.length > 0) {
                data.append("Photo", files[0]);
            }
            $.ajax({
                url: AjaxUrl,
                type: "POST",
                contentType: false,         // 告诉jQuery不要去這置Content-Type
                processData: false,         // 告诉jQuery不要去處理發送的數據
                dataType: "html",
                data: data,
                //data: {"EmployeeID": EmployeeID.value,},
            }).done(function (result) {
                $("#mainpanel").html(result);
                IndexEventFunction();
                SuccessSweetAlert('成功');

            }).fail(function (e) { SweetAlert("驗證失敗") });
        } else {
            swal.close();
        }
    });

}


///post
function SaveFormDataUseAjaxPost2(AjaxUrl) {
    swal({
        title: '確定要送出這筆資料嗎?',
        //text: "You won't be able to revert this!",
        type: 'warning',
        buttons: {
            confirm: {
                text: '確定',
                className: 'btn btn-success'
            },
            cancel: {
                text: '取消',
                visible: true,
                className: 'btn btn-danger'
            }
        }
    }).then((Delete) => {
        if (Delete) {
            let myForm = document.querySelector("#FormData");
            var validator = $("#FormData").validate({
                debug: true
            }
            );
            console.log(validator);
            let data = new FormData(myForm);
            //data.append("DepartmentID","2");
            let files = $("#LeaveFile").get(0).files;
            if (files.length > 0) {
                data.append("Attachment", files[0]);
            }
            $.ajax({
                url: AjaxUrl,
                type: "POST",
                contentType: false,         // 告?jQuery不要去這置Content-Type
                processData: false,         // 告?jQuery不要去處理發送的數據
                dataType: "json",
                data: data,
                //data: {"EmployeeID": EmployeeID.value,},
            }).done(function (result) {

                //$("#mainpanel").html(result);
                SuccessSweetAlert('送出申請，待主管審核');
                funLeaveIndex()
                $('#addModal').modal('hide');
            }).fail(function (e) { SweetAlert("此日期已申請過, 請先確認日期") }
            );
        } else {
            swal.close();
        }
    });

}


///post
function SaveFormDataUseAjaxPost3(AjaxUrl) {
    swal({
        title: '確定要送出這筆資料嗎?',
        //text: "You won't be able to revert this!",
        type: 'warning',
        buttons: {
            confirm: {
                text: '確定',
                className: 'btn btn-success'
            },
            cancel: {
                text: '取消',
                visible: true,
                className: 'btn btn-danger'
            }
        }
    }).then((Delete) => {
        if (Delete) {
            let myForm = document.querySelector("#FormData");
            var validator = $("#FormData").validate({
                debug: true
            }
            );
            console.log(validator);
            let data = new FormData(myForm);
            //data.append("DepartmentID","2");
            let files = $("#LeaveFile").get(0).files;
            if (files.length > 0) {
                data.append("Attachment", files[0]);
            }
            $.ajax({
                url: AjaxUrl,
                type: "POST",
                contentType: false,         // 告?jQuery不要去這置Content-Type
                processData: false,         // 告?jQuery不要去處理發送的數據
                dataType: "json",
                data: data,
                //data: {"EmployeeID": EmployeeID.value,},
            }).done(function (result) {


                let myForm = document.querySelector("#FormData");
                var validator = $("#FormData").validate({
                    debug: true
                }
                );
                console.log(validator);
                let data = new FormData(myForm);
                //data.append("DepartmentID","2");
                let files = $("#empFile").get(0).files;
                if (files.length > 0) {
                    data.append("Photo", files[0]);
                }
                $.ajax({
                    url: AjaxUrl,
                    type: "POST",
                    contentType: false,         // 告诉jQuery不要去這置Content-Type
                    processData: false,         // 告诉jQuery不要去處理發送的數據
                    dataType: "html",
                    data: data,
                    //data: {"EmployeeID": EmployeeID.value,},
                }).done(function (result) {
                    $("#mainpanel").html(result);
                    IndexEventFunction();
                    SuccessSweetAlert('成功');

                }).fail(function (e) { SweetAlert("驗證失敗") });
            }).fail(function (e) { SweetAlert("此日期已申請過, 請先確認日期") }
            );
        } else {
            swal.close();
        }
    });



}


    /////////////////////正///////////////////////////////////