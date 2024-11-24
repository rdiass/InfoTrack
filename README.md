# Project: InfoTrack settlement service

## Overview

This document provides an overview of booking settlement's service for InfoTrack.

## Prerequisites

* **.NET Core SDK:** Download and install the latest .NET Core SDK from https://dotnet.microsoft.com/download. Ensure you have the appropriate version matching your project's requirements.
* **Code Editor or IDE:** Visual Studio, Visual Studio Code, or any other code editor/IDE that supports .NET Core development.

## Running the Application

1. **Clone the Repository:**
	```bash
   git clone https://github.com/rdiass/InfoTrack.git
	```
1. Navigate to InfoTrack root folder project
1. Open the terminal
1. Restore NuGet Packages:
	```bash
	dotnet restore
	```
1. Build the Application:
	```bash
	dotnet build
	```
1. Run the Application:
	```bash
	dotnet run --project .\InfoTrack.Api\
    ```
1. Run the tests
	```bash
	dotnet test
    ```

## Testing with Swagger

This project leverages Swashbuckle.AspNetCore to generate Swagger documentation for the API. Once you run the application, the Swagger UI will be accessible at the following URL by default:

	https://localhost:7216/swagger/index.html

The Swagger UI provides a user-friendly interface to explore the API endpoints, including the BookSettlementAsync method defined in the SettlementController. You can:

* View the API documentation with detailed descriptions of the endpoint, parameters, and expected responses.
* Verify the functionality of the **/api/settlement/book** endpoint by providing bookingDate and name and checking the returned id for the booking.

API Documentation for **/api/settlement/book**

### Endpoint:

	POST /api/settlement/book

### Body: A JSON object with the following properties:

	{
	  "name": "John Smith",
	  "bookingTime": "09:30"
	}

### Response:

1. *200 OK*: Returns the id of the booking.
```
{
    "bookingId": "d90f8c55-90a5-4537-a99d-c68242a6012b"
}
```
1. *400 BadRequest*: Indicates an Invalid format for bookingTime or Empty name.
1. *409 Conflict*: All slots for the specified bookingTime are already taken.


## Example Usage in Swagger

1. Open the Swagger UI at the URL mentioned above.
1. Locate the Settlement section and expand it.
1. Click on the */api/settlement/book* operation.
1. Enter valid booking time and name. For example:

```bash
{
  "name": "John Smith",
  "bookingTime": "09:30"
}
```

5. Click the "Try it out" button.
1. Observe the response body, which should display the id of the booking or error response.


## Proposed Architecture

### Layered Architecture:

The solution is structured into distinct layers:

1. Presentation Layer:
	- Handles incoming HTTP requests and returns responses.
	- Validates incoming data using data annotations and custom validation attributes.
	
1. Business Logic Layer:
	- Encapsulates the business logic for booking settlements.
	- Validates booking times and manages the booking process.
	
3. Data Access Layer: 
	- Handles data access operations, including adding, retrieving, and updating bookings.
	- Uses an in-memory repository for simplicity, but can be easily replaced with a database-backed repository.

4. Contracts Layer:
	- Defines interfaces for the business logic and data access layers.
	- Includes data transfer objects (DTOs) for data exchange between layers.

### Tests

#### Unit Tests:
- Focus on individual units of code.
- Faster to execute and easier to maintain.
- Help identify and fix bugs early in the development process.

#### Integration Tests:
- Verify the interaction between different layers.
- Ensure that components work together as expected.
- Help identify integration issues and configuration problems.


## Design Pattern: Repository Pattern

The primary design pattern used in this settlement booking is the Repository Pattern. This pattern abstracts the data access logic, making it easier to test, maintain, and swap out different data storage mechanisms (e.g., in-memory, database, cloud storage).

### Benefits of the Repository Pattern:
#### 1. Improved Testability:

- By isolating data access logic, you can write unit tests that focus on the business logic without relying on a real database.
- You can mock the repository to simulate different scenarios and test your service layer in isolation.

#### 2. Enhanced Maintainability:

- Centralized data access logic makes it easier to modify or update data access operations.
- Changes to the underlying data storage mechanism can be made without affecting the rest of the application.

#### 3. Increased Flexibility:

- The repository pattern allows you to easily switch between different data storage implementations (e.g., in-memory, database, or a hybrid approach).
- You can add new data access operations without modifying the service layer.

#### 4. Cleaner Code:

- Separating data access concerns from business logic results in cleaner and more focused code.
- The service layer can focus on business rules and workflows, while the repository handles data retrieval and persistence.

## Design Pattern: Result Pattern

The Result pattern is a functional programming technique used to represent either a successful result or a failure. It's particularly useful for handling errors and exceptional conditions in a more declarative and concise way.

### Benefits

By using the Result pattern, you can improve the readability and maintainability of your code, making it easier to handle errors and exceptional conditions.

#### 1. Clear Error Handling
- Explicitly handles both success and failure cases.
#### 2. Concise Code
- Reduces the need for try-catch blocks and error checking.
#### 3. Functional Programming Style
- Aligns with functional programming principles.
#### 4. Customizable Error Messages
- Allows for detailed error messages.



## For further implementation

### Database-Level Locking

- I'm using a SemaphoreSlim to ensure that only one request can access the booking resource at a time, preventing overbooking in a multi-threaded or multi-user environment.
- To ensure data consistency and prevent overbooking in a distributed environment, consider using database-level locking, distributed locking mechanisms (e.g., Redis or Azure Distributed Locks), or message queues.

#### Example (using Entity Framework Core):

```
public async Task<Guid> AddBookingAsync(DateTime bookingTime, string name)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return bookingId;
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

#### Example (using Redis):

```
    var lockKey = $"booking_lock_{bookingTime.Ticks}";
    bool acquiredLock = await _redisCache.LockAsync(lockKey, TimeSpan.FromSeconds(30));

    if (acquiredLock)
    {
        try
        {
            // ... your booking logic ...
        }
        finally
        {
            await _redisCache.ReleaseLockAsync(lockKey);
        }
    }
    else
    {
        // Handle the case where the lock could not be acquired
    }
```


### Retry Logic with Polly

- To enhance the reliability of your application and handle transient errors like database connection issues or network failures, consider implementing a retry mechanism using Polly.

#### Example:

```
public async Task<Guid> AddBookingAsync(DateTime bookingTime, string name)
{
    return await _retryPolicy.ExecuteAsync(async () =>
    {
        // ... database insertion ...
        return bookingId;
    });
}
```