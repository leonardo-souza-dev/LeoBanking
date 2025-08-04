# LeoBanking API

API REST para sistema bancário simplificado onde é possível realizar cadastro de clientes e transferências entre contas.


## Documentação da API

### 1️⃣ `POST /clients` 
> Cadastra um cliente com as seguintes informações: ID
(único), nome, número da conta (único) e saldo em conta.

#### Request:
```bash
curl --location 'http://localhost:5000/api/v1/clients' \
--header 'Content-Type: application/json' \
--data '{
    "name": "Leo",
    "balance": 123
}'
```

#### Response:
* 201 Created
```json
{
    "id": 1,
    "name": "Leo",
    "accountNumber": 1,
    "accountBalance": 123
}
```
* 400 Bad Request
* 500 Internal Server Error

---

### 2️⃣ `GET /clients` 
> Lista todos os clientes cadastrados.

#### Request:
```bash
curl --location 'http://localhost:5000/api/v1/clients' 
```

#### Response:
* 200 Ok
```json
[
    {
        "id": 1,
        "name": "Leo",
        "accountNumber": 1,
        "accountBalance": 123.0
    },
    {
        "id": 2,
        "name": "Joao",
        "accountNumber": 2,
        "accountBalance": 100000.0
    }
]
```
* 500 Internal Server Error

---

### 3️⃣ `GET /clients/{accountNumber:int}` 
> Busca um cliente por Número da Conta

#### Request:
```bash
curl --location 'http://localhost:5000/api/v1/clients/2'
```

#### Response:
* 200 Ok
```json
{
    "id": 2,
    "name": "Joao",
    "accountNumber": 2,
    "accountBalance": 100000.0
}
```
* 404 Not Found - Cliente não encontrado
* 400 Bad Request
* 500 Internal Server Error

---

### 4️⃣ `POST /transfers` 
> Realiza uma transferência entre duas contas. A conta de origem precisa ter saldo suficiente para a realização da transferência, e o valor da transferência deve ser maior que 0 (zero) e menor ou igual a R$ 10.000,00.

#### Request:
```bash
curl --location 'http://localhost:5000/api/v1/transfers' \
--header 'Content-Type: application/json' \
--data '{
    "originAccountNumber": 1,
    "destinationAccountNumber": 2,
    "amount": 123
}'
```

#### Response:
* 201 Created
```json
{
    "id": 1,
    "originAccountNumber": 1,
    "destinationAccountNumber": 2,
    "amount": 123,
    "success": true,
    "createdAt": "2025-08-02T10:11:25.2568781-03:00"
}
```
* 422 Unprocessable Entity - Conta de origem não tem saldo
* 500 Internal Server Error

---

### 5️⃣ `GET /transfers/{accountNumber:int}`
> Buscar as transferências com e sem sucesso relacionadas a uma conta

#### Request:
```bash
curl --location 'http://localhost:5000/api/v1/transfers/1'
```

#### Response:
* 200 Ok
```json
[
    {
        "id": 5,
        "originAccountNumber": 1,
        "destinationAccountNumber": 2,
        "amount": 12222.0,
        "success": false,
        "createdAt": "2025-08-04T20:12:12.4378989"
    },
    {
        "id": 4,
        "originAccountNumber": 1,
        "destinationAccountNumber": 2,
        "amount": 1.0,
        "success": true,
        "createdAt": "2025-08-04T12:32:07.3689436"
    },
    {
        "id": 3,
        "originAccountNumber": 1,
        "destinationAccountNumber": 3,
        "amount": 1.0,
        "success": true,
        "createdAt": "2025-08-03T12:12:06.2593592"
    }
]
```
* 500 Internal Server Error

---

## Pré-requisitos

* .NET 8 SDK

---

## Stack

* .NET 8
* EF Core 9
* NUnit para Testes de Unidade e Integração

## Rodando a aplicação

```bash
$ dotnet run --project ./LeoBanking.Api/LeoBanking.Api.csproj
```

## Rodando os testes

```bash
$ dotnet test
```

A aplicação estará disponível em: `http://localhost:5000`