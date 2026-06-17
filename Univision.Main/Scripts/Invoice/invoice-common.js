function GetInvoiceLang() {
  return $("input:radio[name='invoice_lang']:checked").val();
}

function GetInvoiceType() {
  return $("input:radio[name='invoice_type']:checked").val();
}

function GetPreInvoiceType() {
  return $("input:radio[name='pre_pii_seq']:checked").data("invoice_type");
}

function GetPicSeq() {
  return $("input:radio[name='pic_seq']:checked").val();
}

function GetPicSeqObj() {
  return $("input:radio[name='pic_seq']:checked");
}

function GetPiiSeqObj() {
  return $("input:radio[name='pre_pii_seq']:checked");
}

function GetVATType() {
  return $("input:radio[name='vat_type']:checked").val();
}

function GetProjectType() {
  return $("#pjt_type").val();
}

$('#is_custom_title').change(function () {
  if ($(this).prop("checked")) {
    $("#invoice_title").prop("readonly", "")
  } else {
    $("#invoice_title").prop("readonly", "readonly")
  }
})

$('#is_custom_contents').change(function () {
  if ($(this).prop("checked")) {
    $("#invoice_contents").prop("readonly", "")
  } else {
    $("#invoice_contents").prop("readonly", "readonly")
  }
})

function CallContractDetail(c_seq) {
  ajxCall({
    url: window.url_contract_detail,
    data: {
      c_seq: c_seq
    },
    dataType: 'html',
    type: 'POST',
    suc: function (html) {
      $('#contract_info').show();
      $('#contract_info_div').html(html);
      $('#btn_renew_contract').hide();
      $('#btnMakeContractFile').hide();
    },

  })
}

function PinToTopToggle() {
  //pin_to_top_contract
  if ($("#contract_info").hasClass("top-head-contract")) {
    $("#pin_to_top_contract").removeClass("tx-primary").addClass("tx-indigo");
    $("#contract_info").removeClass("top-head-contract");
  } else {
    $("#pin_to_top_contract").removeClass("tx-indigo").addClass("tx-primary");
    $("#contract_info").addClass("top-head-contract");
  }
}



function popclose() {
  window.opener.document.location.href = window.opener.document.URL;
  window.close();
}

function setTextValue(org, target) {
  var text = $(org).val();
  //후보자명, 포지선 등 REPLACE 기능 넣을 예정
  $(target).val(text);
  //GenerateInvoicePrint()
}

function Modal_Slim(title, html, show_footer) {
  $("#modal-base").find(".modal-header-title").text(title);
  $("#modal-base").find(".modal-body").html(html);
  if (show_footer) {
    $("#modal-base").find(".modal-footer").show();
  } else {
    $("#modal-base").find(".modal-footer").hide()
  }
  $("#modal-base").modal("show");
}

function fSetAddDay(addDay) {
  var join_dt_str = $('#join_dt').val();
  if (join_dt_str == '') {
    join_dt_str = getDateStr(new Date());
  }
  //console.log(join_dt_str)
  if (addDay > 0) {
    $('#expire_guarantee').datepicker("update", dateAdd(join_dt_str, addDay, "d"));
  }
}

function getDateStr(date) {
  // GET YYYY, MM AND DD FROM THE DATE OBJECT
  var yyyy = date.getFullYear().toString();
  var mm = (date.getMonth() + 1).toString();
  var dd = date.getDate().toString();

  // CONVERT mm AND dd INTO chars
  var mmChars = mm.split('');
  var ddChars = dd.split('');

  // CONCAT THE STRINGS IN YYYY-MM-DD FORMAT
  return yyyy + '-' + (mmChars[1] ? mm : "0" + mmChars[0]) + '-' + (ddChars[1] ? dd : "0" + ddChars[0]);

}

