using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProyectoSolveCore.Models;

public partial class ModelData : DbContext
{
    public ModelData()
    {
    }

    public ModelData(DbContextOptions<ModelData> options)
        : base(options)
    {
    }

    public virtual DbSet<Aprobacione> Aprobaciones { get; set; }

    public virtual DbSet<AspNetSessionState> AspNetSessionStates { get; set; }

    public virtual DbSet<Bitacora> Bitacoras { get; set; }

    public virtual DbSet<Conductore> Conductores { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<FichaMantencion> FichaMantencions { get; set; }

    public virtual DbSet<Kilometraje> Kilometrajes { get; set; }

    public virtual DbSet<PeriodosMantenimiento> PeriodosMantenimientos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesPermiso> RolesPermisos { get; set; }

    public virtual DbSet<Solicitude> Solicitudes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosRole> UsuariosRoles { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-QAAO9ODD;Database=ProyectoSolve;Trusted_Connection=True;TrustServerCertificate=True;User Id=UserSystem;Password=52368993Nc;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aprobacione>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdJefe).HasColumnName("id_jefe");
            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .HasColumnName("motivo");

            entity.HasOne(d => d.IdJefeNavigation).WithMany(p => p.Aprobaciones)
                .HasForeignKey(d => d.IdJefe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Aprobaciones_Usuarios");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Aprobaciones)
                .HasForeignKey(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Aprobaciones_Solicitudes");
        });

        modelBuilder.Entity<AspNetSessionState>(entity =>
        {
            entity.HasKey(e => e.SessionId);

            entity.ToTable("AspNetSessionState");

            entity.Property(e => e.SessionId).HasMaxLength(88);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Expires).HasColumnType("datetime");
            entity.Property(e => e.LockDate).HasColumnType("datetime");
            entity.Property(e => e.LockDateLocal).HasColumnType("datetime");
            entity.Property(e => e.SessionItemLong).HasColumnType("image");
        });

        modelBuilder.Entity<Bitacora>(entity =>
        {
            entity.ToTable("Bitacora");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.LitrosCombustible).HasColumnName("litros_combustible");
            entity.Property(e => e.Observacion).HasColumnName("observacion");
            entity.Property(e => e.Folio).HasColumnName("folio");
            entity.Property(e => e.IdConductor).HasColumnName("id_conductor");
            entity.Property(e => e.IdKilometraje).HasColumnName("id_kilometraje");
            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdConductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bitacora_Conductores");

            entity.HasOne(d => d.IdKilometrajeNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdKilometraje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bitacora_Kilometrajes");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bitacora_Solicitudes");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bitacora_Vehiculos");
        });

        modelBuilder.Entity<Conductore>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Eliminado).HasColumnName("eliminado");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaEmision)
                .HasColumnType("date")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("date")
                .HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NumeroPoliza).HasColumnName("numero_poliza");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Conductores)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conductores_Usuarios");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Departamento1)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("departamento");
        });

        modelBuilder.Entity<FichaMantencion>(entity =>
        {
            entity.ToTable("FichaMantencion");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FechaMantencion)
                .HasColumnName("fecha_mantencion")
                .HasColumnType("datetime");
            entity.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdConductor).HasColumnName("id_conductor");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Kilometraje)
                .HasColumnName("kilometraje");

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.FichaMantencions)
                .HasForeignKey(d => d.IdConductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FichaMantencion_Conductores");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.FichaMantencions)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FichaMantencion_Vehiculos");
        });

        modelBuilder.Entity<Kilometraje>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.KilometrajeFinal)
                .HasColumnName("kilometraje_final");
            entity.Property(e => e.KilometrajeInicial)
                .HasColumnName("kilometraje_inicial");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Kilometrajes)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kilometrajes_Vehiculos");
        });

        modelBuilder.Entity<PeriodosMantenimiento>(entity =>
        {
            entity.ToTable("PeriodosMantenimiento");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PeriodoKilometraje)
                .HasColumnName("periodo_kilometraje");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Permiso1)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("permiso");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Rol)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<RolesPermiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Usuarios_permisos");

            entity.ToTable("Roles_Permisos");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Permisos_Permisos");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Roles_Permisos_Roles");
        });

        modelBuilder.Entity<Solicitude>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Destino)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("destino");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaLlegada)
                .HasColumnType("datetime")
                .HasColumnName("fecha_llegada");
            entity.Property(e => e.FechaSalida)
                .HasColumnType("datetime")
                .HasColumnName("fecha_salida");
            entity.Property(e => e.FechaSolicitado)
                .HasColumnType("datetime")
                .HasColumnName("fecha_solicitado");
            entity.Property(e => e.IdConductor).HasColumnName("id_conductor");
            entity.Property(e => e.IdSolicitante).HasColumnName("id_solicitante");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Motivo)
                .HasMaxLength(256)
                .HasColumnName("motivo");
            entity.Property(e => e.NumeroPasajeros).HasColumnName("numero_pasajeros");
            entity.Property(e => e.Pasajeros)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("pasajeros");

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.IdConductor)
                .HasConstraintName("FK_Solicitudes_Conductores");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.IdSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitudes_Usuarios");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitudes_Vehiculos");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Apellido)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Clave)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnName("clave");
            entity.Property(e => e.DireccionImg)
                .HasMaxLength(256)
                .HasColumnName("direccion_img");
            entity.Property(e => e.Eliminado).HasColumnName("eliminado");
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Rut)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("rut");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdDepartamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Departamentos");
        });

        modelBuilder.Entity<UsuariosRole>(entity =>
        {
            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.UsuariosRoles)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuariosRoles_Roles");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuariosRoles)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuariosRoles_Usuarios");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DireccionImg)
                .HasMaxLength(256)
                .HasColumnName("direccion_img");
            entity.Property(e => e.Eliminado).HasColumnName("eliminado");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdPeriodoKilometraje).HasColumnName("id_periodo_kilometraje");
            entity.Property(e => e.Marca)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("modelo");
            entity.Property(e => e.Patente)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("patente");

            entity.HasOne(d => d.IdPeriodoKilometrajeNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.IdPeriodoKilometraje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculos_PeriodosMantenimiento");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
