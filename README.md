# 🎬 Film Tracker (.NET)

A .NET movie tracking application for managing a personal watchlist, organizing movies into "to watch" and "watched" categories, and tracking viewing history. Built as a console app with plans for API integration and a future web interface.

---

## Project Roadmap

### Stage 1: Console Application (MVP)

Build a simple console-based movie tracker with core functionality:

- Add movies to a personal list  
- Organize movies into:
  - "To Watch"
  - "Watched"  
- Mark movies as watched  
- Remove movies from the list  
- Search movies by title  

**Data storage:**
- In-memory (List)

---

### Stage 2: Application Structure & Features

Refactor the project for better structure and scalability:

- Introduce clean architecture:
  - Models (e.g., Movie)
  - Services (business logic)
  - UI layer (console menu)

Add new features:
- Movie rating (1–10)  
- Personal notes / comments  
- Date watched  
- Genre support and filtering  

---

### Stage 3: External API Integration

Integrate a movie database API (e.g., TMDb):

- Search movies via API  
- Fetch:
  - Descriptions  
  - Ratings  
  - Posters  
- Add movies directly from API results  
- Display detailed movie information  

---

### Stage 4: Web Application (ASP.NET)

Transform into a full-featured web application:

- Build UI with ASP.NET  
  - Home (movie list)  
  - Movie details  
  - Add/search movies  

Features:
- Filter by status ("To Watch" / "Watched")  
- Search functionality  
- Display posters and descriptions  

Optional:
- User authentication  
- Personal user accounts  

---

## Goals

- Practice C# and .NET development  
- Learn clean architecture principles  
- Work with external APIs  
- Build a full-stack web application  

---

## Tech Stack

- C# / .NET  
- Console Application  
- (Planned) ASP.NET  
- (Planned) External API integration  
