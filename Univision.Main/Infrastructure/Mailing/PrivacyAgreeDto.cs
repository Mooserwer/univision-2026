using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
  public class PrivacyAgreeDto : MailDto
  {
    /// <summary>
    /// 메일 제목
    /// </summary>
    public string canname { get; set; }
    public string url { get; set; }

  }

  public class PrivacyAgreeTemplete : TempleteDto
  {
    public PrivacyAgreeTemplete()
    {
      this.MailSubject = "[유니코써치] 개인정보 수집 및 제공 동의서";

			this.MailBody = @"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> 
<html xmlns='http://www.w3.org/1999/xhtml' lang='ko' xml:lang='ko'> 
<head>
<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
<meta http-equiv='Cache-Control' content='no-cache' />
<style type='text/css'>
@font-face{
 font-family: NanumGothic, ng;
 src:url('https://www.unicosearch.com/font/NanumGothic.eot');
 src:local(※), url('https://www.unicosearch.com/font/NanumGothic.woff') format('woff')
}
</style>
<!-- 웹폰트 ie9설정-->
<!--[if gte IE 9]>
  <link rel='stylesheet' href='/css/ie9pix.css' />
<![endif]-->
<title>유니코써치</title>
</head>
<body style='margin:0;padding:0;font-family:NanumGothic,ng,dotum;font-size:12px;color:#666;line-height:18px;white-space:normal'>
<!-- 페이지시작 -->
<div style='position:relative;'>
	<div style='width:740px; margin:0 auto;'>
		<div><img src='https://www.unicosearch.com/_images/account/t_04.gif' /></div>
		<div style='float:left;'><img src='https://www.unicosearch.com/_images/account/04top_bg.gif' alt='탑이미지' /></div>
		<div style='padding:15px'>
			<!-- ////// [메일컨텐츠시작]  /////// -->
			 <div  style='border:1px solid #ccc; border-radius:15px; padding:20px; margin-bottom:40px;'>			
				<div style='padding:30px;'>
					안녕하세요? <span style='font-weight:bold'>{{canname}} 님</span>
         <br/><br/>
         (주)유니코써치는 인재추천 및 채용절차 진행을 위하여 아래와 같이 개인정보 수집·이용 및 제3자(채용사) 제공에 동의를 받고자 합니다.<br/><br/>
         제공해 주신 {{canname}}님의 소중한 개인정보는 채용을 위한 절차 이외에 어떤 목적으로도 사용되지 않으며 당사의 개인정보보호 정책에 따라 기술적·물리적인 방법으로 적극 보호되고 있습니다.
					<div style='padding:20px 0;'>
					   <span style='color:#28757c; font-weight:bold;'>아래의 링크주소를 클릭하시면 개인정보 수집 및 제공 동의 화면으로 이동합니다.</span>
						 <br/><br/>
						 동의 내용을 상세히 확인하신 후 여부를 결정해 주시고, 확인 버튼을 눌러 간단히 진행하여 주시기 바랍니다. <br/>
             정보 제공에 동의하지 않으실 경우 채용을 위한 서비스 이용에 제한이 있을 수 있습니다
					</div>
					<div style='background:#f4f4f4; border:1px solid #e2e1e1; padding:10px; font-weight:bold;font-size:13px;'>
						동의 진행하기 : <a href='{{url}}'>{{url}}</a>
					</div>
				</div>
			</div>
			<!-- ////// [메일컨텐츠끝]  /////// -->
			<div style='padding-top:15px'>
				<div style='padding-left:5px'>
					<ul style='list-style:none; margin:0; padding:0;'>
						<li>
							<a href='https://www.unicosearch.com/insight/news/2040' target='_blank'><img src='https://www.unicosearch.com/_images/account/footermark_01.gif' alt='고용서비스 우수기관 선정' style='border:none;padding-right:6px;' /></a>
							<a href='https://www.unicosearch.com/insight/news/2017' target='_blank'><img src='https://www.unicosearch.com/_images/account/footermark_04.gif' alt='대한민국 일하기 좋은 300대 기업 선정' style='border:none;' /></a>
						</li>
					</ul>
				</div>
				<div style='padding:10px 0 0 5px;'><img src='https://www.unicosearch.com/_images/account/footer_mail_2020.png' alt='서울특별시 강남구 테헤란로87길 36 도심공항타워 17층, 25층  TEL : 82-2-551-2300  /  Fax : 82-2-551-4959' />	</div>			
			</div>
		</div>
	</div>
</div> <!-- e : wrapper -->
</body>
</html>
";

    }


  }

	
}