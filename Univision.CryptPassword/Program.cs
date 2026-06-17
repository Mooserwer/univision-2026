using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;

namespace Univision.CryptPassword
{
    class Program
    {
        /// <summary>
        /// 패스워드 암호화 프로세스
        /// SHA256 암호화 방식이며 
        /// 전체 패스워드를 암호화 하는 로직과
        /// 특정 유저만 암호화하는 로직이 분리되어 있으며
        /// 이미 암호화 된 패스워드는 두번 중복되지 않는다.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //패스워드 전체변경 로직
            // ChangePasswordAllUser();
            ///특정 유저만 비번업데이트
            ChangePasswordOneUser(184);
            //Test();
        }

        static void Test()
        {
            string txt = ";dfjklwerhklrjhqklwehkrqhwejfl;asjfl;asjfl;jasl;dfjasdl;fjasl;djafdl;sjfl;df";
            string result = ComputeSha256Hash(txt);
            Console.WriteLine(result);
            Console.WriteLine(result.Length);
            Console.ReadLine();                
        }


        /// <summary>
        /// 전체 유저 패스워드 변경
        /// </summary>
        static void ChangePasswordAllUser()
        {
            AccountEntityRepository re = new AccountEntityRepository();
            List<uv_user> list = re.ListUvUser();
            string org_pwd = string.Empty;
            string crypt_pwd = string.Empty;

            List<uv_user> listCrypt = new List<uv_user>();
            foreach(uv_user user in list)
            {
                org_pwd = string.Empty;
                crypt_pwd = string.Empty;
                if (user.pwd.Length != 64)
                {
                    org_pwd = user.pwd;
                    crypt_pwd = ComputeSha256Hash(org_pwd);
                    user.pwd = crypt_pwd;
                    listCrypt.Add(user);
                }
            }

            re.UpadateCryptPasswordByUserAll(listCrypt);
        }
        /// <summary>
        /// 특정 한명만 비번 업데이트
        /// </summary>
        /// <param name="user_seq">uv_user 테이블 uv_seq</param>
        static void ChangePasswordOneUser(int user_seq)
        {
            AccountEntityRepository re = new AccountEntityRepository();
            uv_user user = re.SelectUser(user_seq); 

            if(user != null && !string.IsNullOrWhiteSpace(user.pwd) && user.pwd.Length < 64)
            {
                string org_pwd = user.pwd;
                string crypt_pwd = ComputeSha256Hash(org_pwd);
                
                user.pwd = crypt_pwd;

                re.UpdateCryptPassword(user);
                
            }
        }
        /// <summary>
        /// 암호화
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
