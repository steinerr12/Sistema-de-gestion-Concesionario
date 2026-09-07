using System;

class Program
{
    static void Main(string[] args)
    {
        // Instancia de las clases de cada módulo
        AsistenciayPuntualidad asistencia = new AsistenciayPuntualidad();
        GestorTurnosVacaciones turnosVacaciones = new GestorTurnosVacaciones();
        

        int opcion = 0;

        do
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("        SISTEMA DE GESTIÓN DE CONCESIONARIO       ");
            Console.WriteLine("==================================================");
            Console.WriteLine("1. Control de Asistencia y Puntualidad");
            Console.WriteLine("2. Calculadora de Nómina y Sueldos");
            Console.WriteLine("3. Gestor de Turnos y Vacaciones");
            Console.WriteLine("4. Control de Ventas e Inventario de Vehículos");
            Console.WriteLine("5. Salir del Sistema");
            Console.WriteLine("==================================================");
            Console.Write("Seleccione una opción: ");

            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                switch (opcion)
                {
                    case 1:
                        asistencia.Menu();
                        break;
                    case 2:  //Nomina y sueldo
                        
                        break;
                    case 3: //Turno y Vacaciones
                        turnosVacaciones.Menu();
                        break;
                    case 4: //venta e inventario
                      
                        break;
                    case 5:
                        Console.WriteLine("\nSaliendo del sistema... ¡Hasta luego!");
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

        } while (opcion != 5);
    }
}