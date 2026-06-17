



function Modal_Print_Slim(title, html) {
  $("#modal-print").find(".modal-header-title").text(title);
  $("#modal-print").find(".modal-body").html(html);
  $("#modal-print").modal("show");
}

//=========================================================================
//#region 기본정보

var invoice_step1 = {
  rules: {
    //기본
    invoice_lang: {
      required: true
    },
    invoice_type: {
      required: true
    },
    tax_req_dt: {
      dateISO: true,
      required: true
    },
  },

  messages: {
    //기본
    invoice_lang: {
      required: "필수 선택 항목 입니다."
    },
    invoice_type: {
      required: "필수 선택 항목 입니다."
    },
    tax_req_dt: {
      dateISO: "올바른 날짜 형식(YYYY-MM-DD)이 아닙니다.",
      required: "필수 입력 항목 입니다. <br/>- 인보이스 발행일과 동시에 세금계산서 발행의 경우 [인보이스 발행일]을 눌러주세요. <br/>- 계산서 발행일이 상이할 경우 희망하시는 날짜를 입력해주세요."
    },
  }
}

var validator_step1 = $("#frmInvoiceStep1").validate($.extend({}, valid_option, invoice_step1));



function chkStep1() {
  if ($("#frmInvoiceStep1").valid()) {
    $(".part_show_inv_step2").show();
    $("#btn_chk_step1").hide();
    setPicSeq();
    $("#frmInvoiceStep2").valid()
    return true;
  } else {

    $("#btn_chk_step1").show();
    return false;
  }
}

function setInvoiceLang() {
  var lang = GetInvoiceLang();

  $(".part_show_lang_kor").hide();
  $(".part_show_lang_eng").hide();
  if (lang == 0) {
    $(".part_show_lang_kor").show();
  } else if (lang == 1) {
    $(".part_show_lang_eng").show();
  }

  $("#frmInvoiceStep2").valid()
  setInvoiceTitleDefault();
  setInvoiceContentDefault();
}



function setBillingDate(obj) {
  var billing_dt = $('#billing_dt').val();

  if (!billing_dt || billing_dt == "") {
    ErrorAlert("인보이스 빌링 요청일을 먼저 설정해주세요.");
    return;
  }

  $('#tax_req_dt').datepicker("update", billing_dt);
  validator_step1.element('#tax_req_dt')
}

$("#btn_chk_step1").click(chkStep1);


$("input:radio[name='invoice_lang']").on("change", setInvoiceLang);
$("input:radio[name='invoice_type']").on("change", function () {
  
  if (GetInvoiceType() == 2) {
    $('.col_retainer_input').show();
    $('#retainer_amt').val(comma(window.retainer_amt));
  } else {
    $('.col_retainer_input').hide();
    $('#retainer_amt').val(0);
  }
  TotalFeeChange();
  setInvoiceTitleDefault();
  setInvoiceContentDefault();
});



//#endregion
//=========================================================================
//#region 후보자 선택
function setPicSeq() {
  var $pic = GetPicSeqObj();
  var join_dt = $pic.data("join_dt");
  if ($pic.length > 0) {
    $('.part_show_pic_seq').show();
  }
  $('#candidate_name_kor').val($pic.data("kor_name"));
  $('#candidate_name_eng').val($pic.data("eng_name"));
  $('#join_dt').val(join_dt);

  $('#btn_set_join_dt').data("date", join_dt);
  if (Number(window.guarantee_day) > 0) {
    fSetAddDay(Number(window.guarantee_day));
  } else {
    $('#expire_guarantee').val("")
  }
  
  $('#ann_income').val(comma($pic.data("ann_income")));
  $('#bill_currency_cd').val($pic.data("currency_cd"));
  $('#bill_currency_cd').change();

  validator_step2.element('#candidate_name_eng');
}

var invoice_step2 = {
  rules: {
    //기본
    pic_seq: {
      required: true
    },
    candidate_name_kor: {
      required: function () { return (GetInvoiceLang() == 0 && GetPicSeq() > 0) }
    },
    candidate_name_eng: {
      required: function () { return (GetInvoiceLang() == 1 && GetPicSeq() > 0) }
    },
    candidate_source: {
      required: true
    },
    candidate_source_txt: {
      required: function () { return ($('#candidate_source').val() > 990); }
    },
    candidate_position: {
      required: true
    }

  },

  messages: {
    //기본
    pic_seq: {
      required: "인보이스를 발행할 후보자를 선택해주세요."
    },
    candidate_name_kor: {
      required: "필수 입력 항목 입니다."
    },
    candidate_name_eng: {
      required: "영문 인보이스 발행 시 후보자의 영문이름은 필수 입력 항목 입니다."
    },
    candidate_source: {
      required: "후보자 소스를 입력하여 주십시오."
    },
    candidate_source_txt: {
      required: "기타에 대한 상세 내용을 입력하여 주십시오."
    },
    candidate_position: {
      required: "후보자 최종 직급을 입력하여 주십시오."
    }
  }
}

