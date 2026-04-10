


using Microsoft.EntityFrameworkCore;
using Rules_Engine_API.Models;

namespace Rules_Engine_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<EvaluationSession> EvaluationSessions { get; set; }
    public DbSet<AttackEvaluationRecord> AttackEvaluationRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ─── EvaluationSession ────────────────────────────────────
        modelBuilder.Entity<EvaluationSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Payload).IsRequired();
            entity.Property(e => e.SourceIp).HasMaxLength(45);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.Property(e => e.DetectedAttackType).HasMaxLength(500);
            entity.Property(e => e.MatchedAttacksJson).HasColumnType("nvarchar(max)");

            // Indexes
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.AssignedTo);
        });

        // ─── AttackEvaluationRecord ───────────────────────────────
        modelBuilder.Entity<AttackEvaluationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AttackName).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Pattern).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

            // One session → many attack evaluations
            entity.HasOne(e => e.EvaluationSession)
                  .WithMany(s => s.AttackEvaluations)
                  .HasForeignKey(e => e.EvaluationSessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.EvaluationSessionId);
            entity.HasIndex(e => e.AttackName);
            entity.HasIndex(e => new { e.EvaluationSessionId, e.AttackName });
        });
    }
}