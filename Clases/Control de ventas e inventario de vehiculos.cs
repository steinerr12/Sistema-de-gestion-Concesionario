using System;
using System.Collections.Generic;
using System.Linq;

public class Vehiculo
{
    public string Codigo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int Año { get; set; }
    public double Precio { get; set; }
    public bool Disponible { get; set; }
}

public class ControlVentasInventario
{
    // Porcentaje de impuesto fijo 
    private const double PORCENTAJE_IMPUESTO = 13.0;
    // Catálogo compartido de vehículos para que tu compañera pueda marcar ventas sobre ellos
    public static List<Vehiculo> InventarioVehiculos = new List<Vehiculo>()
    {
        new Vehiculo { Codigo = "V01", Marca = "Toyota", Modelo = "Corolla", Año = 2024, Precio = 22000.00, Disponible = true },
        new Vehiculo { Codigo = "V02", Marca = "Nissan", Modelo = "Frontier", Año = 2023, Precio = 28000.00, Disponible = true },
        new Vehiculo { Codigo = "V03", Marca = "Honda", Modelo = "Civic", Año = 2024, Precio = 25500.00, Disponible = true },
        new Vehiculo { Codigo = "V04", Marca = "Hyundai", Modelo = "Tucson", Año = 2025, Precio = 31000.00, Disponible = true },
        new Vehiculo { Codigo = "V05", Marca = "Kia", Modelo = "Sportage", Año = 2023, Precio = 27500.00, Disponible = true }
    };

