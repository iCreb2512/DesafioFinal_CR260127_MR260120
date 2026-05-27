// ============================================================
// DESAFÍO FINAL - PROGRAMACIÓN DE ALGORITMOS - CICLO I 2026
// Sistema Integral de Gestión de Biblioteca Universitaria
// Universidad Don Bosco
// Versión IA - Reescritura profesional con mejoras estructurales
// ============================================================

/*Requisitos técnicos: usar structs para las tres entidades, arreglos estáticos con capacidad máxima de 10 libros, 5 usuarios y 10 préstamos (sin usar List ni clases),
 persistencia en archivos CSV y TXT, y navegación por menús y submenús en consola. Debe incluir: registro con validaciones completas (unicidad de códigos, formato de correo, rango de años, campos obligatorios), 
 búsqueda por código y por nombre parcial, eliminación desplazando el arreglo, gestión de devoluciones que restaure inventario, reporte de libros agrupados por categoría usando una matriz bidimensional,
y exportación de préstamos activos a un archivo de texto. Usa convenciones de C# con PascalCase, constantes para los límites, colores en consola para mensajes de éxito y error, y comenta las estructuras de control principales
(for, while, do-while, switch) porque es un proyecto académico universitario.*/


using System;
using System.IO;

// ============================================================
// ESTRUCTURAS DE DATOS
// Cada struct modela una entidad del dominio del sistema.
// ============================================================

/// <summary>Representa un libro del catálogo de la biblioteca.</summary>
struct Libro
{
    public string Codigo;               // Código alfanumérico de 8 caracteres (ej. LIB00001)
    public string Titulo;               // Título completo del libro
    public string Autor;                // Nombre del autor
    public string Editorial;            // Editorial que publicó el libro
    public int    AnioPublicacion;      // Año de publicación (1900 - año actual)
    public string Categoria;            // Categoría: Ciencias, Literatura, Historia, Tecnología, Otros
    public int    EjemplaresDisponibles;// Cantidad de ejemplares disponibles en inventario
}

/// <summary>Representa un estudiante registrado en el sistema.</summary>
struct Usuario
{
    public string Carne;    // Carné universitario de 8 dígitos numéricos
    public string Nombre;   // Nombre completo
    public string Carrera;  // Carrera que cursa
    public string Correo;   // Correo electrónico válido (debe contener @ y punto)
    public string Telefono; // Número de teléfono de contacto
    public string Estado;   // "activo" | "inactivo"
}

/// <summary>Representa el registro de un préstamo de libro.</summary>
struct Prestamo
{
    public string CodigoPrestamo;  // Código único generado automáticamente (P0000001)
    public string CarneUsuario;    // Carné del usuario que realiza el préstamo
    public string CodigoLibro;     // Código del libro prestado
    public string FechaPrestamo;   // Fecha del préstamo (dd/mm/yyyy)
    public string FechaDevolucion; // Fecha estimada de devolución (dd/mm/yyyy)
    public string EstadoPrestamo;  // "activo" | "devuelto"
}

// ============================================================
// CLASE PRINCIPAL
// ============================================================
class Program
{
    // --------------------------------------------------------
    // CONSTANTES DE CAPACIDAD
    // --------------------------------------------------------
    const int MAX_LIBROS   = 10;
    const int MAX_USUARIOS = 5;
    const int MAX_PRESTAMOS= 10;

    // --------------------------------------------------------
    // ARREGLOS EN MEMORIA (estructuras de datos del sistema)
    // --------------------------------------------------------
    static Libro[]   libros   = new Libro[MAX_LIBROS];
    static Usuario[] usuarios = new Usuario[MAX_USUARIOS];
    static Prestamo[] prestamos= new Prestamo[MAX_PRESTAMOS];

    static int totalLibros    = 0;
    static int totalUsuarios  = 0;
    static int totalPrestamos = 0;

    // --------------------------------------------------------
    // RUTAS DE ARCHIVOS DE PERSISTENCIA
    // --------------------------------------------------------
    static readonly string RUTA_LIBROS    = "Data/libros.csv";
    static readonly string RUTA_USUARIOS  = "Data/usuarios.txt";
    static readonly string RUTA_PRESTAMOS = "Data/prestamos.txt";

    // ============================================================
    // PUNTO DE ENTRADA DEL PROGRAMA
    // ============================================================
    static void Main(string[] args)
    {
        InicializarSistema();

        bool salir = false;

        // DO-WHILE: muestra el menú al menos una vez y repite hasta que el usuario decida salir
        do
        {
            MostrarMenuPrincipal();
            string opcion = Console.ReadLine()?.Trim() ?? "";

            // SWITCH-CASE: enruta la opción al módulo correspondiente
            switch (opcion)
            {
                case "1": MenuLibros();    break;
                case "2": MenuUsuarios();  break;
                case "3": MenuPrestamos(); break;
                case "4":
                    PersistirTodo();
                    Exito("Datos guardados. ¡Hasta luego!");
                    salir = true;
                    break;
                default:
                    Error("Opción inválida. Ingrese un número del 1 al 4.");
                    break;
            }

        } while (!salir);
    }

