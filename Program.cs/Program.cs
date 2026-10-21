
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

class Program
{
    static List<Documento> documentos = new List<Documento>();

    static string archivo = "documentos.txt";

    // Indica si es seguro guardar cambios en el archivo.
    static bool persistenciaSegura = true;

    static string[] tiposDocumento =
    {
        "Solicitud administrativa",
        "Reclamo",
        "Oficio",
        "Memorando",
        "Solicitud de documentación o de copia de historia clínica"
    };

    static string[] estadosDocumento =
    {
        "Recibido",
        "En revisión",
        "Derivado",
        "Observado",
        "Atendido"
    };

    static void Main()
    {
        CargarDocumentos();

        int opcion;

        do
        {
            MostrarMenu();
            opcion = LeerOpcion();

            switch (opcion)
            {
                case 1:
                    RegistrarDocumento();
                    break;

                case 2:
                    BuscarDocumento();
                    break;

                case 3:
                    ListarDocumentos();
                    break;

                case 4:
                    OrdenarDocumentos();
                    break;

                case 5:
                    ActualizarEstado();
                    break;

                case 6:
                    Console.WriteLine("Saliendo del programa...");
                    break;

                default:
                    Console.WriteLine(
                        "Opción inválida. Seleccione un número del 1 al 6.");
                    break;
            }

            if (opcion != 6)
            {
                Console.WriteLine(
                    "\nPresione una tecla para continuar...");
                Console.ReadKey();
            }

            Console.Clear();

        } while (opcion != 6);
    }

    static void MostrarMenu()
    {
        Console.WriteLine("======================================");
        Console.WriteLine("        MESA DE PARTES DIGITAL");
        Console.WriteLine("======================================");
        Console.WriteLine("1. Registrar documento");
        Console.WriteLine("2. Buscar documento");
        Console.WriteLine("3. Mostrar todos los documentos");
        Console.WriteLine("4. Ordenar documentos por código");
        Console.WriteLine("5. Actualizar estado");
        Console.WriteLine("6. Salir");
        Console.WriteLine("======================================");
    }

    static int LeerOpcion()
    {
        Console.Write("Seleccione una opción: ");
        string entrada = Console.ReadLine() ?? "";

        int opcion;

        if (int.TryParse(entrada, out opcion))
        {
            return opcion;
        }

        return 0;
    }

    // REGISTRO

    static void RegistrarDocumento()
    {
        if (!persistenciaSegura)
        {
            Console.WriteLine(
                "No se pueden registrar documentos porque no se pudo " +
                "garantizar la seguridad del archivo.");
            return;
        }

        Console.WriteLine("\n--- REGISTRO DE DOCUMENTO ---");

        string dni = LeerDniValido();
        string nombre = LeerTextoObligatorio("Nombre del remitente: ");
        string tipo = LeerTipoDocumento();
        string asunto = LeerTextoObligatorio("Asunto del documento: ");
        DateTime fecha = LeerFechaValida();

        string codigo = GenerarCodigoExpediente();

        Documento nuevoDocumento = new Documento(
            codigo,
            dni,
            nombre,
            tipo,
            asunto,
            fecha,
            "Recibido"
        );

        documentos.Add(nuevoDocumento);

        if (GuardarDocumentos())
        {
            Console.WriteLine("\nDocumento registrado correctamente.");
            Console.WriteLine("Código de expediente: " + codigo);
            Console.WriteLine("Estado inicial: Recibido");
        }
        else
        {
            // Se revierte el registro en memoria si no se pudo guardar.
            documentos.Remove(nuevoDocumento);

            Console.WriteLine(
                "No se pudo guardar el documento. " +
                "El registro se canceló en memoria.");
        }
    }

    static string LeerDniValido()
    {
        while (true)
        {
            Console.Write("DNI del remitente (8 dígitos): ");
            string dni = Console.ReadLine() ?? "";

            bool valido = dni.Length == 8;

            for (int i = 0; i < dni.Length; i++)
            {
                if (dni[i] < '0' || dni[i] > '9')
                {
                    valido = false;
                    break;
                }
            }

            if (valido)
            {
                return dni;
            }

            Console.WriteLine(
                "Error: el DNI debe contener exactamente 8 dígitos numéricos.");
        }
    }

