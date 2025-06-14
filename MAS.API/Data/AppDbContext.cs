using Microsoft.EntityFrameworkCore;
using MAS.API.Models.Entities;

namespace MAS.API.Data


{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options )
            : base( options ) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet <DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet <Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Appointment>()
                 .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Doctor>(d => d.UserId);


            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@clinic.com",
                    PasswordHash = "$2a$11$K.RmH4EMR9Y5L5RhNZ4pRuTqGCVn7rE4J4Ua4sGu9J7Fj0XF4TfHW", // hasło: admin123
                    FirstName = "Admin",
                    LastName = "System",
                    Role = "Admin",
                    CreatedAt = DateTime.Now,
                    IsEmailVerified = true

                },
                new User
                {

                    Id = 2,
                    Email = "dr.kowalski@clinic.com",
                    PasswordHash = "$2a$11$K.RmH4EMR9Y5L5RhNZ4pRuTqGCVn7rE4J4Ua4sGu9J7Fj0XF4TfHW", // hasło: admin123
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Role = "Doctor",
                    CreatedAt = DateTime.Now,
                    IsEmailVerified = true


                }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {

                    Id = 1,
                    UserId = 2,
                    Specialization = "Kardiolog",
                    LicenseNumber = "1234567"


                }
                );
        }
        
        



    }
}
