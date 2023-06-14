namespace ProyectoSolveCore.Models;

public partial class AspNetSessionState
{
    /// <summary>
    /// Identificador único de la sesión.
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// Fecha y hora en que se creó la sesión.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Fecha y hora en que expira la sesión.
    /// </summary>
    public DateTime Expires { get; set; }

    /// <summary>
    /// Fecha y hora en que se bloqueó la sesión.
    /// </summary>
    public DateTime LockDate { get; set; }

    /// <summary>
    /// Fecha y hora local en que se bloqueó la sesión.
    /// </summary>
    public DateTime LockDateLocal { get; set; }

    /// <summary>
    /// Cookie de bloqueo de la sesión.
    /// </summary>
    public int LockCookie { get; set; }

    /// <summary>
    /// Tiempo de espera de la sesión en minutos.
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// Indica si la sesión está bloqueada.
    /// </summary>
    public bool Locked { get; set; }

    /// <summary>
    /// Bytes que contiene los datos de sesión cortos.
    /// </summary>
    public byte[] SessionItemShort { get; set; }

    /// <summary>
    /// Bytes que contiene los datos de sesión largos.
    /// </summary>
    public byte[] SessionItemLong { get; set; }

    /// <summary>
    /// Indicadores de la sesión.
    /// </summary>
    public int Flags { get; set; }
}
