# TaskManagement

## Overview

TaskManagement is a simple yet robust web API designed to manage Users and Tasks with automated task assignment and reassignment logic. The system ensures fair distribution of tasks among users and tracks the assignment history, supporting dynamic user and task management.

✅ This project uses .NET 8 Minimal APIs with Carter — no controllers or MVC. All endpoints are defined using concise and lightweight route mappings directly in the API project.
✅ Implements CQRS using MediatR for a clear separation between commands and queries.
✅ Uses FluentValidation to validate incoming requests.
✅ Leverages AutoMapper to map between domain models and DTOs.

## Features

* **User Management**: Create and retrieve users, each identified by a unique name.
* **Task Management**: Create and retrieve tasks, each with a unique title and a state (`Waiting`, `InProgress`, `Completed`).
* **Automatic Task Assignment**: When a task is created, it is automatically assigned to an available user. If no users are available, the task remains in the `Waiting` state.
* **Task Reassignment**: Every 2 minutes, all tasks are reassigned to a different random user, following these rules:
  * The new user cannot be the current or immediately previous assignee.
  * The new user can be any user who was assigned two or more rounds ago.
  * If no users are available, the task remains in the `Waiting` state.
* **Assignment Coverage**: Every task must be assigned to all users at least once (including users created after the task, unless the task is already `Completed`).
* **Task Completion**: Once a task has been assigned to all users, it is marked as `Completed` and remains unassigned.

## Architecture

* **Clean Architecture**:

  * `TaskManagement.Application`: Application logic, DTOs, validation, and service interfaces.
  * `TaskManagement.Domain`: Domain entities, enums, and domain services.
  * `TaskManagement.Infrastructure`: Data access, repository implementations, background services, and configuration.
  * `TaskManagement.API`: Minimal API endpoints for Users and Tasks.
  * `test/`: Unit and integration tests for all major components.

* **Key Components**:

  * **Entities**: `Users`, `Tasks`, `TaskAssignmentHistory`
  * **Background Service**: Periodically reassigns tasks according to business rules.
  * **Repositories**: Abstract data access for users, tasks, and assignment history.
  * **Validation**: Ensures data integrity for user and task creation.

## Getting Started

### Prerequisites

* \[.NET 8.0 SDK or later]
* I use SQLite for database operations

### Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/abdelrahmanalimohamed/TaskManagement.git
   cd TaskManagement
   ```
2. **Restore dependencies**

   ```bash
   dotnet restore
   ```
3. **Apply database migrations**

   ```bash
   dotnet ef database update --project src/Infrastructure/TaskManagement.Infrastructure/ --startup-project src/Presentation/TaskManagement.API/
   ```
4. **Run the API**

   ```bash
   dotnet run --project src/Presentation/TaskManagement.API/
   ```

### API Usage
- **Base URL**: `http://localhost:5011/`
- **Endpoints**:
  - `POST /api/users/create` - Create a new user
  - `GET /api/users/get-all?PageNumber=2&PageSize=100` - List all users
  - `POST /api/tasks/create` - Create a new task
  - `GET /api/tasks/get-all?PageNumber=2&PageSize=100` - List all tasks

### Postman Collection

Inside the project, a **Postman request collection** is included to help you test the API endpoints easily:

1. Open [Postman](https://www.postman.com/).
2. Click **Import**.
3. Choose the included Postman collection file from the project directory.
4. Use the collection to test all API features without writing custom requests manually.

> This saves time and ensures you are using the correct request format and headers.

## Business Logic Details

* **User Creation**: Names must be unique.
* **Task Creation**: Titles must be unique. Tasks are assigned to a user if available; otherwise, they remain in `Waiting`.
* **Task Reassignment**: Every 2 minutes, a background service reassigns tasks, ensuring no immediate repeats and that all users eventually receive each task.
* **Completion**: Tasks are marked as `Completed` and unassigned after being assigned to all users.

## Testing

### Test Projects

The solution includes two test projects:

1. **Unit Tests**
   Located in the `test/TaskManagement.UnitTests` project.
   This project contains unit tests for:

   * **Domain logic**
   * **Application services**
   * **Infrastructure components**
     These are organized in separate folders by concern for better structure and maintainability.

2. **Integration Tests**
   Located in the `test/TaskManagement.IntegrationTests` project.
   This project tests the full endpoints workflow.

### Run All Tests

```bash
dotnet test
```
---
