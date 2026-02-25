# Bank System

A backend simulation of a secure online banking system built with ASP.NET Core.  
The project focuses on transactional integrity, security, asynchronous processing, and clean architecture principles.

---

## Overview

This system models core banking functionality:

- Client registration
- App user authentication (PIN / password)
- Automatic account and card generation
- Secure money transfers
- Deposit operations
- Transaction history
- Savings accounts with automated interest accrual

The architecture follows Onion Architecture and emphasizes security, consistency, and extensibility.

---

## Core Features

### Authentication & Security
- JWT authentication (HMAC-SHA256)
- Refresh token mechanism
- Password and PIN hashing with BCrypt
- Brute-force protection (3 failed attempts → 3-hour lock)
- Global exception handling
- AES-GCM encryption for PAN storage
- Secure refresh token hashing
- Unique IIN validation and structural verification

### Banking Logic
- Automatic IBAN generation (Mod97 validation)
- PAN generation (Luhn algorithm)
- Money represented as value structures
- Transfers between accounts
- Deposit transactions
- Card replacement and deletion
- Multiple savings account types
- Strict transaction isolation with `BeginTransactionAsync` + execution strategy
- Outbox pattern for reliable message processing

### Asynchronous Processing
- RabbitMQ via MassTransit
- Savings account creation through messaging
- Automated interest accrual via Hosted Services
- Event-driven communication
- Outbox pattern for consistency

### Infrastructure
- PostgreSQL 17
- Entity Framework Core
- Dockerized deployment
- Serilog detailed logging
- FluentValidation
- Swagger (OpenAPI)
- Unit tests (xUnit)

---

## Installation

### 1. Clone the repository

```bash
git clone https://github.com/Aisultan0419/BankSystem
cd BankSystem
````

### 2. Ensure you have:

* Docker
* Docker Compose

### 3. Run the project

```bash
docker-compose up --build
```

---

### Database configuration

* PostgreSQL Port: 5440
* Database: BankSystem
* User: postgres

---

## How to Use

1. Register a Client (requires unique IIN).
2. Register an App User linked to that Client.
3. Log in using PIN or password.
4. Copy the JWT token.
5. Open Swagger UI.
6. Click **Authorize** and paste:

```
Bearer {your_token}
```

You can now perform secure banking operations.

---

## Architecture

* Onion Architecture
* Separation of concerns
* Application / Domain / Infrastructure layers
* DTO pattern
* Dependency Injection
* Value Objects for financial precision
* Transaction-safe service design

---

## Security Considerations

This project simulates production-grade security patterns:

* No plaintext sensitive data stored
* Encrypted PAN storage
* Hashed passwords and PINs
* Token-based authentication with refresh rotation
* Brute-force attack mitigation
* Strict transactional consistency

---

## Author

Developed by Aisultan Kalibek
GitHub: [https://github.com/Aisultan0419](https://github.com/Aisultan0419)