    static string LeerTextoObligatorio(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string texto = (Console.ReadLine() ?? "").Trim();

            if (texto.Length == 0)
            {
                Console.WriteLine("Error: este campo es obligatorio.");
            }
            else if (texto.Contains("|"))
            {
                Console.WriteLine(
                    "Error: no utilice el carácter | en este campo.");
            }
            else
            {
                return texto;
            }
        }
    }

    static string LeerTipoDocumento()
    {
        while (true)
        {
            Console.WriteLine("\nTipos de documento:");

            for (int i = 0; i < tiposDocumento.Length; i++)
            {
                Console.WriteLine(
                    (i + 1) + ". " + tiposDocumento[i]);
            }

            Console.Write("Seleccione el tipo de documento: ");
            string entrada = Console.ReadLine() ?? "";

            int opcion;

            if (int.TryParse(entrada, out opcion) &&
                opcion >= 1 &&
                opcion <= tiposDocumento.Length)
            {
                return tiposDocumento[opcion - 1];
            }

            Console.WriteLine(
                "Error: seleccione una opción válida de la lista.");
        }
    }

    static DateTime LeerFechaValida()
    {
        while (true)
        {
            Console.Write("Fecha de recepción (dd/MM/yyyy): ");
            string entrada = Console.ReadLine() ?? "";

            DateTime fecha;

            bool valido = DateTime.TryParseExact(
                entrada,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fecha
            );

            if (valido)
            {
                return fecha;
            }

            Console.WriteLine(
                "Error: ingrese una fecha válida en formato dd/MM/yyyy.");
        }
    }

    static string GenerarCodigoExpediente()
    {
        int anioActual = DateTime.Now.Year;
        int mayorCorrelativo = 0;

        string prefijo = "MP-" + anioActual + "-";

        foreach (Documento documento in documentos)
        {
            if (documento.CodigoExpediente.StartsWith(prefijo))
            {
                string parteNumerica =
                    documento.CodigoExpediente.Substring(prefijo.Length);

                int numero;

                if (int.TryParse(parteNumerica, out numero))
                {
                    if (numero > mayorCorrelativo)
                    {
                        mayorCorrelativo = numero;
                    }
                }
            }
        }

        int siguiente = mayorCorrelativo + 1;

        return prefijo + siguiente.ToString("D4");
    }

    // BÚSQUEDA

    static void BuscarDocumento()
    {
        Console.WriteLine("\n--- BÚSQUEDA DE DOCUMENTOS ---");
        Console.WriteLine("1. Buscar por código de expediente");
        Console.WriteLine("2. Buscar por DNI del remitente");
        Console.Write("Seleccione el tipo de búsqueda: ");

        string entrada = Console.ReadLine() ?? "";
        int opcion;

        if (!int.TryParse(entrada, out opcion) ||
            (opcion != 1 && opcion != 2))
        {
            Console.WriteLine("Opción de búsqueda inválida.");
            return;
        }

        if (opcion == 1)
        {
            Console.Write("Ingrese el código de expediente: ");
            string codigo = (Console.ReadLine() ?? "").Trim();

            bool encontrado = false;

            foreach (Documento documento in documentos)
            {
                if (string.Equals(
                    documento.CodigoExpediente,
                    codigo,
                    StringComparison.OrdinalIgnoreCase))
                {
                    MostrarDocumento(documento);
                    encontrado = true;
                    break;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine(
                    "No se encontró un documento con ese código.");
            }
        }
        else
        {
            string dni = LeerDniValido();
            bool encontrado = false;

            foreach (Documento documento in documentos)
            {
                if (documento.DniRemitente == dni)
                {
                    MostrarDocumento(documento);
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine(
                    "No se encontraron documentos asociados a ese DNI.");
            }
        }
    }

    // LISTADO

    static void ListarDocumentos()
    {
        Console.WriteLine("\n--- DOCUMENTOS REGISTRADOS ---");

        if (documentos.Count == 0)
        {
            Console.WriteLine("No hay documentos registrados.");
            return;
        }

        foreach (Documento documento in documentos)
        {
            MostrarDocumento(documento);
        }

        Console.WriteLine("Total de documentos: " + documentos.Count);
    }

    static void MostrarDocumento(Documento documento)
    {
        Console.WriteLine("\n--------------------------------------");
        Console.WriteLine("Código de expediente: " +
            documento.CodigoExpediente);
        Console.WriteLine("DNI del remitente: " +
            documento.DniRemitente);
        Console.WriteLine("Nombre del remitente: " +
            documento.NombreRemitente);
        Console.WriteLine("Tipo de documento: " +
            documento.TipoDocumento);
        Console.WriteLine("Asunto: " + documento.Asunto);
        Console.WriteLine("Fecha de recepción: " +
            documento.FechaRecepcion.ToString("dd/MM/yyyy"));
        Console.WriteLine("Estado: " + documento.Estado);
        Console.WriteLine("--------------------------------------");
    }

    // ORDENAMIENTO POR BURBUJA

    static void OrdenarDocumentos()
    {
        Console.WriteLine("\n--- ORDENAMIENTO POR CÓDIGO ---");

        if (documentos.Count == 0)
        {
            Console.WriteLine("No hay documentos para ordenar.");
            return;
        }

        for (int i = 0; i < documentos.Count - 1; i++)
        {
            for (int j = 0; j < documentos.Count - 1 - i; j++)
            {
                int comparacion = string.Compare(
                    documentos[j].CodigoExpediente,
                    documentos[j + 1].CodigoExpediente,
                    StringComparison.Ordinal
                );

                if (comparacion > 0)
                {
                    Documento temporal = documentos[j];
                    documentos[j] = documentos[j + 1];
                    documentos[j + 1] = temporal;
                }
            }
        }

        Console.WriteLine("Documentos ordenados correctamente.");
        ListarDocumentos();
    }

    // ACTUALIZACIÓN DEL ESTADO

    static void ActualizarEstado()
    {
        Console.WriteLine("\n--- ACTUALIZACIÓN DE ESTADO ---");
        Console.Write("Ingrese el código de expediente: ");

        string codigo = (Console.ReadLine() ?? "").Trim();

        Documento documentoEncontrado = null;

        foreach (Documento documento in documentos)
        {
            if (string.Equals(
                documento.CodigoExpediente,
                codigo,
                StringComparison.OrdinalIgnoreCase))
            {
                documentoEncontrado = documento;
                break;
            }
        }

        if (documentoEncontrado == null)
        {
            Console.WriteLine("No existe un documento con ese código.");
            return;
        }

        Console.WriteLine(
            "Estado actual: " + documentoEncontrado.Estado);

        Console.WriteLine("\nSeleccione el nuevo estado:");

        for (int i = 0; i < estadosDocumento.Length; i++)
        {
            Console.WriteLine(
                (i + 1) + ". " + estadosDocumento[i]);
        }

        Console.Write("Opción: ");
        string entrada = Console.ReadLine() ?? "";

        int opcion;

        if (!int.TryParse(entrada, out opcion) ||
            opcion < 1 ||
            opcion > estadosDocumento.Length)
        {
            Console.WriteLine(
                "Opción inválida. El estado no fue modificado.");
            return;
        }

        string estadoAnterior = documentoEncontrado.Estado;
        documentoEncontrado.Estado = estadosDocumento[opcion - 1];

        if (GuardarDocumentos())
        {
            Console.WriteLine("Estado actualizado y guardado correctamente.");
            MostrarDocumento(documentoEncontrado);
        }
        else
        {
            // Recupera el estado anterior si falla el guardado.
            documentoEncontrado.Estado = estadoAnterior;

            Console.WriteLine(
                "No se pudo guardar el cambio. " +
                "Se conserva el estado anterior en memoria.");
        }
    }

    // PERSISTENCIA: CARGA DE DOCUMENTOS

    static void CargarDocumentos()
    {
        documentos.Clear();

        if (!File.Exists(archivo))
        {
            Console.WriteLine(
                "No existe un archivo previo. Se iniciará un registro nuevo.");
            return;
        }

        try
        {
            string[] lineas = File.ReadAllLines(archivo, Encoding.UTF8);
            List<Documento> documentosCargados =
                new List<Documento>();

            foreach (string linea in lineas)
            {
                Documento documento;

                if (!IntentarConvertirLinea(linea, out documento))
                {
                    throw new FormatException(
                        "El archivo contiene una línea inválida.");
                }

                // Evita aceptar códigos repetidos en el archivo.
                foreach (Documento existente in documentosCargados)
                {
                    if (existente.CodigoExpediente ==
                        documento.CodigoExpediente)
                    {
                        throw new FormatException(
                            "El archivo contiene códigos duplicados.");
                    }
                }

                documentosCargados.Add(documento);
            }

            // Solo se reemplaza la lista cuando toda la lectura es válida.
            documentos = documentosCargados;

            Console.WriteLine(
                "Documentos recuperados: " + documentos.Count);
        }
        catch (Exception ex) when (
            ex is IOException ||
            ex is UnauthorizedAccessException ||
            ex is FormatException)
        {
            documentos.Clear();
            persistenciaSegura = false;

            Console.WriteLine(
                "ERROR: no se pudieron recuperar los documentos.");
            Console.WriteLine("Detalle: " + ex.Message);
            Console.WriteLine(
                "Por seguridad, no se permitirán nuevos registros ni cambios.");
            Console.WriteLine(
                "Revise el archivo antes de volver a ejecutar el programa.");
        }
    }

    static bool IntentarConvertirLinea(
        string linea,
        out Documento documento)
    {
        documento = null;

        string[] campos = linea.Split('|');

        if (campos.Length != 7)
        {
            return false;
        }

        string codigo = campos[0];
        string dni = campos[1];
        string nombre = campos[2];
        string tipo = campos[3];
        string asunto = campos[4];
        string textoFecha = campos[5];
        string estado = campos[6];

        if (!EsDniValido(dni) ||
            string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(asunto))
        {
            return false;
        }

        if (!EsTipoValido(tipo) || !EsEstadoValido(estado))
        {
            return false;
        }

        DateTime fecha;

        if (!DateTime.TryParseExact(
            textoFecha,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out fecha))
        {
            return false;
        }

        // Comprueba el formato MP-AAAA-NNNN.
        if (codigo.Length != 12 ||
            !codigo.StartsWith("MP-") ||
            codigo[7] != '-')
        {
            return false;
        }

        int anio;
        int correlativo;

        if (!int.TryParse(codigo.Substring(3, 4), out anio) ||
            !int.TryParse(codigo.Substring(8, 4), out correlativo) ||
            anio < 1 ||
            correlativo < 1)
        {
            return false;
        }

        documento = new Documento(
            codigo, dni, nombre, tipo, asunto, fecha, estado);

        return true;
    }

    static bool EsDniValido(string dni)
    {
        if (dni.Length != 8)
        {
            return false;
        }

        for (int i = 0; i < dni.Length; i++)
        {
            if (dni[i] < '0' || dni[i] > '9')
            {
                return false;
            }
        }

        return true;
    }

    static bool EsTipoValido(string tipo)
    {
        foreach (string tipoPermitido in tiposDocumento)
        {
            if (tipo == tipoPermitido)
            {
                return true;
            }
        }

        return false;
    }

    static bool EsEstadoValido(string estado)
    {
        foreach (string estadoPermitido in estadosDocumento)
        {
            if (estado == estadoPermitido)
            {
                return true;
            }
        }

        return false;
    }

    // PERSISTENCIA: GUARDADO DE DOCUMENTOS

    static bool GuardarDocumentos()
    {
        if (!persistenciaSegura)
        {
            Console.WriteLine(
                "Guardado bloqueado para proteger el archivo existente.");
            return false;
        }

        string archivoTemporal = archivo + ".tmp";

        try
        {
            List<string> lineas = new List<string>();

            foreach (Documento documento in documentos)
            {
                string linea =
                    documento.CodigoExpediente + "|" +
                    documento.DniRemitente + "|" +
                    documento.NombreRemitente + "|" +
                    documento.TipoDocumento + "|" +
                    documento.Asunto + "|" +
                    documento.FechaRecepcion.ToString("dd/MM/yyyy") + "|" +
                    documento.Estado;

                lineas.Add(linea);
            }

            // Primero se escribe un archivo temporal.
            File.WriteAllLines(
                archivoTemporal, lineas, new UTF8Encoding(false));

            // Después se sustituye el archivo anterior.
            File.Move(archivoTemporal, archivo, true);

            return true;
        }
        catch (Exception ex) when (
            ex is IOException ||
            ex is UnauthorizedAccessException)
        {
            Console.WriteLine("ERROR al guardar los documentos.");
            Console.WriteLine("Detalle: " + ex.Message);

            try
            {
                if (File.Exists(archivoTemporal))
                {
                    File.Delete(archivoTemporal);
                }
            }
            catch (IOException)
            {
                Console.WriteLine(
                    "No se pudo eliminar el archivo temporal.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(
                    "No hay permisos para eliminar el archivo temporal.");
            }

            return false;
        }
    }
}