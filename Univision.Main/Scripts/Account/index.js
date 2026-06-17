$(document).ready(function () {
  if ($('#id').val() != "") {
    $('#pwd').focus();
  } else {
    $('#id').focus();
  }
});

var _serverAuthSeq = 0;
var _isExternal = $("#isExternal").val();
var _isAuth = 0;
var _isAsyncStep = false;

$("#frmLogin").submit(function (e) {
  //console.log("test");
  e.preventDefault();
  if (!$('#wizard-login').steps('next')) {
    $('#wizard-login').steps('finish');
  }
});

$('#wizard-login').steps({
  headerTag: 'span',
  bodyTag: 'section',
  transitionEffect: "slideLeft",
  autoFocus: true,
  titleTemplate: "<span style='display:none'>#title#</span>",
  labels: {
    finish: "Sign-in",
    next: "Next",
    previous: "Previous",

  },
  onStepChanging: function (event, currentIndex, newIndex) {
    //form.validate().settings.ignore = ":disabled,:hidden";
    //return form.valid();
    console.log(currentIndex, newIndex, _isAsyncStep);
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

    if (currentIndex == 0 && !_isAsyncStep) {
      ajxCall({
        url: '/Account/SendSmsAuthCode',
        data: $("#frmLogin").serialize(),
        dataType: 'json',
        type: 'POST',
        loading: true,
        loading_target: '#wizard-login',
        suc: function (data) {
          if (data.ok) {
            NormalAlert("회원님의 휴대폰으로 인증문자를 발송했습니다. 인증번호를 입력후 로그인해 주십시오.")
            _serverAuthSeq = data.a_seq;
            _isAsyncStep = true;
            $('#wizard-login').steps('next');
          } else {
            ErrorAlert(data.message);
          }
          $('body').LoadingOverlay('hide')
        },
        err: function (data) {
          ErrorAlert('SMS 발송 도중 오류가 발생 했습니다.');
        }
      })
      return false;
    } else {
      return true;
    }

    //return true;
  },
  onStepChanged: function (event, currentIndex, newIndex) {
    //console.log(currentIndex)
    if (currentIndex == 1) {
      _isAsyncStep = false;
      $('#authCode').focus();
    }
  },
  onFinishing: function (event, currentIndex) {
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
      if (_serverAuthSeq === 0 || $("#authCode").val() == "") {
        ErrorAlert("SMS 인증 코드를 입력 하여 주십시오.");
        $("#authCode").focus();
        return false;
      }
    }

    var pwd = "";
    //암호화
    //pwd = SHA256($("#pwd").val().trim());
    pwd = $("#pwd").val().trim();
    var action = $("#frmLogin").attr("action");
    ajxCall({
      type: "POST",
      url: action,
      dataType: "json",
      data: {
        id: $("#id").val().trim(),
        pwd: pwd,
        isExternal: _isExternal,
        a_seq: _serverAuthSeq,
        code: $("#authCode").val(),
        isRemember: ($('#remember').prop("checked") ? 1 : 0)
      },
      loading: true,
      loading_target: '#wizard-login',
      suc: function (data) {
        if (data !== null && data.isLogin) {
          $('body').LoadingOverlay('show')
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
      err: function (data) {
        ErrorAlert("Server Error : " + data);
      }
    });
  }

});