var validator_step2 = $("#frmInvoiceStep2").validate($.extend({}, valid_option, invoice_step2));

function chkStep2() {

  if ($("#frmInvoiceStep2").valid()) {
    CallContractDetail($('#client_seq').val());
    $(".part_show_inv_step3").show();
    $("#btn_chk_step2").hide();
    $("#frmInvoiceStep3").valid()

    return true;
  } else {

    $("#btn_chk_step2").show();
    return false;
  }
}


$("input:radio[name='pic_seq']").on("change", setPicSeq);

$("#btn_chk_step2").click(chkStep2);


$('#candidate_source').change(function () {
  var val = $(this).val();
  $('#candidate_source_txt').hide();
  if (val > 990) {
    $('#candidate_source_txt').val("");
    $('#candidate_source_txt').show();
  } else {
    $('#candidate_source_txt').val($("#candidate_source option:selected").text());
  }
  validator_step2.element('#candidate_source');
  validator_step2.element('#candidate_source_txt');
})

$('#candidate_position').change(function () {
  $('#candidate_position_txt').val($("#candidate_position option:selected").text());
  validator_step2.element('#candidate_position');
})

//#endregion
//=========================================================================
//#region 고객사
function chkStep3() {

  if ($("#frmInvoiceStep3").valid()) {
    $(".part_show_inv_step4").show();
    $("#btn_chk_step3").hide();
    $("#frmInvoiceStep4").valid()
    return true;
  } else {

    $("#btn_chk_step3").show();
    return false;
  }
}

var invoice_step3 = {
  rules: {
    //기본
    client_name_kor: {
      required: function () { return (GetInvoiceLang() == 0) }
    },
    client_name_eng: {
      required: function () { return (GetInvoiceLang() == 1) }
    },
    ceo_kor: {
      required: function () { return (GetInvoiceLang() == 0) }
    },
    addr_kor: {
      required: function () { return (GetInvoiceLang() == 0) }
    },
    client_biz_code: {
      required: function () { return (GetInvoiceLang() == 0) },
      number: true,
      minlength: 10,
      maxlength: 10,
    },
  },

  messages: {
    //기본
    client_name_kor: {
      required: "국문 인보이스 발행 시 국문 회사명은 필수 입력항목입니다."
    },
    client_name_eng: {
      required: "영문 인보이스 발행 시 영문 회사명은 필수 입력항목입니다."
    },
    ceo_kor: {
      required: "국문 인보이스 발행 시 대표자명은 필수 입력항목입니다."
    },
    addr_kor: {
      required: "국문 인보이스 발행 시 회사 주소는 필수 입력항목입니다."
    },
    client_biz_code: {
      required: "국문 인보이스 발행 시 사업자등록번호는 필수 입력항목입니다. (10자리, 숫자만)",
      number: "사업자등록번호는 10자리 숫자로만 입력해주세요.",
      minlength: "사업자등록번호는 10자리 숫자로만 입력해주세요.",
      maxlength: "사업자등록번호는 10자리 숫자로만 입력해주세요.",
    },
  }
}


var validator_step3 = $("#frmInvoiceStep3").validate($.extend({}, valid_option, invoice_step3));



$("#btn_chk_step3").click(chkStep3);
//#endregion
//=========================================================================
//#region 고객사 담당
function GetIsNewContact() {
  var cc_seq = $("#cc_seq").val();
  if (!cc_seq || cc_seq == 0) {
    return true;
  } else {
    return false
  }
}

function GetIsNewTaxContact() {
  var ctc_seq = $("#ctc_seq").val();
  if (!ctc_seq || ctc_seq == 0) {
    return true;
  } else {
    return false
  }
}

function PopContract() {
  if ($("#c_seq").val() == 0) {
    NormalAlert("고객사 정보가 없습니다.")
    return false;
  }
  ajxCall({
    url: window.url_search_invoice_contact,
    data: {
      p_seq: $("#p_seq").val(),
      c_seq: $("#client_seq").val()
    },
    dataType: 'html',
    type: 'POST',
    loading: true,
    suc: function (html) {
      Modal_Slim("인보이스 담당자 (CONTACT) 조회", html);
    }
  });
}