    // ============================================================
    // INICIALIZACIÓN
    // Crea la carpeta de datos y carga los archivos al arranque.
    // ============================================================
    static void InicializarSistema()
    {
        if (!Directory.Exists("Data"))
            Directory.CreateDirectory("Data");

        CargarLibros();
        CargarUsuarios();
        CargarPrestamos();

        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║     SISTEMA DE BIBLIOTECA UNIVERSITARIA              ║");
        Console.WriteLine("║     Universidad Don Bosco  |  Ciclo I 2026           ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine($"  Libros cargados: {totalLibros} | Usuarios: {totalUsuarios} | Préstamos: {totalPrestamos}");
    }

    // ============================================================
    // MENÚ PRINCIPAL
    // ============================================================
    static void MostrarMenuPrincipal()
    {
        Console.WriteLine("\n┌─────────────────────────────────────┐");
        Console.WriteLine("│          MENÚ PRINCIPAL             │");
        Console.WriteLine("├─────────────────────────────────────┤");
        Console.WriteLine("│  1. Gestión de Libros               │");
        Console.WriteLine("│  2. Gestión de Usuarios             │");
        Console.WriteLine("│  3. Gestión de Préstamos            │");
        Console.WriteLine("│  4. Guardar y Salir                 │");
        Console.WriteLine("└─────────────────────────────────────┘");
        Console.Write("  Opción: ");
    }

    // ============================================================
    // MÓDULO A — GESTIÓN DE LIBROS
    // ============================================================
    static void MenuLibros()
    {
        bool volver = false;

        // DO-WHILE: submenú repite hasta que el usuario elija volver
        do
        {
            Console.WriteLine("\n┌─────────────────────────────────────┐");
            Console.WriteLine("│         GESTIÓN DE LIBROS           │");
            Console.WriteLine("├─────────────────────────────────────┤");
            Console.WriteLine("│  1. Registrar libro                 │");
            Console.WriteLine("│  2. Buscar por código               │");
            Console.WriteLine("│  3. Listar todos                    │");
            Console.WriteLine("│  4. Eliminar libro                  │");
            Console.WriteLine("│  5. Reporte por categorías          │");
            Console.WriteLine("│  6. Volver                          │");
            Console.WriteLine("└─────────────────────────────────────┘");
            Console.Write("  Opción: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RegistrarLibro();          break;
                case "2": BuscarLibroPorCodigo();    break;
                case "3": ListarLibros();            break;
                case "4": EliminarLibro();           break;
                case "5": ReporteCategorias();       break;
                case "6": volver = true;             break;
                default:  Error("Opción inválida."); break;
            }

        } while (!volver);
    }

    // ------------------------------------------------------------
    // Registrar libro con validaciones completas
    // ------------------------------------------------------------
    static void RegistrarLibro()
    {
        // Verificar capacidad del arreglo antes de continuar
        if (totalLibros >= MAX_LIBROS)
        {
            Error($"Capacidad máxima alcanzada ({MAX_LIBROS} libros).");
            return;
        }

        Titulo("REGISTRAR NUEVO LIBRO");
        Libro libro = new Libro();

        // --- Validación del código (8 caracteres alfanuméricos, único) ---
        while (true)
        {
            Console.Write("  Código (8 alfanuméricos, ej. LIB00001): ");
            libro.Codigo = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (libro.Codigo.Length != 8)
                Error("El código debe tener exactamente 8 caracteres.");
            else if (!EsAlfanumerico(libro.Codigo))
                Error("El código solo puede contener letras y números.");
            else if (BuscarIndiceLibro(libro.Codigo) != -1)
                Error("Ya existe un libro con ese código.");
            else
                break; // Código válido y único
        }

        libro.Titulo     = LeerObligatorio("  Título       : ");
        libro.Autor      = LeerObligatorio("  Autor        : ");
        libro.Editorial  = LeerObligatorio("  Editorial    : ");
        libro.AnioPublicacion      = LeerEnteroRango("  Año (1900-" + DateTime.Now.Year + "): ", 1900, DateTime.Now.Year);
        libro.Categoria  = LeerObligatorio("  Categoría    : ");
        libro.EjemplaresDisponibles= LeerEnteroPositivo("  Ejemplares   : ");

        // Guardar en el arreglo e incrementar contador
        libros[totalLibros] = libro;
        totalLibros++;

        Exito($"Libro '{libro.Titulo}' registrado. ({totalLibros}/{MAX_LIBROS} espacios usados)");
    }

    // ------------------------------------------------------------
    // Buscar libro por código e imprimir sus datos
    // ------------------------------------------------------------
    static void BuscarLibroPorCodigo()
    {
        Console.Write("\n  Código del libro: ");
        string codigo = Console.ReadLine()?.Trim().ToUpper() ?? "";

        int idx = BuscarIndiceLibro(codigo);

        if (idx != -1)
            ImprimirLibro(libros[idx]);
        else
            Error("No se encontró ningún libro con ese código.");
    }

    // ------------------------------------------------------------
    // Listar todos los libros usando un ciclo FOR
    // ------------------------------------------------------------
    static void ListarLibros()
    {
        if (totalLibros == 0) { Error("No hay libros registrados."); return; }

        Titulo($"CATÁLOGO DE LIBROS  [{totalLibros}/{MAX_LIBROS}]");

        // FOR: recorre el arreglo desde la posición 0 hasta totalLibros - 1
        for (int i = 0; i < totalLibros; i++)
        {
            Console.WriteLine($"\n  ─── Libro #{i + 1} ───────────────────────────");
            ImprimirLibro(libros[i]);
        }
    }

    // ------------------------------------------------------------
    // Eliminar libro desplazando el arreglo (sin huecos)
    // ------------------------------------------------------------
    static void EliminarLibro()
    {
        Console.Write("\n  Código del libro a eliminar: ");
        string codigo = Console.ReadLine()?.Trim().ToUpper() ?? "";

        int idx = BuscarIndiceLibro(codigo);
        if (idx == -1) { Error("No se encontró ningún libro con ese código."); return; }

        string titulo = libros[idx].Titulo;

        // FOR: desplaza todos los elementos posteriores una posición hacia la izquierda
        for (int i = idx; i < totalLibros - 1; i++)
            libros[i] = libros[i + 1];

        totalLibros--;
        Exito($"Libro '{titulo}' eliminado correctamente.");
    }

    // ------------------------------------------------------------
    // Búsqueda lineal: retorna índice o -1
    // ------------------------------------------------------------
    static int BuscarIndiceLibro(string codigo)
    {
        // FOR: recorre todos los libros registrados en búsqueda lineal
        for (int i = 0; i < totalLibros; i++)
            if (libros[i].Codigo == codigo) return i;
        return -1;
    }

    // ------------------------------------------------------------
    // Imprime todos los campos de un libro formateado
    // ------------------------------------------------------------
    static void ImprimirLibro(Libro l)
    {
        Console.WriteLine($"  Código      : {l.Codigo}");
        Console.WriteLine($"  Título      : {l.Titulo}");
        Console.WriteLine($"  Autor       : {l.Autor}");
        Console.WriteLine($"  Editorial   : {l.Editorial}");
        Console.WriteLine($"  Año         : {l.AnioPublicacion}");
        Console.WriteLine($"  Categoría   : {l.Categoria}");
        Console.WriteLine($"  Ejemplares  : {l.EjemplaresDisponibles}");
    }

    // ============================================================
    // MÓDULO B — GESTIÓN DE USUARIOS
    // ============================================================
    static void MenuUsuarios()
    {
        bool volver = false;

        do
        {
            Console.WriteLine("\n┌─────────────────────────────────────┐");
            Console.WriteLine("│        GESTIÓN DE USUARIOS          │");
            Console.WriteLine("├─────────────────────────────────────┤");
            Console.WriteLine("│  1. Registrar usuario               │");
            Console.WriteLine("│  2. Buscar por carné                │");
            Console.WriteLine("│  3. Buscar por nombre               │");
            Console.WriteLine("│  4. Listar todos                    │");
            Console.WriteLine("│  5. Cambiar estado de usuario       │");
            Console.WriteLine("│  6. Volver                          │");
            Console.WriteLine("└─────────────────────────────────────┘");
            Console.Write("  Opción: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RegistrarUsuario();        break;
                case "2": BuscarUsuarioCarne();      break;
                case "3": BuscarUsuarioNombre();     break;
                case "4": ListarUsuarios();          break;
                case "5": CambiarEstadoUsuario();    break;
                case "6": volver = true;             break;
                default:  Error("Opción inválida."); break;
            }

        } while (!volver);
    }

    // ------------------------------------------------------------
    // Registrar usuario con validaciones
    // ------------------------------------------------------------
    static void RegistrarUsuario()
    {
        if (totalUsuarios >= MAX_USUARIOS)
        {
            Error($"Capacidad máxima alcanzada ({MAX_USUARIOS} usuarios).");
            return;
        }

        Titulo("REGISTRAR NUEVO USUARIO");
        Usuario u = new Usuario();

        // --- Validación del carné (8 dígitos numéricos, único) ---
        while (true)
        {
            Console.Write("  Carné (8 dígitos): ");
            u.Carne = Console.ReadLine()?.Trim() ?? "";

            if (u.Carne.Length != 8)
                Error("El carné debe tener exactamente 8 dígitos.");
            else if (!EsSoloDigitos(u.Carne))
                Error("El carné solo puede contener dígitos numéricos.");
            else if (BuscarIndiceUsuario(u.Carne) != -1)
                Error("Ya existe un usuario con ese carné.");
            else
                break; // Carné válido y único
        }

        u.Nombre   = LeerObligatorio("  Nombre completo : ");
        u.Carrera  = LeerObligatorio("  Carrera         : ");

        // --- Validación del correo (debe contener @ y punto después del @) ---
        while (true)
        {
            Console.Write("  Correo electrónico: ");
            u.Correo = Console.ReadLine()?.Trim() ?? "";
            int posAt = u.Correo.IndexOf('@');

            if (posAt == -1)
                Error("El correo debe contener '@'.");
            else if (u.Correo.IndexOf('.', posAt) == -1)
                Error("El correo debe tener un punto después del '@'.");
            else
                break; // Correo válido
        }

        u.Telefono = LeerObligatorio("  Teléfono        : ");
        u.Estado   = "activo"; // Los nuevos usuarios son activos por defecto

        usuarios[totalUsuarios] = u;
        totalUsuarios++;

        Exito($"Usuario '{u.Nombre}' registrado correctamente con carné {u.Carne}.");
    }

    // ------------------------------------------------------------
    // Buscar usuario por carné exacto
    // ------------------------------------------------------------
    static void BuscarUsuarioCarne()
    {
        Console.Write("\n  Carné del usuario: ");
        string carne = Console.ReadLine()?.Trim() ?? "";
        int idx = BuscarIndiceUsuario(carne);

        if (idx != -1) ImprimirUsuario(usuarios[idx]);
        else Error("No se encontró ningún usuario con ese carné.");
    }

    // ------------------------------------------------------------
    // Buscar usuario por nombre (búsqueda parcial, case-insensitive)
    // ------------------------------------------------------------
    static void BuscarUsuarioNombre()
    {
        Console.Write("\n  Nombre o parte del nombre: ");
        string termino = Console.ReadLine()?.Trim().ToLower() ?? "";
        bool encontrado = false;

        // FOR: recorre todos los usuarios para búsqueda parcial
        for (int i = 0; i < totalUsuarios; i++)
        {
            if (usuarios[i].Nombre.ToLower().Contains(termino))
            {
                ImprimirUsuario(usuarios[i]);
                encontrado = true;
            }
        }

        if (!encontrado) Error("No se encontraron usuarios con ese nombre.");
    }

    // ------------------------------------------------------------
    // Listar todos los usuarios
    // ------------------------------------------------------------
    static void ListarUsuarios()
    {
        if (totalUsuarios == 0) { Error("No hay usuarios registrados."); return; }

        Titulo($"LISTADO DE USUARIOS  [{totalUsuarios}/{MAX_USUARIOS}]");

        for (int i = 0; i < totalUsuarios; i++)
        {
            Console.WriteLine($"\n  ─── Usuario #{i + 1} ──────────────────────────");
            ImprimirUsuario(usuarios[i]);
        }
    }

    // ------------------------------------------------------------
    // Cambiar el estado activo/inactivo de un usuario
    // (funcionalidad adicional: bloquear usuarios morosos)
    // ------------------------------------------------------------
    static void CambiarEstadoUsuario()
    {
        Console.Write("\n  Carné del usuario: ");
        string carne = Console.ReadLine()?.Trim() ?? "";
        int idx = BuscarIndiceUsuario(carne);

        if (idx == -1) { Error("No se encontró ningún usuario con ese carné."); return; }

        Console.WriteLine($"  Estado actual: {usuarios[idx].Estado}");
        Console.WriteLine("  1. Activo  |  2. Inactivo");
        Console.Write("  Nuevo estado: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                usuarios[idx].Estado = "activo";
                Exito("Estado actualizado a 'activo'.");
                break;
            case "2":
                usuarios[idx].Estado = "inactivo";
                Exito("Estado actualizado a 'inactivo'.");
                break;
            default:
                Error("Opción inválida.");
                break;
        }
    }

    // ------------------------------------------------------------
    // Búsqueda lineal de usuario por carné: retorna índice o -1
    // ------------------------------------------------------------
    static int BuscarIndiceUsuario(string carne)
    {
        for (int i = 0; i < totalUsuarios; i++)
            if (usuarios[i].Carne == carne) return i;
        return -1;
    }

    // ------------------------------------------------------------
    // Imprime todos los campos de un usuario formateado
    // ------------------------------------------------------------
    static void ImprimirUsuario(Usuario u)
    {
        Console.WriteLine($"  Carné     : {u.Carne}");
        Console.WriteLine($"  Nombre    : {u.Nombre}");
        Console.WriteLine($"  Carrera   : {u.Carrera}");
        Console.WriteLine($"  Correo    : {u.Correo}");
        Console.WriteLine($"  Teléfono  : {u.Telefono}");
        Console.WriteLine($"  Estado    : {u.Estado.ToUpper()}");
    }

    // ============================================================
    // MÓDULO C — GESTIÓN DE PRÉSTAMOS
    // ============================================================
    static void MenuPrestamos()
    {
        bool volver = false;

        do
        {
            Console.WriteLine("\n┌─────────────────────────────────────┐");
            Console.WriteLine("│       GESTIÓN DE PRÉSTAMOS          │");
            Console.WriteLine("├─────────────────────────────────────┤");
            Console.WriteLine("│  1. Registrar préstamo              │");
            Console.WriteLine("│  2. Registrar devolución            │");
            Console.WriteLine("│  3. Préstamos activos de usuario    │");
            Console.WriteLine("│  4. Actualizar estado de préstamo   │");
            Console.WriteLine("│  5. Exportar reporte activos        │");
            Console.WriteLine("│  6. Volver                          │");
            Console.WriteLine("└─────────────────────────────────────┘");
            Console.Write("  Opción: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RegistrarPrestamo();       break;
                case "2": RegistrarDevolucion();     break;
                case "3": PrestamosActivosUsuario(); break;
                case "4": ActualizarEstadoPrestamo();break;
                case "5": ExportarReporteActivos();  break;
                case "6": volver = true;             break;
                default:  Error("Opción inválida."); break;
            }

        } while (!volver);
    }

    // ------------------------------------------------------------
    // Registrar un nuevo préstamo con todas las validaciones de negocio
    // ------------------------------------------------------------
    static void RegistrarPrestamo()
    {
        if (totalPrestamos >= MAX_PRESTAMOS)
        {
            Error($"Capacidad máxima de préstamos alcanzada ({MAX_PRESTAMOS}).");
            return;
        }

        Titulo("REGISTRAR NUEVO PRÉSTAMO");

        // 1. Validar que el usuario existe y está activo
        Console.Write("  Carné del usuario : ");
        string carne = Console.ReadLine()?.Trim() ?? "";
        int idxUsuario = BuscarIndiceUsuario(carne);

        if (idxUsuario == -1)       { Error("No existe ningún usuario con ese carné.");               return; }
        if (usuarios[idxUsuario].Estado != "activo") { Error("El usuario está inactivo.");             return; }

        // 2. Validar que el libro existe y tiene ejemplares disponibles
        Console.Write("  Código del libro  : ");
        string codigoLibro = Console.ReadLine()?.Trim().ToUpper() ?? "";
        int idxLibro = BuscarIndiceLibro(codigoLibro);

        if (idxLibro == -1)                                    { Error("No existe ningún libro con ese código.");         return; }
        if (libros[idxLibro].EjemplaresDisponibles <= 0)       { Error("No hay ejemplares disponibles de ese libro.");    return; }

        // 3. Capturar fechas con formato validado
        string fechaPrestamo   = LeerFecha("  Fecha préstamo (dd/mm/yyyy)    : ");
        string fechaDevolucion = LeerFecha("  Fecha devolución (dd/mm/yyyy)  : ");

        // 4. Construir el objeto préstamo
        Prestamo p = new Prestamo();
        p.CodigoPrestamo  = "P" + (totalPrestamos + 1).ToString("D7"); // Código autoincremental: P0000001
        p.CarneUsuario    = carne;
        p.CodigoLibro     = codigoLibro;
        p.FechaPrestamo   = fechaPrestamo;
        p.FechaDevolucion = fechaDevolucion;
        p.EstadoPrestamo  = "activo";

        // 5. Actualizar inventario y guardar
        libros[idxLibro].EjemplaresDisponibles--;
        prestamos[totalPrestamos] = p;
        totalPrestamos++;

        Exito($"Préstamo {p.CodigoPrestamo} registrado.");
        Console.WriteLine($"  Libro: {libros[idxLibro].Titulo} — Quedan {libros[idxLibro].EjemplaresDisponibles} ejemplar(es).");
    }

    // ------------------------------------------------------------
    // Registrar devolución de un libro
    // ------------------------------------------------------------
    static void RegistrarDevolucion()
    {
        Console.Write("\n  Código del préstamo: ");
        string codigo = Console.ReadLine()?.Trim().ToUpper() ?? "";

        int idx = BuscarIndicePrestamo(codigo);
        if (idx == -1)                                         { Error("No se encontró ningún préstamo con ese código."); return; }
        if (prestamos[idx].EstadoPrestamo == "devuelto")       { Error("Este préstamo ya fue devuelto anteriormente.");   return; }

        // Marcar como devuelto y reponer ejemplar al inventario
        prestamos[idx].EstadoPrestamo = "devuelto";
        int idxLibro = BuscarIndiceLibro(prestamos[idx].CodigoLibro);

        if (idxLibro != -1)
        {
            libros[idxLibro].EjemplaresDisponibles++;
            Exito($"Devolución registrada. Libro '{libros[idxLibro].Titulo}' → {libros[idxLibro].EjemplaresDisponibles} ejemplar(es) disponibles.");
        }
        else
        {
            Exito("Devolución registrada (libro ya no figura en catálogo).");
        }
    }

    // ------------------------------------------------------------
    // Mostrar préstamos activos de un usuario específico
    // ------------------------------------------------------------
    static void PrestamosActivosUsuario()
    {
        Console.Write("\n  Carné del usuario: ");
        string carne = Console.ReadLine()?.Trim() ?? "";

        Titulo($"PRÉSTAMOS ACTIVOS — USUARIO {carne}");
        bool hayResultados = false;

        // FOR: recorre todos los préstamos filtrando por carné y estado activo
        for (int i = 0; i < totalPrestamos; i++)
        {
            if (prestamos[i].CarneUsuario == carne && prestamos[i].EstadoPrestamo == "activo")
            {
                Console.WriteLine($"  Código   : {prestamos[i].CodigoPrestamo}");
                Console.WriteLine($"  Libro    : {prestamos[i].CodigoLibro}");
                Console.WriteLine($"  Prestado : {prestamos[i].FechaPrestamo}");
                Console.WriteLine($"  Devolver : {prestamos[i].FechaDevolucion}");
                Console.WriteLine("  ─────────────────────────────────────────");
                hayResultados = true;
            }
        }

        if (!hayResultados) Console.WriteLine("  Este usuario no tiene préstamos activos.");
    }

    // ------------------------------------------------------------
    // Actualizar el estado de un préstamo manualmente
    // ------------------------------------------------------------
    static void ActualizarEstadoPrestamo()
    {
        Console.Write("\n  Código del préstamo: ");
        string codigo = Console.ReadLine()?.Trim().ToUpper() ?? "";

        int idx = BuscarIndicePrestamo(codigo);
        if (idx == -1) { Error("Préstamo no encontrado."); return; }

        Console.WriteLine($"  Estado actual: {prestamos[idx].EstadoPrestamo}");
        Console.WriteLine("  1. Activo  |  2. Devuelto");
        Console.Write("  Nuevo estado: ");

        // SWITCH-CASE: actualiza el estado según la opción elegida
        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                prestamos[idx].EstadoPrestamo = "activo";
                Exito("Estado actualizado a 'activo'.");
                break;
            case "2":
                prestamos[idx].EstadoPrestamo = "devuelto";
                Exito("Estado actualizado a 'devuelto'.");
                break;
            default:
                Error("Opción inválida.");
                break;
        }
    }

