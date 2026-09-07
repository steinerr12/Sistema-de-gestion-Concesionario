using System;
using System.Collections.Generic;
using System.Linq;

public class GestorTurnosVacaciones
{
    private static readonly string[] DiasSemana =
    {
        "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"
    };

    public void Menu()
    {
        int op = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("--- GESTOR DE TURNOS Y VACACIONES ---");
            Console.WriteLine("1. Ver empleados");
            Console.WriteLine("2. Asignar turno a un empleado");
            Console.WriteLine("3. Ver calendario de turnos");
            Console.WriteLine("4. Solicitar vacaciones");
            Console.WriteLine("5. Ver vacaciones solicitadas");
            Console.WriteLine("6. Volver");
            Console.Write("\nOpción: ");

            if (int.TryParse(Console.ReadLine(), out op))
            {
                switch (op)
                {
                    case 1: ListarEmpleados(); break;
                    case 2: AsignarTurno(); break;
                    case 3: VerCalendario(); break;
                    case 4: SolicitarVacaciones(); break;
                    case 5: VerVacaciones(); break;
                    case 6: break;
                    default: Console.WriteLine("Opción incorrecta."); break;
                }
            }
            else
            {
                Console.WriteLine("Ingrese un número válido.");
            }

            if (op != 6)
            {
                Console.WriteLine("\nPresiona una tecla para continuar...");
                Console.ReadKey();
            }

        } while (op != 6);
    }

    private void ListarEmpleados()
    {
        Console.WriteLine("\nLista de empleados:");
        foreach (var e in DatosCompartidos.ListaEmpleados)
        {
            Console.WriteLine($"[{e.Codigo}] {e.Nombre} - {e.Puesto}");
        }
    }

    private Empleado BuscarEmpleado(string codigo)
    {
        return DatosCompartidos.ListaEmpleados.FirstOrDefault(e => e.Codigo == codigo);
    }

    private void AsignarTurno()
    {
        Console.Write("\nCódigo del empleado: ");
        string cod = Console.ReadLine()?.Trim().ToUpper();
        var emp = BuscarEmpleado(cod);

        if (emp == null)
        {
            Console.WriteLine("El código no existe.");
            return;
        }

        Console.WriteLine("Días: " + string.Join(", ", DiasSemana));
        Console.Write("Días de la semana (separados por coma, ej. Lunes, Miércoles, Viernes): ");
        string diasTexto = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(diasTexto))
        {
            Console.WriteLine("Debes ingresar al menos un día.");
            return;
        }

        // Separamos por coma y limpiamos espacios en cada día ingresado
        string[] diasIngresados = diasTexto.Split(',')
            .Select(d => d.Trim())
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .ToArray();

        // Validamos cada día antes de pedir las horas, así no se pide nada si hay un error
        var diasValidos = new List<string>();
        var diasInvalidos = new List<string>();

        foreach (var d in diasIngresados)
        {
            if (TryParseDiaEspanol(d, out string diaFormateado))
            {
                if (!diasValidos.Contains(diaFormateado)) // evita duplicados si el usuario repite un día
                {
                    diasValidos.Add(diaFormateado);
                }
            }
            else
            {
                diasInvalidos.Add(d);
            }
        }

        if (diasInvalidos.Any())
        {
            Console.WriteLine($"Los siguientes días no son válidos y se ignorarán: {string.Join(", ", diasInvalidos)}");
        }

        if (!diasValidos.Any())
        {
            Console.WriteLine("No se ingresó ningún día válido. No se asignó ningún turno.");
            return;
        }

        // Pedimos y validamos la hora de inicio hasta que sea válida
        TimeSpan horaInicioTS;
        string horaInicio;
        do
        {
            Console.Write("Hora de inicio (formato HH:mm, ej. 08:00): ");
            horaInicio = Console.ReadLine()?.Trim();

            if (!TryParseHora(horaInicio, out horaInicioTS))
            {
                Console.WriteLine("Formato inválido. Usa HH:mm en formato de 24 horas (ej. 08:00, 16:30).");
            }
        } while (!TryParseHora(horaInicio, out horaInicioTS));

        // Pedimos y validamos la hora de fin, asegurando que sea posterior a la de inicio
        TimeSpan horaFinTS;
        string horaFin;
        do
        {
            Console.Write("Hora de fin (formato HH:mm, ej. 16:00): ");
            horaFin = Console.ReadLine()?.Trim();

            if (!TryParseHora(horaFin, out horaFinTS))
            {
                Console.WriteLine("Formato inválido. Usa HH:mm en formato de 24 horas (ej. 08:00, 16:30).");
            }
            else if (horaFinTS <= horaInicioTS)
            {
                Console.WriteLine("La hora de fin debe ser posterior a la hora de inicio.");
            }

        } while (!TryParseHora(horaFin, out horaFinTS) || horaFinTS <= horaInicioTS);

       
        horaInicio = horaInicioTS.ToString(@"hh\:mm");
        horaFin = horaFinTS.ToString(@"hh\:mm");

        foreach (var diaFormateado in diasValidos)
        {
            // Si ya tenía turno asignado ese día, lo reemplaza
            var turnoExistente = DatosCompartidos.ListaTurnos
                .FirstOrDefault(t => t.CodigoEmpleado == emp.Codigo && string.Equals(t.Dia, diaFormateado, StringComparison.OrdinalIgnoreCase));

            if (turnoExistente != null)
            {
                turnoExistente.HoraInicio = horaInicio;
                turnoExistente.HoraFin = horaFin;
            }
            else
            {
                DatosCompartidos.ListaTurnos.Add(new Turno
                {
                    CodigoEmpleado = emp.Codigo,
                    Dia = diaFormateado, // Guardamos directamente el string formateado (ej. "Lunes")
                    HoraInicio = horaInicio,
                    HoraFin = horaFin
                });
            }
        }

        Console.WriteLine($"\nTurno asignado a {emp.Nombre}: {string.Join(", ", diasValidos)} de {horaInicio} a {horaFin}");
    }

    // Valida que el texto ingresado sea una hora válida en formato HH:mm (24 horas)
    private bool TryParseHora(string texto, out TimeSpan hora)
    {
        return TimeSpan.TryParseExact(
            texto,
            new[] { @"h\:mm", @"hh\:mm" },
            System.Globalization.CultureInfo.InvariantCulture,
            out hora);
    }

    // Busca coincidencia en el arreglo e ignora tildes o mayúsculas básicas
    private bool TryParseDiaEspanol(string texto, out string diaEncontrado)
    {
        diaEncontrado = null;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        string entrada = texto.Trim().ToLower()
            .Replace("é", "e")
            .Replace("á", "a");

        foreach (var dia in DiasSemana)
        {
            string diaNormalizado = dia.ToLower()
                .Replace("é", "e")
                .Replace("á", "a");

            if (diaNormalizado == entrada)
            {
                diaEncontrado = dia; // Retorna la versión bien escrita (ej. "Miércoles")
                return true;
            }
        }

        return false;
    }

    private void VerCalendario()
    {
        Console.WriteLine("\n================ CALENDARIO SEMANAL DE TURNOS ================");

        if (!DatosCompartidos.ListaTurnos.Any())
        {
            Console.WriteLine("Aún no hay turnos asignados.");
            return;
        }

        foreach (var emp in DatosCompartidos.ListaEmpleados)
        {
            Console.WriteLine($"\n{emp.Nombre} ({emp.Codigo}) - {emp.Puesto}:");
            var turnosEmp = DatosCompartidos.ListaTurnos.Where(t => t.CodigoEmpleado == emp.Codigo).ToList();

            foreach (var dia in DiasSemana)
            {
                var turno = turnosEmp.FirstOrDefault(t => string.Equals(t.Dia, dia, StringComparison.OrdinalIgnoreCase));
                string linea = turno != null ? $"{turno.HoraInicio} - {turno.HoraFin}" : "Libre";
                Console.WriteLine($"   {dia,-10}: {linea}");
            }
        }
    }

    private void SolicitarVacaciones()
    {
        Console.Write("\nCódigo del empleado: ");
        string cod = Console.ReadLine()?.Trim().ToUpper();
        var emp = BuscarEmpleado(cod);

        if (emp == null)
        {
            Console.WriteLine("El código no existe.");
            return;
        }

        Console.Write("Fecha de inicio (dd/mm/aaaa): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime inicio))
        {
            Console.WriteLine("Fecha inválida.");
            return;
        }

        Console.Write("Fecha de fin (dd/mm/aaaa): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime fin))
        {
            Console.WriteLine("Fecha inválida.");
            return;
        }

        if (fin < inicio)
        {
            Console.WriteLine("La fecha de fin no puede ser anterior a la fecha de inicio.");
            return;
        }

        // Evita que dos personas del mismo puesto pidan vacaciones que se traslapen
        var conflicto = DatosCompartidos.SolicitudesVacaciones.FirstOrDefault(s =>
            s.Puesto == emp.Puesto &&
            s.CodigoEmpleado != emp.Codigo &&
            inicio <= s.FechaFin && fin >= s.FechaInicio);

        if (conflicto != null)
        {
            Console.WriteLine($"\n[RECHAZADO] El empleado {conflicto.CodigoEmpleado} (mismo puesto: {emp.Puesto}) " +
                $"ya tiene vacaciones del {conflicto.FechaInicio.ToShortDateString()} al {conflicto.FechaFin.ToShortDateString()}, " +
                "que se cruzan con las fechas solicitadas. No pueden ausentarse dos personas del mismo puesto al mismo tiempo.");
            return;
        }

        DatosCompartidos.SolicitudesVacaciones.Add(new SolicitudVacacion
        {
            CodigoEmpleado = emp.Codigo,
            Puesto = emp.Puesto,
            FechaInicio = inicio,
            FechaFin = fin
        });

        Console.WriteLine($"\nVacaciones registradas para {emp.Nombre} del {inicio.ToShortDateString()} al {fin.ToShortDateString()}.");
    }

    private void VerVacaciones()
    {
        Console.WriteLine("\nSolicitudes de vacaciones registradas:");

        if (DatosCompartidos.SolicitudesVacaciones.Count == 0)
        {
            Console.WriteLine("No hay solicitudes registradas.");
            return;
        }

        foreach (var s in DatosCompartidos.SolicitudesVacaciones)
        {
            var emp = BuscarEmpleado(s.CodigoEmpleado);
            string nombre = emp != null ? emp.Nombre : s.CodigoEmpleado;
            Console.WriteLine($"{nombre} ({s.Puesto}): {s.FechaInicio.ToShortDateString()} al {s.FechaFin.ToShortDateString()}");
        }
    }
}