function btnNewContract() {
  ContactInputShowHide("show");

  window.cc_seq = $("#cc_seq").val();
  window.client_contact_name = $("#client_contact_name").val();
  window.client_contact_email = $("#client_contact_email").val();
  window.client_contact_tel = $("#client_contact_tel").val();
  window.client_contact_tel2 = $("#client_contact_tel2").val();
  window.client_contact_div = $("#client_contact_div").val();
  window.client_contact_pos = $("#client_contact_pos").val();
  window.client_contact_gender_m = $("#client_contact_gender_m").prop("checked");
  window.client_contact_gender_f = $("#client_contact_gender_f").prop("checked");
  window.client_contact_gender_u = $("#client_contact_gender_u").prop("checked");

  $("#cc_seq").val(0);
  $("#client_contact_name").val("");
  $("#client_contact_gender_m").prop("checked", false);
  $("#client_contact_gender_f").prop("checked", false);
  $("#client_contact_gender_u").prop("checked", false);
  $("#client_contact_email").val("");
  $("#client_contact_tel").val("");
  $("#client_contact_tel2").val("");
  $("#client_contact_div").val("");
  $("#client_contact_pos").val("");
}

function ContactInputShowHide(mode) {
  if (mode == "show") {
    $("#client_contact_name").removeAttr("readonly");
    $("#client_contact_gender_m").removeAttr("disabled");
    $("#client_contact_gender_f").removeAttr("disabled");
    $("#client_contact_gender_u").removeAttr("disabled");
    $("#client_contact_email").removeAttr("readonly");
    $("#client_contact_tel").removeAttr("readonly");
    $("#client_contact_tel2").removeAttr("readonly");
    $("#client_contact_div").removeAttr("readonly");
    $("#client_contact_pos").removeAttr("readonly");

    $(".part_contract_input_mode").show();
    $(".part_contract_select_mode").hide();
  } else {
    $(".part_contract_input_mode").hide();
    $(".part_contract_select_mode").show();

    $("#client_contact_name").attr("readonly", "readonly");
    $("#client_contact_email").attr("readonly", "readonly");
    $("#client_contact_tel").attr("readonly", "readonly");
    $("#client_contact_tel2").attr("readonly", "readonly");
    $("#client_contact_div").attr("readonly", "readonly");
    $("#client_contact_pos").attr("readonly", "readonly");
    $("#client_contact_gender_m").attr("disabled", "disabled");
    $("#client_contact_gender_f").attr("disabled", "disabled");
    $("#client_contact_gender_u").attr("disabled", "disabled");
  }

  validator_step4.element("#client_contact_name");
  validator_step4.element("#client_contact_tel2");
  validator_step4.element("#client_contact_email");
  validator_step4.element("#client_contact_div");
  validator_step4.element("#client_contact_pos");
  validator_step4.element("#client_contact_gender_m");
}

//프로젝트 담당자 저장
function NewContractSubmit() {

  var name = validator_step4.element("#client_contact_name");
  var tel = validator_step4.element("#client_contact_tel2");
  var email = validator_step4.element("#client_contact_email");
  var div = validator_step4.element("#client_contact_div");
  var pos = validator_step4.element("#client_contact_pos");
  var gender = validator_step4.element("#client_contact_gender_m");

  if (!name || !tel || !email || !div || !pos || !gender) {
    ErrorAlert('필수 항목을 모두 입력 후 진행해 주세요.');
    return;
  }

  ajxCall({
    url: window.url_invoice_new_contact_submit,
    type: "POST",
    dataType: 'json',
    data: {
      c_seq: $("#client_seq").val()
      , p_seq: $("#p_seq").val()
      , name: $("#client_contact_name").val()
      , gender: $("input[name='client_contact_gender']:checked").val()
      , email: $("#client_contact_email").val()
      , phone: $("#client_contact_tel").val()
      , cell_phone: $("#client_contact_tel2").val()
      , division: $("#client_contact_div").val()
      , position: $("#client_contact_pos").val()
    },
    loading: true,
    suc: function (json) {
      if (json.ok) {
        $('#cc_seq').val(json.cc_seq);
        ContactInputShowHide("hide");
      }
      else {
        ErrorAlert(json.message);
      }
    }
  });

}

function NewContractReset() {
  $("#cc_seq").val(window.cc_seq);
  $("#client_contact_name").val(window.client_contact_name);
  $("#client_contact_email").val(window.client_contact_email);
  $("#client_contact_tel").val(window.client_contact_tel);
  $("#client_contact_tel2").val(window.client_contact_tel2);
  $("#client_contact_div").val(window.client_contact_div);
  $("#client_contact_pos").val(window.client_contact_pos);
  $("#client_contact_gender_m").prop("checked", window.client_contact_gender_m);
  $("#client_contact_gender_f").prop("checked", window.client_contact_gender_f);
  $("#client_contact_gender_u").prop("checked", window.client_contact_gender_u);

  ContactInputShowHide("hide");
}



function PopEtax() {

  if ($("#c_seq").val() == 0) {
    NormalAlert("고객사 정보가 없습니다.")
    return false;
  }

  ajxCall({
    url: window.url_search_invoice_tax_contact,
    data: {
      p_seq: $("#p_seq").val(),
      c_seq: $("#client_seq").val()
    },
    dataType: 'html',
    type: 'POST',
    loading: true,
    suc: function (html) {
      Modal_Slim("계산서 담당자 (TAX MANAGER) 조회", html);
    }
  });
}

