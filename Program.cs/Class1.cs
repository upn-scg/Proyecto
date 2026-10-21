
using System;

public class Documento
{
    public string CodigoExpediente { get; set; }
    public string DniRemitente { get; set; }
    public string NombreRemitente { get; set; }
    public string TipoDocumento { get; set; }
    public string Asunto { get; set; }
    public DateTime FechaRecepcion { get; set; }
    public string Estado { get; set; }

    public Documento(
        string codigoExpediente,
        string dniRemitente,
        string nombreRemitente,
        string tipoDocumento,
        string asunto,
        DateTime fechaRecepcion,
        string estado)
    {
        CodigoExpediente = codigoExpediente;
        DniRemitente = dniRemitente;
        NombreRemitente = nombreRemitente;
        TipoDocumento = tipoDocumento;
        Asunto = asunto;
        FechaRecepcion = fechaRecepcion;
        Estado = estado;
    }
}
