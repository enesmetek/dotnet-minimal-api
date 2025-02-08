# .NET Minimal API

An API built without controllers to create high-performance web services using **.NET 8**.

---

## 📌 Architectural Overview

This project demonstrates the use of **.NET 8 Minimal APIs**, focusing on building lightweight and efficient web services without the overhead of traditional MVC controllers. Key features include:

- **High Performance**: Streamlined request pipeline for faster responses.
- **Simplicity**: Reduced boilerplate code, making the application easier to understand and maintain.
- **Modern Practices**: Utilization of the latest features in .NET 8.

---

## 🏗️ Project Components

### 🌐 Library.Api

The **Library.Api** project serves as the main application, handling HTTP requests and responses. It leverages Minimal APIs to define endpoints directly in the `Program.cs` file, simplifying the setup and routing process.

### 🧪 Library.Api.Tests.Integration

The **Library.Api.Tests.Integration** project contains integration tests to ensure the API functions correctly. These tests validate the end-to-end functionality of the API endpoints.

---

## 🚀 Running the Project Locally

### 📌 Prerequisites

- **.NET 8.0 SDK** installed on your system.

### 🔧 Setup Instructions

1. **Clone the repository**:
   ```sh
   git clone https://github.com/enesmetek/dotnet-minimal-api.git
   ```
2. **Navigate into the project directory**:
   ```sh
   cd dotnet-minimal-api
   ```
3. **Build the solution**:
   ```sh
   dotnet build
   ```
4. **Run the application**:
   ```sh
   dotnet run --project src/Library.Api
   ```

   The API will be accessible at `https://localhost:5001` by default.

---

## 📡 API Endpoints

Below are the available endpoints in **Library.Api**:

| HTTP Method | Endpoint             | Description               |
|-------------|----------------------|---------------------------|
| `GET`       | `/api/books`         | Retrieve all books        |
| `GET`       | `/api/books/{id}`    | Retrieve a specific book  |
| `POST`      | `/api/books`         | Create a new book         |
| `PUT`       | `/api/books/{id}`    | Update an existing book   |
| `DELETE`    | `/api/books/{id}`    | Delete a book             |

---

## 📜 License

This project is licensed under the **MIT License**.

---

## 🤝 Contributing

Contributions are welcome! Feel free to submit a pull request or open an issue.

---

## 📧 Contact

For any questions or issues, please reach out via GitHub Issues or email me at **[emkafali@gmail.com]**.

---

### 📢 Star the Repository ⭐

If you found this project useful, consider giving it a star on GitHub! 😊
