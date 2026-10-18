# Sistema de Préstamos de Biblioteca

Aplicación de consola para administrar lectores, libros y préstamos de una biblioteca. Permite registrar y consultar información, editar y eliminar registros, controlar las devoluciones y calcular la disponibilidad de ejemplares.

## Funcionalidades

- Crear, listar, actualizar y eliminar lectores.
- Crear, listar, actualizar y eliminar libros.
- Consultar los ejemplares disponibles de cada libro.
- Registrar, listar, actualizar y eliminar préstamos.
- Registrar la devolución de un préstamo.
- Mostrar el estado de cada préstamo: `Prestado`, `Vencido` o `Devuelto`.
- Demostrar herencia y polimorfismo mediante `Persona` y `Lector`.
- Crear automáticamente la base de datos y sus tablas al iniciar.

## Tecnologías y dependencias

| Tecnología | Uso |
| --- | --- |
| C# | Lenguaje de programación y lógica de la aplicación. |
| .NET 8 | Plataforma de ejecución y compilación; el proyecto usa `net8.0`. |
| ADO.NET | Acceso a datos mediante conexiones, comandos, parámetros y transacciones. |
| SQLite | Base de datos relacional local, almacenada en un archivo. No requiere servidor independiente. |
| `Microsoft.Data.Sqlite` 8.0.11 | Proveedor ADO.NET que conecta la aplicación con SQLite. |
| SQL | Definición de tablas, restricciones, índices y consultas CRUD. |

La dependencia NuGet externa del proyecto es `Microsoft.Data.Sqlite`. Las demás APIs usadas, como `System.Data`, `System.IO` y `System.Reflection`, forman parte de .NET 8.

## Requisitos

- Windows, Linux o macOS.
- .NET 8 SDK o una versión compatible con el destino `net8.0`.
- Acceso de escritura al directorio donde se guardará el archivo de base de datos.

## Compilar y ejecutar

Desde la carpeta que contiene `PrestamoBiblioteca.csproj`:

```powershell
dotnet restore
dotnet build
dotnet run
```

En una terminal que no sea PowerShell se pueden usar los mismos comandos. `dotnet run` compila el proyecto si hace falta y abre el menú interactivo.

## Uso del menú

Al iniciar, la aplicación prepara la base de datos y presenta estas opciones:

| Opción | Acción |
| --- | --- |
| 1–4 | Crear, listar, actualizar o eliminar lectores. |
| 5–8 | Crear, listar, actualizar o eliminar libros. |
| 9 | Registrar un préstamo activo. La fecha del préstamo se toma del día actual; se solicita la fecha límite y los IDs de lector y libro. |
| 10 | Listar préstamos y su estado. |
| 11 | Actualizar los datos de un préstamo. |
| 12 | Registrar la devolución de un préstamo usando la fecha actual. |
| 13 | Eliminar un préstamo. |
| 14 | Mostrar el ejemplo de polimorfismo `Persona`/`Lector`. |
| 0 | Salir. |

Las fechas se solicitan en formato `AAAA-MM-DD`. Para crear préstamos primero deben existir el lector y el libro; sus IDs aparecen en las opciones de listado.

## Base de datos

Por defecto, el archivo SQLite se crea en:

```text
%LOCALAPPDATA%/UMG/PrestamosBiblioteca/biblioteca.db
```

En Windows, `%LOCALAPPDATA%` normalmente apunta a la carpeta local de datos de la cuenta de usuario. La aplicación crea el directorio si todavía no existe.

Para usar otra ubicación, define la variable de entorno `PRESTAMO_BIBLIOTECA_DB` antes de iniciar el programa. Ejemplo en PowerShell:

```powershell
$env:PRESTAMO_BIBLIOTECA_DB = "C:\datos\biblioteca.db"
dotnet run
```

El esquema se encuentra en `crear_tablas.sql`. El proyecto lo incluye como recurso embebido; `InicializadorBaseDatos` lo lee y ejecuta al arrancar. Las tablas son:

- `Lectores`: código, nombre, correo y teléfono. El código es único.
- `Libros`: código, título, autor y total de ejemplares. El código es único y la cantidad no puede ser negativa.
- `Prestamos`: lector, libro, fecha de préstamo, fecha límite y fecha de devolución opcional.

Las claves foráneas relacionan préstamos con lectores y libros. Al eliminar un lector o un libro, SQLite elimina también sus préstamos asociados (`ON DELETE CASCADE`). Se crean índices para las referencias de préstamos a lectores y libros.

La disponibilidad se calcula restando del total los préstamos que aún no tienen fecha de devolución. Al registrar o cambiar un préstamo activo, el repositorio valida que exista al menos un ejemplar disponible y realiza la operación dentro de una transacción. Las consultas utilizan parámetros para separar los valores de entrada del SQL.

## Diseño del programa

El código está organizado en capas sencillas, cada una con una responsabilidad concreta:

```text
Program
  └── Aplicacion: MenuBiblioteca
	  └── Persistencia: RepositorioBiblioteca
		  └── SQLite

Dominio: Persona, Lector, Libro y Prestamo
Persistencia: InicializadorBaseDatos y RepositorioBiblioteca
```

### Entrada y coordinación

`Program.cs` es el punto de entrada. Inicializa el archivo de base de datos, crea el repositorio con la cadena de conexión y entrega ese repositorio al menú. Los errores que impiden iniciar se muestran en consola.

### Aplicación

`MenuBiblioteca.cs` implementa la interacción de consola. Presenta las opciones, recoge y valida entradas sencillas (campos obligatorios, números y fechas), llama al repositorio y muestra los resultados. Las excepciones de cada operación se capturan para informar el error sin cerrar inmediatamente el menú.

### Dominio

Las clases de `Dominio/` representan los conceptos del sistema:

- `Persona` es una clase abstracta con código, nombre, correo y teléfono; valida los campos de texto y declara información virtual.
- `Lector` hereda de `Persona` y sobrescribe `ObtenerInformacion`, lo que permite mostrar polimorfismo.
- `Libro` guarda código, título, autor y cantidad de ejemplares; valida los textos y rechaza cantidades negativas.
- `Prestamo` relaciona los IDs de lector y libro con sus fechas. Valida IDs positivos y que la fecha límite y la devolución no precedan a la fecha del préstamo. `EstaActivo` indica si aún no se ha registrado devolución.

### Persistencia

`InicializadorBaseDatos.cs` determina la ruta del archivo, configura la conexión con claves foráneas habilitadas y crea las tablas al inicio.

`RepositorioBiblioteca.cs` concentra las operaciones SQL de consulta y modificación. Abre una conexión por operación, usa parámetros en las escrituras, devuelve listados como `DataTable` y maneja transacciones en el alta y actualización de préstamos.

## Estructura de archivos

```text
PrestamoBIblioteca/
├── Dominio/
│   ├── Persona.cs
│   ├── Lector.cs
│   ├── Libro.cs
│   └── Prestamo.cs
├── InicializadorBaseDatos.cs
├── MenuBiblioteca.cs
├── Program.cs
├── RepositorioBiblioteca.cs
├── crear_tablas.sql
├── PrestamoBiblioteca.csproj
└── README.md
```

## Notas

- La base de datos almacena fechas como texto en formato ISO `yyyy-MM-dd`, que SQLite puede comparar como fechas.
- El estado `Vencido` se calcula al consultar los préstamos, comparando la fecha límite con la fecha local actual; no se guarda como columna.
- El programa es de consola y guarda los datos localmente en SQLite. No incluye interfaz gráfica, servidor web ni sincronización remota.
