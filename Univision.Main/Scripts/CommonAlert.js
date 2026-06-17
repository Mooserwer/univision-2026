function ErrorAlert(msg, func) {
    alertify.alert()
        .setHeader('<i class="fas fa-exclamation-triangle" style="color:#E62020"></i>오류')
        .setting({
            label: '확인',
            message: msg
        })
        .set({
            'onok': function () {
                if (func) func();
            }
        }).show();
}

function NormalAlert(msg, func) {
    alertify.alert()
        .setHeader('<i class="fas fa-bell blue-600"></i>알림')
        .setting({
            label: '확인',
            message: msg
        })
        .set({
            'onok': function () {
                if (func) func();
            }
        }).show();
}

function ConfirmAlert(title, msg, ok_func, cancel_func) {

    alertify.confirm()
        .set({
            'labels': { ok: '확인', cancel: '취소' },
            'onok': function () {
                if (ok_func) ok_func();
                confirmObj = true;
            },
            'oncancel': function () {
                if (cancel_func) cancel_func();
                confirmObj = false;
                return;
            }
        })
        .setHeader('<i class="fas fa-exclamation-triangle" style="color:#FFDC2E"></i>' + title)
        .setting({
            message: msg
        })
        .show();
}

function ConfirmAlert2(title, msg) {

    alertify.confirm()
        .set({
            'labels': { ok: '확인', cancel: '취소' },
            'onok': function () {
                return true;
            },
            'oncancel': function () {
                return false;
            },
        })
        .setHeader('<i class="fas fa-exclamation-triangle" style="color:#FFDC2E"></i>' + title)
        .setting({
            message: msg,
        })
        .show();
}