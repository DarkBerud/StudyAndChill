# StudyAndChill

> **A comprehensive, full-stack educational management platform (SaaS) designed to automate class scheduling, student relationship management, and financial operations.**

StudyAndChill simplifies the operational workflow for independent teachers and educational institutions. It provides a robust backend architecture built with .NET 8 and a responsive, modern frontend built with React.

## 🚀 Key Features

* **Role-Based Access Control (RBAC):** Secure JWT authentication separating logic and permissions for Administrators, Teachers, and Students.
* **Smart Scheduling System:** Advanced collision-detection algorithms to prevent overlapping class sessions for both teachers and students.
* **Financial Automation (Split Payments):** Seamless integration with the **Asaas Payment Gateway**. Features an automated Webhook handler that listens for payment confirmations in real-time and automatically calculates and distributes teacher commissions.
* **Automated Background Jobs:** Utilizes .NET `IHostedService` to run daily background tasks, such as scanning for expiring contracts and generating proactive system notifications.
* **Audit Logging:** Enterprise-grade security through Entity Framework Core interceptors (`SaveChangesAsync`) that automatically track all data mutations (who changed what, and when) storing old and new JSON values.
* **Dynamic Holidays & Availability:** Complex logic allowing teachers to define specific availability slots, override them with holiday schedules, and automatically adjust the financial reporting.

## 🛠️ Tech Stack

### Backend
* **C# / .NET 8** (ASP.NET Core Web API)
* **Entity Framework Core** (Code-First approach)
* **PostgreSQL** (Relational Database)
* **Docker & Docker Compose** (Containerization)
* **JWT Auth** (JSON Web Tokens)
* **Swagger / OpenAPI** (API Documentation)

### Frontend
* **React 18** (Vite)
* **TypeScript** (Static typing for robust code)
* **Material UI (MUI)** (Component library and custom theming)
* **Axios** (API requests and interceptors)
* **React Router** (Navigation)

## ⚙️ How to Run the Project Locally

To run this project, you will need **Docker**, **.NET 8 SDK**, and **Node.js** installed on your machine.

### 1. Backend (API & Database)
The backend is fully containerized. To start the PostgreSQL database and the .NET API simultaneously:

1. Clone the repository:
   ```bash
   git clone [https://github.com/DarkBerud/StudyAndChill.git](https://github.com/DarkBerud/StudyAndChill.git)
   cd StudyAndChill

2. Start the Docker containers:
   ```bash
   docker-compose up --build

3. The API willbe available at http://localhost:8080.

4. You can access the Swagger documentation at http://localhost:8080/swagger.

### 2. Frontend (React Web App)

1. Go to the web folder:
    ```bash
    cd studyandchill-web

2. Install the dependencies:
    ```bash
    npm install

3. Start the development server:
    ```bash
    npm run dev

4. The application will be running at http://localhost:5173.


##
This project is currently under active development as part of my continuous learning journey in Software Engineering.
