﻿using Microsoft.EntityFrameworkCore;

namespace ProyectoSolveCore.Models;

/// <summary>
/// Representa el contexto de base de datos para el modelo de datos.
/// </summary>
public partial class ModelData : DbContext
{
    public ModelData()
    {
    }
    /// <summary>
    /// Constructor de la clase ModelData que acepta opciones de DbContext.
    /// </summary>
    public ModelData(DbContextOptions<ModelData> options)
        : base(options)
    {
    }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de aprobaciones.
    /// </summary>
    public virtual DbSet<Aprobacion> Aprobaciones { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de bitacoras.
    /// </summary>
    public virtual DbSet<Bitacora> Bitacoras { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de categorias.
    /// </summary>
    public virtual DbSet<Categoria> Categorias { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de conductores.
    /// </summary>
    public virtual DbSet<Conductor> Conductores { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de departamentos.
    /// </summary>
    public virtual DbSet<Departamento> Departamentos { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de fichamantenciones.
    /// </summary>
    public virtual DbSet<Fichamantencion> Fichamantencions { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de kilometrajes.
    /// </summary>
    public virtual DbSet<Kilometraje> Kilometrajes { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de periodosmantenimientos.
    /// </summary>
    public virtual DbSet<Periodosmantenimiento> Periodosmantenimientos { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de permisos.
    /// </summary>
    public virtual DbSet<Permiso> Permisos { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de roles.
    /// </summary>
    public virtual DbSet<Role> Roles { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de rolespermisos.
    /// </summary>
    public virtual DbSet<RolesPermiso> RolesPermisos { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de solicitudes.
    /// </summary>
    public virtual DbSet<Solicitud> Solicitudes { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de usuarios.
    /// </summary>
    public virtual DbSet<Usuario> Usuarios { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de usuariosroles.
    /// </summary>
    public virtual DbSet<Usuariosrole> Usuariosroles { get; set; }
    /// <summary>
    /// Obtiene o establece el conjunto de entidades para la tabla de vehículos.
    /// </summary>
    public virtual DbSet<Vehiculo> Vehiculos { get; set; }
    //Configura el entorno de la base de datos. Le asignamos la cadena de conexión
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=proyectosolve;uid=UserSystem;password=52368993Nc", 
            ServerVersion.Parse("10.4.28-mariadb"));
    //Configura el modelo en base a las tablas de la base de datos 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Aprobacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aprobaciones");

            entity.HasIndex(e => e.IdSolicitud, "fk_aprobaciones_solicitudes");

            entity.HasIndex(e => e.IdJefe, "fk_aprobaciones_usuarios");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha");
            entity.Property(e => e.IdJefe)
                .HasColumnType("int(11)")
                .HasColumnName("id_jefe");
            entity.Property(e => e.IdSolicitud)
                .HasColumnType("int(11)")
                .HasColumnName("id_solicitud");
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .HasColumnName("motivo");

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

            entity.ToTable("bitacora");

            entity.HasIndex(e => e.IdConductor, "fk_bitacora_conductores");

            entity.HasIndex(e => e.IdKilometraje, "fk_bitacora_kilometrajes");

            entity.HasIndex(e => e.IdSolicitud, "fk_bitacora_solicitudes");

            entity.HasIndex(e => e.IdVehiculo, "fk_bitacora_vehiculos");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20)")
                .HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha");
            entity.Property(e => e.Folio)
                .HasColumnType("bigint(20)")
                .HasColumnName("folio");
            entity.Property(e => e.IdConductor)
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdKilometraje)
                .HasColumnType("int(11)")
                .HasColumnName("id_kilometraje");
            entity.Property(e => e.IdSolicitud)
                .HasColumnType("int(11)")
                .HasColumnName("id_solicitud");
            entity.Property(e => e.IdVehiculo)
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.LitrosCombustible)
                .HasColumnType("int(11)")
                .HasColumnName("litros_combustible");
            entity.Property(e => e.Observacion)
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnName("observacion");

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

            entity.ToTable("categorias");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Categoria1)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("categoria");
        });

        modelBuilder.Entity<Conductor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("conductores");

            entity.HasIndex(e => e.IdUsuario, "fk_conductores_usuarios");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Eliminado)
                .HasColumnName("eliminado");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.FechaEmision)
                .HasColumnName("fecha_emision");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_usuario");
            entity.Property(e => e.NumeroPoliza)
                .HasColumnType("int(11)")
                .HasColumnName("numero_poliza");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Conductores)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_conductores_usuarios");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departamentos");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Departamento1)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("departamento");
        });

        modelBuilder.Entity<Fichamantencion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fichamantencion");

            entity.HasIndex(e => e.IdConductor, "fk_fichamantencion_conductores");

            entity.HasIndex(e => e.IdVehiculo, "fk_fichamantencion_vehiculos");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaMantencion)
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_mantencion");
            entity.Property(e => e.IdConductor)
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdVehiculo)
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.Kilometraje)
                .HasColumnType("int(11)")
                .HasColumnName("kilometraje");

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

            entity.ToTable("kilometrajes");

            entity.HasIndex(e => e.IdVehiculo, "fk_kilometrajes_vehiculos");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdVehiculo)
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.KilometrajeFinal)
                .HasColumnType("int(11)")
                .HasColumnName("kilometraje_final");
            entity.Property(e => e.KilometrajeInicial)
                .HasColumnType("int(11)")
                .HasColumnName("kilometraje_inicial");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Kilometrajes)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_kilometrajes_vehiculos");
        });

        modelBuilder.Entity<Periodosmantenimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("periodosmantenimiento");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.PeriodoKilometraje)
                .HasColumnType("int(11)")
                .HasColumnName("periodo_kilometraje");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permisos");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Permiso1)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("permiso");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Rol)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<RolesPermiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles_permisos");

            entity.HasIndex(e => e.IdRol, "fk_roles_permisos_roles");

            entity.HasIndex(e => e.IdPermiso, "fk_usuarios_permisos_permisos");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.IdPermiso)
                .HasColumnType("int(11)")
                .HasColumnName("id_permiso");
            entity.Property(e => e.IdRol)
                .HasColumnType("int(11)")
                .HasColumnName("id_rol");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuarios_permisos_permisos");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_roles_permisos_roles");
        });

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("solicitudes");

            entity.HasIndex(e => e.IdConductor, "fk_solicitudes_conductores");

            entity.HasIndex(e => e.IdSolicitante, "fk_solicitudes_usuarios");

            entity.HasIndex(e => e.IdVehiculo, "fk_solicitudes_vehiculos");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Destino)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("destino");
            entity.Property(e => e.Estado)
                .HasColumnType("int(11)")
                .HasColumnName("estado");
            entity.Property(e => e.FechaLlegada)
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_llegada");
            entity.Property(e => e.FechaSalida)
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_salida");
            entity.Property(e => e.FechaSolicitado)
                .HasColumnType("datetime(3)")
                .HasColumnName("fecha_solicitado");
            entity.Property(e => e.IdConductor)
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdSolicitante)
                .HasColumnType("int(11)")
                .HasColumnName("id_solicitante");
            entity.Property(e => e.IdVehiculo)
                .HasColumnType("int(11)")
                .HasColumnName("id_vehiculo");
            entity.Property(e => e.Motivo)
                .HasMaxLength(256)
                .HasColumnName("motivo");
            entity.Property(e => e.NumeroPasajeros)
                .HasColumnType("int(11)")
                .HasColumnName("numero_pasajeros");
            entity.Property(e => e.Pasajeros)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("pasajeros");

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

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.IdDepartamento, "fk_usuarios_departamentos");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
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
            entity.Property(e => e.Eliminado)
                .HasColumnName("eliminado");
            entity.Property(e => e.IdDepartamento)
                .HasColumnType("int(11)")
                .HasColumnName("id_departamento");
            entity.Property(e => e.Login)
                .HasColumnName("login");
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
                .HasConstraintName("fk_usuarios_departamentos");
        });

        modelBuilder.Entity<Usuariosrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuariosroles");

            entity.HasIndex(e => e.Idrol, "fk_usuariosroles_roles");

            entity.HasIndex(e => e.Idusuario, "fk_usuariosroles_usuarios");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Idrol)
                .HasColumnType("int(11)")
                .HasColumnName("idrol");
            entity.Property(e => e.Idusuario)
                .HasColumnType("int(11)")
                .HasColumnName("idusuario");

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

            entity.ToTable("vehiculos");

            entity.HasIndex(e => e.IdCategoria, "fk_vehiculos_categorias");

            entity.HasIndex(e => e.IdConductor, "fk_vehiculos_conductores");

            entity.HasIndex(e => e.IdPeriodoKilometraje, "fk_vehiculos_periodosmantenimiento");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.DireccionImg)
                .HasMaxLength(256)
                .HasColumnName("direccion_img");
            entity.Property(e => e.Eliminado)
                .HasColumnName("eliminado");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.IdCategoria)
                .HasColumnType("int(11)")
                .HasColumnName("id_categoria");
            entity.Property(e => e.IdConductor)
                .HasColumnType("int(11)")
                .HasColumnName("id_conductor");
            entity.Property(e => e.IdPeriodoKilometraje)
                .HasColumnType("int(11)")
                .HasColumnName("id_periodo_kilometraje");
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
            entity.Property(e => e.Year)
                .IsRequired()
                .HasMaxLength(10)
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