function dateAdd(sDate, v, t) {
  var yy = parseInt(sDate.substr(0, 4), 10);
  var mm = parseInt(sDate.substr(5, 2), 10);
  var dd = parseInt(sDate.substr(8), 10);

  if (t == "d") {
    d = new Date(yy, mm - 1, dd + v);
  } else if (t == "m") {
    d = new Date(yy, mm - 1 + v, dd);
  } else if (t == "y") {
    d = new Date(yy + v, mm - 1, dd);
  } else {
    d = new Date(yy, mm - 1, dd + v);
  }

  yy = d.getFullYear();
  mm = d.getMonth() + 1; mm = (mm < 10) ? '0' + mm : mm;
  dd = d.getDate(); dd = (dd < 10) ? '0' + dd : dd;

  return '' + yy + '-' + mm + '-' + dd;
}

function datediffDay(t1, t2) {
  var one_day = 1000 * 60 * 60 * 24;

  var x = t1.split("-");
  var y = t2.split("-");

  var date1 = new Date(x[0], (x[1] - 1), x[2]);
  var date2 = new Date(y[0], (y[1] - 1), y[2]);

 

  _Diff = Math.ceil((date2.getTime() - date1.getTime()) / (one_day));

  return _Diff
}

function getInvoiceTitleDefault() {
  var inv_lang = GetInvoiceLang();
  var inv_type = GetInvoiceType();
  var vat_type = GetVATType();
  var pjt_type = GetProjectType();;

  var title = "";
  if (inv_lang == 0) {
    switch (inv_type) {
      case "0": //성공
        if (vat_type == 3) {
          title = "인재추천 서비스 수수료 청구";
        }
        else {
          title = "채용컨설팅 용역비 청구";
        }
        break;
      case "1": //선수금
      case "2": //잔금
        title = "채용컨설팅 용역비 청구";
        break;
      case "3": //컨설팅
        title = getPjtTypeTitle(pjt_type) + " 용역비 청구";
        break;
      case "4": //환불
        if (vat_type == 3) {
          title = "인재추천 서비스 수수료 반환";
        }
        else {
          title = "채용컨설팅 용역비 반환";
        }
        
        break;
      case "5": //취소
        if (vat_type == 3) {
          title = "인재추천 서비스 수수료 취소";
        }
        else {
          title = "채용컨설팅 용역비 취소";
        }

        break;
      
    }
  } else if (inv_lang == 1) {
    switch (inv_type) {
      case "0": //성공
      case "2": //잔금
        title = "Recruitment Service Fee";
        break;
      case "1": //선수금
        title = "Retainer Fee-Korea Recruitment";
        break;
      case "3": //컨설팅
        title = "Assessment Fee";
        break;
      case "4": //환불
        title = "Invoice for Refund -Korea Recruitment";
      case "5": //환불
        title = "Cancellation of Invoice-Korea Recruitment";
        break;
    }
  }

  return title;

}

