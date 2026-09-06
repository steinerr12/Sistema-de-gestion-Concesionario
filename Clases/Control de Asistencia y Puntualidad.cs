// Control de Asistencia y Puntualidad.cs
using System;
using System.Collections.Generic;

public class AsistenciayPuntualidad
{
    // =========================================================================
    // Codigos de los empleados
    // - EMP01: Carlos Pérez
    // - EMP02: María Gómez
    // - EMP03: Juan Rodríguez
    // - EMP04: Ana Martínez
    // - EMP05: Luis Fernández
    // =========================================================================

    private class Empleado
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    private class Asistencia
    {
        public string CodigoEmpleado { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime? Salida { get; set; }
        public bool Tarde { get; set; }
    }

    private List<Empleado> empleados = new List<Empleado>()
    {
        new Empleado { Codigo = "EMP01", Nombre = "Carlos Pérez" },
        new Empleado { Codigo = "EMP02", Nombre = "María Gómez" },
        new Empleado { Codigo = "EMP03", Nombre = "Juan Rodríguez" },
        new Empleado { Codigo = "EMP04", Nombre = "Ana Martínez" },
        new Empleado { Codigo = "EMP05", Nombre = "Luis Fernández" }
    };

    private List<Asistencia> registros = new List<Asistencia>();
    private readonly TimeSpan horaLimite = new TimeSpan(8, 0, 0);

    public void Menu()
    {
        int op = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("--- CONTROL DE ASISTENCIA Y PUNTUALIDAD ---");
            Console.WriteLine("1. Ver empleados");
            Console.WriteLine("2. Marcar entrada");
            Console.WriteLine("3. Marcar salida");
            Console.WriteLine("4. Ver historial");
            Console.WriteLine("5. Volver");
            Console.Write("\nOpción: ");

            if (int.TryParse(Console.ReadLine(), out op))
            {
                switch (op)
                {
                    case 1:
                        ListarEmpleados();
                        break;
                    case 2:
                        MarcarEntrada();
                        break;
                    case 3:
                        MarcarSalida();
                        break;
                    case 4:
                        VerHistorial();
                        break;
                    case 5:
                        break;
                    default:
                        Console.WriteLine("Opción incorrecta.");
                        break;
                }
            }

            if (op != 5)
            {
                Console.WriteLine("\nPresiona una tecla para continuar...");
                Console.ReadKey();
            }

        } while (op != 5);
    }

    private void ListarEmpleados()
    {
        Console.WriteLine("\nLista de empleados:");
        foreach (var e in empleados)
        {
            Console.WriteLine($"[{e.Codigo}] {e.Nombre}");
        }
    }

    private void MarcarEntrada()
    {
        Console.Write("\nIngresa el código del empleado: ");
        string cod = Console.ReadLine()?.Trim().ToUpper();

        Empleado emp = null;
        foreach (var e in empleados)
        {
            if (e.Codigo == cod)
            {
                emp = e;
                break;
            }
        }

        if (emp == null)
        {
            Console.WriteLine("El código no existe.");
            return;
        }

        DateTime ahora = DateTime.Now;
        bool tarde = ahora.TimeOfDay > horaLimite;

        registros.Add(new Asistencia
        {
            CodigoEmpleado = emp.Codigo,
            Entrada = ahora,
            Salida = null,
            Tarde = tarde
        });

        Console.WriteLine($"Entrada registrada para {emp.Nombre} a las {ahora.ToShortTimeString()}");
        if (tarde)
        {
            Console.WriteLine("Nota: El empleado llegó tarde.");
        }
    }

    private void MarcarSalida()
    {
        Console.Write("\nIngresa el código del empleado para la salida: ");
        string cod = Console.ReadLine()?.Trim().ToUpper();

        Asistencia regActivo = null;
        foreach (var r in registros)
        {
            if (r.CodigoEmpleado == cod && r.Salida == null)
            {
                regActivo = r;
                break;
            }
        }

        if (regActivo != null)
        {
            regActivo.Salida = DateTime.Now;
            Console.WriteLine($"Salida registrada a las {regActivo.Salida.Value.ToShortTimeString()}");
        }
        else
        {
            Console.WriteLine("No hay un registro de entrada pendiente para este código.");
        }
    }

    private void VerHistorial()
    {
        Console.WriteLine("\nHistorial de asistencias:");
        if (registros.Count == 0)
        {
            Console.WriteLine("Aún no hay registros.");
            return;
        }

        foreach (var r in registros)
        {
            string salidaStr = r.Salida.HasValue ? r.Salida.Value.ToShortTimeString() : "En turno";
            string tardeStr = r.Tarde ? "Sí" : "No";
            Console.WriteLine($"Empleado: {r.CodigoEmpleado} | Fecha: {r.Entrada.ToShortDateString()} | Entrada: {r.Entrada.ToShortTimeString()} | Salida: {salidaStr} | Tarde: {tardeStr}");
        }
    }
}