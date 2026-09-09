using System;
using System.Collections.Generic;

public class AsistenciayPuntualidad
{
    private TimeSpan horaLimite = new TimeSpan(8, 0, 0); // Decklaramos la hora de llegada

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
        foreach (var e in DatosCompartidos.ListaEmpleados)
        {
            Console.WriteLine($"[{e.Codigo}] {e.Nombre} - {e.Puesto}");
        }
    }

    private void MarcarEntrada()
    {
        Console.Write("\nIngresa el código del empleado: ");
        string cod = Console.ReadLine()?.Trim().ToUpper();

        var emp = DatosCompartidos.ListaEmpleados.Find(e => e.Codigo == cod);

        if (emp == null)
        {
            Console.WriteLine("El código no existe.");
            return;
        }

        // Pedimos la fecha y la hora manual 
        Console.Write("Fecha de entrada (dd/mm/aaaa): ");
        string fIngresada = Console.ReadLine();

        Console.Write("Hora de entrada (ej. 08:30): ");
        string hIngresada = Console.ReadLine();

        // Intentamos armar la fecha con lo que puso el usuario
        if (!DateTime.TryParse(fIngresada, out DateTime fecha) || !TimeSpan.TryParse(hIngresada, out TimeSpan hora))
        {
            Console.WriteLine("\n[ERROR] Fecha u hora inválida.");
            return;
        }

        DateTime entradaFinal = fecha.Date + hora;
        bool tarde = entradaFinal.TimeOfDay > horaLimite;

        // Lo guardamos en la lista global para que la nómina lo lea bien
        DatosCompartidos.HistorialAsistencias.Add(new RegistroAsistencia
        {
            CodigoEmpleado = emp.Codigo,
            Entrada = entradaFinal,
            Salida = null,
            Tarde = tarde
        });

        Console.WriteLine($"\nEntrada registrada para {emp.Nombre} el {entradaFinal:dd/MM/yyyy} a las {entradaFinal:HH:mm}");
        if (tarde)
        {
            Console.WriteLine("Nota: El empleado llegó tarde.");
        }
    }

    private void MarcarSalida()
    {
        Console.Write("\nIngresa el código del empleado para la salida: ");
        string cod = Console.ReadLine()?.Trim().ToUpper();

        // Buscamos si tiene algún turno abierto 
        var regActivo = DatosCompartidos.HistorialAsistencias
            .Find(r => r.CodigoEmpleado.Equals(cod, StringComparison.OrdinalIgnoreCase) && r.Salida == null);

        if (regActivo == null)
        {
            Console.WriteLine("No hay un registro de entrada pendiente para este empleado.");
            return;
        }

        Console.WriteLine($"Entrada pendiente encontrada del día: {regActivo.Entrada:dd/MM/yyyy HH:mm}");

        Console.Write("Fecha de salida (dd/mm/aaaa): ");
        string fIngresada = Console.ReadLine();

        Console.Write("Hora de salida (ej. 17:00): ");
        string hIngresada = Console.ReadLine();

        if (!DateTime.TryParse(fIngresada, out DateTime fecha) || !TimeSpan.TryParse(hIngresada, out TimeSpan hora))
        {
            Console.WriteLine("\n[ERROR] Fecha u hora inválida.");
            return;
        }

        DateTime salidaFinal = fecha.Date + hora;

        if (salidaFinal <= regActivo.Entrada)
        {
            Console.WriteLine("\n[ERROR] La salida no puede ser antes o al mismo tiempo que la entrada.");
            return;
        }

        regActivo.Salida = salidaFinal;
        Console.WriteLine($"\nSalida registrada con éxito para el {salidaFinal:dd/MM/yyyy} a las {salidaFinal:HH:mm}");
    }

    private void VerHistorial()
    {
        Console.WriteLine("\nHistorial de asistencias:");
        if (DatosCompartidos.HistorialAsistencias.Count == 0)
        {
            Console.WriteLine("Aún no hay registros.");
            return;
        }

        foreach (var r in DatosCompartidos.HistorialAsistencias)
        {
            string salidaStr = r.Salida.HasValue ? r.Salida.Value.ToString("dd/MM/yyyy HH:mm") : "En turno";
            string tardeStr = r.Tarde ? "Sí" : "No";
            Console.WriteLine($"EMP: {r.CodigoEmpleado} | Entrada: {r.Entrada:dd/MM/yyyy HH:mm} | Salida: {salidaStr} | Tarde: {tardeStr}");
        }
    }
}