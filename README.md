# BookVault API

REST API for managing a digital book library — built with **ASP.NET Core 9**, **Entity Framework Core** and **SQL Server**.

Includes a built-in single-page frontend (made by AI) for full CRUD operations.

---

## Features

- **Books** — full CRUD with pagination, search, filtering by category, sorting (by title, rating, date, author)
- **Authors** — CRUD with delete protection (cannot remove author who has books)
- **Categories** — create and browse, many-to-many relationship with books
- **Reviews** — add reviews with 1–5 star rating, automatic average recalculation
- **Validation** — FluentValidation for all input DTOs (ISBN format, rating range, required fields)
- **Error handling** — global ExceptionMiddleware returning consistent JSON error responses
- **Frontend** — single-page HTML/CSS/JS app served from `wwwroot/`

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server (LocalDB / Express) |
| Validation | FluentValidation |
| Frontend | Vanilla HTML / CSS / JavaScript |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or LocalDB
- (Optional) [Postman](https://www.postman.com/downloads/) for API testing

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/nszolc/BookVault.API.git
   cd BookVault.API
   ```

2. **Update connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=BookVaultDb;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"
   }
   ```

3. **Apply migrations and seed the database**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open in browser**: `http://localhost:5161`

---

## API Endpoints

### Books

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/books` | List books (paginated, filterable, sortable) |
| `GET` | `/api/books/{id}` | Book details with author, categories, reviews |
| `POST` | `/api/books` | Create a new book |
| `PUT` | `/api/books/{id}` | Update a book |
| `DELETE` | `/api/books/{id}` | Delete a book (cascades reviews) |
| `GET` | `/api/books/{id}/reviews` | Get reviews for a book |
| `POST` | `/api/books/{id}/reviews` | Add a review (recalculates average rating) |

### Authors

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/authors` | List all authors |
| `GET` | `/api/authors/{id}` | Author details with their books |
| `POST` | `/api/authors` | Create a new author |
| `PUT` | `/api/authors/{id}` | Update an author |
| `DELETE` | `/api/authors/{id}` | Delete (only if author has no books) |

### Categories

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/categories` | List all categories |
| `POST` | `/api/categories` | Create a new category |
| `GET` | `/api/categories/{id}/books` | Books in a specific category |

### Query Parameters (GET /api/books)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 10 | Results per page |
| `search` | string | — | Search by title or author name |
| `categoryId` | int | — | Filter by category |
| `sortBy` | string | "title" | Sort by: `title`, `rating`, `date`, `author` |

---

## Project Structure

```
BookVault.API/
├── Controllers/          # API controllers (Books, Authors, Categories)
├── Data/                 # DbContext and database configuration
├── DTO/                  # Data Transfer Objects
│   ├── Authors/          #   AuthorDto, CreateAuthorDto, UpdateAuthorDto
│   ├── Books/            #   BookDto, BookDetailDto, CreateBookDto, UpdateBookDto
│   ├── Categories/       #   CreateCategoryDto
│   └── Reviews/          #   ReviewDto, CreateReviewDto
├── Middleware/            # ExceptionMiddleware (global error handling)
├── Migrations/           # EF Core database migrations
├── Models/               # Entity models (Book, Author, Category, Review)
├── Validators/           # FluentValidation validators
├── wwwroot/              # Frontend (index.html)
├── appsettings.json      # Configuration and connection string
└── Program.cs            # App entry point and middleware pipeline
```

---

## Data Model

```
Author (1) ──── (N) Book (N) ──── (N) Category
                     │
                     └── (1:N) Review
```

- **Author → Book**: one-to-many (one author, many books)
- **Book ↔ Category**: many-to-many (a book can have multiple categories)
- **Book → Review**: one-to-many (cascade delete)

---

## Example Requests

**Create an author:**
```http
POST /api/authors
Content-Type: application/json

{
    "firstName": "Andrzej",
    "lastName": "Sapkowski",
    "bio": "Polish fantasy writer",
    "birthDate": "1948-06-21"
}
```

**Create a book:**
```http
POST /api/books
Content-Type: application/json

{
    "title": "The Last Wish",
    "isbn": "9780316029186",
    "description": "The first book in The Witcher series",
    "pageCount": 288,
    "publishedDate": "1993-01-01",
    "authorId": 1,
    "categoryIds": [1, 3]
}
```

**Add a review:**
```http
POST /api/books/1/reviews
Content-Type: application/json

{
    "rating": 5,
    "comment": "Absolutely brilliant!",
    "reviewerName": "BookLover42"
}
```

---

## Validation Rules

| DTO | Field | Rule |
|-----|-------|------|
| CreateBookDto | Title | Required, max 200 chars |
| CreateBookDto | ISBN | Required, 10 or 13 digits |
| CreateBookDto | PageCount | Between 1 and 9999 |
| CreateBookDto | CategoryIds | At least one category |
| CreateReviewDto | Rating | Between 1 and 5 |
| CreateReviewDto | ReviewerName | Required |
| CreateAuthorDto | FirstName | Required, max 100 chars |
| CreateAuthorDto | LastName | Required, max 100 chars |