function getInvoiceContentDefault() {
  var inv_lang = GetInvoiceLang();
  var inv_type = GetInvoiceType();
  var vat_type = GetVATType();
  var pjt_type = GetProjectType();

  var contents = "";

  if (inv_lang == 0) {
    contents = "귀사의 발전을 진심으로 기원합니다.\n";
    switch (inv_type) {
      case "0": //성공
        if (vat_type == 3) {
          contents += "인재추천{%후보자이름%}에 관한 서비스 용역수수료를 청구하오니 하기 예금계좌로 지급하여 주시면 감사하겠습니다. ";
        }
        else {
          contents += "인재선발{%후보자이름%}에 관한 용역비를 청구하오니 하기 예금계좌로 지급하여 주시면 감사하겠습니다.";
        }
        break;
      case "1": //선수금
        contents += "인재추천에 관한 용역계약서 관련 선임료를 청구하오니 세금계산서 발행일로부터 14일 이내에 하기 예금계좌로 지급하여 주시면 감사하겠습니다. ";
        break;
      case "2": //잔금
        contents += "인재선발{%후보자이름%}에 관한 용역비를 청구하오니 하기 예금계좌로 지급하여 주시면 감사하겠습니다.";
        break;
      case "3": //컨설팅
        contents += getPjtTypeTitle(pjt_type) + "에 관한 용역비를 청구하오니 하기 예금계좌로 지급하여 주시면 감사하겠습니다. ";
        break;
      case "4": //환불
        if (vat_type == 3) {
          contents += "채용된 후보자가 보증기간 이내 퇴사하여 용역수수료 환불을 아래와 같이 처리하오니 확인하여 주시기 바랍니다.";
        } else {
          contents += "채용된 후보자가 보증기간 이내 퇴사하여 용역비 환불을 아래와 같이 처리하오니 확인하여 주시기 바랍니다.";
        }
        break;
      case "5": //취소
        if (vat_type == 3) {
          contents += "인재추천에 관한 서비스 용역수수료를 아래와 같이 처리하오니, 확인하여 주시기 바랍니다.";
        } else {
          contents += "인재선발에 관한 용역비를 아래와 같이 처리하오니, 확인하여 주시기 바랍니다.";
        }
        break;
      
    }
  } else if (inv_lang == 1) {
    switch (inv_type) {
      case "0": //성공
      case "2": //잔금
        contents = "We appreciate your continued partnership with Unico Search, and we are pleased to submit herein our invoice for the recruitment service of ";
        contents += "{%후보자이름%}{%포지션%} with commencement date as of {%발행일%}";
        break;
      case "1": //선수금
        contents = "We appreciate your continued partnership with Unico Search, and we are pleased to submit herein our invoice for the recruitment service of ";
        contents += "{%포지션%} in Korea.";
        break;
      case "3": //컨설팅
        contents = "We appreciate your continued partnership with Unico Search and we are pleased to submit herein our invoice for our services rendered to you for the assessment services.";
        break;
      case "4": //환불
        contents = "We appreciate your continued partnership with Unico Search and we submit herein our refund invoice for our services to you for the recruitment of {%후보자이름%}{%포지션%} in Korea.";
        break;
      case "5": //취소
        contents = "We appreciate your continued partnership with Unico Search and we submit herein our Cancel invoice for our services to you for the recruitment of {%후보자이름%}{%포지션%} in Korea.";
        break;
    }
  }
  return contents;//getReplaceInvContents(contents);
}


function GetBankInfo(lang) {
  if (!lang) {
    lang = GetInvoiceLang();
  }
  var bank_name = $("#deposit_bank option:checked").text();
  var bank_account = $('#deposit_bank').val();
  if ($('#bill_currency_cd').val() != "KRW") {
    bank_name = "국민은행"
    bank_account = "389868-11-010539";
  }
  //#inv_bank_info_kor
  var info_text = "";
  if (lang == 0) {
    info_text = "- 은행 : " + bank_name + "<br/>";
    info_text += "- 계좌번호 : " + bank_account + "<br/>";
    info_text += "- 예금주 : (주)유니코써치";
  } else {
    info_text = "- Bank : " + BankNameToEng(bank_name) + "<br/>";
    info_text += "- Account Number : " + bank_account + "<br/>";
    info_text += "- Account Name : Unico Search Inc.";
  }
  return info_text;
}

function BankNameToEng(bank_name) {
  if (bank_name == "국민은행") {
    return "KOOKMIN BANK";
  } else if (bank_name == "신한은행") {
    return "SHINHAN BANK";
  } else if (bank_name == "우리은행") {
    return "WOORI BANK";
  } else if (bank_name == "하나은행") {
    return "KEB HANA BANK";
  } else if (bank_name == "농협") {
    return "NATIONAL AGRICULTURAL";
  } else if (bank_name == "국민은행(외화전용)") {
    return "KOOKMIN BANK";
  }
}