function btnNewTaxContract() {
  TaxContactInputShowHide("show");

  window.ctc_seq = $("#ctc_seq").val();
  window.client_etax_name = $("#client_etax_name").val();
  window.client_etax_email = $("#client_etax_email").val();
  window.client_etax_tel = $("#client_etax_tel").val();
  window.client_etax_tel2 = $("#client_etax_tel2").val();

  $("#ctc_seq").val(0);
  $("#client_etax_name").val("");
  $("#client_etax_email").val("");
  $("#client_etax_tel").val("");
  $("#client_etax_tel2").val("");
}

$('#btn-new-etax-reset').click(function () {
  $("#ctc_seq").val(window.ctc_seq);
  $("#client_etax_name").val(window.client_etax_name);
  $("#client_etax_email").val(window.client_etax_email);
  $("#client_etax_tel").val(window.client_etax_tel);
  $("#client_etax_tel2").val(window.client_etax_tel2);

  TaxContactInputShowHide("hide");
});

function TaxContactInputShowHide(mode) {
  if (mode == "show") {
    $("#client_etax_name").removeAttr("readonly");
    $("#client_etax_email").removeAttr("readonly");
    $("#client_etax_tel").removeAttr("readonly");
    $("#client_etax_tel2").removeAttr("readonly");

    $(".part_etax_input_mode").show();
    $(".part_etax_select_mode").hide();
  } else {
    $(".part_etax_input_mode").hide();
    $(".part_etax_select_mode").show();

    $("#client_etax_name").attr("readonly", "readonly");
    $("#client_etax_email").attr("readonly", "readonly");
    $("#client_etax_tel").attr("readonly", "readonly");
    $("#client_etax_tel2").attr("readonly", "readonly");
  }

  validator_step4.element("#client_etax_name");
  validator_step4.element("#client_etax_tel2");
  validator_step4.element("#client_etax_email");
}

function btnNewTaxContract() {
  TaxContactInputShowHide("show");

  window.ctc_seq = $("#ctc_seq").val();
  window.client_etax_name = $("#client_etax_name").val();
  window.client_etax_email = $("#client_etax_email").val();
  window.client_etax_tel = $("#client_etax_tel").val();
  window.client_etax_tel2 = $("#client_etax_tel2").val();

  $("#ctc_seq").val(0);
  $("#client_etax_name").val("");
  $("#client_etax_email").val("");
  $("#client_etax_tel").val("");
  $("#client_etax_tel2").val("");
}


function NewEtaxReset() {
  $("#ctc_seq").val(window.ctc_seq);
  $("#client_etax_name").val(window.client_etax_name);
  $("#client_etax_email").val(window.client_etax_email);
  $("#client_etax_tel").val(window.client_etax_tel);
  $("#client_etax_tel2").val(window.client_etax_tel2);

  TaxContactInputShowHide("hide");
}

function NewEtaxSubmit() {

  var name = validator_step4.element("#client_etax_name");
  var tel = validator_step4.element("#client_etax_tel2");
  var email = validator_step4.element("#client_etax_email");

  if (!name || !tel || !email) {
    ErrorAlert('필수 항목을 모두 입력 후 진행해 주세요.');
    return;
  }

  ajxCall({
    url: window.url_invoice_new_etax_submit,
    type: "POST",
    dataType: 'json',
    data: {
      c_seq: $("#client_seq").val()
      , name: $("#client_etax_name").val()
      , email: $("#client_etax_email").val()
      , phone: $("#client_etax_tel").val()
      , cell_phone: $("#client_etax_tel2").val()
      , deposit_manager: $("#client_etax_name").val()
      , deposit_email: $("#client_etax_email").val()
    },
    loading: true,
    suc: function (json) {
      if (json.ok) {
        $('#ctc_seq').val(json.ctc_seq);
        TaxContactInputShowHide("hide");
      }
      else {
        ErrorAlert(json.message);
      }
    }
  });
}

function chkStep4() {
  if ($("#frmInvoiceStep4").valid()) {
    $(".part_show_inv_step5").show();
    $("#btn_chk_step4").hide();
    $("#frmInvoiceStep5").valid()
    return true;
  } else {
    $("#btn_chk_step4").show();

    return false;
  }
}

var invoice_step4 = {
  rules: {

    //담당자 정보
    client_contact_name: {
      required: true
    },
    client_contact_email: {
      required: true,
      email: true
    },
    //client_contact_tel2: {
    //  required: true
    //},
    client_contact_div: {
      required: GetIsNewContact
    },
    client_contact_pos: {
      required: GetIsNewContact
    },
    client_contact_gender: {
      required: GetIsNewContact
    },
    //계산서 담당자 정보
    client_etax_name: {
      required: true
    },
    client_etax_email: {
      required: true,
      email: true
    },
    //client_etax_tel2: {
    //  required: true
    //},
  },

  messages: {
    //기본
    client_name_kor: {
      required: "국문 인보이스 발행 시 국문 회사명은 필수 입력항목입니다."
    },
    client_name_eng: {
      required: "영문 인보이스 발행 시 영문 회사명은 필수 입력항목입니다."
    },
    ceo_kor: {
      required: "국문 인보이스 발행 시 대표자명은 필수 입력항목입니다."
    },
    addr_kor: {
      required: "국문 인보이스 발행 시 회사 주소는 필수 입력항목입니다."
    },
  }
}


