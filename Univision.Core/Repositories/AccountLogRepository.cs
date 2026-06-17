using Dapper;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Repositories
{
    public class AccountLogRepository : BaseRepository
    {
        public AccountLogRepository(UnivisionContext db) : base(db)
        {

        }
        public AccountLogRepository()
        {

        }

        public async Task client_contract_log_fee_type(int cc_seq, int idx = 1, string event_type = "", int event_user = 0)
        {
            try
            {
                SqlParameter event_idx = new SqlParameter("@event_idx", idx);
                SqlParameter param_event_type = new SqlParameter("@event_type", event_type);
                SqlParameter event_uv_seq = new SqlParameter("@event_uv_seq", event_user);
                SqlParameter db_cc_seq = new SqlParameter("@cc_seq", cc_seq);


                await db.Database.ExecuteSqlCommandAsync("exec sp_client_contract_log_fee_type", event_idx, param_event_type, event_uv_seq
                    , db_cc_seq);

            }
            catch //(Exception e)
            {
              
            }
        }

        public async Task uv_user_log(uv_user user, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
        {
            try
            {
               

                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@event_idx", event_idx),
                    new SqlParameter("@event_type", event_type),
                    new SqlParameter("@event_uv_seq", event_uv_seq),
                    new SqlParameter("@uv_seq", user.uv_seq),
                    new SqlParameter("@ud_seq", user.ud_seq),
                    new SqlParameter("@ua_seq", user.ua_seq),
                    new SqlParameter("@user_id", user.user_id),
                    new SqlParameter("@pwd", user.pwd),
                    new SqlParameter("@rank_name", user.rank_name),
                    new SqlParameter("@email", user.email),
                    new SqlParameter("@tel", user.tel),
                    new SqlParameter("@hp", user.hp),
                    new SqlParameter("@name", user.name),
                    new SqlParameter("@create_dt", user.create_dt),
                    new SqlParameter("@modify_dt", user.modify_dt),
                    new SqlParameter("@img_dir", user.img_dir),
                    new SqlParameter("@img_origin_path", user.img_origin_path),
                    new SqlParameter("@img_path", user.img_path),

                };

                await db.Database.ExecuteSqlCommandAsync("exec sp_uv_user_log  @event_idx, @event_type, @event_uv_seq	 @uv_seq , @ud_seq , @ua_seq , @user_id , @pwd , @rank_name , @email , @tel , @hp , @name , @img_dir , @img_origin_path , @img_path , @create_dt , @modify_dt , @entry_date , @retire_date  ", parameters);

            }
            catch (Exception)
            {

            }
        }
    }
}