    // ------------------------------------------------------------
    // Exportar reporte de préstamos activos a archivo .txt
    // ------------------------------------------------------------
    static void ExportarReporteActivos()
    {
        string ruta = "Data/reporte_prestamos_activos.txt";

        try
        {
            StreamWriter sw = new StreamWriter(ruta);
            sw.WriteLine("╔══════════════════════════════════════════════╗");
            sw.WriteLine("║   REPORTE DE PRÉSTAMOS ACTIVOS               ║");
            sw.WriteLine($"║   Generado: {DateTime.Now:dd/MM/yyyy HH:mm}                  ║");
            sw.WriteLine("╚══════════════════════════════════════════════╝");
            sw.WriteLine();

            int contador = 0;

            // FOR: escribe en el archivo cada préstamo en estado activo
            for (int i = 0; i < totalPrestamos; i++)
            {
                if (prestamos[i].EstadoPrestamo == "activo")
                {
                    sw.WriteLine($"  [{contador + 1}] Código : {prestamos[i].CodigoPrestamo}");
                    sw.WriteLine($"      Usuario  : {prestamos[i].CarneUsuario}");
                    sw.WriteLine($"      Libro    : {prestamos[i].CodigoLibro}");
                    sw.WriteLine($"      Devolver : {prestamos[i].FechaDevolucion}");
                    sw.WriteLine();
                    contador++;
                }
            }

            sw.WriteLine($"  Total préstamos activos: {contador}");
            sw.Close();

            Exito($"Reporte exportado exitosamente → {ruta}");
        }
        catch (Exception ex)
        {
            Error($"No se pudo exportar el reporte: {ex.Message}");
        }
    }