var validator_step4 = $("#frmInvoiceStep4").validate($.extend({}, valid_option, invoice_step4));
//프로젝트 담당자 저장
$("#btn_pop_contact").click(PopContract);
$("#btn_new_contract").click(btnNewContract);
$("#btn_new_contact_submit").click(NewContractSubmit);
$('#btn_new_contact_reset').click(NewContractReset);


//프로젝트 계산서 담당자 저장
$("#btn_pop_etax").click(PopEtax);
$("#btn_new_etax").click(btnNewTaxContract);
$("#btn_new_etax_submit").click(NewEtaxSubmit);
$('#btn_new_etax_reset').click(NewEtaxReset);

$("#btn_chk_step4").click(chkStep4);

//#endregion
//=========================================================================
//#region 빌링
$(".money_input").bind('keyup', function (event) {
  removeChar2(event);
  inputNumberFormat2(this);
});

$(".money_input").bind('keydown', function () {
  inputNumberFormat2(this);
});

$(".share_fee_input, #ann_income, #fee_rate, #bill_currency_cd, #billing_money, input:radio[name='vat_type']").change(TotalFeeChange);

$("input:radio[name='vat_type']").on("change", function () {
  setInvoiceTitleDefault();
  setInvoiceContentDefault();
});

//정액 체크박스 선택시
$("#billing_type").change(BillingTypeChange);

function BillingTypeChange() {
  if ($("#billing_type").prop("checked")) {
    $("#fee_rate").closest(".part_show_billing_type_no").hide();
    $(".feeChkDiv").removeClass('offset-4').addClass('offset-4');
    $("#billing_money").prop("readonly", false)
    $("#bill_currency_cd").attr("readonly", false)
  } else {
    $("#fee_rate").change();
    $("#fee_rate").closest(".part_show_billing_type_no").show();
    $(".feeChkDiv").removeClass('offset-4');
    $("#billing_money").prop("readonly", true)
    $("#bill_currency_cd").attr("readonly", true)
  }

  TotalFeeChange();
}

function TotalFeeChange() {
  //console.log("total")

  if (!$("#billing_type").prop("checked")) {
    var income = parseInt(uncomma2($("#ann_income").val()));
    var fee = parseFloat($("#fee_rate").val());

    var billing_money = comma(Math.round(income * (fee * 0.01)))

    $("#billing_money").val(billing_money);
  }

  var billing_money = Number(uncomma2($("#billing_money").val()));
  var retainer_amt = Number(uncomma2($("#retainer_amt").val()));

  billing_money = billing_money - retainer_amt;

  var vat_type = GetVATType();
  if (vat_type == 1) {
    var billing_amt = billing_money;
    var billing_vat = Number((billing_money * 0.1).toFixed());
  } else if (vat_type == 0) {
    var billing_vat_cal = (billing_money / 1.1);
    var billing_amt = Math.round((billing_vat_cal / 10) * 10, 0);
    var billing_vat = billing_money - billing_amt
  } else {
    var billing_amt = billing_money;
    var billing_vat = 0;
  }

  $('#billing_total').removeAttr("readonly");
  $('#billing_amt').removeAttr("readonly");
  $('#billing_vat').removeAttr("readonly");
  $('#billing_total').val(comma(billing_amt + billing_vat));
  $('#billing_amt').val(comma(billing_amt));
  $('#billing_vat').val(comma(billing_vat));
  $('#billing_total').attr("readonly", "readonly");
  $('#billing_amt').attr("readonly", "readonly");
  $('#billing_vat').attr("readonly", "readonly");

  if ($("#share_money_type:checked").val() != "1") {
    var rate = 0;
    $("input[name='share_fee_rate']").each(function () {
      var fee = parseFloat($(this).val());
      var share_money = comma(Math.round(billing_amt * (fee * 0.01)))
      $(this).closest(".share_row").find("input[name='share_amt']").val(share_money);
    });
  } else {
    var amt = 0;
    $("input[name='share_amt']").each(function () {
      var camt = parseInt(uncomma($(this).val()));
      var share_fee = Math.round(camt / billing_amt * 100)
      $(this).closest(".share_row").find("input[name='share_fee_rate']").val(share_fee);
    });
  }

  CurrencyChk();
  changeShareFee();
}

