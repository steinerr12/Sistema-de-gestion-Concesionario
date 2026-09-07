using System;
using System.Collections.Generic;
using System.Linq;

public class Vehiculo
{
    public string Codigo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int Anio { get; set; }
    public double Precio { get; set; }
    public bool Disponible { get; set; }
}

public class ControlVentasInventario
{
    // Catálogo de stock del concesionario
    public static List<Vehiculo> InventarioVehiculos = new List<Vehiculo>()
    {
        new Vehiculo { Codigo = "V01", Marca = "Toyota", Modelo = "Corolla", Anio = 2024, Precio = 22000.00, Disponible = true },
        new Vehiculo { Codigo = "V02", Marca = "Nissan", Modelo = "Frontier", Anio = 2023, Precio = 28000.00, Disponible = true },
        new Vehiculo { Codigo = "V03", Marca = "Honda", Modelo = "Civic", Anio = 2024, Precio = 25500.00, Disponible = true },
        new Vehiculo { Codigo = "V04", Marca = "Hyundai", Modelo = "Tucson", Anio = 2025, Precio = 31000.00, Disponible = true },
        new Vehiculo { Codigo = "V05", Marca = "Kia", Modelo = "Sportage", Anio = 2023, Precio = 27500.00, Disponible = true }
    };

    private static int correlativoFactura = 1001;

    public void Menu()
    {
        int opcion = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("=================================================");
            Console.WriteLine("     CONTROL DE VENTAS E INVENTARIO DE AUTOS     ");
            Console.WriteLine("=================================================");
            Console.WriteLine("1. Ver inventario general (Disponibilidad)");
            Console.WriteLine("2. Agregar nuevo vehículo al inventario");
            Console.WriteLine("3. Registrar venta y generar factura");
            Console.WriteLine("4. Reporte de ventas por empleado");
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
                        RegistrarVentaYFacturar();
                        break;
                    case 4:
                        ReporteVentasEmpleado();
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
                v.Codigo, v.Marca, v.Modelo, v.Anio, v.Precio, estado));
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
            Anio = anio,
            Precio = precio,
            Disponible = true
        });

        Console.WriteLine("\n[ÉXITO] Vehículo añadido correctamente al inventario.");
        Console.ReadKey();
    }

    private void RegistrarVentaYFacturar()
    {
        Console.Clear();
        Console.WriteLine("==============================================");
        Console.WriteLine("        VENTA DE VEHÍCULO Y FACTURACIÓN       ");
        Console.WriteLine("==============================================");

        // 1. Validar vendedor
        Console.Write("Código del Vendedor (ej. EMP01): ");
        string codVendedor = Console.ReadLine()?.Trim().ToUpper();

        var empleado = DatosCompartidos.ListaEmpleados.FirstOrDefault(e => e.Codigo.Equals(codVendedor, StringComparison.OrdinalIgnoreCase));
        if (empleado == null)
        {
            Console.WriteLine("\n[ERROR] El código de empleado no existe.");
            Console.ReadKey();
            return;
        }

        // 2. Validar vehículo disponible
        Console.Write("Código del Vehículo a vender (ej. V01): ");
        string codVehiculo = Console.ReadLine()?.Trim().ToUpper();

        var vehiculo = InventarioVehiculos.FirstOrDefault(v => v.Codigo.Equals(codVehiculo, StringComparison.OrdinalIgnoreCase));
        if (vehiculo == null)
        {
            Console.WriteLine("\n[ERROR] El vehículo no existe en el sistema.");
            Console.ReadKey();
            return;
        }

        if (!vehiculo.Disponible)
        {
            Console.WriteLine("\n[ALERTA] Este vehículo ya ha sido vendido.");
            Console.ReadKey();
            return;
        }

        // 3. Datos del comprador
        Console.Write("Nombre completo del cliente: ");
        string cliente = Console.ReadLine()?.Trim();

        // 4. Cálculos comerciales
        double subtotal = vehiculo.Precio;
        double tasaIva = 13.0; // 13% IVA
        double impuesto = subtotal * (tasaIva / 100);
        double total = subtotal + impuesto;

        // 5. Generación de factura y actualización de estado
        Factura nuevaFactura = new Factura
        {
            Numero = correlativoFactura++,
            CodigoVendedor = empleado.Codigo,
            Cliente = cliente,
            DescripcionVehiculo = $"{vehiculo.Marca} {vehiculo.Modelo} ({vehiculo.Anio})",
            Precio = subtotal,
            PorcentajeImpuesto = tasaIva,
            Total = total,
            Fecha = DateTime.Now
        };

        DatosCompartidos.ListaFacturas.Add(nuevaFactura);
        vehiculo.Disponible = false; // Se descuenta de la disponibilidad

        // 6. Impresión del comprobante
        Console.Clear();
        Console.WriteLine("============================================================");
        Console.WriteLine($"                FACTURA N°: {nuevaFactura.Numero}           ");
        Console.WriteLine("============================================================");
        Console.WriteLine($"Fecha: {nuevaFactura.Fecha:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Atendido por: {empleado.Nombre} ({empleado.Puesto})");
        Console.WriteLine($"Cliente     : {nuevaFactura.Cliente}");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"Vehículo    : {nuevaFactura.DescripcionVehiculo}");
        Console.WriteLine($"Subtotal    : ${nuevaFactura.Precio,10:F2}");
        Console.WriteLine($"IVA (13%)   : ${impuesto,10:F2}");
        Console.WriteLine($"TOTAL PAGADO: ${nuevaFactura.Total,10:F2}");
        Console.WriteLine("============================================================");
        Console.WriteLine("[ÉXITO] Venta registrada. El bono ya se reflejará en la Nómina.");
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void ReporteVentasEmpleado()
    {
        Console.Clear();
        Console.WriteLine("==========================================================================");
        Console.WriteLine("                      HISTORIAL DE VENTAS POR EMPLEADO                    ");
        Console.WriteLine("==========================================================================");

        if (DatosCompartidos.ListaFacturas.Count == 0)
        {
            Console.WriteLine("No se han emitido facturas en esta sesión.");
        }
        else
        {
            foreach (var fact in DatosCompartidos.ListaFacturas)
            {
                var vendedor = DatosCompartidos.ListaEmpleados.FirstOrDefault(e => e.Codigo.Equals(fact.CodigoVendedor, StringComparison.OrdinalIgnoreCase));
                string nombre = vendedor != null ? vendedor.Nombre : fact.CodigoVendedor;

                Console.WriteLine($"Factura #{fact.Numero} | Vendedor: {nombre} ({fact.CodigoVendedor})");
                Console.WriteLine($"Cliente: {fact.Cliente,-20} | Auto: {fact.DescripcionVehiculo}");
                Console.WriteLine($"Monto Total: ${fact.Total:F2} | Fecha: {fact.Fecha:dd/MM/yyyy}");
                Console.WriteLine(new string('-', 74));
            }
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}