    // ------------------------------------------------------------
    // Búsqueda lineal de préstamo por código: retorna índice o -1
    // ------------------------------------------------------------
    static int BuscarIndicePrestamo(string codigo)
    {
        for (int i = 0; i < totalPrestamos; i++)
            if (prestamos[i].CodigoPrestamo == codigo) return i;
        return -1;
    }

    // ============================================================
    // MÓDULO D — REPORTE DE CATEGORÍAS (USO DE MATRIZ)
    // ============================================================
    static void ReporteCategorias()
    {
        // Categorías predefinidas del sistema
        string[] categorias = { "Ciencias", "Literatura", "Historia", "Tecnología", "Otros" };

        // MATRIZ [5 filas x 2 columnas]:
        //   columna 0 = cantidad de títulos en la categoría
        //   columna 1 = total de ejemplares en la categoría
        int[,] matriz = new int[5, 2];

        // FOR externo: recorre todos los libros
        for (int i = 0; i < totalLibros; i++)
        {
            bool categorizado = false;

            // FOR interno: busca en qué categoría encaja el libro
            for (int j = 0; j < categorias.Length - 1; j++)
            {
                if (libros[i].Categoria.ToLower() == categorias[j].ToLower())
                {
                    matriz[j, 0]++;                                    // Sumar título
                    matriz[j, 1] += libros[i].EjemplaresDisponibles;  // Sumar ejemplares
                    categorizado = true;
                    break;
                }
            }

            // Si no coincidió con ninguna categoría → va a "Otros"
            if (!categorizado)
            {
                matriz[4, 0]++;
                matriz[4, 1] += libros[i].EjemplaresDisponibles;
            }
        }

        Titulo("REPORTE POR CATEGORÍAS");
        Console.WriteLine($"  {"Categoría",-14} {"Títulos",8} {"Ejemplares",12}");
        Console.WriteLine("  " + new string('─', 36));

        // FOR: imprime la matriz fila por fila
        for (int j = 0; j < categorias.Length; j++)
            Console.WriteLine($"  {categorias[j],-14} {matriz[j, 0],8} {matriz[j, 1],12}");

        Console.WriteLine("  " + new string('─', 36));
        Console.WriteLine($"  {"TOTAL",-14} {totalLibros,8}");
    }