function CurrencyChk() {

  var cur_cd = $("#bill_currency_cd").val();
  if (cur_cd != "KRW") {
    if (!(window.bank_acc) || window.bank_acc != "389868-11-010539") {
      window.bank_acc = $('#deposit_bank').val();
      $('#deposit_bank').val("389868-11-010539")
    }
  } else {
    if ($('#deposit_bank').val() == "389868-11-010539") {
      if (window.bank_acc)
        $('#deposit_bank').val(window.bank_acc)
    }
  }

  $(".print_fee_currency").text(cur_cd)
  if (cur_cd != "" && cur_cd != "KRW") {
    $(".print_fee_currency").addClass("tx-primary");
    ajxCall({
      url: window.url_exchange_partial,
      data: {
        currency: cur_cd
      },
      type: 'POST',
      loading: true,
      loading_target: ".bill_exch",
      bef: function () {
      },
      suc: function (rst) {
        if (rst && rst.ok) {
          if (rst.data && rst.data.ex_seq > 0) {
            var per_won = rst.data.per_won;
            var per_rate = rst.data.ex_rate;
            var org_money = uncomma2($('#billing_money').val())
            var result_won = ((org_money * per_won) / per_rate).toFixed();
            $('#billing_won').val(comma2(result_won.toString()))
          }
        }
      }
    })
  } else {
    $('#billing_won').val($('#billing_money').val())
    $(".print_fee_currency").removeClass("tx-primary");
  }
}

function changeShareFee() {
  var money = 0;
  var rate = 0;
  if ($("#share_money_type:checked").val() == "1") {
    billing_amt = uncomma($('#billing_amt').val());
    $("input[name='share_amt']").each(function () {
      money += parseFloat(uncomma($(this).val()));
    });
    $('#inv_total_share_amt').val(money);
    if (money < billing_amt || isNaN(money)) {
      $('#total_share_per').removeClass('text-danger').addClass('text-danger');
      $('#total_share_per').text("(" + comma(money) + ") - 매출 금액의 합은 '" + comma(billing_amt) + "'가 되어야합니다.")
    } else if (money > billing_amt) {
      $('#total_share_per').removeClass('text-danger').addClass('text-danger');
      $('#total_share_per').text("(" + comma(money) + ") - 배분 금액은 '" + comma(billing_amt) + "'를 초과할 수 없습니다.")
    } else {
      $('#total_share_per').removeClass('text-danger')
      $('#total_share_per').text("(" + comma(money) + ")")
    }
  } else {
    $("input[name='share_fee_rate']").each(function () {
      rate += parseFloat($(this).val());

      if (rate > 100) {
        ErrorAlert("매출 배분 수수료율은 100%를 초과할 수 없습니다.");
        $('#inv_total_share_per').val(rate);
        $('#total_share_per').removeClass('text-danger').addClass('text-danger');
        $('#total_share_per').text("(" + rate + "%) - 매출 배분 수수료율은 100%를 초과할 수 없습니다.")
        return false;
      }
    });
    $('#inv_total_share_per').val(rate);
    if (rate < 100 || isNaN(rate)) {
      $('#total_share_per').removeClass('text-danger').addClass('text-danger');
      $('#total_share_per').text("(" + rate + "%) - 매출 배분 수수료율합은 100%가 되어야합니다.")
    } else {
      $('#total_share_per').removeClass('text-danger')
      $('#total_share_per').text("(" + rate + "%)")
    }
  }
  //GenerateInvoicePrint();
}

//분배 타입 설정
function SetShareMoneyType() {
  if ($("#share_money_type:checked").val() == "1") {
    $(".share_row").each(function () {
      $(this).find("input[name='share_amt']").removeAttr("readonly");
      $(this).find("input[name='share_fee_rate']").attr("readonly", "readonly");
    })
  } else {
    $(".share_row").each(function () {
      $(this).find("input[name='share_fee_rate']").removeAttr("readonly");
      $(this).find("input[name='share_amt']").attr("readonly", "readonly");
    })
  }

  changeShareFee();
}

$('#share_money_type').click(SetShareMoneyType);

//매출배분 추가 버튼 클릭시,
$("#btn-add-share").click(function () {

  share_arr = [];
  $("#share_list").find("input[name='data.uv_seq']").each(function () {
    share_arr.push($(this).val());
  });

  ajxCall({
    url: window.url_add_invoice_share_user,
    data: {},
    dataType: 'html',
    type: 'GET',
    suc: function (html) {
      Modal_Slim("컨설턴트 조회", html);
    }
  });
});

function removeShareRow(obj) {
  $(obj).closest('.share_row').remove();
  TotalFeeChange();
}

var invoice_step5 = {
  rules: {

    //부가세 구분
    vat_type: {
      required: true
    },
    billing_money: {
      notOnlyZero: true
    },
    bill_currency_cd: {
      required: true
    }

  },

  messages: {
    //기본
    vat_type: {
      required: "부가세 구분은 필수 선택 항목입니다."
    },
    billing_money: {
      notOnlyZero: "발행금액은 0일 수 없습니다."
    },
    required: {
      required: "금액단위는 필수 선택 항목입니다."
    },
  }
}

