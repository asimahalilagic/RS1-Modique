# Modique - E-commerce Platform

E-commerce platforma sa Angular frontend-om i .NET backend-om.

## 📁 Struktura Projekta

```
rs1-2025-26-Modique/
├── Modique.Backend/          # .NET Backend API
│   ├── Modique.API/          # API projekat
│   ├── Modique.Application/  # Application layer
│   ├── Modique.Domain/       # Domain entities
│   ├── Modique.Infrastructure/ # Infrastructure layer
│   └── Modique.sln           # Solution fajl
│
└── Modique.Frontend/         # Angular Frontend
    ├── src/                  # Source kod
    ├── public/               # Static assets
    ├── angular.json          # Angular konfiguracija
    └── package.json          # Dependencies
```

## ⚡ Brzo Pokretanje

### Backend

```bash
cd Modique.Backend
dotnet restore
dotnet run --project Modique.API
```

Backend API će biti dostupan na: `https://localhost:7034`

### Frontend

```bash
cd Modique.Frontend
npm install
npm start
```

Frontend aplikacija će se otvoriti na: `http://localhost:4200`

## 🔐 Autentifikacija

- **Email**: `admin@modique.local`
- **Lozinka**: `Admin123!`

## 📋 Funkcionalnosti

✅ **Proizvodi** - Pregled, pretraga, filtriranje  
✅ **Kategorije** - Organizacija proizvoda  
✅ **Košarica** - Upravljanje narudžbama  
✅ **Omiljeno** - Lista omiljenih proizvoda  
✅ **Autentifikacija** - Prijava i registracija  
✅ **Responsive Design** - Mobilni, tablet, desktop

## 🛠️ Tehnologije

### Backend
- .NET 9.0
- Entity Framework Core
- JWT Authentication

### Frontend
- Angular 19
- Bootstrap 5
- Font Awesome
- SCSS

## 📝 Napomene

- Backend API mora biti pokrenut prije pokretanja frontend-a
- CORS je konfigurisan za `http://localhost:4200`
- Sve tekstualne poruke su na bosanskom jeziku
