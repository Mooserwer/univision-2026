function callWeeklyCalendar() {

    $.ajax({
        url: window.weekly_url, //'@Url.Action("WeeklyCalendarView", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#calendarBody');
        }
    })
        .done(function (view) {
            hideLoading('#calendarBody');
            $('#calendarBody').html(view);
        })
        .always(function () {
            hideLoading('#calendarBody');
        });
}


function callMeetingRoomStatusList() {

    $.ajax({
        url: window.meetingroom_url, //'@Url.Action("MeetingRoomStatusList", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#meetingRoomBody');
        }
    })
        .done(function (view) {
            hideLoading('#meetingRoomBody');
            $('#meetingRoomBody').html(view);

        })
        .always(function () {
            hideLoading('#meetingRoomBody');
        });
}



function callVacationApprList() {

    $.ajax({
        url: window.vacationappr_url, //'@Url.Action("VacationApprList", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#vacationApprBody');
        }
    })
        .done(function (view) {
            hideLoading('#vacationApprBody');
            if (view.trim() !== "") {
                $('#vacationApprBody').html(view);
                $('#vcationAppr').removeClass('d-none');
            } else {
                $('#vcationAppr').removeClass('d-none').addClass('d-none');
            }


        })
        .always(function () {
            hideLoading('#vacationApprBody');
        });
}
function callNoticeList() {

    $.ajax({
        url: window.notice_url, //'@Url.Action("NoticeList", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#noticeBody');
        }
    })
        .done(function (view) {
            hideLoading('#noticeBody');
            $('#noticeBody').html(view);

        })
        .always(function () {
            hideLoading('#noticeBody');
        });
}


function callClientList() {
    $.ajax({
        url: window.client_url, //'@Url.Action("ClientList", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#clientBody');
        }
    })
        .done(function (view) {
            hideLoading('#clientBody');
            $('#clientBody').html(view);

        })
        .always(function () {
            hideLoading('#clientBody');
        });
}


function callOpenPrjList() {
    $.ajax({
        url: window.openprj_url, //'@Url.Action("OpenProjectList", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#openprjBody');
        }
    })
        .done(function (view) {
            hideLoading('#openprjBody');
            $('#openprjBody').html(view);

        })
        .always(function () {
            hideLoading('#openprjBody');
        });
}

function callCoworkPrjList() {
    $.ajax({
        url: window.cowork_url, //'@Url.Action("CoworkProjectList", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#CoworkprjBody');
        }
    })
        .done(function (view) {
            hideLoading('#CoworkprjBody');
            $('#CoworkprjBody').html(view);

        })
        .always(function () {
            hideLoading('#CoworkprjBody');
        });
}

function callInvoiceList() {
    
    $.ajax({
        url: window.invoice_url, //'@Url.Action("BilledInvoiceList", "Dashboard")',
        data: {
            width: $(window).width()
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#invoiceBody');
        }
    })
        .done(function (view) {
            hideLoading('#invoiceBody');
            $('#invoiceBody').html(view);

        })
        .always(function () {
            hideLoading('#invoiceBody');
        });
}

function callUnBilledInvoceList() {

    $.ajax({
        url: window.unbilled_url, //'@Url.Action("UnBilledInvoiceList", "Dashboard")',
        data: {
            width: $(window).width()
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#unbilledBody');
        }
    })
        .done(function (view) {
            hideLoading('#unbilledBody');
            $('#unbilledBody').html(view);

        })
        .always(function () {
            hideLoading('#unbilledBody');
        });
}


function callKPIList() {

    $.ajax({
        url: window.kpi_url, //'@Url.Action("KPIList", "Dashboard")',
        data: {
        },
        dataType: 'html',
        type: 'POST',
        beforeSend: function () {
            showLoading('#kpiBody');
        }
    })
        .done(function (view) {
            hideLoading('#kpiBody');
            $('#kpiBody').html(view);

        })
        .always(function () {
            hideLoading('#kpiBody');
        });
}
callVacationApprList();
callWeeklyCalendar();
callMeetingRoomStatusList();
callNoticeList();
callClientList();
callOpenPrjList();
callCoworkPrjList();
callUnBilledInvoceList();
callInvoiceList();
callKPIList();

$(window).resize(function () {
    callUnBilledInvoceList();
    callInvoiceList();
});