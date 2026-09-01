namespace GestionTicketsCombustible.Domain.Entities;

// Controla la numeración consecutiva de tickets (RF-08).
// Una fila por año cuando el reinicio anual está activo; el servicio de
// generación (Fase 2.4) hace el incremento dentro de una transacción
// para evitar números duplicados bajo peticiones concurrentes.
public class SecuenciaTicket
{
    public int Id { get; set; }
    public int Anio { get; set; }
    public int UltimoNumero { get; set; }
}