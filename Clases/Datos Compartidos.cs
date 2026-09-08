using System;
using System.Collections.Generic;

public class Empleado
{
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Puesto { get; set; }
    public double TarifaHora { get; set; }
}

public class RegistroAsistencia
{
    public string CodigoEmpleado { get; set; }
    public DateTime Entrada { get; set; }
    public DateTime? Salida { get; set; }
    public bool Tarde { get; set; }
}

public class SolicitudVacacion
{
    public string CodigoEmpleado { get; set; }
    public string Puesto { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}
public class Turno
{
    public string CodigoEmpleado { get; set; }
    public string Dia { get; set; }
    public string HoraInicio { get; set; }
    public string HoraFin { get; set; }
}

public class Factura
{
    public int Numero { get; set; }
    public string CodigoVendedor { get; set; }
    public string Cliente { get; set; }
    public string CodigoVehiculo { get; set; }
    public string DescripcionVehiculo { get; set; }
    public double Precio { get; set; }
    public double PorcentajeImpuesto { get; set; }
    public double Total { get; set; }
    public DateTime Fecha { get; set; }
}


public static class DatosCompartidos
{
    // Los 5 empleados oficiales que todos los módulos deben usar
    public static List<Empleado> ListaEmpleados = new List<Empleado>()
    {
        new Empleado { Codigo = "EMP01", Nombre = "Carlos Pérez", Puesto = "Vendedor", TarifaHora = 12.50 },
        new Empleado { Codigo = "EMP02", Nombre = "María Gómez", Puesto = "Vendedor", TarifaHora = 12.50 },
        new Empleado { Codigo = "EMP03", Nombre = "Juan Rodríguez", Puesto = "Mecánico", TarifaHora = 15.00 },
        new Empleado { Codigo = "EMP04", Nombre = "Ana Martínez", Puesto = "Administrativo", TarifaHora = 14.00 },
        new Empleado { Codigo = "EMP05", Nombre = "Luis Fernández", Puesto = "Mecánico", TarifaHora = 15.00 }
    };

    // Listas donde se guardará la información compartida
    public static List<RegistroAsistencia> HistorialAsistencias = new List<RegistroAsistencia>();
    public static List<SolicitudVacacion> SolicitudesVacaciones = new List<SolicitudVacacion>();
    public static List<Turno> ListaTurnos = new List<Turno>();
    public static List<Factura> ListaFacturas = new List<Factura>();
}