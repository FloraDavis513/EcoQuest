using Microsoft.EntityFrameworkCore;

namespace EcoQuest
{
    public partial class eco_questContext : DbContext
    {
        public eco_questContext()
        {
        }

        public eco_questContext(DbContextOptions<eco_questContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CmdaExec> CmdaExecs { get; set; } = null!;
        public virtual DbSet<FlywaySchemaHistory> FlywaySchemaHistories { get; set; } = null!;
        public virtual DbSet<GameBoard> GameBoards { get; set; } = null!;
        public virtual DbSet<GameBoardsQuestion> GameBoardsQuestions { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductsForBoard> ProductsForBoards { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                /*
                optionsBuilder.UseNpgsql("Server=localhost;Port=8082;Database=eco_quest;UID=postgres;PWD=qwerty123iop");
                */
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CmdaExec>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("cmda_exec");

                entity.Property(e => e.CmdaOutput).HasColumnName("cmda_output");
            });

            modelBuilder.Entity<FlywaySchemaHistory>(entity =>
            {
                entity.HasKey(e => e.InstalledRank)
                    .HasName("flyway_schema_history_pk");

                entity.ToTable("flyway_schema_history");

                entity.HasIndex(e => e.Success, "flyway_schema_history_s_idx");

                entity.Property(e => e.InstalledRank)
                    .ValueGeneratedNever()
                    .HasColumnName("installed_rank");

                entity.Property(e => e.Checksum).HasColumnName("checksum");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .HasColumnName("description");

                entity.Property(e => e.ExecutionTime).HasColumnName("execution_time");

                entity.Property(e => e.InstalledBy)
                    .HasMaxLength(100)
                    .HasColumnName("installed_by");

                entity.Property(e => e.InstalledOn)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("installed_on")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Script)
                    .HasMaxLength(1000)
                    .HasColumnName("script");

                entity.Property(e => e.Success).HasColumnName("success");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("type");

                entity.Property(e => e.Version)
                    .HasMaxLength(50)
                    .HasColumnName("version");
            });

            modelBuilder.Entity<GameBoard>(entity =>
            {
                entity.ToTable("game_board");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .HasColumnName("name");

                entity.Property(e => e.NumFields).HasColumnName("num_fields");
            });

            modelBuilder.Entity<GameBoardsQuestion>(entity =>
            {
                entity.HasKey(e => new { e.GameBoardId, e.QuestionId })
                    .HasName("game_boards_questions_pkey");

                entity.ToTable("game_boards_questions");

                entity.Property(e => e.GameBoardId).HasColumnName("game_board_id");

                entity.Property(e => e.QuestionId).HasColumnName("question_id");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Colour)
                    .HasColumnType("character varying")
                    .HasColumnName("colour");

                entity.Property(e => e.Name)
                    .HasColumnType("character varying")
                    .HasColumnName("name");

                entity.Property(e => e.Round).HasColumnName("round");
            });

            modelBuilder.Entity<ProductsForBoard>(entity =>
            {
                entity.HasKey(e => new { e.GameBoardId, e.ProductId })
                    .HasName("products_for_boards_pkey");

                entity.ToTable("products_for_boards");

                entity.Property(e => e.GameBoardId).HasColumnName("game_board_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.NumOfRepeating).HasColumnName("num_of_repeating");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("questions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Answer)
                    .HasColumnType("character varying")
                    .HasColumnName("answer");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ShortText)
                    .HasColumnType("character varying")
                    .HasColumnName("short_text");

                entity.Property(e => e.Text)
                    .HasColumnType("character varying")
                    .HasColumnName("text");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("fkdnt39hlm1bcye9ivenccipd5s");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Uuid)
                    .HasName("sessions_pkey");

                entity.ToTable("sessions");

                entity.Property(e => e.Uuid)
                    .ValueGeneratedNever()
                    .HasColumnName("uuid");

                entity.Property(e => e.IdCurrentQuestion)
                    .HasColumnType("bigint")
                    .HasColumnName("id_current_question");

                entity.Property(e => e.State)
                    .HasColumnType("character varying")
                    .HasColumnName("state");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}