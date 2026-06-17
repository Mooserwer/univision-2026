
//모바일 버전 개인정보 항목 열람 로그 저장.
function setElementOpenLog(obj, uv_seq) {

    //특정 상황에서 필요.
    //ex) project contact, tax contact 신규등록 -> 취소 할 때 등...
    if ($(obj).find(".element-block").length === 0) {
        return;
    }

    var element_seq = $(obj).attr("element_seq").toString();
    var page_name = $(obj).attr("page_name").toString();
    var table_name = $(obj).attr("table_name").toString();
    var item_name = $(obj).attr("item_name").toString();
    var etc = "";

    if ($(obj).attr("etc") !== undefined) {
        etc = $(obj).attr("etc").toString();
    }

    var cookieName = Base64.encode(uv_seq + "_" + page_name + "_" + table_name + "_" + element_seq + "_" + item_name);

    var isOk;

    //cookie에 기존 데이터 있으면 불러온다. 없으면 undefined
    var data = getCookie(cookieName);

    //없으면 
    if (data === "") {
        //서버에 로그저장.
        isOk = saveElementOpenLogToServer(uv_seq, element_seq, page_name, table_name, item_name, etc);

        //서버에 저장이 제대로 되었다면.
        if (isOk) {
            //새로 생성.
            setCookie(cookieName, item_name);

            //현재 항목의 블록 제거.
            $(obj).find(".element-block").remove();
            $(obj).removeClass("element-wrapper");
        }
    } else {
        //현재 항목의 블록 제거.
        $(obj).find(".element-block").remove();
        $(obj).removeClass("element-wrapper");
    }
}

//현재 페이지의 모바일 화면 블록 해제.
function removeElementBlock(obj, uv_seq) {
    
    //obj 가 없는 경우, 메인 페이지로드
    if (obj === undefined || obj === null) {

        $(".element-wrapper").each(function () {
            
            var element_seq = $(this).attr("element_seq").toString();
            var page_name = $(this).attr("page_name").toString();
            var table_name = $(this).attr("table_name").toString();
            var item_name = $(this).attr("item_name").toString();

            var cookieName = Base64.encode(uv_seq + "_" + page_name + "_" + table_name + "_" + element_seq + "_" + item_name);

            //cookie에 기존 데이터 있으면 불러온다. 없으면 undefined
            var data = getCookie(cookieName);

            //cookie에 있다면 블록 해제.
            if (data !== "" || element_seq === "0") {
                //현재 항목의 블록 제거.
                $(this).find(".element-block").remove();
                $(this).removeClass("element-wrapper");
            }
        });
    }
    //obj가 있는 경우, 메인페이지의 하위 서브 페이지 로드.
    else {
        //다중 obj일 경우,
        if (obj.length > 1) {

            $(obj).each(function () {

                var element_seq = $(this).attr("element_seq").toString();
                var page_name = $(this).attr("page_name").toString();
                var table_name = $(this).attr("table_name").toString();
                var item_name = $(this).attr("item_name").toString();

                var cookieName = Base64.encode(uv_seq + "_" + page_name + "_" + table_name + "_" + element_seq + "_" + item_name);

                //cookie에 기존 데이터 있으면 불러온다. 없으면 undefined
                var data = getCookie(cookieName);

                //cookie에 있다면 블록 해제.
                if (data !== "" || element_seq === "0") {
                    //현재 항목의 블록 제거.
                    $(this).find(".element-block").remove();
                    $(this).removeClass("element-wrapper");
                }
            });
        }
        else {
            var element_seq = $(obj).attr("element_seq").toString();
            var page_name = $(obj).attr("page_name").toString();
            var table_name = $(obj).attr("table_name").toString();
            var item_name = $(obj).attr("item_name").toString();

            var cookieName = Base64.encode(uv_seq + "_" + page_name + "_" + table_name + "_" + element_seq + "_" + item_name);

            //cookie에 기존 데이터 있으면 불러온다. 없으면 undefined
            var data = getCookie(cookieName);

            //cookie에 있다면 블록 해제.
            if (data !== "" || element_seq === "0") {
                //현재 항목의 블록 제거.
                $(obj).find(".element-block").remove();
                $(obj).removeClass("element-wrapper");
            }
        }

    }


}

