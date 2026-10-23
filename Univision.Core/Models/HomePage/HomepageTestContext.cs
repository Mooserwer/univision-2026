using System.Data.Entity;

namespace Univision.Core.Models.HomePage
{
  /// <summary>
  /// [테스트] PrivacyCreateTest 플로우 전용 홈페이지 컨텍스트.
  /// 신규 개인정보 동의 엔티티만 홈페이지 DB 의 *_test 테이블로 매핑.
  /// 리포지토리에 생성자로 넘겨 사용: new HomepageNPrivacyEntityRepository(new HomepageTestContext())
  /// </summary>
  public class HomepageTestContext : HomepageContext
  {
    static HomepageTestContext()
    {
      Database.SetInitializer<HomepageTestContext>(null);   // 스키마 검사/생성 안 함
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<home_n_privacy_agree>().ToTable("n_privacy_agree_test");
      modelBuilder.Entity<home_n_privacy_agree_dtl>().ToTable("n_privacy_agree_dtl_test");
    }
  }
}
