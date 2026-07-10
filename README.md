# Atlas

![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux-4EAA25)
![Stack](https://img.shields.io/badge/stack-.NET%209%20%2B%20Avalonia-512BD4)
![Status](https://img.shields.io/badge/status-active%20development-ffb703)

> A cross-platform desktop library for organizing files, folders, shortcuts, and executables without moving them.

Atlas is a personal productivity app designed to help you manage digital resources in a clean, flexible, and intuitive way. Instead of relying only on traditional folder structures, Atlas lets you create custom categories, enrich items with tags, pin favorites, and browse everything through search and smart views while keeping your files in their original locations.

## Core Features

- Unlimited categories with support for empty categories
- Drag and drop support for files, shortcuts, and executables
- Native file icons and contextual actions
- Instant search and filtered browsing
- Favorites, and other "Smart-Tags" (predefined)
- Compatible with Windows and Linux

## Application Flow

1. Create one or more categories
2. Drag folders, files, or shortcuts into Atlas
3. Drop them into a category or the default Uncategorized area
4. Atlas stores metadata and the original location
5. Browse resources through categories or search
6. Double-click to open items using the operating system

## Tech Stack

| Layer | Technology |
| --- | --- |
| Language | C# |
| Framework | .NET 9 |
| UI | Avalonia UI |
| Architecture | MVVM |
| Database | SQLite |
| ORM | Entity Framework Core |

## Main Architecture

### Models
- Category
- Item
- Tag
- Workspace

### ViewModels
- MainWindowViewModel
- CategoryViewModel
- ItemViewModel

## Relationships
- Workspace has many Categories
- Category has many Items
- Item has many Tags

## Smart Categories (Possible)
- Favorites
- Broken Links
- By File Type

## Item Actions
- Open
- Open file location
- Move to category
- Rename within Atlas
- Add or remove tags
- Add or remove favorites
- Remove from workspace

## Category Rules
- Categories may remain empty
- Categories containing items cannot be deleted
- Uncategorized always exists
- Items may belong to only one category
- Items may have multiple tags

## Roadmap
- 0.x — Core development and architecture iteration
- 1.x — Daily-use polish, quality-of-life improvements, and new features
- 2.0 — Full feature-complete vision for the original design

## Project Status
Atlas is currently in active development and is being shaped around a practical, personal-library workflow.
