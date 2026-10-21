
using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    // Lista principal de documentos en memoria.
    static List<Documento> documentos = new List<Documento>();

    static string[] tiposDocumento =
    {
        "Solicitud administrativa",
        "Reclamo",
        "Oficio",
        "Memorando",
        "Solicitud de documentación o de copia de historia clínica"
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
                    Console.WriteLine(
                        "La búsqueda se implementará en la etapa 3.");
                    break;

                case 3:
                    Console.WriteLine(
                        "El listado se implementará en la etapa 3.");
                    break;

                case 4:
                    Console.WriteLine(
                        "El ordenamiento se implementará en la etapa 3.");
                    break;

                case 5:
                    Console.WriteLine(
                        "La actualización se implementará en la etapa 3.");
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
        Console.WriteLine("     MESA DE PARTES DIGITAL");
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

    static void RegistrarDocumento()
    {
        Console.WriteLine("\n--- REGISTRO DE DOCUMENTO ---");

        string dni = LeerDniValido();
        string nombre = LeerTextoObligatorio("Nombre del remitente: ");

        string tipo = LeerTipoDocumento();
        string asunto = LeerTextoObligatorio("Asunto del documento: ");
        DateTime fecha = LeerFechaValida();

        // El código se genera automáticamente.
        string codigo = GenerarCodigoExpediente();

        // Todo documento nuevo comienza como recibido.
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

        // La persistencia se incorporará en la etapa 4.
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
            string texto = Console.ReadLine() ?? "";

            texto = texto.Trim();

            if (texto.Length == 0)
            {
                Console.WriteLine(
                    "Error: este campo es obligatorio.");
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

        // Busca el mayor correlativo del año en la lista actual.
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
}