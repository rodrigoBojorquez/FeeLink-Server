# FeeLink Server

FeeLink Server es una API RESTful para el proyecto de IOT, FeeLink.

---

## 🚀 Como empezar

### Prerequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [EF Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### Instalacion

1. **Clonar el repositorio:**

   ```sh
   git clone https://github.com/rodrigoBojorquez/FeeLink-Server
   cd FeeLink-Server
   ```

2. **Ejecutar migraciones y seeders:**

   ```sh
   dotnet ef database update -p FeeLink.Api
   ```

3. **Ejecutar el servidor:**

    - Preferiblemente desde Visual Studio, Rider, o:
   ```sh
   dotnet run --project FeeLink.Api
   ```

---

## 🌐 Documentacion de la API

- OpenAPI/Swagger UI es accesible en:  
  `https://localhost:5002/swagger`

---

## 🛠 Branching

- Siempre asegurate de estar en la rama `dev` antes de actualizar tu repositorio, para tener acceso a los cambios mas recientes.

---

## 🤝 Contribuir

1. Crear una nueva rama (`git checkout -b tu-nombre`).
2. Hacer commit de tus cambios (`git commit -m 'Agregando algo'`).
3. Hacer push a la rama (`git push origin tu-nombre`).
4. Crear un Pull Request en GitHub.

---

#### **Si tienes dudas o comentarios, hazmelos saber: @rodrigoBojorquez**