    // ============================================================
    // PERSISTENCIA — ARCHIVOS
    // ============================================================

    /// <summary>Guarda libros en formato CSV.</summary>
    static void GuardarLibros()
    {
        try
        {
            using StreamWriter sw = new StreamWriter(RUTA_LIBROS);
            for (int i = 0; i < totalLibros; i++)
                sw.WriteLine($"{libros[i].Codigo},{libros[i].Titulo},{libros[i].Autor},{libros[i].Editorial},{libros[i].AnioPublicacion},{libros[i].Categoria},{libros[i].EjemplaresDisponibles}");
        }
        catch (Exception ex) { Error($"Error al guardar libros: {ex.Message}"); }
    }

    /// <summary>Carga libros desde el CSV al iniciar.</summary>
    static void CargarLibros()
    {
        if (!File.Exists(RUTA_LIBROS)) return;

        try
        {
            using StreamReader sr = new StreamReader(RUTA_LIBROS);
            string linea;

            // WHILE: lee línea a línea hasta EOF o hasta llenar el arreglo
            while ((linea = sr.ReadLine()) != null && totalLibros < MAX_LIBROS)
            {
                string[] p = linea.Split(',');
                if (p.Length == 7)
                {
                    libros[totalLibros].Codigo               = p[0];
                    libros[totalLibros].Titulo               = p[1];
                    libros[totalLibros].Autor                = p[2];
                    libros[totalLibros].Editorial            = p[3];
                    libros[totalLibros].AnioPublicacion      = int.Parse(p[4]);
                    libros[totalLibros].Categoria            = p[5];
                    libros[totalLibros].EjemplaresDisponibles= int.Parse(p[6]);
                    totalLibros++;
                }
            }
        }
        catch (Exception ex) { Error($"Error al cargar libros: {ex.Message}"); }
    }

