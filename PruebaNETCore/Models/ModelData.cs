using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;

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

    public virtual DbSet<Bitacora> Bitacoras { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Conductore> Conductores { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Fichamantencion> Fichamantencions { get; set; }

    public virtual DbSet<Kilometraje> Kilometrajes { get; set; }

    public virtual DbSet<Periodosmantenimiento> Periodosmantenimientos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesPermiso> RolesPermisos { get; set; }

    public virtual DbSet<Solicitude> Solicitudes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Usuariosrole> Usuariosroles { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=proyectosolve;uid=root;password=52368993Nc", ServerVersion.Parse("10.4.28-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Aprobacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aprobaciones", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdSolicitud, "fk_aprobaciones_solicitudes");

            entity.HasIndex(e => e.IdJefe, "fk_aprobaciones_usuarios");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasComment("TRIAL")
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha");
            entity.Property(e => e.IdJefe)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_jefe");
            entity.Property(e => e.IdSolicitud)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_solicitud");
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("motivo");
            entity.Property(e => e.Trial816)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial816");

            entity.HasOne(d => d.IdJefeNavigation).WithMany(p => p.Aprobaciones)
                .HasForeignKey(d => d.IdJefe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aprobaciones_usuarios");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Aprobaciones)
                .HasForeignKey(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aprobaciones_solicitudes");
        });

        modelBuilder.Entity<Bitacora>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bitacora", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdConductor, "fk_bitacora_conductores");

            entity.HasIndex(e => e.IdKilometraje, "fk_bitacora_kilometrajes");

            entity.HasIndex(e => e.IdSolicitud, "fk_bitacora_solicitudes");

            entity.HasIndex(e => e.IdVehiculo, "fk_bitacora_vehiculos");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("bigint(20)")
                .HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasComment("TRIAL")
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha");
            entity.Property(e => e.Folio)
                .HasComment("TRIAL")
                .HasColumnType("bigint(20)")
                .HasColumnName("folio");
            entity.Property(e => e.IdConductor)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdKilometraje)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_kilometraje");
            entity.Property(e => e.IdSolicitud)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_solicitud");
            entity.Property(e => e.IdVehiculo)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.LitrosCombustible)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("litros_combustible");
            entity.Property(e => e.Observacion)
                .IsRequired()
                .HasMaxLength(250)
                .HasComment("TRIAL")
                .HasColumnName("observacion");
            entity.Property(e => e.Trial820)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial820");

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdConductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bitacora_conductores");

            entity.HasOne(d => d.IdKilometrajeNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdKilometraje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bitacora_kilometrajes");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bitacora_solicitudes");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bitacora_vehiculos");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categorias", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Categoria1)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("categoria");
            entity.Property(e => e.Trial820)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial820");
        });

        modelBuilder.Entity<Conductore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("conductores", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdUsuario, "fk_conductores_usuarios");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Eliminado)
                .HasComment("TRIAL")
                .HasColumnName("eliminado");
            entity.Property(e => e.Estado)
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEmision)
                .HasComment("TRIAL")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.FechaVencimiento)
                .HasComment("TRIAL")
                .HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_usuario");
            entity.Property(e => e.NumeroPoliza)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("numero_poliza");
            entity.Property(e => e.Trial820)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial820");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Conductores)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_conductores_usuarios");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departamentos", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Departamento1)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("departamento");
            entity.Property(e => e.Trial820)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial820");
        });

        modelBuilder.Entity<Fichamantencion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fichamantencion", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdConductor, "fk_fichamantencion_conductores");

            entity.HasIndex(e => e.IdVehiculo, "fk_fichamantencion_vehiculos");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaMantencion)
                .HasComment("TRIAL")
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_mantencion");
            entity.Property(e => e.IdConductor)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdVehiculo)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.Kilometraje)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("kilometraje");
            entity.Property(e => e.Trial823)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial823");

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.Fichamantencions)
                .HasForeignKey(d => d.IdConductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fichamantencion_conductores");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Fichamantencions)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fichamantencion_vehiculos");
        });

        modelBuilder.Entity<Kilometraje>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("kilometrajes", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdVehiculo, "fk_kilometrajes_vehiculos");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FechaCreacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdVehiculo)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.KilometrajeFinal)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("kilometraje_final");
            entity.Property(e => e.KilometrajeInicial)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("kilometraje_inicial");
            entity.Property(e => e.Trial823)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial823");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Kilometrajes)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_kilometrajes_vehiculos");
        });

        modelBuilder.Entity<Periodosmantenimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("periodosmantenimiento", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.PeriodoKilometraje)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("periodo_kilometraje");
            entity.Property(e => e.Trial823)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial823");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permisos", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Permiso1)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("permiso");
            entity.Property(e => e.Trial823)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial823");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Rol)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("rol");
            entity.Property(e => e.Trial823)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial823");
        });

        modelBuilder.Entity<RolesPermiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles_permisos", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdRol, "fk_roles_permisos_roles");

            entity.HasIndex(e => e.IdPermiso, "fk_usuarios_permisos_permisos");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.IdPermiso)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_permiso");
            entity.Property(e => e.IdRol)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_rol");
            entity.Property(e => e.Trial826)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial826");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuarios_permisos_permisos");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_roles_permisos_roles");
        });

        modelBuilder.Entity<Solicitude>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("solicitudes", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdConductor, "fk_solicitudes_conductores");

            entity.HasIndex(e => e.IdSolicitante, "fk_solicitudes_usuarios");

            entity.HasIndex(e => e.IdVehiculo, "fk_solicitudes_vehiculos");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Destino)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("destino");
            entity.Property(e => e.Estado)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("estado");
            entity.Property(e => e.FechaLlegada)
                .HasComment("TRIAL")
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_llegada");
            entity.Property(e => e.FechaSalida)
                .HasComment("TRIAL")
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_salida");
            entity.Property(e => e.FechaSolicitado)
                .HasComment("TRIAL")
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_solicitado");
            entity.Property(e => e.IdConductor)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdSolicitante)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_solicitante");
            entity.Property(e => e.IdVehiculo)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.Motivo)
                .HasMaxLength(256)
                .HasComment("TRIAL")
                .HasColumnName("motivo");
            entity.Property(e => e.NumeroPasajeros)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("numero_pasajeros");
            entity.Property(e => e.Pasajeros)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("pasajeros");
            entity.Property(e => e.Trial826)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial826");

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.IdConductor)
                .HasConstraintName("fk_solicitudes_conductores");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.IdSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_solicitudes_usuarios");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_solicitudes_vehiculos");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuarios", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdDepartamento, "fk_usuarios_departamentos");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Apellido)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("apellido");
            entity.Property(e => e.Clave)
                .IsRequired()
                .HasMaxLength(256)
                .HasComment("TRIAL")
                .HasColumnName("clave");
            entity.Property(e => e.DireccionImg)
                .HasMaxLength(256)
                .HasComment("TRIAL")
                .HasColumnName("direccion_img");
            entity.Property(e => e.Eliminado)
                .HasComment("TRIAL")
                .HasColumnName("eliminado");
            entity.Property(e => e.IdDepartamento)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_departamento");
            entity.Property(e => e.Login)
                .HasComment("TRIAL")
                .HasColumnName("login");
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Rut)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("rut");
            entity.Property(e => e.Trial826)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial826");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdDepartamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuarios_departamentos");
        });

        modelBuilder.Entity<Usuariosrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuariosroles", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.Idrol, "fk_usuariosroles_roles");

            entity.HasIndex(e => e.Idusuario, "fk_usuariosroles_usuarios");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Idrol)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("idrol");
            entity.Property(e => e.Idusuario)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("idusuario");
            entity.Property(e => e.Trial829)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial829");

            entity.HasOne(d => d.IdrolNavigation).WithMany(p => p.Usuariosroles)
                .HasForeignKey(d => d.Idrol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuariosroles_roles");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Usuariosroles)
                .HasForeignKey(d => d.Idusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuariosroles_usuarios");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("vehiculos", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdCategoria, "fk_vehiculos_categorias");

            entity.HasIndex(e => e.IdConductor, "fk_vehiculos_conductores");

            entity.HasIndex(e => e.IdPeriodoKilometraje, "fk_vehiculos_periodosmantenimiento");

            entity.Property(e => e.Id)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.DireccionImg)
                .HasMaxLength(256)
                .HasComment("TRIAL")
                .HasColumnName("direccion_img");
            entity.Property(e => e.Eliminado)
                .HasComment("TRIAL")
                .HasColumnName("eliminado");
            entity.Property(e => e.Estado)
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.IdCategoria)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_categoria");
            entity.Property(e => e.IdConductor)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdPeriodoKilometraje)
                .HasComment("TRIAL")
                .HasColumnType("int(11)")
                .HasColumnName("id_periodo_kilometraje");
            entity.Property(e => e.Marca)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("modelo");
            entity.Property(e => e.Patente)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("patente");
            entity.Property(e => e.Trial829)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial829");
            entity.Property(e => e.Year)
                .IsRequired()
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("year");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("fk_vehiculos_categorias");

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.IdConductor)
                .HasConstraintName("fk_vehiculos_conductores");

            entity.HasOne(d => d.IdPeriodoKilometrajeNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.IdPeriodoKilometraje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vehiculos_periodosmantenimiento");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
