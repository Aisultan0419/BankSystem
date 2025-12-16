# Bank system

This project simulates a real online banking system, featuring user registration and login, account and card creation, as well as deposit and transfer transactions. All accounts and cards are securely protected and stored.

### Features

- Client and app user registration and login with PIN codes or passwords
- Automatic generation of accounts and cards
- Viewing card details
- Deleting a client (only if no app user exists)
- Depositing funds to a specific card
- Checking transaction history
- Transferring money to any other card

### Installation

1. Clone the repository:
```bash
git clone https://github.com/Aisultan0419/BankSystem
cd BankSystem
```
2. Make sure that you have Docker and Docker compose installed
3. Run the project using Docker Compose
```bash
docker-compose up --build
```
The application uses PostgreSQL with the following configuration:
- Port: 5440
- Database: BankSystem
- User: postgres


### How to use
All API endpoints and request examples are available via Swagger UI:

First, you must register a **client**.
Then, register an **app user** using the IIN provided during client registration to establish the connection.

After that, you can *log in* using either a password or a PIN code.
Once logged in, copy the received token and paste it into the **Authorize** section at the top of the Swagger page in the following format:

```bash
Bearer {your_token}
```

### Technologies
- C#
- .NET 9
- ASP.NET Core
- PostgreSQL 17
- Docker 
- Swagger (OpenAPI)
- Entity Framework Core
- XUnit 
- Serilog
- FluentValidation

### Notes and limitations
- The project is intended for educational purposes
- No real banking data is used

### Project Status

This project is actively developed as a learning and portfolio project.

### Author

Developed by Aisultan Kalibek
GitHub: https://github.com/Aisultan0419