var validator_step5 = $("#frmInvoiceStep5").validate($.extend({}, valid_option, invoice_step5));

function checkShareOk() {
  var rate = 0;
  var money = 0;
  if ($("#share_money_type:checked").val() == "1") {
    billing_amt = uncomma($('#billing_amt').val());
    $("input[name='share_amt']").each(function () {
      money += parseFloat(uncomma($(this).val()));
    });
    return money == billing_amt;
  } else {
    $("input[name='share_fee_rate']").each(function () {
      rate += parseFloat($(this).val());
    });
    return rate == 100;
  }
}

function chkStep5() {

  var chk2 = checkShareOk()


  if ($("#frmInvoiceStep5").valid() && chk2) {
    $(".part_show_inv_step6").show();
    $("#btn_chk_step5").hide();
    $("#frmInvoiceStep6").valid()
    return true;
  } else {
    $("#btn_chk_step5").show();

    return false;
  }
}

$("#btn_chk_step5").click(chkStep5);
//#endregion
//=========================================================================
//#region 인보이스 내용


var invoice_step6 = {
  rules: {

    //부가세 구분
    is_po_no: {
      required: true
    },
    is_open_name: {
      required: true
    },
    deposit_bank: {
      required: true
    },
    invoice_title: {
      required: true
    },
    invoice_contents: {
      required: true
    },
  },

}

var validator_step6 = $("#frmInvoiceStep6").validate($.extend({}, valid_option, invoice_step6));


function setInvoiceTitleDefault() {
  var title = getInvoiceTitleDefault()
  $('#invoice_title').val(title);
  validator_step6.element('#invoice_title');
}

function setInvoiceContentDefault() {
  var contents = getInvoiceContentDefault()
  $('#invoice_contents').val(contents);
  validator_step6.element('#invoice_contents');
}

//#endregion
//=========================================================================
//#region 미리보기

function showPreview() {
  var chk1 = $("#frmInvoiceStep1").valid();
  var chk2 = $("#frmInvoiceStep2").valid();
  var chk3 = $("#frmInvoiceStep3").valid();
  var chk4 = $("#frmInvoiceStep4").valid();
  var chk5 = $("#frmInvoiceStep5").valid();
  var chk6 = $("#frmInvoiceStep6").valid();

  if (chk1 && chk2 && chk3 && chk4 && chk5 && chk6) {
    var inv_html = GenerateInvoicePrint();
    $(".printarea").html(inv_html);
    Modal_Print_Slim("인보이스 미리보기", inv_html);
    
  } else {
    if (!chk1)
      validator_step1.focusInvalid();
    else if (!chk2)
      validator_step2.focusInvalid();
    else if (!chk3)
      validator_step3.focusInvalid();
    else if (!chk4)
      validator_step4.focusInvalid();
    else if (!chk5)
      validator_step5.focusInvalid();
    else if (!chk6)
      validator_step6.focusInvalid();

    ErrorAlert("누락된 항목이 있습니다. 확인 후 다시 시도 해주세요.");
  }
}

function chkSubmit() {
  var chk1 = $("#frmInvoiceStep1").valid();
  var chk2 = $("#frmInvoiceStep2").valid();
  var chk3 = $("#frmInvoiceStep3").valid();
  var chk4 = $("#frmInvoiceStep4").valid();
  var chk5 = $("#frmInvoiceStep5").valid();
  var chk6 = $("#frmInvoiceStep6").valid();

  if (chk1 && chk2 && chk3 && chk4 && chk5 && chk6) {
    return true;

  } else {
    if (!chk1)
      validator_step1.focusInvalid();
    else if (!chk2)
      validator_step2.focusInvalid();
    else if (!chk3)
      validator_step3.focusInvalid();
    else if (!chk4)
      validator_step4.focusInvalid();
    else if (!chk5)
      validator_step5.focusInvalid();
    else if (!chk6)
      validator_step6.focusInvalid();

    ErrorAlert("누락된 항목이 있습니다. 확인 후 다시 시도 해주세요.");
    return false;
  }
}

//#endregion
//=========================================================================

