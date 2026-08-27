using Microsoft.EntityFrameworkCore;
using backend.clinicalbackend.models;
namespace backend.clinicalbackend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorAvailability> DoctorAvailabilities =>
        Set<DoctorAvailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(user => user.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(user => user.RefreshTokens)
            .WithOne(refreshToken => refreshToken.User)
            .HasForeignKey(refreshToken => refreshToken.UserId);

        modelBuilder.Entity<Doctor>()
            .HasIndex(doctor => doctor.UserId)
            .IsUnique();

        modelBuilder.Entity<Doctor>()
            .HasIndex(doctor => doctor.DepartmentId);

        modelBuilder.Entity<Doctor>()
            .HasOne(doctor => doctor.User)
            .WithOne(user => user.Doctor)
            .HasForeignKey<Doctor>(doctor => doctor.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(doctor => doctor.Department)
            .WithMany(department => department.Doctors)
            .HasForeignKey(doctor => doctor.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DoctorAvailability>()
            .HasIndex(availability => new
            {
                availability.DoctorId,
                availability.StartTime
            });

        modelBuilder.Entity<DoctorAvailability>()
            .HasOne(availability => availability.Doctor)
            .WithMany(doctor => doctor.Availabilities)
            .HasForeignKey(availability => availability.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

