# Library Management UI — Frontend

> **Angular 19 standalone SPA** — part of the Library Management System technical assessment.

## Overview

A responsive Angular 19 frontend for the Library Management System REST API. Built with standalone components, reactive forms, and plain CSS — no UI framework dependency.

## Features

- **Authentication** — Login page with JWT token management
- **Role-based routing** — Auth guard and role guard for protected routes
- **Dashboard** — Summary cards with role-based metrics
- **Branch CRUD** — List, search, create, edit, delete, toggle active status
- **Book CRUD** — List with debounced search, pagination, create, edit, delete
- **Member CRUD** — List with search, pagination, create, edit, delete
- **Borrow/Return** — View active borrows, return books, overdue filter
- **Reservation Queue** — View reservations, fulfill/cancel with status badges
- **Reports** — Borrow history, overdue books, member activity with date filtering
- **Responsive Design** — Mobile-friendly layout with collapsible sidebar
- **Toast Notifications** — Success/error/warning/info notifications
- **Loading States** — Spinner overlay for async operations

## Tech Stack

| Technology | Version |
|-----------|---------|
| Angular | 19 (standalone) |
| TypeScript | ~5.x |
| RxJS | 7.x |
| CSS | Plain (no framework) |

## Prerequisites

- **Node.js** 18+
- **Angular CLI** 19+ (`npm install -g @angular/cli`)
- **Backend API** running at `http://localhost:5243`

## Setup

```bash
cd frontend/library-management-ui
npm install
```

## Running the App

```bash
ng serve
```

The app runs on `http://localhost:4200`.

## Configuration

The API URL is configured in `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5243/api/v1'
};
```

## Project Structure

```
src/app/
├── components/
│   ├── auth/              — Login page
│   ├── dashboard/         — Summary dashboard
│   ├── layout/            — App layout (sidebar + header)
│   ├── branches/          — Branch CRUD (list, form, detail)
│   ├── books/             — Book CRUD (list, form, detail)
│   ├── members/           — Member CRUD (list, form, detail)
│   ├── borrows/           — Borrow/Return (list, form, detail)
│   ├── reservations/      — Reservation queue (list)
│   └── reports/           — Reports page (tabs)
├── guards/                — Auth guard, role guard
├── interceptors/          — Auth interceptor, error interceptor
├── models/                — TypeScript interfaces for all entities
├── services/              — API service layer
└── shared/                — Toast, spinner components
```

## Authentication Flow

1. User logs in via `/login`
2. Token stored in `sessionStorage`
3. Auth interceptor attaches `Authorization: Bearer <token>` to all requests
4. Auth guard protects routes; role guard restricts admin/librarian pages
5. 401 responses redirect to login; 403 redirects to home

## API Integration

All API calls use the `ApiService` base class which handles:
- Base URL from environment config
- JWT header injection
- Error handling with standardized error messages

Each domain has its own service (e.g., `BranchService`, `BookService`) wrapping `HttpClient` calls.

## License

This project is part of a technical assessment for a Software Engineer (.NET) role.
