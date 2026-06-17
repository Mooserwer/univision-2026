/*
(function (document, window, $) {
    'use strict';

    var Site = window.Site;
    $(document).ready(function () {
        Site.run();
    });
})(document, window, jQuery);
*/
$(function () {

    alertify.defaults.transition = "zoom";
    alertify.defaults.theme.ok = "btn btn-primary";
    alertify.defaults.theme.cancel = "btn btn-danger";
    alertify.defaults.theme.input = "form-control";

    var _serverAuthSeq = 0;
    var _isExternal = $("#isExternal").val();
    var _isAuth = 0;

    
    //외부접속의 경우, sms인증 div 활성화
    if (_isExternal === "1") {
        $("#smsDiv").css("display", "block");
    }

    $("#btnSendSms").click(function () {

        if ($("#id").val().trim() === "") {
            ErrorAlert("아이디를 입력하세요");
            $("#id").focus();
            return false;
        }

        if ($("#pwd").val().trim() === "") {
            ErrorAlert("비밀번호를 입력하세요");
            $("#pwd").focus();
            return false;
        }

        $.ajax({
            url: '/Account/SendSmsAuthCode',
            data: $("#frmLogin").serialize(),
            dataType: 'json',
            type: 'POST',
            beforeSend: function () {
                $("#btnSendSms").attr("disabled", "disabled");
            }
        })
        .done(function (json) {
            $("#btnSendSms").removeAttr("disabled");
            
            if (json.ok) {
                NormalAlert(json.message);

                _serverAuthSeq = json.a_seq;

                $("#authCode").removeAttr("disabled");
                $("#btnSendSms").attr("disabled", "disabled");
                //$("#btnCheckCode").css("display", "block");
            } else {
                ErrorAlert(json.message);
            }
        })
        .fail(function (e) {
            $("#btnSendSms").removeAttr("disabled");
            ErrorAlert('SMS 발송 도중 오류가 발생 했습니다.');
        });
    });

    $("#btnCheckCode").click(function () {
        
        if (_serverAuthSeq === 0 || $("#authCode").val() === "") {
            ErrorAlert("올바른 본인확인 인증번호를 입력하세요.");
            return false;
        }

        $.ajax({
            url: '/Account/CheckAuthCode',
            data: {
                id: $("#id").val(),
                a_seq: _serverAuthSeq,
                code: $("#authCode").val()
            },
            dataType: 'json',
            type: 'POST',
            beforeSend: function () {
                $("#btnCheckCode").attr("disabled", "disabled");
                $("#authCode").attr("readonly", true);
            }
        })
        .done(function (json) {
            
            if (json.ok) {
                _isAuth = 1;
                $("#btnCheckCode").attr("disabled", "disabled");
                $("#authCode").attr("disabled", "disabled");
                $("#authCode").attr("readonly", true);

                NormalAlert(json.message);
            } else {
                $("#btnCheckCode").removeAttr("disabled");
                $("#authCode").attr("readonly", false);
                ErrorAlert(json.message);
            }
        })
        .fail(function (e) {
            $("#btnCheckCode").removeAttr("disabled");
            $("#authCode").attr("readonly", false);
            
            ErrorAlert('인증코드 확인 중, 서버 오류가 발생 했습니다.');
        });

    });

    $("#frmLogin").submit(function (e) {

        if ($("#id").val().trim() === "") {
            ErrorAlert("아이디를 입력하세요.");
            $("#id").focus();
            return false;
        }

        if ($("#pwd").val().trim() === "") {
            ErrorAlert("비밀번호를 입력하세요.");
            $("#pwd").focus();
            return false;
        }
        
        //외부접속의 경우.
        if (_isExternal === "1") {
            //인증 받지 않았을 경우,
            if (_isAuth === 0 && _serverAuthSeq === 0) {
                ErrorAlert("외부 접속시, SMS 인증을 받으셔야 합니다.");
                return false;
            } 
        }
       
        e.preventDefault();
        //패스워드 암호화
        var pwd = "";
        //암호화
        //pwd = SHA256($("#pwd").val().trim());
        pwd = $("#pwd").val().trim();
        var action = $("#frmLogin").attr("action");
        $.ajax({
            type: "POST",
            url: action,
            dataType: "json",
            data: {
                id: $("#id").val().trim(),
                pwd: pwd,
                isExternal: _isExternal,
                a_seq: _serverAuthSeq,
                code: $("#authCode").val()
            },
            success: function (data) {
                if (data !== null && data.isLogin) {

                    if (window.returnUrl === "") {
                        location.href = window.login_url; //"@Url.Action("Index", "Dashboard")";
                    }
                    else {
                        location.href = window.returnUrl;
                    }


                } else {
                    if (data.message && data.message !== "") {
                        NormalAlert(data.message);
                    } else {
                        ErrorAlert("아이디 또는 패스워드가 틀립니다.");
                    }

                }

            },
            error: function () {
                ErrorAlert("Server Error");
            }
        });
    });
});