# TuttoAuto 🚗

**Database course project**  
Design and implementation of a database for an automotive portal providing services like car sales, financing, insurance comparisons, and penalty fine history.

---

## 📋 Project Overview

The goal of **TuttoAuto** is to manage a rich automobile portal supporting:

- Purchase and sale of cars
- Viewing and comparing financing packages
- Viewing and comparing insurance offers
- Managing users’ fine histories

Users can be *private individuals* or *dealerships*, acting as sellers or buyers. The system also supports direct messaging, historic sale and insurance contract records, and fine listings tied to a user’s vehicles.

---

## 🧠 Concept and Structure

### Key Entities & Relationships

- **Users** (`Utenti`): Identified by email; store personal info, type (`Privato` or `Concessionaria`), plus optional `CodiceFiscale` or `PartitaIVA`.
- **Cars** (`Auto`): Attributes like `Targa`, `AnnoImmatricolazione`, `Potenza`, owner, fuel type, and model.
- **Sales Listings** (`Annunci_Vendita`): Refer to a car, seller, (optional) buyer, price, listing date, sale date, and region.
- **Messages** (`Messaggi`): Between users with sender, recipient, timestamp, and content.
- **Manufacturers & Models**: `Case_Produttrici` and `Modelli`, including model stats and safety rating (Euro NCAP).
- **Financing Offers** (`Proposte_Finanziamento`): Number of installments, rate, interest, and bank.
- **Insurance Offers & Contracts**: Offers (`Offerte_Assicurative`) per model and insurance company; user contracts (`Contratti_Assicurativi`) with optional discounts.
- **Fines** (`Multe`): Violation records tied to a user and car.
- Supporting lookup tables: **Nations**, **Fuel Types**, **Regions**, **Services**, and **Contract Discounts**.

Your documentation also defines constraints for data consistency (e.g. sale date logic, non-self-messaging, unique model names per manufacturer, valid numeric ranges) as well as refined conceptual-to-logical translations.

---

## ⚙️ Setup & Usage

### 1. Clone the repository

```bash
git clone https://github.com/FabioNotaro2001/TuttoAuto.git
cd TuttoAuto

2. Initialize the Database

Use your preferred SQL environment (MySQL, MariaDB, or similar) and execute the provided DDL script (e.g. schema.sql) to:

    Create lookup tables (regions, nations, fuel types)

    Create tables for users, cars, listings, messages, manufacturers, models, financing, insurance offers & contracts, fines

    Define constraints: primary/foreign keys, uniqueness, and CHECK rules (e.g. Prezzo > 0, VotoEuroNCAP BETWEEN 0 AND 5, valid email format)
3. Seed Initial Data (Optional)

Initialize lookup tables such as:

    Italian regions

    Nations for banks and manufacturers

    Common fuel types (benzina, diesel, metano, elettricità, etc.)

    Example services for insurance policies

    Sample banks and insurance companies
⚠️ Design Highlights & Notes

    Duplicate or derived data was minimized—for instance, we dropped AdattaNeopatentati redundancies (since it can be inferred from Potenza).

    The Stipulazioni table (user–insurance association with contract count) is preserved to efficiently compute discounts.

    Extensive data validity constraints are enforced with SQL CHECK constraints.

    Logical structure ensures correct handling of business rules (e.g. a user cannot insure the same car with overlapping contract dates).
🎯 Final Thoughts

TuttoAuto should serve as a complete example of database-driven design for a multifaceted web application, covering entity modeling, relational integrity, business logic implementation, and operational queries.

Feel free to adapt and extend it for more complex features—such as user dashboards, automated discount computation, or interactive search filters.
