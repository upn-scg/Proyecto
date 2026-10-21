

using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static List<Documento> documentos = new List<Documento>();

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

    // ETAPA 2: REGISTRO DE DOCUMENTOS

    static void RegistrarDocumento()
    {
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

        Console.WriteLine("\nDocumento registrado correctamente.");
        Console.WriteLine("Código de expediente: " + codigo);
        Console.WriteLine("Estado inicial: Recibido");
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

    // ETAPA 3: BÚSQUEDA DE DOCUMENTOS

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

    // ETAPA 3: LISTADO DE DOCUMENTOS

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

        Console.WriteLine(
            "Total de documentos: " + documentos.Count);
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

    // ETAPA 3: ORDENAMIENTO POR BURBUJA

    static void OrdenarDocumentos()
    {
        Console.WriteLine("\n--- ORDENAMIENTO POR CÓDIGO ---");

        if (documentos.Count == 0)
        {
            Console.WriteLine("No hay documentos para ordenar.");
            return;
        }

        // Compara documentos vecinos e intercambia los que
        // están en el orden incorrecto.
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

    // ETAPA 3: ACTUALIZACIÓN DEL ESTADO

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
            Console.WriteLine(
                "No existe un documento con ese código.");
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

        documentoEncontrado.Estado = estadosDocumento[opcion - 1];

        Console.WriteLine("Estado actualizado correctamente.");
        MostrarDocumento(documentoEncontrado);
    }
}
