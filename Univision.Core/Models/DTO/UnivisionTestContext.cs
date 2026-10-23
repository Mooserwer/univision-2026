using System.Data.Entity;

namespace Univision.Core.Models.DTO
{
  /// <summary>
  /// [테스트] PrivacyCreateTest 플로우 전용 컨텍스트.
  /// 운영 컨텍스트와 동일하되 개인정보 동의/프로필 메모 엔티티만 *_test 테이블로 매핑.
  /// (Fluent API ToTable 이 [Table] 어노테이션보다 우선)
  /// 리포지토리에 생성자로 넘겨 사용: new NPrivacyEntityRepository(new UnivisionTestContext())
  /// </summary>
  public class UnivisionTestContext : UnivisionContext
  {
    static UnivisionTestContext()
    {
      Database.SetInitializer<UnivisionTestContext>(null);   // 스키마 검사/생성 안 함
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<n_privacy_agree>().ToTable("n_privacy_agree_test");
      modelBuilder.Entity<n_privacy_agree_dtl>().ToTable("n_privacy_agree_dtl_test");
      modelBuilder.Entity<profile_memo>().ToTable("R_PROFILE_MEMO_TEST");
    }
  }
}
