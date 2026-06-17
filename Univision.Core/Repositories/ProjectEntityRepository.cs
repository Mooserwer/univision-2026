using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Candidate;

namespace Univision.Core.Repositories
{
  public class ProjectEntityRepository : BaseRepository
  {
    public ProjectEntityRepository(UnivisionContext db) : base(db)
    {

    }
    public ProjectEntityRepository()
    {

    }

    public async Task<inorder> SelectInorderOneAsync(int i_seq)
    {
      try
      {
        return await db.inorders
                       .Where(x => x.i_seq == i_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<makeup_request> SelectMakeupRequestOneAsync(int mr_idx)
    {
      try
      {
        return await db.makeup_requests
                       .Where(x => x.mr_idx == mr_idx)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<inorder_memo> SelectInorderMemoOneAsync(int im_seq)
    {
      try
      {
        return await db.inorder_memos
                       .Where(x => x.im_seq == im_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<inorder_director>> SelectInorderDirectorListAsync(int i_seq)
    {
      try
      {
        return await db.Inorder_directors
                           .Where(x => x.i_seq == i_seq)
                           .ToListAsync();

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<inorder_file>> SelectInorderFileListAsync(int i_seq)
    {
      try
      {
        return await db.inorder_files
                       .Where(x => x.i_seq == i_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<inorder_file> SelectInorderFileOneAsync(int if_seq)
    {
      try
      {
        return await db.inorder_files
                       .Where(x => x.if_seq == if_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrDeleteInorderFile(inorder_file inf, string flag)
    {
      try
      {
        if (flag == "D")
          //db.Entry(cr).State = EntityState.Deleted;
          db.Entry(inf).State = EntityState.Modified;
        else
          db.Entry(inf).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateInorder(InorderCreateUpdateModel model, string state, int event_uv_seq = 0, int event_idx = 1)
    {
      try
      {
        ProjectLogRepository log = new ProjectLogRepository();
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              //인오더 기본정보 수정.
              db.Entry(model.data).State = EntityState.Modified;
              await log.Inorder_log(model.data, "U", event_uv_seq, event_idx);
            }
            else
            {
              //인오더 기본정보 신규 등록
              db.Entry(model.data).State = EntityState.Added;
            }
            await db.SaveChangesAsync();

            //파일 등록
            if (model.fileList != null)
              foreach (var file in model.fileList)
              {
                file.i_seq = model.data.i_seq;
                db.Entry(file).State = EntityState.Added;
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

    public async Task DeleteInorder(InorderCreateUpdateModel model, string state, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        ProjectLogRepository log = new ProjectLogRepository();
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (model.fileList != null)
              foreach (var file in model.fileList)
              {
                db.Entry(file).State = EntityState.Deleted;
              }

            //인오더 기본정보 수정.
            db.Entry(model.data).State = EntityState.Deleted;
            await log.Inorder_log(model.data, "D", event_uv_seq, event_idx);

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
    /// 인오더 메모 생성, 수정, 삭제
    /// </summary>
    /// <param name="memo"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrUpdateInorderMemo(inorder_memo memo, string flag)
    {
      try
      {
        if (flag == "D")
          db.Entry(memo).State = EntityState.Deleted;
        else if (flag == "U")
          db.Entry(memo).State = EntityState.Modified;
        else
          db.Entry(memo).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 키 프로젝트 단건 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<pjt_mykey> SelectProjectMyKeyOneAsync(int p_seq, int uv_seq)
    {
      try
      {
        return await db.pjt_mykeys
                       .Where(x => x.p_seq == p_seq && x.uv_seq == uv_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 전담코웤 프로젝트 알람 생성
    /// </summary>
    /// <param name="mykey"></param>
    /// <param name="aMessage"></param>
    /// <param name="aList"></param>
    /// <returns></returns>
    public async Task CreateOrRemoveMyKey(pjt_mykey mykey, string state="C")
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "C")
            {
              db.Entry(mykey).State = EntityState.Added;

            }
            else if (state == "D")
            {
              db.Entry(mykey).State = EntityState.Deleted;
            }
            
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }




    /// <summary>
    /// 프로젝트 단건 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<project> SelectProjectOneAsync(int p_seq)
    {
      try
      {
        return await db.projects
                       .Where(x => x.p_seq == p_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task ProjectModifyUpdate(int p_seq, int uv_seq)
    {
      try
      {
        var result = db.projects.SingleOrDefault(b => b.p_seq == p_seq);
        if (result != null)
        {
          result.modify_dt = DateTime.UtcNow;
          result.modify_user = uv_seq;
          db.SaveChanges();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 등록 혹은 업데이트
    /// </summary>
    /// <returns></returns>
    public async Task CreateOrUpdateProject(ProjectCreateUpdateModel model, string state, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              ProjectLogRepository log = new ProjectLogRepository();
              //수정건 삭제 먼저.
              //기존 AM 정보 삭제
              if (model.deleteAmList.Count > 0)
              {
                foreach (var dAm in model.deleteAmList)
                {
                  db.Entry(dAm).State = EntityState.Deleted;
                }
                //await log.pjt_director_log(model.deleteAmList, "D", event_uv_seq, 0);
              }


              //기존 Searcher 삭제.
              if (model.deleteSearcherList.Count > 0)
              {
                foreach (var dSearcher in model.deleteSearcherList)
                {
                  db.Entry(dSearcher).State = EntityState.Deleted;
                }
                // await log.pjt_manager_log(model.deleteSearcherList, "D", event_uv_seq, 0);
              }


              ////기존 산업 삭제
              //if (model.deleteBusiList.Count > 0)
              //    foreach (var dBusi in model.deleteBusiList)
              //    {
              //        db.Entry(dBusi).State = EntityState.Deleted;
              //    }

              //기존 직무 삭제
              //if (model.deleteJobList.Count > 0)
              //    foreach (var dJob in model.deleteJobList)
              //    {
              //        db.Entry(dJob).State = EntityState.Deleted;
              //    }

              //기존 희망근무지 삭제
              if (model.deletePlaceList.Count > 0)
              {
                foreach (var dPlace in model.deletePlaceList)
                {
                  db.Entry(dPlace).State = EntityState.Deleted;
                }

                //   await log.pjt_place_log(model.deletePlaceList, "D", event_uv_seq, 0);
              }



              //기존 키워드 삭제
              if (model.deleteKeywordList.Count > 0)
              {
                foreach (var dKeyword in model.deleteKeywordList)
                {
                  db.Entry(dKeyword).State = EntityState.Deleted;
                }
                //  await log.pjt_keyword_log(model.deleteKeywordList, "D", event_uv_seq, 0);
              }


              //프로젝트 기본정보 수정.
              db.Entry(model.data).State = EntityState.Modified;
              await log.project_log(model.data, "U", event_uv_seq, 1);

            }
            else
            {
              //프로젝트 기본정보 신규 등록
              db.Entry(model.data).State = EntityState.Added;
            }

            //프로젝트 seq를 얻어야 하므로 먼저 한번 저장.
            await db.SaveChangesAsync();

            //프로젝트 AM
            if (model.amList != null)
              foreach (var am in model.amList)
              {
                am.p_seq = model.data.p_seq;
                db.Entry(am).State = EntityState.Added;
              }

            //프로젝트 Searcher
            if (model.searcherList != null)
              foreach (var searcher in model.searcherList)
              {
                searcher.p_seq = model.data.p_seq;
                db.Entry(searcher).State = EntityState.Added;
              }

            //프로젝트 산업 정보
            //if (model.busiList != null)
            //    foreach (var busi in model.busiList)
            //    {
            //        busi.p_seq = model.data.p_seq;
            //        db.Entry(busi).State = EntityState.Added;
            //    }

            //프로젝트 직무 정보
            //if (model.jobList != null)
            //    foreach (var job in model.jobList)
            //    {
            //        job.p_seq = model.data.p_seq;
            //        db.Entry(job).State = EntityState.Added;
            //    }

            //프로젝트 희망 근무지
            if (model.placeList != null)
              foreach (var place in model.placeList)
              {
                place.p_seq = model.data.p_seq;
                db.Entry(place).State = EntityState.Added;
              }

            //프로젝트 키워드 정보
            if (model.keywordList != null)
              foreach (var keyword in model.keywordList)
              {
                keyword.p_seq = model.data.p_seq;
                db.Entry(keyword).State = EntityState.Added;
              }

            //저장
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 업데이트
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task UpdateProjectAsync(project project, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        db.Entry(project).State = EntityState.Modified;
        ProjectLogRepository log = new ProjectLogRepository();
        await log.project_log(project, "U", event_uv_seq, 1);
        //저장
        await db.SaveChangesAsync();

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 - 평판조회 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task CreateOrUpdateProjectRep(ProjectRepCreateUpdateModel model, string state, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              ProjectLogRepository log = new ProjectLogRepository();
              //수정건 삭제 먼저.
              //기존 AM 정보 삭제
              if (model.deleteAmList.Count > 0)
              {
                foreach (var dAm in model.deleteAmList)
                {
                  db.Entry(dAm).State = EntityState.Deleted;
                }
                await log.pjt_director_log(model.deleteAmList, "D", event_uv_seq, 1);
              }


              //기존 Searcher 삭제.
              if (model.deleteSearcherList.Count > 0)
              {
                foreach (var dSearcher in model.deleteSearcherList)
                {
                  db.Entry(dSearcher).State = EntityState.Deleted;
                }

                await log.pjt_manager_log(model.deleteSearcherList, "D", event_uv_seq, 1);
              }


              //프로젝트 기본정보 수정.
              db.Entry(model.data).State = EntityState.Modified;

              await log.project_log(model.data, "U", event_uv_seq, 1);
            }
            else
            {
              //프로젝트 기본정보 신규 등록
              db.Entry(model.data).State = EntityState.Added;
            }

            //프로젝트 seq를 얻어야 하므로 먼저 한번 저장.
            await db.SaveChangesAsync();

            //프로젝트 AM
            if (model.amList != null)
              foreach (var am in model.amList)
              {
                am.p_seq = model.data.p_seq;
                db.Entry(am).State = EntityState.Added;
              }

            //프로젝트 Searcher
            if (model.searcherList != null)
              foreach (var searcher in model.searcherList)
              {
                searcher.p_seq = model.data.p_seq;
                db.Entry(searcher).State = EntityState.Added;
              }

            //저장
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 Am 정보 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_director>> SelectPjtDirectorListAsync(int p_seq)
    {
      try
      {
        return await db.pjt_directors
                           .Where(x => x.p_seq == p_seq)
                           .ToListAsync();

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 Searcher 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_manager>> SelectPjtContactListAsync(int p_seq)
    {
      try
      {
        var list = await db.pjt_managers
                           .Where(x => x.p_seq == p_seq)
                           .ToListAsync();


        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 근무지 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_place>> SelectPjtPlaceListAsync(int p_seq)
    {
      try
      {
        return await db.pjt_places
                       .Where(x => x.p_seq == p_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 산업정보 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_business_code>> SelectPjtBusiCodeListAsync(int p_seq)
    {
      try
      {
        return await db.pjt_business_codes
                       .Where(x => x.p_seq == p_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 직무정보 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_job_code>> SelectPjtJobCodeListAsync(int p_seq)
    {
      try
      {
        return await db.pjt_job_codes
                       .Where(x => x.p_seq == p_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 키워드 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_keyword>> SelectPjtKeywordListAsync(int p_seq)
    {
      try
      {
        return await db.pjt_keywords
                       .Where(x => x.p_seq == p_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<pjt_recandidate>> SelectPjtRecandidateListAsync(int p_seq)
    {
      try
      {
        return await db.pjt_recandidates
                       .Where(x => x.p_seq == p_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 관심후보 단건 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<pjt_recandidate> SelectPjtRecandidateOneAsync(int p_seq, int c_seq)
    {
      try
      {
        return await db.pjt_recandidates
                       .Where(x => x.p_seq == p_seq && x.c_seq == c_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<pjt_recandidate> SelectPjtRecandidateOneAsync(int pic_seq)
    {
      try
      {
        return await db.pjt_recandidates
                       .Where(x => x.pic_seq == pic_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 관심후보 등록 - 후보자제안에서 등록.
    /// </summary>
    /// <param name="recandidate"></param>
    /// <param name="history"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task CreateOrUpdatePjtRecandidate(pjt_recandidate recandidate, pjt_recandidate_history history)
    {
      try
      {

        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            db.Entry(recandidate).State = EntityState.Added;

            //저장
            await db.SaveChangesAsync();

            history.pic_seq = recandidate.pic_seq;
            db.Entry(history).State = EntityState.Added;

            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 관심후보 등록 - 후보자제안에서 등록.
    /// </summary>
    /// <param name="recandidate"></param>
    /// <param name="history"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task UpdatePjtRecandidate(pjt_recandidate recandidate, List<pjt_recandidate_history> histories)
    {
      try
      {

        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            db.Entry(recandidate).State = EntityState.Modified;

            //저장
            await db.SaveChangesAsync();
            if(histories.Count > 0)
            {
              foreach(var history in histories)
              {
                db.Entry(history).State = EntityState.Modified;
              }
            }

            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 후보자 히스토리 단건 조회.
    /// </summary>
    /// <param name="prc_seq"></param>
    /// <returns></returns>
    public async Task<pjt_recandidate_history> SelectPjtRecHistory(int prc_seq)
    {
      try
      {
        return await db.pjt_recandidate_historys
                       .Where(x => x.prc_seq == prc_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<pjt_recandidate_history>> SelectPjtRecHisListAsync(int pic_seq)
    {
      try
      {
        return await db.pjt_recandidate_historys
                       .Where(x => x.pic_seq == pic_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrDeletePjtRecandidate(pjt_recandidate recan, string state = "D")
    {
      try
      {
        if (state == "D")
        {
          db.Entry(recan).State = EntityState.Deleted;
        }
        else if (state == "U")
        {
          db.Entry(recan).State = EntityState.Modified;
        }

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 히스토리 등록, 수정, 삭제
    /// </summary>
    /// <param name="history"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task CreateOrUpdatePjtRecHistory(pjt_recandidate_history history, schedule sch, List<schedule_attend> attendList, string state, int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            ProjectLogRepository log = new ProjectLogRepository();
            if (state == "U")
            {
              db.Entry(history).State = EntityState.Modified;
              await log.pjt_recandidate_history_log(history, state, event_uv_seq, event_idx);
            }
            else if (state == "D")
            {
              db.Entry(history).State = EntityState.Deleted;
              await log.pjt_recandidate_history_log(history, state, event_uv_seq, event_idx);

              if (sch != null)
              {
                db.Entry(sch).State = EntityState.Deleted;
                await log.schedule_log(sch, "D", event_uv_seq, event_idx);
              }


              if (attendList != null)
              {
                foreach (var attend in attendList)
                  db.Entry(attend).State = EntityState.Deleted;

                await log.schedule_attend_log(attendList, "D", event_uv_seq, event_idx);
              }

            }
            else
              db.Entry(history).State = EntityState.Added;

            //프로젝트 seq를 얻어야 하므로 먼저 한번 저장.
            await db.SaveChangesAsync();




            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateMakeupRequest(makeup_request request, pjt_recandidate_history history, string state)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            ProjectLogRepository log = new ProjectLogRepository();
            if (state == "U")
            {
              if (history != null)
                db.Entry(history).State = EntityState.Modified;

              db.Entry(request).State = EntityState.Modified;


            }
            else
            {
              if (history != null)
                db.Entry(request).State = EntityState.Added;

              db.Entry(history).State = EntityState.Added;

            }


            //프로젝트 seq를 얻어야 하므로 먼저 한번 저장.
            await db.SaveChangesAsync();




            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 인보이스 단건 조회.
    /// </summary>
    /// <param name="pii_seq"></param>
    /// <returns></returns>
    public async Task<pjt_invoice_info> SelectPjtInvoiceInfoOneAsnyc(int pii_seq)
    {
      try
      {
        return await db.pjt_invoice_infos
                       .Where(x => x.pii_seq == pii_seq && x.is_deleted != 1)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    /// <summary>
    /// 인보이스 info 업데이트
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public async Task UpdatePjtInvoice(pjt_invoice_info info, int event_uv_seq = 0)
    {
      try
      {
        db.Entry(info).State = EntityState.Modified;
        //저장
        ProjectLogRepository log = new ProjectLogRepository();
        await log.pjt_invoice_info_log(info, "U", event_uv_seq, 1);
        await db.SaveChangesAsync();

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 인보이스 생성
    /// </summary>
    /// <param name="info"></param>
    /// <param name="sales"></param>
    /// <returns></returns>
    public async Task CreatePjtInvoice(pjt_invoice_info info, List<pjt_invoice_sales> sales)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            db.Entry(info).State = EntityState.Added;

            //저장
            await db.SaveChangesAsync();

            foreach (var sale in sales)
            {
              sale.pii_seq = info.pii_seq;
              db.Entry(sale).State = EntityState.Added;
            }
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 메모 조회
    /// </summary>
    /// <param name="pm_seq"></param>
    /// <returns></returns>
    public async Task<pjt_memo> SelectPjtMemoOneAsync(int pm_seq)
    {
      try
      {
        return await db.pjt_memos
                       .Where(x => x.pm_seq == pm_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 메모 생성 또는 삭제
    /// </summary>
    /// <param name="pm"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrDeletePjtMemo(pjt_memo pm, string flag, int event_uv_seq = 0)
    {
      try
      {
        if (flag == "D")
          db.Entry(pm).State = EntityState.Deleted;
        else
          db.Entry(pm).State = EntityState.Added;

        ProjectLogRepository log = new ProjectLogRepository();
        await log.pjt_memo_log(pm, flag, event_uv_seq, 1);

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 관련파일 삭제
    /// </summary>
    /// <param name="history"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task CreateOrUpdatePjtFile(pjt_file file, string state)
    {
      try
      {
        if (state == "D")
        {
          db.Entry(file).State = EntityState.Deleted;
        }
        else
        {
          db.Entry(file).State = EntityState.Added;
        }

        //프로젝트 seq를 얻어야 하므로 먼저 한번 저장.
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<pjt_file> SelectPjtFileOneAsync(int pf_seq)
    {
      try
      {
        return await db.pjt_files
                       .Where(x => x.pf_seq == pf_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task DeletePjtFile(pjt_file pf)
    {
      try
      {
        
        db.Entry(pf).State = EntityState.Modified;
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 코멘트 단건 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="psb_seq"></param>
    /// <returns></returns>
    public async Task<pjt_share_board> SelectProjectBoardOneAsync(int p_seq, int psb_seq)
    {
      try
      {
        return await db.pjt_share_boards
                       .Where(x => x.p_seq == p_seq && x.psb_seq == psb_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 공유 코멘트 댓글 단건 조회. 
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="psb_seq"></param>
    /// <param name="psr_seq"></param>
    /// <returns></returns>
    public async Task<pjt_share_reply> SelectProjectBoardReplyOneAsync(int p_seq, int psb_seq, int psr_seq)
    {
      try
      {
        return await db.pjt_share_replys
                       .Where(x => x.p_seq == p_seq && x.psb_seq == psb_seq && x.psr_seq == psr_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 공유 코멘트 댓글 리스트 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="psb_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_share_reply>> SelectProjectBoardReplyListAsync(int p_seq, int psb_seq)
    {
      try
      {
        return await db.pjt_share_replys
                       .Where(x => x.p_seq == p_seq && x.psb_seq == psb_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 코멘트 등록, 수정
    /// </summary>
    /// <param name="board"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task CreateOrUpdatePjtBoard(pjt_share_board board, string state, int event_uv_seq = 0)
    {
      try
      {

        if (state == "U")
          db.Entry(board).State = EntityState.Modified;

        else
          db.Entry(board).State = EntityState.Added;

        ProjectLogRepository log = new ProjectLogRepository();
        await log.pjt_share_board_log(board, state, event_uv_seq, 1);
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 코멘트 삭제.
    /// </summary>
    /// <param name="board"></param>
    /// <param name="replyList"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task DeletePjtBoard(pjt_share_board board, List<pjt_share_reply> replyList, int event_uv_seq = 0)
    {
      try
      {
        ProjectLogRepository log = new ProjectLogRepository();
        db.Entry(board).State = EntityState.Deleted;
        await log.pjt_share_board_log(board, "D", event_uv_seq, 1);
        if (replyList.Count > 0)
        {
          foreach (var reply in replyList)
          {
            db.Entry(reply).State = EntityState.Deleted;
          }

          await log.pjt_share_reply_log(replyList, "D", event_uv_seq, 1);

        }


        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 코멘트 댓글 등록, 수정.
    /// </summary>
    /// <param name="reply"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task CreateOrUpdatePjtBoardReply(pjt_share_reply reply, string state, int event_uv_seq = 0)
    {
      try
      {
        ProjectLogRepository log = new ProjectLogRepository();
        if (state == "U")
          db.Entry(reply).State = EntityState.Modified;

        else
          db.Entry(reply).State = EntityState.Added;

        await log.pjt_share_reply_log(reply, state, event_uv_seq, 1);
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task DeletePjtBoardReply(pjt_share_reply reply, int event_uv_seq = 0)
    {
      try
      {
        db.Entry(reply).State = EntityState.Deleted;
        ProjectLogRepository log = new ProjectLogRepository();
        await log.pjt_share_reply_log(reply, "D", event_uv_seq, 1);

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 전담코웤 프로젝트 알람 생성
    /// </summary>
    /// <param name="project"></param>
    /// <param name="aMessage"></param>
    /// <param name="aList"></param>
    /// <returns></returns>
    public async Task CreateCoWorkProjectAlarm(project project, alarm_message aMessage, List<alarm_user> aList)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            db.Entry(project).State = EntityState.Modified;

            db.Entry(aMessage).State = EntityState.Added;

            //저장
            await db.SaveChangesAsync();


            foreach (var aUser in aList)
            {
              aUser.am_seq = aMessage.am_seq;
              db.Entry(aUser).State = EntityState.Added;
            }
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<project_read_history> SelectProjectReadOneAsync(int p_seq, int uv_seq)
    {
      try
      {
        return await db.project_read_historys
                       .Where(x => (x.p_seq == p_seq && x.read_user == uv_seq))
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateProjectRead(project_read_history model, string state)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              db.Entry(model).State = EntityState.Modified;
            }
            else
            {
              db.Entry(model).State = EntityState.Added;
            }

            //저장
            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }
}
