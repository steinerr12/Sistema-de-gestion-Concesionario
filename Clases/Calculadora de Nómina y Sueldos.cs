using System;
using System.Linq;

public class CalculadoraNominaSueldos
{
    public void Menu()
    {
        int opcion = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("        CALCULADORA DE NÓMINA Y SUELDOS       ");
            Console.WriteLine("==============================================");
            Console.WriteLine("1. Generar nómina general (Todos los empleados)");
            Console.WriteLine("2. Consultar / Calcular nómina por empleado");
            Console.WriteLine("3. Volver al Menú Principal");
            Console.WriteLine("==============================================");
            Console.Write("Seleccione una opción: ");

            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                switch (opcion)
                {
                    case 1:
                        GenerarNominaGeneral();
                        break;
                    case 2:
                        ConsultarNominaIndividual();
                        break;
                    case 3:
                        break;
                    default:
                        Console.WriteLine("\n[ERROR] Opción no válida.");
                        Console.ReadKey();
                        break;
                }
            }
            else
            {
                Console.WriteLine("\n[ERROR] Ingrese un número válido.");
                Console.ReadKey();
            }
        } while (opcion != 3);
    }

    private void GenerarNominaGeneral()
    {
        Console.Clear();
        Console.WriteLine("=========================================================================================");
        Console.WriteLine("                                NÓMINA GENERAL DEL CONCESIONARIO                         ");
        Console.WriteLine("=========================================================================================");

        // Si no hay registros de asistencia aún, usamos una jornada estándar (ej. 44 horas semanales)
        bool hayRegistros = DatosCompartidos.HistorialAsistencias.Any(a => a.Salida.HasValue);
        double horasPorDefecto = 44.0;

        if (!hayRegistros)
        {
            Console.WriteLine(">> Nota: No hay marcas registradas en el módulo de Asistencias aún.");
            Console.WriteLine($">> Se calculará en base a jornada estándar semanal ({horasPorDefecto} horas).\n");
        }

        foreach (var emp in DatosCompartidos.ListaEmpleados)
        {
            double horas = ObtenerHorasEmpleado(emp.Codigo);
            if (horas == 0) horas = horasPorDefecto;

            CalcularYMostrarEmpleado(emp, horas);
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void ConsultarNominaIndividual()
    {
        Console.Clear();
        Console.WriteLine("==============================================");
        Console.WriteLine("         CONSULTA DE NÓMINA INDIVIDUAL        ");
        Console.WriteLine("==============================================");
        Console.Write("Ingrese el código del empleado (ej. EMP01): ");
        string codigo = Console.ReadLine()?.Trim().ToUpper();

        var emp = DatosCompartidos.ListaEmpleados.FirstOrDefault(e => e.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase));

        if (emp != null)
        {
            double horas = ObtenerHorasEmpleado(emp.Codigo);

            // Si el reloj marcador no tiene registros de salida para este empleado
            if (horas == 0)
            {
                Console.Write($"No hay asistencias registradas para {emp.Nombre}. Ingrese horas trabajadas: ");
                if (!double.TryParse(Console.ReadLine(), out horas) || horas < 0)
                {
                    horas = 44; // Valor por defecto si ingresan dato inválido
                }
            }

            Console.Clear();
            CalcularYMostrarEmpleado(emp, horas);
        }
        else
        {
            Console.WriteLine("\n[ERROR] Empleado no encontrado.");
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private double ObtenerHorasEmpleado(string codigoEmpleado)
    {
        double totalHoras = 0;
        var asistencias = DatosCompartidos.HistorialAsistencias
            .Where(a => a.CodigoEmpleado.Equals(codigoEmpleado, StringComparison.OrdinalIgnoreCase) && a.Salida.HasValue);

        foreach (var reg in asistencias)
        {
            totalHoras += (reg.Salida.Value - reg.Entrada).TotalHours;
        }

        return totalHoras;
    }

    private void CalcularYMostrarEmpleado(Empleado emp, double horas)
    {
        // 1. Salario Bruto
        double salarioBruto = horas * emp.TarifaHora;

        // 2. Bonificación por ventas (si tiene facturas emitidas en DatosCompartidos)
        double totalVentas = DatosCompartidos.ListaFacturas
            .Where(f => f.CodigoVendedor.Equals(emp.Codigo, StringComparison.OrdinalIgnoreCase))
            .Sum(f => f.Total);

        double bonoDesempeno = (emp.Puesto.Equals("Vendedor", StringComparison.OrdinalIgnoreCase)) ? totalVentas * 0.03 : 0.0;

        // 3. Retenciones de Ley (ISSS 3% + AFP 7.25% = 10.25%)
        double retencionesLey = salarioBruto * 0.1025;

        // 4. Sueldo Neto Final
        double sueldoNeto = (salarioBruto + bonoDesempeno) - retencionesLey;

        Console.WriteLine($"Código: {emp.Codigo} | Nombre: {emp.Nombre,-15} | Puesto: {emp.Puesto,-12}");
        Console.WriteLine($"Tarifa/Hora: ${emp.TarifaHora:F2} | Horas Calculadas: {horas:F2} hrs");
        Console.WriteLine($"Salario Bruto : ${salarioBruto,10:F2}");
        Console.WriteLine($"Bono/Ventas   : +${bonoDesempeno,9:F2} (Ventas registradas: ${totalVentas:F2})");
        Console.WriteLine($"Retenciones   : -${retencionesLey,9:F2} (10.25% AFP/ISSS)");
        Console.WriteLine($"SUELDO NETO   : ${sueldoNeto,10:F2}");
        Console.WriteLine(new string('-', 75));
    }
}