    /// <summary>Guarda usuarios en formato pipe-separated.</summary>
    static void GuardarUsuarios()
    {
        try
        {
            using StreamWriter sw = new StreamWriter(RUTA_USUARIOS);
            for (int i = 0; i < totalUsuarios; i++)
                sw.WriteLine($"{usuarios[i].Carne}|{usuarios[i].Nombre}|{usuarios[i].Carrera}|{usuarios[i].Correo}|{usuarios[i].Telefono}|{usuarios[i].Estado}");
        }
        catch (Exception ex) { Error($"Error al guardar usuarios: {ex.Message}"); }
    }

    /// <summary>Carga usuarios desde el archivo al iniciar.</summary>
    static void CargarUsuarios()
    {
        if (!File.Exists(RUTA_USUARIOS)) return;

        try
        {
            using StreamReader sr = new StreamReader(RUTA_USUARIOS);
            string linea;

            while ((linea = sr.ReadLine()) != null && totalUsuarios < MAX_USUARIOS)
            {
                string[] p = linea.Split('|');
                if (p.Length == 6)
                {
                    usuarios[totalUsuarios].Carne    = p[0];
                    usuarios[totalUsuarios].Nombre   = p[1];
                    usuarios[totalUsuarios].Carrera  = p[2];
                    usuarios[totalUsuarios].Correo   = p[3];
                    usuarios[totalUsuarios].Telefono = p[4];
                    usuarios[totalUsuarios].Estado   = p[5];
                    totalUsuarios++;
                }
            }
        }
        catch (Exception ex) { Error($"Error al cargar usuarios: {ex.Message}"); }
    }

