
//날짜형식 포맷
//yyyy-mm-dd
//onkeyup="return DateFormat(event, this)
//onkeypress="return DateFormat(event, this)
function DateFormat(e, obj) {

    var num_arr = [
        97, 98, 99, 100, 101, 102, 103, 104, 105, 96,
        48, 49, 50, 51, 52, 53, 54, 55, 56, 57
    ];

    var key_code = (e.which) ? e.which : e.keyCode;
    var len = obj.value.length;

    if (len > 9)
        return false;


    if (num_arr.indexOf(Number(key_code)) !== -1) {

        if (len === 4)
            obj.value += "-";

        if (len === 7)
            obj.value += "-";
    }
}

//년월형식 포맷
//yyyy-mm
//onkeyup="return DateFormat(event, this)
//onkeypress="return DateFormat(event, this)
function MonthFormat(e, obj) {
    var num_arr = [
        97, 98, 99, 100, 101, 102, 103, 104, 105, 96,
        48, 49, 50, 51, 52, 53, 54, 55, 56, 57
    ];

    var key_code = (e.which) ? e.which : e.keyCode;
    var len = obj.value.length;

    if (len > 7)
        return false;
    
    if (num_arr.indexOf(Number(key_code)) !== -1) {

        if (len === 4)
            obj.value += "-";
    }
}

//휴대폰 자동 하이픈
//010-0000-0000
//onkeyup='MobileAutoHypen(event, this)'
//onkeydown='MobileAutoHypen(event, this)'
//onkeypress='MobileAutoHypen(event, this)'
function MobileAutoHypen(event, obj) {
    var key = event.charCode || event.keyCode || 0;
    $text = $(obj);
    if (key !== 8 && key !== 9) {
        if ($text.val().length === 3) {
            $text.val($text.val() + '-');
        }
        if ($text.val().length === 8) {
            $text.val($text.val() + '-');
        }
    }

    // Key 8번 백스페이스, Key 9번 탭, Key 46번 Delete 부터 0 ~ 9까지, Key 96 ~ 105까지 넘버패트
    // 한마디로 JQuery 0 ~~~ 9 숫자 백스페이스, 탭, Delete 키 넘버패드외에는 입력못함
    return (key === 8 || key === 9 || key === 46 || (key >= 48 && key <= 57) || (key >= 96 && key <= 105));
}

function PhoneAutoHypen(event, obj) {
    
    var key = event.charCode || event.keyCode || 0;
    if (key >= 65) {
        var formatNum = obj.value
        obj.value = RemoveDash2(obj.value);
        var regex = /^(01[016789]{1}|02|0[3-9]{1}[0-9]{1})-?([0-9]{3,4})-?([0-9]{4})$/

        //if (regex.test(obj.value)) {
        if (regex.test(obj.value)) {
            var arr = regex.exec(obj.value);
            if (arr && arr.length > 1) {
                formatNum = arr[1] + "-" + (arr[2] ? arr[2] + "-" : "") + (arr[3] ? arr[3] : "");
            }
        }
        obj.value = formatNum;
        
        if (document.getElementById(obj.name).form && document.getElementById(obj.name).form.id) {
            $("#" + document.getElementById(obj.name).form.id).formValidation('revalidateField', obj.name);
        }
    }
    // Key 8번 백스페이스, Key 9번 탭, Key 46번 Delete 부터 0 ~ 9까지, Key 96 ~ 105까지 넘버패트
    // 한마디로 JQuery 0 ~~~ 9 숫자 백스페이스, 탭, Delete 키 넘버패드외에는 입력못함
    return (key === 8 || key === 9 || key === 46 || (key >= 48 && key <= 57) || (key >= 96 && key <= 105));
}

