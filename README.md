# Sistema de Biblioteca Universitaria

Aplicación de consola desarrollada en C# para administrar información básica de una biblioteca universitaria. El sistema utiliza archivos locales para conservar los datos generados durante su ejecución.

## Funcionalidades

- Gestión de libros.
- Registro de usuarios.
- Control de préstamos.
- Almacenamiento local en archivos CSV y TXT.
- Menús interactivos y validación de datos.

## Tecnologías

- C#
- .NET 6 o superior
- Persistencia mediante archivos locales

## Estructura

- `IA.cs`: lógica principal del sistema.
- `Parejas.cs`: módulos y operaciones complementarias.
- `Data/`: datos generados por la aplicación.
- `prompt.txt`: material de apoyo utilizado durante el desarrollo.

## Ejecución

Requisitos: Git y el SDK de .NET 6 o superior.

```bash
git clone https://github.com/iCreb2512/DesafioFinal_CR260127_MR260120.git
cd DesafioFinal_CR260127_MR260120
dotnet run
```

La carpeta `Data/` se crea automáticamente. Los archivos `libros.csv`, `usuarios.txt` y `prestamos.txt` se generan al guardar datos por primera vez.

## Demostración

[Ver video explicativo en Canva](https://www.canva.com/design/DAHK3cKST5I/COc-8tEe4DBUBCW8YlEbJA/watch?utm_content=DAHK3cKST5I&utm_campaign=designshare&utm_medium=link2&utm_source=uniquelinks&utlId=h34a302263a)

## Autoría

Proyecto académico realizado por Edwin Benjamín Contreras Romero y José Roberto Miranda Rosales.