    /// <summary>Guarda préstamos en formato pipe-separated.</summary>
    static void GuardarPrestamos()
    {
        try
        {
            using StreamWriter sw = new StreamWriter(RUTA_PRESTAMOS);
            for (int i = 0; i < totalPrestamos; i++)
                sw.WriteLine($"{prestamos[i].CodigoPrestamo}|{prestamos[i].CarneUsuario}|{prestamos[i].CodigoLibro}|{prestamos[i].FechaPrestamo}|{prestamos[i].FechaDevolucion}|{prestamos[i].EstadoPrestamo}");
        }
        catch (Exception ex) { Error($"Error al guardar préstamos: {ex.Message}"); }
    }

    /// <summary>Carga préstamos desde el archivo al iniciar.</summary>
    static void CargarPrestamos()
    {
        if (!File.Exists(RUTA_PRESTAMOS)) return;

        try
        {
            using StreamReader sr = new StreamReader(RUTA_PRESTAMOS);
            string linea;

            while ((linea = sr.ReadLine()) != null && totalPrestamos < MAX_PRESTAMOS)
            {
                string[] p = linea.Split('|');
                if (p.Length == 6)
                {
                    prestamos[totalPrestamos].CodigoPrestamo  = p[0];
                    prestamos[totalPrestamos].CarneUsuario    = p[1];
                    prestamos[totalPrestamos].CodigoLibro     = p[2];
                    prestamos[totalPrestamos].FechaPrestamo   = p[3];
                    prestamos[totalPrestamos].FechaDevolucion = p[4];
                    prestamos[totalPrestamos].EstadoPrestamo  = p[5];
                    totalPrestamos++;
                }
            }
        }
        catch (Exception ex) { Error($"Error al cargar préstamos: {ex.Message}"); }
    }

