
function funcCopy(text) {
  try {
    navigator.clipboard.writeText(text)
    toastr.info('"' + text + '" 내용이 클립보드에 저장되었습니다.');
  } catch (error) {
    toastr.error('복사 실패!');
  }
}

function FindBusinessSingle(callBusinessObj, code1, code2) {

    callBusiObj = $("#" + callBusinessObj);
    var data = {
      code1: callBusiObj.find(".busiCode1").val(),
      code2: callBusiObj.find(".busiCode2").val()
    };
    callFancyBox('/Partial/BusinessSingleApi', data, '산업 키워드', '640', '500');
}

function FindJobSingle(callJobObject, code1, code2) {

  callJobObj = $("#" + callJobObject);
  var data = {
    code1: callJobObj.find(".jobCode1").val(),
    code2: callJobObj.find(".jobCode2").val()
  };
  callFancyBox('/Partial/JobSingleApi', data, '직무 키워드', '640', '500');
}

function fSetCandCookie(cook_name, obj) {
    var val = obj.checked ? "1" : "0";
    setCookieOrg(cook_name, val, 30);
}

function fSetSelectCookie(cook_name, obj) {
  var val = obj.value;
  setCookieOrg(cook_name, val, 30);
}

function setCookieOrg(cName, cValue, cDay) {
    var expire = new Date();
    expire.setDate(expire.getDate() + cDay);
    cookies = cName + '=' + escape(cValue) + '; path=/ '; // 한글 깨짐을 막기위해 escape(cValue)를 합니다.
    if (typeof cDay != 'undefined') cookies += ';expires=' + expire.toGMTString() + ';';
    document.cookie = cookies;
}

function getCookieOrg(cookieName){
  var cookieValue = null;
  if (document.cookie) {
    var array = document.cookie.split((escape(cookieName) + '='));
    if (array.length >= 2) {
      var arraySub = array[1].split(';');
      cookieValue = unescape(arraySub[0]);
    }
  }
  return cookieValue;
}

function popover_help(tag, target_id, title, contents, zindex = 0, pment = "bottom", expire=180) {

  var popover_tag = 'po-tag-' + tag;

  if (getCookieOrg(popover_tag) != 1) {

    var popover_style = '';
    if (zindex) {
      popover_style = "style='z-index:" + zindex + ";'"
    }

  var vtemplate = `
<div class="popover shadow" role="tooltip" `+ popover_style+`>
  <div class="arrow border-primary"></div>
  <h3 class="popover-header bg-primary text-light"></h3>
  <div class="popover-body"></div>
  <div class="float-right pr-1 pb-1">
    <input type="checkbox" class="popover-check-close" data-tag="`+ popover_tag + `"> 오늘은 다시 보지 않기
  </div>
</div>`;


  $(target_id).popover({
    trigger: "manual",
    html: true,
    placement: pment,
    template: vtemplate,
    title: title + '<a href="#" class="close text-light" data-dismiss="alert" style="bottom: 5px;position: relative;color:#fff">&times;</a>',
    content: contents
  })

  
    $(target_id).popover("show");
    $(document).on("click", ".popover .close", function () {
      $(this).parents(".popover").popover('hide');
    });

    $(document).on("click", ".popover .popover-check-close", function () {
      var tag = $(this).attr("data-tag");
      setCookieOrg(tag, 1, expire);
      $(this).parents(".popover").popover('hide');

    });
  }


}


function openCandidate(val1) {
    var win_yn = true;
    if (document.getElementById("candidate_new_win")) 
        var win_yn = document.getElementById("candidate_new_win").checked;
    var url = "/Candidate/CandidateDetail?c_seq=" + val1;

    if (win_yn) {
        var win = window.open(url + "&is_pop=1", "WIN_CAN_" + val1, "scrollbars=yes,resizable=yes,width=850,height=750");
        win.focus();
    } else {
        document.location.href = url;
    }
}


function openCandidatePop(val1) {
  
  var url = "/Candidate/CandidateDetail?c_seq=" + val1;
  var win = window.open(url + "&is_pop=1", "WIN_CAN_" + val1, "scrollbars=yes,resizable=yes,width=850,height=750");
  win.focus();
  
}

var fCompareObj = function (a, b) {
    var i = 0, j;
    if (typeof a == "object" && a) {
        if (Array.isArray(a)) {
            if (!Array.isArray(b) || a.length != b.length) return false;
            for (j = a.length; i < j; i++) if (!fCompareObj(a[i], b[i])) return false;
            return true;
        } else {
            for (j in b) if (b.hasOwnProperty(j)) i++;
            for (j in a) if (a.hasOwnProperty(j)) {
                if (!fCompareObj(a[j], b[j])) return false;
                i--;
            }
            return !i;
        }
    }
    return a === b;
};

//공통 AJAX호출
function ajxCall(obj_option) {
    //ajax_default 옵션
    var ajax_option_default = {
        type: 'POST',
        url: '',
        data: '',
        dataType: 'json',
        //login: true,
        loading: false,
        loading_target: 'main',
        beforeSend: function () {
            if (this.loading) {
                $(this.loading_target).LoadingOverlay('show'); //로딩화면 표시
            }
        },
        success: function (data, textStatus, xhr) {
          this.suc(data, textStatus, xhr);
        },
        error: function (xhr, textStatus, errorThrown) {
            toastr.error(xhr, 'Error!');
            this.err(xhr);
        },
        complete: function (xhr, status) {
            if (this.loading) {
                $(this.loading_target).LoadingOverlay('hide'); //로딩화면 표시
            }
            this.comp(xhr, status);
        },
        suc: function () { }, // 사용자 success  -> suc
        err: function () { }, // 사용자 error    -> err
        comp: function () { }, // 사용자 complete -> comp
    }

    var obj_option = $.fn.extend({}, ajax_option_default, obj_option || {});
    $.ajax(obj_option);
}

function isNullOrEmpty(str) {
  return str === null || str == "undefined" || str.match(/^ *$/) !== null;
}
