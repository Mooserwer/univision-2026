function DownloadFile(element_seq, type) {

    if (element_seq === undefined || element_seq === 0) {
        ErrorAlert("파일을 찾을 수 없습니다.");
    }
    //파일 존재여부 체크 후 있을 때 다운로드 url 리다이렉트 한다.

    $.ajax({
        url: '/DownloadFile/ExistsFile',
        method: 'POST',
        data: { element_seq: element_seq, type: type },
        dataType: "json",
        success: function (data, textStatus, response) {

            if (data.result)
                location.href = data.url;
            else
                ErrorAlert("파일을 찾을 수 없습니다.");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ErrorAlert('파일 다운로드 중, 오류가 발생 했습니다.<br/> [' + xhr.status + "]" + decodeURIComponent(xhr.statusText));
        }
    });
}
