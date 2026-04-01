# Film-Tracker (Console App)

A simple console-based application for managing a personal movie watchlist. Users can add movies, organize them into "To Watch" and "Watched" categories, and manage their collection.

## Goal

Build a product that allows users to maintain a list of movies, track what they want to watch, and mark movies as watched.

## Features

- ➕ Add Movie: enter a movie title and choose status (To Watch / Watched)  
- 📋 View Movies: display all movies grouped by status  
- 🔄 Update Status: mark a movie as watched  
- ❌ Delete Movie: remove a movie from the list  
- 🔍 Search: search movies by title  

## User Interface

1. Add movie  
2. Show movies  
3. Mark as watched  
4. Delete movie  
5. Search  
0. Exit  

## Data Model

Each movie contains:
- Title (string)  
- Status (ToWatch / Watched)  

## Data Storage

- In-memory collection (List<Movie>)   

## Scope

This version intentionally excludes external APIs, databases, user authentication, and graphical interfaces.

## Definition of Done

- Movies can be added  
- Movies can be displayed  
- Status can be updated  
- Movies can be deleted  
- Application runs without errors  