function GetIsKorInvoice() {
  if (GetInvoiceLang() == 0) {
    return true;
  } else {
    return false
  }
}

function print_datestr(s) {
  var s = s.split(/\D/),
    dt = new Date(s[0], s[1] - 1, s[2]);
  return dt.toLocaleString('en-CA', {
    month: 'long',
    day: 'numeric',
    year: 'numeric'
  });
}

function getReplaceInvContents(cont) {

  if ($("input:radio[name='is_open_name']:checked").val() == 1)
    cont = cont.replace(/{%후보자이름%}/gi, (GetIsKorInvoice() ? "(" + $("#candidate_name_kor").val() + ")" : $("#candidate_name_eng").val() + " as "))
  else
    cont = cont.replace(/{%후보자이름%}/gi, "");
  cont = cont.replace(/{%포지션%}/gi, $("#pjt_position_str").val())
  cont = cont.replace(/{%발행일%}/gi, (GetIsKorInvoice() ? $('#billing_dt').val() : print_datestr($('#billing_dt').val())))
  return cont;
}
function replaceInvContents() {
  var cont = $('#invoice_contents').val();

  return getReplaceInvContents(cont).replace(/\r\n|\r|\n/g, "<br />");
}

function GenerateInvoicePrint() {


  var html = "<div class='row'>";
  html += "  <div class='col'>";
  html += "    <h1 class='invoice-title'><img src='/Content/images/unicosearch_logo.png' style='width:150px;' /></h1>";
  html += "  </div>";
  html += "  <div class='col'>";
  html += "    <label class='section-label-sm tx-gray-500'>" + (GetIsKorInvoice() ? "발신" : "Billed From") + "</label>";
  html += "    <h6>" + (GetIsKorInvoice() ? "유니코써치" : "Unico Search Inc") + "</h6>";
  html += "    <p>" + (GetIsKorInvoice() ? "서울 강남구 테헤란로 87길 36, 도심공항타워 17F, 15F, 25F" : "17th Floor, 36, Teheran-ro 87-gil, Gangnam-gu, Seoul, Korea 06164") + "</p>";
  html += "    <p></p><p></p>";
  html += "  </div>";
  html += "</div>";


  html += "<div class='row mg-t-20'>";
  html += "  <div class='col'>";
  html += "    <label class='section-label-sm tx-gray-500'>" + (GetIsKorInvoice() ? "인보이스 정보" : "Invoice Information") + "</label>";
  html += "    <p class='invoice-info-row'>";
  html += "      <span>Invoice No:</span>";
  html += "      <span class='text-danger'>" + (GetIsKorInvoice() ? "[인보이스 발행 시 생성 됩니다.]" : "[인보이스 발행 시 생성 됩니다.]") + "</span>";
  html += "    </p>";
  html += "    <p class='invoice-info-row'>";
  html += "      <span>Project ID:</span>";
  html += "      <span>" + $('#p_seq').val() + "</span>";
  html += "    </p>";
  html += "    <p class='invoice-info-row'>";
  html += "      <span>" + (GetIsKorInvoice() ? "발행일자" : "Billing Date") + ":</span>";
  html += "      <span>" + (GetIsKorInvoice() ? $('#billing_dt').val() : print_datestr($('#billing_dt').val())) + "</span>";
  html += "    </p>";
  //html += "    <p class='invoice-info-row'>";
  //html += "      <span>Due Date:</span>";
  //html += "      <span id='inv_text_due_dt'>2025-02-11</span>";
  //html += "    </p>";
  html += "  </div><!-- col -->";
  html += "  <div class='col'>";
  html += "    <label class='section-label-sm tx-gray-500'>" + (GetIsKorInvoice() ? "수신" : "Billed To") + "</label>";
  html += "    <div class='billed-to'>";
  html += "      <h6>" + (GetIsKorInvoice() ? $('#client_name_kor').val() : $('#client_name_eng').val()) + "</h6>";
  html += "      <p>";
  html += "        <span>" + (GetIsKorInvoice() && $('#ceo_kor').val() != "" ? $('#ceo_kor').val() : "") + "</span><br/>";
  html += "        <span>" + (GetIsKorInvoice() && $('#addr_kor').val() != "" ? $('#addr_kor').val() : "") + "</span>";
  html += "      </p>";
  html += "    </div>";
  html += "  </div><!-- col -->";
  html += "</div><!-- row -->";
  html += "<div class='row bd-t mg-t-50 pd-t-30'>";
  html += "  <div class='col-md'>";
  html += "    <h5 class='font-weight-bold'>" + (GetIsKorInvoice() ? "제목" : "Title") + " : " + $('#invoice_title').val() + "</h5>";
  html += "    <p class='pd-t-20'>" + replaceInvContents() + "</p>";
  html += "  </div>";
  html += "</div>";
  html += "<div class='table-responsive mg-t-20'>";
  html += "  <table class='table table-invoice'>";
  html += "    <thead>";
  html += "      <tr>";
  html += "        <th class='wd-20p' id='inv_text_table_title1'>" + (GetIsKorInvoice() ? "구분" : "Type") + "</th>";
  html += "        <th class='wd-40p' id='inv_text_table_title2'>" + (GetIsKorInvoice() ? "내용" : "Description") + "</th>";
  html += "        <th class='tx-right' id='inv_text_table_title3'></th>";
  html += "        <th class='tx-right' id='inv_text_table_title4'>" + (GetIsKorInvoice() ? "금액" : "Amount") + "</th>";
  html += "      </tr>";
  html += "    </thead>";
  html += "    <tbody>";
  html += "      <tr>";
  html += "        <td>" + (GetIsKorInvoice() ? "수수료" : "Fee") + "</td>";
  html += "        <td class='tx-12' >" + (GetIsKorInvoice() ? "인재추천 서비스 수수료" : "Retainer Fee") + "</td>";
  html += "        <td class='tx-12'></td>";
  html += "        <td class='tx-right'>" + $('#billing_amt').val() + " " + $('#bill_currency_cd').val() + "</td>";
  html += "      </tr>";
  html += "      <tr>";
  html += "        <td colspan='2' rowspan='3' class='valign-middle'>";
  html += "          <div class='invoice-notes'>";
  html += "            <label class='section-label-sm tx-gray-500'>" + (GetIsKorInvoice() ? "입금은행" : "Bank information") + "</label>";
  html += "            <p class='pd-l-10'>" + GetBankInfo() + "</p>";
  html += "          </div>";
  html += "        </td>";
  html += "        <td class='tx-right'>" + (GetIsKorInvoice() ? "소계" : "Sub-Total") + "</td>";
  html += "        <td colspan='2' class='tx-right'>" + $('#billing_amt').val() + " " + $('#bill_currency_cd').val() + "</td>";
  html += "      </tr>";
  html += "      <tr>";
  html += "        <td class='tx-right'>" + (GetIsKorInvoice() ? "부가세" : "VAT") + "</td>";
  if ($("input:radio[name='vat_type']:checked").val() == 2 || $("input:radio[name='vat_type']:checked").val() == 3) {
    html += "        <td colspan='2' class='tx-right'>-</td>";
  } else {
    html += "        <td colspan='2' class='tx-right'>" + $('#billing_vat').val() + " " + $('#bill_currency_cd').val() + "</td>";
  }
  html += "      </tr>";
  html += "      <tr>";
  html += "        <td class='tx-right tx-uppercase tx-bold tx-inverse'>" + (GetIsKorInvoice() ? "합계" : "Total Due") + "</td>";
  html += "        <td colspan='2' class='tx-right'>";
  html += "          <span class='tx-primary tx-bold tx-indigo' id='inv_text_billing_money'>";
  html += $('#billing_total').val() + " " + $('#bill_currency_cd').val()
  html += "          </span>";
  html += "        </td>";
  html += "      </tr>";
  html += "    </tbody>";
  html += "  </table>";
  html += "</div><!-- table-responsive -->";
  html += "<hr class='mg-b-60'>";


  return html;
}