//전화번호 자동 하이픈
//onKeyup="OnCheckPhone(this);
function OnCheckPhone(oTa) {
    var oForm = oTa.form;
    var sMsg = oTa.value;
    var onlynum = "";
    var imsi = 0;
    onlynum = RemoveDash2(sMsg);  //하이픈 입력시 자동으로 삭제함
    onlynum = checkDigit(onlynum);  // 숫자만 입력받게 함
    var retValue = "";

    if (event.keyCode !== 12) {
        if (onlynum.substring(0, 2) === '02') {  // 서울전화번호일 경우  10자리까지만 나타나교 그 이상의 자리수는 자동삭제
            if (GetMsgLen(onlynum) <= 1) oTa.value = onlynum;
            if (GetMsgLen(onlynum) === 2) oTa.value = onlynum + "-";
            if (GetMsgLen(onlynum) === 4) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 3);
            if (GetMsgLen(onlynum) === 4) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 4);
            if (GetMsgLen(onlynum) === 5) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 5);
            if (GetMsgLen(onlynum) === 6) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 6);
            if (GetMsgLen(onlynum) === 7) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 5) + "-" + onlynum.substring(5, 7);;
            if (GetMsgLen(onlynum) === 8) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 6) + "-" + onlynum.substring(6, 8);
            if (GetMsgLen(onlynum) === 9) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 5) + "-" + onlynum.substring(5, 9);
            if (GetMsgLen(onlynum) === 10) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 6) + "-" + onlynum.substring(6, 10);
            if (GetMsgLen(onlynum) === 11) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 6) + "-" + onlynum.substring(6, 10);
            if (GetMsgLen(onlynum) === 12) oTa.value = onlynum.substring(0, 2) + "-" + onlynum.substring(2, 6) + "-" + onlynum.substring(6, 10);
        }
        if (onlynum.substring(0, 2) === '05') {  // 05로 시작되는 번호 체크
            if (onlynum.substring(2, 3) === 0) {  // 050으로 시작되는지 따지기 위한 조건문
                if (GetMsgLen(onlynum) <= 3) oTa.value = onlynum;
                if (GetMsgLen(onlynum) === 4) oTa.value = onlynum + "-";
                if (GetMsgLen(onlynum) === 5) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 5);
                if (GetMsgLen(onlynum) === 6) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 6);
                if (GetMsgLen(onlynum) === 7) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 7);
                if (GetMsgLen(onlynum) === 8) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8);
                if (GetMsgLen(onlynum) === 9) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 7) + "-" + onlynum.substring(7, 9);;
                if (GetMsgLen(onlynum) === 10) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8) + "-" + onlynum.substring(8, 10);
                if (GetMsgLen(onlynum) === 11) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 7) + "-" + onlynum.substring(7, 11);
                if (GetMsgLen(onlynum) === 12) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8) + "-" + onlynum.substring(8, 12);
                if (GetMsgLen(onlynum) === 13) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8) + "-" + onlynum.substring(8, 12);
            } else {
                if (GetMsgLen(onlynum) <= 2) oTa.value = onlynum;
                if (GetMsgLen(onlynum) === 3) oTa.value = onlynum + "-";
                if (GetMsgLen(onlynum) === 4) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 4);
                if (GetMsgLen(onlynum) === 5) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 5);
                if (GetMsgLen(onlynum) === 6) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6);
                if (GetMsgLen(onlynum) === 7) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7);
                if (GetMsgLen(onlynum) === 8) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6) + "-" + onlynum.substring(6, 8);
                if (GetMsgLen(onlynum) === 9) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 9);
                if (GetMsgLen(onlynum) === 10) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6) + "-" + onlynum.substring(6, 10);
                if (GetMsgLen(onlynum) === 11) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 11);
                if (GetMsgLen(onlynum) === 12) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 11);
            }
        }

        if (onlynum.substring(0, 2) === '03' || onlynum.substring(0, 2) === '04' || onlynum.substring(0, 2) === '06' || onlynum.substring(0, 2) === '07' || onlynum.substring(0, 2) === '08') {  // 서울전화번호가 아닌 번호일 경우(070,080포함 // 050번호가 문제군요)
            if (GetMsgLen(onlynum) <= 2) oTa.value = onlynum;
            if (GetMsgLen(onlynum) === 3) oTa.value = onlynum + "-";
            if (GetMsgLen(onlynum) === 4) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 4);
            if (GetMsgLen(onlynum) === 5) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 5);
            if (GetMsgLen(onlynum) === 6) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6);
            if (GetMsgLen(onlynum) === 7) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7);
            if (GetMsgLen(onlynum) === 8) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6) + "-" + onlynum.substring(6, 8);
            if (GetMsgLen(onlynum) === 9) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 9);
            if (GetMsgLen(onlynum) === 10) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6) + "-" + onlynum.substring(6, 10);
            if (GetMsgLen(onlynum) === 11) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 11);
            if (GetMsgLen(onlynum) === 12) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 11);

        }
        if (onlynum.substring(0, 2) === '01') {  //휴대폰일 경우
            if (GetMsgLen(onlynum) <= 2) oTa.value = onlynum;
            if (GetMsgLen(onlynum) === 3) oTa.value = onlynum + "-";
            if (GetMsgLen(onlynum) === 4) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 4);
            if (GetMsgLen(onlynum) === 5) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 5);
            if (GetMsgLen(onlynum) === 6) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6);
            if (GetMsgLen(onlynum) === 7) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7);
            if (GetMsgLen(onlynum) === 8) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 8);
            if (GetMsgLen(onlynum) === 9) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 9);
            if (GetMsgLen(onlynum) === 10) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 6) + "-" + onlynum.substring(6, 10);
            if (GetMsgLen(onlynum) === 11) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 11);
            if (GetMsgLen(onlynum) === 12) oTa.value = onlynum.substring(0, 3) + "-" + onlynum.substring(3, 7) + "-" + onlynum.substring(7, 11);
        }

        if (onlynum.substring(0, 1) === '1') {  // 1588, 1688등의 번호일 경우
            if (GetMsgLen(onlynum) <= 3) oTa.value = onlynum;
            if (GetMsgLen(onlynum) === 4) oTa.value = onlynum + "-";
            if (GetMsgLen(onlynum) === 5) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 5);
            if (GetMsgLen(onlynum) === 6) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 6);
            if (GetMsgLen(onlynum) === 7) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 7);
            if (GetMsgLen(onlynum) === 8) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8);
            if (GetMsgLen(onlynum) === 9) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8);
            if (GetMsgLen(onlynum) === 10) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8);
            if (GetMsgLen(onlynum) === 11) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8);
            if (GetMsgLen(onlynum) === 12) oTa.value = onlynum.substring(0, 4) + "-" + onlynum.substring(4, 8);
        }
    }
}

function RemoveDash2(sNo) {
    var reNo = "";
    for (var i = 0; i < sNo.length; i++) {
        if (sNo.charAt(i) !== "-") {
            reNo += sNo.charAt(i);
        }
    }
    return reNo;
}

function GetMsgLen(sMsg) { // 0-127 1byte, 128~ 2byte
    var count = 0;
    for (var i = 0; i < sMsg.length; i++) {
        if (sMsg.charCodeAt(i) > 127) {
            count += 2;
        }
        else {
            count++;
        }
    }
    return count;
}

function checkDigit(num) {
    var Digit = "1234567890";
    var string = num;
    var len = string.length;
    var retVal = "";

    for (i = 0; i < len; i++) {
        if (Digit.indexOf(string.substring(i, i + 1)) >= 0) {
            retVal = retVal + string.substring(i, i + 1);
        }
    }
    return retVal;
}