    public void Menu()
    {
        int opcion = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("=================================================");
            Console.WriteLine("        INVENTARIO DE VEHÍCULOS ACTUALES         ");
            Console.WriteLine("=================================================");
            Console.WriteLine("1. Ver inventario general (Disponibilidad)");
            Console.WriteLine("2. Agregar nuevo vehículo al inventario");
            Console.WriteLine("3. Crear factura de venta");
            Console.WriteLine("4. Ver facturas emitidas");
            Console.WriteLine("5. Volver al Menú Principal");
            Console.WriteLine("=================================================");
            Console.Write("Seleccione una opción: ");

            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                switch (opcion)
                {
                    case 1:
                        MostrarInventario();
                        break;
                    case 2:
                        AgregarVehiculo();
                        break;
                    case 3:
                        CrearFactura();
                        break;
       
                    case 4:
                        VerFacturas();
                        break;
                    case 5:
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

    private void MostrarInventario()
    {
        Console.Clear();
        Console.WriteLine("================================================================================");
        Console.WriteLine("                       INVENTARIO DE VEHÍCULOS                                 ");
        Console.WriteLine("================================================================================");
        Console.WriteLine(string.Format("{0,-8} {1,-12} {2,-15} {3,-8} {4,-12} {5,-12}", "CÓDIGO", "MARCA", "MODELO", "AÑO", "PRECIO", "ESTADO"));
        Console.WriteLine(new string('-', 80));

        foreach (var v in InventarioVehiculos)
        {
            string estado = v.Disponible ? "DISPONIBLE" : "VENDIDO";
            Console.WriteLine(string.Format("{0,-8} {1,-12} {2,-15} {3,-8} ${4,-11:F2} {5,-12}",
                v.Codigo, v.Marca, v.Modelo, v.Año, v.Precio, estado));
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void AgregarVehiculo()
    {
        Console.Clear();
        Console.WriteLine("==============================================");
        Console.WriteLine("           AGREGAR NUEVO VEHÍCULO             ");
        Console.WriteLine("==============================================");

        Console.Write("Código único (ej. V06): ");
        string codigo = Console.ReadLine()?.Trim().ToUpper();

        if (InventarioVehiculos.Any(v => v.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("\n[ERROR] El código ya existe en el inventario.");
            Console.ReadKey();
            return;
        }

        Console.Write("Marca: ");
        string marca = Console.ReadLine()?.Trim();

        Console.Write("Modelo: ");
        string modelo = Console.ReadLine()?.Trim();

        Console.Write("Año: ");
        if (!int.TryParse(Console.ReadLine(), out int anio) || anio < 1990 || anio > 2030)
        {
            Console.WriteLine("\n[ERROR] Año inválido.");
            Console.ReadKey();
            return;
        }

        Console.Write("Precio ($): ");
        if (!double.TryParse(Console.ReadLine(), out double precio) || precio <= 0)
        {
            Console.WriteLine("\n[ERROR] Precio inválido.");
            Console.ReadKey();
            return;
        }

        InventarioVehiculos.Add(new Vehiculo
        {
            Codigo = codigo,
            Marca = marca,
            Modelo = modelo,
            Año = anio,
            Precio = precio,
            Disponible = true
        });

        Console.WriteLine("\n[ÉXITO] Vehículo añadido correctamente al inventario.");
        Console.ReadKey();
    }
    private void CrearFactura()
    {
        Console.Clear();
        Console.WriteLine("==============================================");
        Console.WriteLine("              CREAR FACTURA DE VENTA          ");
        Console.WriteLine("==============================================");

        
        Console.Write("Código del vendedor (ej. EMP01): ");
        string codigoVendedor = Console.ReadLine()?.Trim().ToUpper();

        var vendedor = DatosCompartidos.ListaEmpleados
            .FirstOrDefault(e => e.Codigo.Equals(codigoVendedor, StringComparison.OrdinalIgnoreCase)
                               && e.Puesto == "Vendedor");

        if (vendedor == null)
        {
            Console.WriteLine("\n[ERROR] Código no válido o el empleado no es Vendedor.");
            Console.ReadKey();
            return;
        }

        Console.Write("Nombre del cliente: ");
        string cliente = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(cliente))
        {
            Console.WriteLine("\n[ERROR] El nombre del cliente no puede estar vacío.");
            Console.ReadKey();
            return;
        }

     
        Console.Write("Código del vehículo (ej. V01): ");
        string codigoVehiculo = Console.ReadLine()?.Trim().ToUpper();

        var vehiculo = InventarioVehiculos
            .FirstOrDefault(v => v.Codigo.Equals(codigoVehiculo, StringComparison.OrdinalIgnoreCase));

        if (vehiculo == null)
        {
            Console.WriteLine("\n[ERROR] No existe un vehículo con ese código.");
            Console.ReadKey();
            return;
        }

        if (!vehiculo.Disponible)
        {
            Console.WriteLine("\n[ERROR] Este vehículo ya fue vendido y no está disponible.");
            Console.ReadKey();
            return;
        }

        // --- Impuesto ---
        double impuesto = vehiculo.Precio * (PORCENTAJE_IMPUESTO / 100);

        double total = vehiculo.Precio + impuesto;
        int numeroFactura = DatosCompartidos.ListaFacturas.Count + 1;

        var factura = new Factura
        {
            Numero = numeroFactura,
            CodigoVendedor = vendedor.Codigo,
            Cliente = cliente,
            CodigoVehiculo = vehiculo.Codigo,
            DescripcionVehiculo = $"{vehiculo.Marca} {vehiculo.Modelo} {vehiculo.Año}",
            Precio = vehiculo.Precio,
            PorcentajeImpuesto = PORCENTAJE_IMPUESTO,
            Total = total,
            Fecha = DateTime.Now
        };

        // Registro de factura
        DatosCompartidos.ListaFacturas.Add(factura);
        vehiculo.Disponible = false;

        Console.Clear();
        Console.WriteLine("==============================================");
        Console.WriteLine("           FACTURA GENERADA CON ÉXITO         ");
        Console.WriteLine("==============================================");
        Console.WriteLine($"Factura N°:      {factura.Numero}");
        Console.WriteLine($"Fecha:           {factura.Fecha:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Vendedor:        {vendedor.Nombre} ({vendedor.Codigo})");
        Console.WriteLine($"Cliente:         {factura.Cliente}");
        Console.WriteLine($"Vehículo:        {factura.DescripcionVehiculo} ({factura.CodigoVehiculo})");
        Console.WriteLine($"Precio:          ${factura.Precio:F2}");
        Console.WriteLine($"Impuesto ({factura.PorcentajeImpuesto}%): ${impuesto:F2}");
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine($"TOTAL:           ${factura.Total:F2}");
        Console.WriteLine("==============================================");

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void VerFacturas()
    {
        Console.Clear();
        Console.WriteLine("================================================================================");
        Console.WriteLine("                          FACTURAS EMITIDAS                                    ");
        Console.WriteLine("================================================================================");

        if (DatosCompartidos.ListaFacturas.Count == 0)
        {
            Console.WriteLine("No hay facturas registradas todavía.");
        }
        else
        {
            Console.WriteLine(string.Format("{0,-6} {1,-10} {2,-15} {3,-25} {4,-10} {5,-10}",
                "N°", "VENDEDOR", "CLIENTE", "VEHÍCULO", "TOTAL", "FECHA"));
            Console.WriteLine(new string('-', 80));

            foreach (var f in DatosCompartidos.ListaFacturas)
            {
                Console.WriteLine(string.Format("{0,-6} {1,-10} {2,-15} {3,-25} ${4,-9:F2} {5,-10}",
                    f.Numero, f.CodigoVendedor, f.Cliente, f.DescripcionVehiculo, f.Total,
                    f.Fecha.ToString("dd/MM/yyyy")));
            }
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}