    /// <summary>Persiste los tres arreglos al disco antes de salir.</summary>
    static void PersistirTodo()
    {
        GuardarLibros();
        GuardarUsuarios();
        GuardarPrestamos();
    }

    // ============================================================
    // MÉTODOS DE VALIDACIÓN (auxiliares reutilizables)
    // ============================================================

    /// <summary>Lee un campo de texto que no puede estar vacío.</summary>
    static string LeerObligatorio(string etiqueta)
    {
        string valor;

        // DO-WHILE: repite hasta que el campo no esté vacío
        do
        {
            Console.Write(etiqueta);
            valor = Console.ReadLine()?.Trim() ?? "";
            if (valor == "") Error("Este campo es obligatorio.");
        } while (valor == "");

        return valor;
    }

    /// <summary>Lee un entero dentro del rango [min, max].</summary>
    static int LeerEnteroRango(string etiqueta, int min, int max)
    {
        int numero;

        // WHILE: repite hasta obtener un entero dentro del rango
        while (true)
        {
            Console.Write(etiqueta);
            string entrada = Console.ReadLine()?.Trim() ?? "";

            if (!int.TryParse(entrada, out numero))
                Error("Ingrese un número entero válido.");
            else if (numero < min || numero > max)
                Error($"El valor debe estar entre {min} y {max}.");
            else
                return numero;
        }
    }

    /// <summary>Lee un entero mayor o igual a cero.</summary>
    static int LeerEnteroPositivo(string etiqueta)
    {
        int numero;

        while (true)
        {
            Console.Write(etiqueta);
            if (!int.TryParse(Console.ReadLine()?.Trim(), out numero))
                Error("Ingrese un número entero válido.");
            else if (numero < 0)
                Error("El valor no puede ser negativo.");
            else
                return numero;
        }
    }

    /// <summary>Lee una fecha en formato dd/mm/yyyy con validación estructural.</summary>
    static string LeerFecha(string etiqueta)
    {
        string fecha;

        // WHILE: repite hasta que la fecha tenga el formato correcto
        while (true)
        {
            Console.Write(etiqueta);
            fecha = Console.ReadLine()?.Trim() ?? "";

            // Validar longitud y posición de separadores '/'
            if (fecha.Length == 10 && fecha[2] == '/' && fecha[5] == '/')
                return fecha;
            else
                Error("Formato inválido. Use dd/mm/yyyy  (ej. 22/05/2026).");
        }
    }

    // ============================================================
    // MÉTODOS AUXILIARES DE VALIDACIÓN DE CADENAS
    // ============================================================

    /// <summary>Devuelve true si todos los caracteres del string son alfanuméricos.</summary>
    static bool EsAlfanumerico(string texto)
    {
        // FOR: verificar carácter por carácter
        for (int i = 0; i < texto.Length; i++)
            if (!char.IsLetterOrDigit(texto[i])) return false;
        return true;
    }

    /// <summary>Devuelve true si todos los caracteres del string son dígitos.</summary>
    static bool EsSoloDigitos(string texto)
    {
        // FOR: verificar carácter por carácter
        for (int i = 0; i < texto.Length; i++)
            if (!char.IsDigit(texto[i])) return false;
        return true;
    }

    // ============================================================
    // MÉTODOS DE PRESENTACIÓN EN CONSOLA
    // ============================================================

    /// <summary>Imprime un mensaje de error en rojo.</summary>
    static void Error(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✘ {mensaje}");
        Console.ResetColor();
    }

    /// <summary>Imprime un mensaje de éxito en verde.</summary>
    static void Exito(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✔ {mensaje}");
        Console.ResetColor();
    }

    /// <summary>Imprime un encabezado de sección.</summary>
    static void Titulo(string texto)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  ═══ {texto} ═══");
        Console.ResetColor();
    }
}