function getCurrentData() {
  var lang = GetInvoiceLang();
  var $pic = GetPicSeqObj();
  var client_name = (lang == 0 ? $("#client_name_kor").val() : $("#client_name_eng").val());
  var bill_currency = $("#bill_currency_cd").val();
  var deposit_bank_name = (bill_currency == "KRW" ? $("#deposit_bank option:checked").text() : "국민은행(외화전용)");
  var deposit_bank_acc = (bill_currency == "KRW" ? $("#deposit_bank").val() : "389868-11-010539");


  
  


  var currentData = {
    pii_seq: $("#pii_seq").val(),
    p_seq: $("#p_seq").val(),

    invoice_lang: lang,
    invoice_type: $("input[name=invoice_type]:checked").val(),
    billing_dt: $("#billing_dt").val(),
    tax_req_dt: $("#tax_req_dt").val(),
    //-------
    prc_seq: $pic.data("prc_seq"),
    join_dt: $pic.data("join_dt"),
    c_seq: $pic.data("c_seq"),
    candidate_name_kor : $("#candidate_name_kor").val(),
    candidate_name_eng: $("#candidate_name_eng").val(),
    expire_guarantee: $("#expire_guarantee").val(),
    candidate_source: $("#candidate_source").val(),
    candidate_source_txt: $("#candidate_source_txt").val(),
    candidate_position: $("#candidate_position").val(),
    candidate_position_txt: $("#candidate_position_txt").val(),
    opt_is_candidate_name_update: $("#opt_is_candidate_name_update").is(":checked") ? 1 : 0,
    //-------
    client_seq: $('#client_seq').val(),
    client_name: client_name,
    client_ceo: $("#ceo_kor").val(),
    client_addr1: $("#addr_kor").val(),
    client_biz_code: $("#client_biz_code").val(),
    opt_is_client_update: $("#opt_is_client_update").is(":checked") ? 1 : 0,
    //-------
    cc_seq: $("#cc_seq").val(),
    client_contact_name: $("#client_contact_name").val(),
    client_contact_tel: $("#client_contact_tel").val(),
    client_contact_email: $("#client_contact_email").val(),
    client_contact_pos: $("#client_contact_pos").val(),
    //-------
    ctc_seq: $("#ctc_seq").val(),
    client_etax_name: $("#client_etax_name").val(),
    client_etax_tel: $("#client_etax_tel").val(),
    client_etax_email: $("#client_etax_email").val(),
    //-------
    vat_type: $("input[name=vat_type]:checked").val(),
    fee_rate: $('#fee_rate').val(),
    billing_type: $("#billing_type").is(":checked") ? 1 : 0,
    ann_income: uncomma2($("#ann_income").val()),
    income_currency_cd: $pic.data("currency_cd"),
    billing_money: uncomma2($("#billing_money").val()),
    bill_currency_cd: $("#bill_currency_cd").val(),
    billing_won: uncomma2($("#billing_won").val()),
    billing_total: uncomma2($("#billing_total").val()),
    billing_amt: uncomma2($("#billing_amt").val()),
    billing_vat: uncomma2($("#billing_vat").val()),
    retainer_amt: uncomma2($("#retainer_amt").val()),
    opt_is_send_only_share: $("#opt_is_send_only_share").is(":checked") ? 1 : 0,
    //-------
    invoice_title: $("#invoice_title").val(),
    invoice_contents: $("#invoice_contents").val(),
    deposit_bank_name: deposit_bank_name,
    deposit_bank_account: deposit_bank_acc,
    is_open_name: $("input[name=is_open_name]:checked").val(),
    is_open_annual_income: $("input[name=is_open_annual_income]:checked").val(),
    is_po_no: $("input[name=is_po_no]:checked").val(),
    remarks: $("#invoice_remark").val(),
    opt_is_success: $("#opt_is_success").is(":checked") ? 1 : 0,

    chargeList: []
  };

  //매출배분 담당자 List
  $("#share_list").find(".share_row").each(function () {
    var chargeData = {
      p_seq: $("#p_seq").val(),
      uv_seq: $(this).find("input[name='data.uv_seq']").val(),
      ud_seq: $(this).find("input[name='data.ud_seq']").val(),
      name: $(this).find("input[name='data.name']").val(),
      sales_rate: $(this).find("input[name='share_fee_rate']").val(),
      sales_money: uncomma2($(this).find("input[name='share_amt']").val()),
      comments: $(this).find("input[name='share_comments']").val(),
    };

    currentData.chargeList.push(chargeData);
  });


  return currentData;
}


function InvoiceSubmit() {
  if (!chkSubmit()) {
    return;
  }
  //form 전송을 위한 데이터 생성.
  //기본정보
  var data = getCurrentData();
  ajxCall({
    url: window.url_invoice_submit,
    type: "POST",
    dataType: 'json',
    data: {
      data: data
    },
    loading: true,
    loading_target: 'body',
    suc: function (json) {
      //
      if (json.ok) {
        NormalAlert("인보이스 발행 요청 완료.", popclose);
      } else {
        ErrorAlert(json.message);
      }
    },
    err: function (jqXHR, textStatus, errorThrown) {
      hideLoading();
      ErrorAlert("알 수 없는 오류가 발생 했습니다.\n관리자에게 문의 하세요.");
    }
  });
}




//=========================================================================
$('#btn_modal_print').click(function () {
  window.print();
})
$("#frmInvoiceStep1").valid();