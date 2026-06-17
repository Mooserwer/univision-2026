using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Candidate;
using Univision.Core.Models.DTO.Response.Candidate;

namespace Univision.Core.Repositories
{
  public class CandidateEntityRepository : BaseRepository
  {
    public CandidateEntityRepository(UnivisionContext db) : base(db)
    {

    }
    public CandidateEntityRepository()
    {

    }

    /// <summary>
    /// 후보자 단건 조회
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<candidate> SelectCandidateOneAsync(int c_seq)
    {
      try
      {
        return await db.candidate
                       .Where(x => x.c_seq == c_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 관심 후보 조회
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<can_interest> SelectCanMyListOneAsync(int c_seq, int user_seq)
    {
      try
      {
        return await db.can_interests
                       .Where(x => x.c_seq == c_seq && x.uv_seq == user_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 외부접속 확인
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<element_open_log> SelectCanOpenLogOneAsync(int c_seq, int user_seq)
    {
      try
      {
        var date_time = DateTime.Now.AddDays(-1);
        return await db.element_open_logs
                       .Where(x => x.element_seq == c_seq && x.uv_seq == user_seq && x.page_name == "candidate" && x.table_name == "candidate" && x.log_date > date_time)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<element_open_log> SelectDirectorOpenLogOneAsync(int d_seq, int user_seq)
    {
      try
      {
        var date_time = DateTime.Now.AddDays(-1);
        return await db.element_open_logs
                       .Where(x => x.element_seq == d_seq && x.uv_seq == user_seq && x.page_name == "candidate" && x.table_name == "director" && x.log_date > date_time)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 검색 조건 불러오기
    /// </summary>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<List<candidate_search_save>> SelectCanSearchSaveListAsync(int uv_seq)
    {
      try
      {
        return await db.candidate_search_saves
                       .Where(x => x.create_user == uv_seq)
                       .OrderByDescending(x => x.css_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<candidate_search_save> SelectCanSearchSaveOneAsync(int css_seq)
    {
      try
      {
        return await db.candidate_search_saves
                       .Where(x => x.css_seq == css_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 관심 후보 등록 or 삭제
    /// </summary>
    /// <param name="ci"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrDeleteCanSearchSave(candidate_search_save css, string flag)
    {
      try
      {
        if (flag == "D")
        {
          db.Entry(css).State = EntityState.Deleted;
        }
        else if (flag == "U") {
          db.Entry(css).State = EntityState.Modified;
        }
        else
        {
          db.Entry(css).State = EntityState.Added;
        }
          

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 관심후보 조회.
    /// </summary>
    /// <param name="cf_seq"></param>
    /// <returns></returns>
    public async Task<can_interest> SelectCanMyListOneAsync(int cf_seq)
    {
      try
      {
        return await db.can_interests
                       .Where(x => x.cf_seq == cf_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 관심 후보 등록 or 삭제
    /// </summary>
    /// <param name="ci"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrDeleteCanMyList(can_interest ci, string flag, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        if (flag == "D")
        {
          db.Entry(ci).State = EntityState.Deleted;
          CandidateLogRepository log = new CandidateLogRepository();
          await log.can_interest_log(ci, flag, event_uv_seq, 1);
        }
        else if (flag == "U")
          db.Entry(ci).State = EntityState.Modified;
        else
          db.Entry(ci).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 메모 조회
    /// </summary>
    /// <param name="cm_seq"></param>
    /// <returns></returns>
    public async Task<can_activity> SelectCanActivityOneAsync(int ca_seq)
    {
      try
      {
        return await db.can_activitys
                       .Where(x => x.ca_seq == ca_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<director_activity> SelectDirectorActivityOneAsync(int da_seq)
    {
      try
      {
        return await db.director_activitys
                       .Where(x => x.da_seq == da_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 메모 등록 or 삭제
    /// </summary>
    /// <param name="cm"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrUpdateCanActivity(can_activity ca, string flag, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        CandidateLogRepository log = new CandidateLogRepository();
        if (flag == "D")
          db.Entry(ca).State = EntityState.Deleted;
        else if (flag == "U")
          db.Entry(ca).State = EntityState.Modified;
        else
          db.Entry(ca).State = EntityState.Added;

        await log.can_activity_log(ca, flag, event_uv_seq, event_idx);

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateDirectorActivity(director_activity da, string flag)
    {
      try
      {
        CandidateLogRepository log = new CandidateLogRepository();
        if (flag == "D")
          db.Entry(da).State = EntityState.Deleted;
        else if (flag == "U")
          db.Entry(da).State = EntityState.Modified;
        else
          db.Entry(da).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 인터뷰 조회
    /// </summary>
    /// <param name="cis_seq"></param>
    /// <returns></returns>
    public async Task<can_interview_sheet> SelectCanInterviewOneAsync(int cis_seq)
    {
      try
      {
        return await db.can_interview_sheets
                       .Where(x => x.cis_seq == cis_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 인터뷰 등록 or 삭제
    /// </summary>
    /// <param name="cis"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrDeleteCanInterview(can_interview_sheet cis, string flag, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        if (flag == "D")
        {
          db.Entry(cis).State = EntityState.Deleted;
          CandidateLogRepository log = new CandidateLogRepository();
          await log.can_interview_sheet_log(cis, flag, event_uv_seq, 1);
        }
        else
          db.Entry(cis).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 이력서 조회
    /// </summary>
    /// <param name="cis_seq"></param>
    /// <returns></returns>
    public async Task<can_resume> SelectCanResumeOneAsync(int cr_seq)
    {
      try
      {
        return await db.can_resumes
                       .Where(x => x.cr_seq == cr_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 이력서 등록 or 삭제
    /// </summary>
    /// <param name="cr"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrDeleteCanResume(can_resume cr, string flag, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        if (flag == "D")
        {
          //db.Entry(cr).State = EntityState.Deleted;
          db.Entry(cr).State = EntityState.Modified;
          CandidateLogRepository log = new CandidateLogRepository();
          await log.CreateOrDeleteCanResume(cr, flag, event_uv_seq, event_idx);

        }
        else
          db.Entry(cr).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrDeleteTempCanResume(tempsaved_can_resume cr, string flag, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        if (flag == "D")
        {
          //db.Entry(cr).State = EntityState.Deleted;
          db.Entry(cr).State = EntityState.Modified;
          CandidateLogRepository log = new CandidateLogRepository();
          await log.tempsaved_can_resume_log(cr, flag, event_uv_seq, event_idx);
        }
        else
          db.Entry(cr).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 학교 단건 조회
    /// </summary>
    /// <param name="schoolName"></param>
    /// <returns></returns>
    public async Task<code_school> SelectSchoolOneAsync(string schoolName, string campus_name)
    {
      try
      {
        return await db.code_schools
                       .Where(x => x.school_name == schoolName)
                       .Where(x => x.campus_name == campus_name)
                       .FirstOrDefaultAsync();

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 회사 단건 조회
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    public async Task<gov_api_company> SelectComapnyOneAsync(string companyName)
    {
      try
      {
        return await db.gov_api_companys
                       .Where(x => x.WKPL_NM == companyName)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 단건 업데이트
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public async Task UpdateCandidateOneAsync(candidate candidate, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        db.Entry(candidate).State = EntityState.Modified;
        CandidateLogRepository log = new CandidateLogRepository();
        await log.candidate_log(candidate, "U", event_uv_seq, event_idx);
        await db.SaveChangesAsync();


      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 경력정보 단건 등록
    /// </summary>
    /// <param name="cc"></param>
    /// <returns></returns>
    public async Task CreateCandidateCareerOneAsync(can_career cc)
    {
      try
      {
        db.Entry(cc).State = EntityState.Added;
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task UpdateCandidateCareerOneAsync(can_career cc)
    {
      try
      {
        db.Entry(cc).State = EntityState.Modified;
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 등록 혹은 업데이트
    /// </summary>
    /// <param name="candi"></param>
    /// <param name="schoolList"></param>
    /// <param name="careerList"></param>
    /// <param name="cjobList"></param>
    /// <param name="placeList"></param>
    /// <param name="jobList"></param>
    /// <param name="busiList"></param>
    /// <param name="foreignList"></param>
    /// <returns></returns>
    public async Task CreateOrUpdateCandidate(CandidateCreateUpdateModel model, string state, List<can_activity> cActivity, string activityState, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        ///테스트 후 할것
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              CandidateLogRepository log = new CandidateLogRepository();
              //수정건 삭제 먼저.
              //기존 학력 정보 삭제
              if (model.deleteSchoolList.Count > 0)
              {
                foreach (var dSchool in model.deleteSchoolList)
                {
                  db.Entry(dSchool).State = EntityState.Deleted;
                }
              }


              //기존 경력정보 삭제.
              if (model.deleteCareerList.Count > 0)
              {
                foreach (var dCareer in model.deleteCareerList)
                {
                  db.Entry(dCareer).State = EntityState.Deleted;
                  await log.can_career_log(model.deleteCareerList, "D", event_uv_seq, 1);
                  //기존 경력직무 정보 삭제.
                  foreach (var dCareerJob in dCareer.jobList)
                  {
                    db.Entry(dCareerJob).State = EntityState.Deleted;
                  }
                }

              }


              //기존 희망근무지 삭제
              if (model.deletePlaceList.Count > 0)
              {
                foreach (var dPlace in model.deletePlaceList)
                {
                  db.Entry(dPlace).State = EntityState.Deleted;
                }
              }


              //기존 직무 삭제
              if (model.deleteJobList.Count > 0)
              {
                foreach (var dJob in model.deleteJobList)
                {
                  db.Entry(dJob).State = EntityState.Deleted;
                }
              }


              //기존 산업 삭제
              if (model.deleteBusiList.Count > 0)
              {
                foreach (var dBusi in model.deleteBusiList)
                {
                  db.Entry(dBusi).State = EntityState.Deleted;
                }
              }


              //기존 외국어능력 삭제
              if (model.deleteForeignList.Count > 0)
              {
                foreach (var dForeign in model.deleteForeignList)
                {
                  db.Entry(dForeign).State = EntityState.Deleted;
                }
              }
              await log.candidate_log(model.data, "U", event_uv_seq, 1);
              await db.SaveChangesAsync();

              //후보자 기본정보 수정.
              db.Entry(model.data).State = EntityState.Modified;

              await db.SaveChangesAsync();
            }
            else
            {
              //후보자 기본정보 신규 등록
              db.Entry(model.data).State = EntityState.Added;
              await db.SaveChangesAsync();
            }

            //후보자 seq와 학교 seq를 얻어야 하므로 먼저 한번 저장.


            //후보자 학력
            if (model.schoolList != null)
              foreach (var school in model.schoolList)
              {
                //새로 만들어야하는 학교 정보가 있는 경우.
                if (school.newSchool != null)
                {

                  db.Entry(school.newSchool).State = EntityState.Added;

                  //sc_seq 를 얻어야 하므로 먼저 저장
                  await db.SaveChangesAsync();

                  //can_school에 새로 만든 학교의 sc_seq 를 맵핑.
                  school.school.sc_seq = school.newSchool.sc_seq;
                }

                school.school.c_seq = model.data.c_seq;
                db.Entry(school.school).State = EntityState.Added;
              }

            //후보자 경력
            if (model.careerList != null)
              foreach (var career in model.careerList)
              {
                career.career.c_seq = model.data.c_seq;
                db.Entry(career.career).State = EntityState.Added;

                //후보자 경력 seq 를 얻어야 하므로 먼저 한번 저장.
                await db.SaveChangesAsync();

                //후보자 경력 직무정보
                if (career.careerJobList != null)
                  foreach (var cJob in career.careerJobList)
                  {
                    cJob.cc_seq = career.career.cc_seq;
                    db.Entry(cJob).State = EntityState.Added;
                  }
              }


            //후보자 희망 근무지
            if (model.placeList != null)
              foreach (var place in model.placeList)
              {
                place.c_seq = model.data.c_seq;
                db.Entry(place).State = EntityState.Added;
              }

            //후보자 직무 정보
            if (model.jobList != null)
              foreach (var job in model.jobList)
              {
                job.c_seq = model.data.c_seq;
                db.Entry(job).State = EntityState.Added;
              }

            //후보자 산업 정보
            if (model.busiList != null)
              foreach (var busi in model.busiList)
              {
                busi.c_seq = model.data.c_seq;
                db.Entry(busi).State = EntityState.Added;
              }

            //후보자 외국어능력 정보
            if (model.foreignList != null)
              foreach (var foreign in model.foreignList)
              {
                foreign.c_seq = model.data.c_seq;
                db.Entry(foreign).State = EntityState.Added;
              }

            //메모장
            if (cActivity != null && cActivity.Count() > 0)
              foreach (var activity in cActivity)
              {
                activity.c_seq = model.data.c_seq;
                db.Entry(activity).State = EntityState.Added;
              }

            //저장
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (DbEntityValidationException dbEx)
          {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
              foreach (var validationError in validationErrors.ValidationErrors)
              {
                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
              }
            }
            tran.Rollback();
            throw dbEx;
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateDirector(DirectorCreateUpdateModel model, string state, string activityState, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        ///테스트 후 할것
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              CandidateLogRepository log = new CandidateLogRepository();
              //수정건 삭제 먼저.
              //기존 학력 정보 삭제
              if (model.deleteSchoolList.Count > 0)
              {
                foreach (var dSchool in model.deleteSchoolList)
                {
                  db.Entry(dSchool).State = EntityState.Deleted;
                }
              }


              //기존 경력정보 삭제.
              if (model.deleteCareerList.Count > 0)
              {
                foreach (var dCareer in model.deleteCareerList)
                {
                  db.Entry(dCareer).State = EntityState.Deleted;
                  //await log.can_career_log(model.deleteCareerList, "D", event_uv_seq, 1);                  

                }

              }


              //기존 직무 삭제
              if (model.deleteJobList.Count > 0)
              {
                foreach (var dJob in model.deleteJobList)
                {
                  db.Entry(dJob).State = EntityState.Deleted;
                }
              }


              //기존 산업 삭제
              if (model.deleteBusiList.Count > 0)
              {
                foreach (var dBusi in model.deleteBusiList)
                {
                  db.Entry(dBusi).State = EntityState.Deleted;
                }
              }


              //            await log.candidate_log(model.data, "U", event_uv_seq, 1);
              await db.SaveChangesAsync();

              //후보자 기본정보 수정.
              db.Entry(model.data).State = EntityState.Modified;

              await db.SaveChangesAsync();
            }
            else
            {
              //후보자 기본정보 신규 등록
              db.Entry(model.data).State = EntityState.Added;
              await db.SaveChangesAsync();
            }

            //후보자 seq와 학교 seq를 얻어야 하므로 먼저 한번 저장.


            //후보자 학력
            if (model.schoolList != null)
              foreach (var school in model.schoolList)
              {
                school.d_seq = model.data.d_seq;
                db.Entry(school).State = EntityState.Added;
              }

            //후보자 경력
            if (model.careerList != null)
              foreach (var career in model.careerList)
              {
                career.d_seq = model.data.d_seq;
                db.Entry(career).State = EntityState.Added;

                //후보자 경력 seq 를 얻어야 하므로 먼저 한번 저장.
                await db.SaveChangesAsync();
              }




            //후보자 직무 정보
            if (model.jobList != null)
              foreach (var job in model.jobList)
              {
                job.d_seq = model.data.d_seq;
                db.Entry(job).State = EntityState.Added;
              }

            //후보자 산업 정보
            if (model.busiList != null)
              foreach (var busi in model.busiList)
              {
                busi.d_seq = model.data.d_seq;
                db.Entry(busi).State = EntityState.Added;
              }

            //저장
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (DbEntityValidationException dbEx)
          {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
              foreach (var validationError in validationErrors.ValidationErrors)
              {
                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
              }
            }
            tran.Rollback();
            throw dbEx;
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 등록 혹은 업데이트
    /// </summary>
    /// <param name="candi"></param>
    /// <param name="schoolList"></param>
    /// <param name="careerList"></param>
    /// <param name="cjobList"></param>
    /// <param name="placeList"></param>
    /// <param name="jobList"></param>
    /// <param name="busiList"></param>
    /// <param name="foreignList"></param>
    /// <returns></returns>
    public async Task TempCreateOrUpdateCandidate(TempCandidateCreateUpdateModel model, string state, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        //테스트 후 진행
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            CandidateLogRepository log = new CandidateLogRepository();
            // 기존 정보 삭제 
            if (model.temp_seq != 0 && model.temp_seq != null)
            {
              tempsaved_candidate tc = await db.tempsaved_candidates.Where(x => x.c_seq == model.temp_seq).FirstOrDefaultAsync();
              if (tc != null)
              {
                db.Entry(tc).State = EntityState.Deleted;
                await log.tempsaved_candidate_log(tc, "D", event_uv_seq, 1);
                await db.SaveChangesAsync();
              }
            }


            //후보자 기본정보 신규 등록
            db.Entry(model.data).State = EntityState.Added;
            await db.SaveChangesAsync();

            //후보자 seq와 학교 seq를 얻어야 하므로 먼저 한번 저장.


            //후보자 학력
            if (model.schoolList != null)
              foreach (var school in model.schoolList)
              {
                //새로 만들어야하는 학교 정보가 있는 경우.
                if (school.newSchool != null)
                {

                  db.Entry(school.newSchool).State = EntityState.Added;

                  //sc_seq 를 얻어야 하므로 먼저 저장
                  await db.SaveChangesAsync();

                  //can_school에 새로 만든 학교의 sc_seq 를 맵핑.
                  school.school.sc_seq = school.newSchool.sc_seq;
                }

                school.school.c_seq = model.data.c_seq;
                db.Entry(school.school).State = EntityState.Added;
              }

            //후보자 경력
            if (model.careerList != null)
              foreach (var career in model.careerList)
              {
                career.career.c_seq = model.data.c_seq;
                db.Entry(career.career).State = EntityState.Added;

                //후보자 경력 seq 를 얻어야 하므로 먼저 한번 저장.
                await db.SaveChangesAsync();

                //후보자 경력 직무정보
                if (career.careerJobList != null)
                  foreach (var cJob in career.careerJobList)
                  {
                    cJob.cc_seq = career.career.cc_seq;
                    db.Entry(cJob).State = EntityState.Added;
                  }
              }


            //후보자 희망 근무지
            if (model.placeList != null)
              foreach (var place in model.placeList)
              {
                place.c_seq = model.data.c_seq;
                db.Entry(place).State = EntityState.Added;
              }

            //후보자 직무 정보
            if (model.jobList != null)
              foreach (var job in model.jobList)
              {
                job.c_seq = model.data.c_seq;
                db.Entry(job).State = EntityState.Added;
              }

            //후보자 산업 정보
            if (model.busiList != null)
              foreach (var busi in model.busiList)
              {
                busi.c_seq = model.data.c_seq;
                db.Entry(busi).State = EntityState.Added;
              }

            //후보자 외국어능력 정보
            if (model.foreignList != null)
              foreach (var foreign in model.foreignList)
              {
                foreign.c_seq = model.data.c_seq;
                db.Entry(foreign).State = EntityState.Added;
              }



            //저장
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (DbEntityValidationException dbEx)
          {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
              foreach (var validationError in validationErrors.ValidationErrors)
              {
                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
              }
            }
            tran.Rollback();
            throw dbEx;
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    /// <summary>
    /// 후보자 학력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_school>> SelectCanSchoolListAsync(int c_seq)
    {
      try
      {
        return await db.can_schools
                           .Where(x => x.c_seq == c_seq)
                           .ToListAsync();

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<director_school>> SelectDirectorSchoolListAsync(int d_seq)
    {
      try
      {
        return await db.director_schools
                           .Where(x => x.d_seq == d_seq)
                           .ToListAsync();

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 경력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_career>> SelectCanCareerListAsync(int c_seq)
    {
      try
      {
        var list = await db.can_careers
                           .Where(x => x.c_seq == c_seq)
                           .ToListAsync();

        foreach (var data in list)
        {
          data.jobList = await db.can_career_jobs
                                 .Where(x => x.cc_seq == data.cc_seq)
                                 .ToListAsync();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<director_career>> SelectDirectorCareerListAsync(int d_seq)
    {
      try
      {
        var list = await db.director_careers
                           .Where(x => x.d_seq == d_seq)
                           .ToListAsync();

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 희망근무지 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_place>> SelectCanPlaceListAsync(int c_seq)
    {
      try
      {
        return await db.can_places
                       .Where(x => x.c_seq == c_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 직무정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_job>> SelectCanJobCodeListAsync(int c_seq)
    {
      try
      {
        return await db.can_jobs
                       .Where(x => x.c_seq == c_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<director_job>> SelectDirectorJobCodeListAsync(int d_seq)
    {
      try
      {
        return await db.director_jobs
                       .Where(x => x.d_seq == d_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    /// <summary>
    /// 후보자 산업정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_business>> SelectCanBusiCodeListAsync(int c_seq)
    {
      try
      {
        return await db.can_businesss
                       .Where(x => x.c_seq == c_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<director_business>> SelectDirectorBusiCodeListAsync(int d_seq)
    {
      try
      {
        return await db.director_businesss
                       .Where(x => x.d_seq == d_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 외국어 능력 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_foreign_lan>> SelectCanForeignLanListAsync(int c_seq)
    {
      try
      {
        return await db.can_foreign_lans
                       .Where(x => x.c_seq == c_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<simple_candidate> SelectSimpleCandidateOneAsync(int sc_seq)
    {
      try
      {
        return await db.simple_candidates
                       .Where(x => x.sc_seq == sc_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<tempsaved_candidate> SelectTempCandidateOneAsync(int c_seq)
    {
      try
      {
        return await db.tempsaved_candidates
                       .Where(x => x.c_seq == c_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateSimpleCandidate(simple_candidate sc, string flag, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        CandidateLogRepository log = new CandidateLogRepository();
        if (flag == "D")
        {
          db.Entry(sc).State = EntityState.Deleted;
        }
        else if (flag == "U")
        {
          db.Entry(sc).State = EntityState.Modified;
        }

        else
          db.Entry(sc).State = EntityState.Added;

        await log.simple_candidate_log(sc, flag, event_uv_seq, 1);
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateTempCandidate(tempsaved_candidate tc, string flag)
    {
      try
      {
        //CandidateLogRepository log = new CandidateLogRepository();
        if (flag == "D")
        {
          db.Entry(tc).State = EntityState.Deleted;
        }
        else if (flag == "U")
        {
          db.Entry(tc).State = EntityState.Modified;
        }

        else
          db.Entry(tc).State = EntityState.Added;

        //await log.simple_candidate_log(sc, flag, event_uv_seq, 1);
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }



    public async Task<director> SelectDirectorOneAsync(int d_seq)
    {
      try
      {
        return await db.directors
                       .Where(x => x.d_seq == d_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }



  }
}
