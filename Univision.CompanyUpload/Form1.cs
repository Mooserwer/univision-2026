using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;

namespace Univision.CompanyUpload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GridviewInit();
        }

        /// <summary>
        /// 데이터그리드뷰 설정
        /// </summary>
        private void GridviewInit()
        {
            dgView.ColumnCount = 24;
            int i = 0;
            dgView.Columns[i++].HeaderText = "인덱스";
            dgView.Columns[i++].HeaderText = "디비 업데이트 여부";
            dgView.Columns[i++].HeaderText = "자료생성년월";
            dgView.Columns[i++].HeaderText = "사업장명";
            dgView.Columns[i++].HeaderText = "사업자등록번호";
            dgView.Columns[i++].HeaderText = "사업장가입상태코드";
            dgView.Columns[i++].HeaderText = "우편번호";
            dgView.Columns[i++].HeaderText = "사업장지번상세주소";
            dgView.Columns[i++].HeaderText = "사업장도로명상세주소";
            dgView.Columns[i++].HeaderText = "고객법정동주소코드";
            dgView.Columns[i++].HeaderText = "고객행정동주소코드";
            dgView.Columns[i++].HeaderText = "법정동주소광역시도코드";
            dgView.Columns[i++].HeaderText = "법정동주소광역시시군구코드";
            dgView.Columns[i++].HeaderText = "법정동주소광역시시군구읍면동코드";
            dgView.Columns[i++].HeaderText = "사업장형태구분코드";
            dgView.Columns[i++].HeaderText = "사업장업종코드";
            dgView.Columns[i++].HeaderText = "사업장업종코드명";
            dgView.Columns[i++].HeaderText = "적용일자";
            dgView.Columns[i++].HeaderText = "재등록일자";
            dgView.Columns[i++].HeaderText = "탈퇴일자";
            dgView.Columns[i++].HeaderText = "가입자수";
            dgView.Columns[i++].HeaderText = "당월고지금액";
            dgView.Columns[i++].HeaderText = "신규취득자수";
            dgView.Columns[i++].HeaderText = "상실가입자수";
            


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://www.data.go.kr/dataset/3046071/fileData.do";
            
          
            var si = new ProcessStartInfo(url);
            Process.Start(si);
            linkLabel1.LinkVisited = true;
        }

        private void btnFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    txtCsvFile.Text = openFileDialog1.FileName;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private async void btnBindDV_Click(object sender, EventArgs e)
        {
            string path = txtCsvFile.Text;
            dgView.Rows.Clear();

            if(string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("파일을 선택하세요");
                return;
            }
            List<CSV_MODEL> list = new List<CSV_MODEL>();
            txtLog.AppendText("파일을 읽고 있습니다." + Environment.NewLine);


            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                int iReadCnt = 0;
                var model = new CSV_MODEL();
                List<string> data = new List<string>();
                while(sr.Peek() > -1)
                {
                    if (iReadCnt == 0)
                    {
                        data = sr.ReadLine().Split(',').ToList();
                        iReadCnt++;
                        continue;
                    }
                  

                    data = sr.ReadLine().Split(',').ToList();
                    if (data.Count == 0)
                    {
                   
                        return;
                    }

                    model = new CSV_MODEL()
                    {
                        인덱스 = iReadCnt,
                        자료생성년월 = data[0],
                        사업장명 = data[1],
                        사업자등록번호 = data[2],
                        사업장가입상태코드 = data[3],
                        우편번호 = data[4],
                        사업장지번상세주소 = data[5],
                        사업장도로명상세주소 = data[6],
                        고객법정동주소코드 = data[7],
                        고객행정동주소코드 = data[8],
                        법정동주소광역시도코드 = data[9],
                        법정동주소광역시시군구코드 = data[10],
                        법정동주소광역시시군구읍면동코드 = data[11],
                        사업장형태구분코드 = data[12],
                        사업장업종코드 = data[13],
                        사업장업종코드명 = data[14],
                        적용일자 = data[15],
                        재등록일자 = data[16],
                        탈퇴일자 = data[17],
                        가입자수 = int.Parse(data[18]),
                        당월고지금액 = long.Parse(data[19]),
                        신규취득자수 = int.Parse(data[20]),
                        상실가입자수 = int.Parse(data[21]),
                        디비업데이트여부 = "준비중"
                    };
                    list.Add(model);
                  
                    iReadCnt++;
                }
                txtLog.AppendText("파일 읽기를 완료하였습니다." + Environment.NewLine);
                pgBar.Minimum = 0;
                pgBar.Maximum = list.Count;
                pgBar.Step = 1;
                pgBar.Value = 0;
                txtLog.AppendText("전체 회사 데이터 " + list.Count.ToString("n0") + "개를 로딩 중에 있습니다."  + Environment.NewLine);

                foreach (var model2 in list)
                {
                    await Task.Delay(1);

                    dgView.Rows.Add(
                      model2.인덱스,
                      model2.디비업데이트여부,
                      model2.자료생성년월,
                      model2.사업장명,
                      model2.사업자등록번호,
                      model2.사업장가입상태코드,
                      model2.우편번호,
                      model2.사업장지번상세주소,
                      model2.사업장도로명상세주소,
                      model2.고객법정동주소코드,
                      model2.고객행정동주소코드,
                      model2.법정동주소광역시도코드,
                      model2.법정동주소광역시시군구코드,
                      model2.법정동주소광역시시군구읍면동코드,
                      model2.사업장형태구분코드,
                      model2.사업장업종코드,
                      model2.사업장업종코드명,
                      model2.적용일자,
                      model2.재등록일자,
                      model2.탈퇴일자,
                      model2.가입자수,
                      model2.당월고지금액,
                      model2.신규취득자수,
                      model2.상실가입자수);
                    pgBar.PerformStep();
                }
                txtLog.AppendText("전체 회사 정보를 로딩 하였습니다." + Environment.NewLine);
                MessageBox.Show("전체 회사 정보를 로딩 하였습니다.\n이제 데이터베이스로 저장하세요");
            }

            
        }

        private async void btnDBUpdate_Click(object sender, EventArgs e)
        {
            winformRepository re = new winformRepository();
            CSV_MODEL model = new CSV_MODEL();
            txtLog.AppendText("회사 정보를 업데이트 하는 중입니다." + Environment.NewLine);
            foreach (DataGridViewRow r in dgView.Rows)
            {
                if (r.Cells[0].Value == null)
                {
                    MessageBox.Show("모든 데이터를 디비에 저장하였습니다.");
                    break;
                }

            
                model = new CSV_MODEL()
                {
                    인덱스 = int.Parse(r.Cells[0].Value.ToString()),
                    디비업데이트여부 = r.Cells[1].Value.ToString(),
                    자료생성년월 = r.Cells[2].Value.ToString(),
                    사업장명 = r.Cells[3].Value.ToString(),
                    사업자등록번호 = r.Cells[4].Value.ToString(),
                    사업장가입상태코드 = r.Cells[5].Value.ToString(),
                    우편번호 = r.Cells[6].Value.ToString(),
                    사업장지번상세주소 = r.Cells[7].Value.ToString(),
                    사업장도로명상세주소 = r.Cells[8].Value.ToString(),
                    고객법정동주소코드 = r.Cells[9].Value.ToString(),
                    고객행정동주소코드 = r.Cells[10].Value.ToString(),
                    법정동주소광역시도코드 = r.Cells[11].Value.ToString(),
                    법정동주소광역시시군구코드 = r.Cells[12].Value.ToString(),
                    법정동주소광역시시군구읍면동코드 = r.Cells[13].Value.ToString(),
                    사업장형태구분코드 = r.Cells[14].Value.ToString(),
                    사업장업종코드 = r.Cells[15].Value.ToString(),
                    사업장업종코드명 = r.Cells[16].Value.ToString(),
                    적용일자 = r.Cells[17].Value.ToString(),
                    재등록일자 = r.Cells[18].Value.ToString(),
                    탈퇴일자 = r.Cells[19].Value.ToString(),
                    가입자수 = int.Parse(r.Cells[20].Value.ToString()),
                    당월고지금액 = long.Parse(r.Cells[21].Value.ToString()),
                    신규취득자수 = int.Parse(r.Cells[22].Value.ToString()),
                    상실가입자수 = int.Parse(r.Cells[23].Value.ToString())
                };

                int iResult = await re.InsertOrUpdate(model);
                await Task.Delay(1);
                txtLog.AppendText(model.인덱스.ToString() + " 번째 " + model.사업장명 + " 회사를 업데이트 하였습니다." + Environment.NewLine);
                switch (iResult)
                {
                    case 0: //에러
                        r.DefaultCellStyle.BackColor = Color.Red;
                        r.Cells[1].Value = "에러";
                        break;
                    case 1: //업데이트
                        r.DefaultCellStyle.BackColor = Color.Pink;
                        r.Cells[1].Value = "업데이트";
                        break;
                    case 2: //저장
                        r.DefaultCellStyle.BackColor = Color.SkyBlue;
                        r.Cells[1].Value = "저장";
                        break;
                }

            }
            txtLog.AppendText("회사 정보 저장을 완료 하였습니다." + Environment.NewLine);
        }
    }
}