function setElementBlock(obj, uv_seq) {
    var width, height;

    //obj 가 없는 경우, 메인 페이지로드
    if (obj === undefined || obj === null) {
        $(".element-wrapper").each(function () {

            width = $(this).css("width");
            height = $(this).css("height");

            var div = "<div class='element-block' style='width:" + width + "; height:" + height + ";'></div > ";
            $(this).append(div.toString());

            $(this).bind("click", function (e) {
                e.preventDefault();
                setElementOpenLog($(this), uv_seq);
            });
        });

    }
    //obj가 있는 경우, 메인페이지의 하위 서브 페이지 로드.
    else {
        //다중 obj일 경우,
        if (obj.length > 1) {

            $(obj).each(function () {

                width = $(this).css("width");
                height = $(this).css("height");

                var div = "<div class='element-block' style='width:" + width + "; height:" + height + ";'></div > ";
                $(this).append(div.toString());

                $(this).bind("click", function (e) {
                    e.preventDefault();
                    setElementOpenLog($(this), uv_seq);
                });
            });
        }
        else {
            width = $(obj).css("width");
            height = $(obj).css("height");

            var div = "<div class='element-block' style='width:" + width + "; height:" + height + ";'></div > ";
            $(obj).append(div.toString());

            $(obj).bind("click", function (e) {
                e.preventDefault();
                setElementOpenLog($(obj), uv_seq);
            });
        }

    }


}

//쿠키 저장.
function setCookie(cname, cvalue) {
    var date = new Date();
    var exDay = 1;
    date.setTime(date.getTime() + (exDay * 24 * 60 * 60 * 1000)); //1일
    //date.setTime(date.getTime() + (exDay * 60 * 1000)); //1분 테스트용
    var expires = "expires=" + date.toUTCString();

    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

//쿠키 가져오기.
function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function removeCookies() {

    var res = document.cookie;
    var multiple = res.split(";");
    for (var i = 0; i < multiple.length; i++) {
        var key = multiple[i].split("=");
        var date = new Date();
        var exDay = -1;
        date.setTime(date.getTime() + (exDay * 24 * 60 * 60 * 1000));
        var expires = "expires=" + date.toUTCString();

        document.cookie = key[0] + "=delcookie;" + expires + ";path=/";
    }
}


function saveElementOpenLogToServer(uv_seq, element_seq, page_name, table_name, item_name, etc) {

    var isOk = false;

    $.ajax({
        url: '/Base/SaveElementItemOpenLog',
        data: {
            uv_seq: uv_seq,
            element_seq: element_seq,
            page_name: page_name,
            table_name: table_name,
            item_name: item_name,
            etc: etc
        },
        dataType: 'json',
        type: 'POST',
        async: false,
        beforeSend: function () {
        }
    })
    .done(function (json) {

        isOk = json.ok;

        if (!isOk) {
            ErrorAlert(json.message);
        }
    })
    .fail(function (e) {
        ErrorAlert('알 수 없는 오류가 발생 했습니다.\n관리자에게 문의 하세요.');
    });

    return isOk;
}

function MoveModifyCheck(uv_seq, element_seq, page_name, returnUrl) {
    
    var cookieName = Base64.encode(uv_seq + "_" + page_name + "_" + element_seq + "_" + "modify");

    //cookie에 기존 데이터 있으면 불러온다. 없으면 undefined
    var data = getCookie(cookieName);
    
    //cookie에 있다면 블록 해제.
    if (data !== "") {
        //return true;
        document.location.href = returnUrl;
    }
    else {
        alertify.confirm()
            .set({
                'labels': { ok: 'OK', cancel: 'Cancel' },
                'onok': function (e) {

                    if (saveMoveModifyLogToServer(uv_seq, element_seq, page_name, returnUrl)) {
                        setCookie(cookieName, "modifyMove");
                        document.location.href = returnUrl;
                    }
                    else {
                        return;
                    }

                },
                'oncancel': function () {
                    return;
                }
            })
            .setHeader(page_name + ' 수정 페이지 이동')
            .setting({
                message: "[Warning] " + page_name + " 수정 페이지로 이동하시겠습니까?<br />이동 기록이 서버에 기록되며 확인시<br />추가 확인절차 없이 7일간 수정이 가능합니다."
            })
            .show();
    }
    
}

function saveMoveModifyLogToServer(uv_seq, element_seq, page_name, returnUrl) {

    var isOk = false;
    
    $.ajax({
        url: '/Base/SaveMoveModifyLog',
        data: {
            uv_seq: uv_seq,
            element_seq: element_seq,
            page_name: page_name,
            returnUrl: encodeURIComponent(returnUrl)
        },
        dataType: 'json',
        type: 'POST',
        async: false,
        beforeSend: function () {
        }
    })
        .done(function (json) {

            isOk = json.ok;

            if (!isOk) {
                ErrorAlert(json.message);
            }
        })
        .fail(function (e) {
            ErrorAlert('알 수 없는 오류가 발생 했습니다.\n관리자에게 문의 하세요.');
        });

    return isOk;
}