function getPjtTypeTitle(val) {
  var answer = ""
  switch (val) {
    case 3:
      answer = "재취업 컨설팅";
      break;
    case 4:
      answer = "채용대행";
      break;
    case 5:
      answer = "Market Mapping";
      break;
    case 5:
      answer = "사외이사 추천";
      break;
    default:
      answer = "평판조회";
      break;
  }
  return answer;
}

//$("#pin_to_top_contract").click(PinToTopToggle);

function InvoicePreview() {
  var lang = GetInvoiceLang();
  var client_name = (lang == 0 ? $("#client_name_kor").val() : $("#client_name_eng").val());
  var candidate_name = (lang == 0 ? $("#candidate_name_kor").val() : $("#candidate_name_eng").val());
  var bill_currency = $("#bill_currency_cd").val();
  var deposit_bank_name = (bill_currency == "KRW" ? $("#deposit_bank option:checked").text() : "국민은행(외화전용)");
  var deposit_bank_acc = (bill_currency == "KRW" ? $("#deposit_bank").val() : "389868-11-010539");

  var currentData = {
    p_seq: $("#p_seq").val(),
    invoice_lang: lang,
    invoice_type: $("input[name=invoice_type]:checked").val(),
    billing_dt: $("#billing_dt").val(),
    //-------
    candidate_name_kor: candidate_name,
    //-------
    client_seq: $('#client_seq').val(),
    client_name: client_name,
    client_ceo: $("#ceo_kor").val(),
    client_addr1: $("#addr_kor").val(),
    client_biz_code: $("#client_biz_code").val(),
    //-------
    vat_type: $("input[name=vat_type]:checked").val(),
    billing_type: $("#billing_type").is(":checked") ? 1 : 0,
    ann_income: uncomma2($("#ann_income").val()),
    billing_money: uncomma2($("#billing_money").val()),
    bill_currency_cd: $("#bill_currency_cd").val(),
    billing_total: uncomma2($("#billing_total").val()),
    billing_amt: uncomma2($("#billing_amt").val()),
    billing_vat: uncomma2($("#billing_vat").val()),
    retainer_amt: uncomma2($("#retainer_amt").val()),
    //-------
    invoice_title: $("#invoice_title").val(),
    invoice_contents: $("#invoice_contents").val(),
    deposit_bank_name: deposit_bank_name,
    deposit_bank_account: deposit_bank_acc,
    is_open_name: $("input[name=is_open_name]:checked").val(),
    is_open_annual_income: $("input[name=is_open_annual_income]:checked").val(),

  };





  $.ajax({
    url: window.url_temp_invoice_create,
    type: "POST",
    dataType: 'json',
    data: {
      data: currentData
    },
    beforeSend: function () {
      showLoading();
    },
    success: function (json) {
      hideLoading();
      if (json.ok) {
        console.log(json);
        downloadTempInv(json.file_url);
      }
      else {
        ErrorAlert(json.message);
      }
    },
    error: function (jqXHR, textStatus, errorThrown) {
      hideLoading();
      ErrorAlert("알 수 없는 오류가 발생 했습니다.\n관리자에게 문의 하세요.");
    }
  });
}

function downloadTempInv(url) {
  fetch(url)
    .then(response => response.blob())
    .then(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.style.display = 'none';
      a.href = url;
      a.download = 'INVOICE(Preview).xlsx'; // Specify the file name
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
    })
    .catch(error => console.error('Error downloading file:', error));
}

$.validator.addMethod("notOnlyZero", function (value, element, param) {
  return this.optional(element) || parseInt(value) > 0 || parseInt(value) < 0;
});