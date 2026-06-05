# Verantwoording – StealMyPaintscheme

## Inhoudsopgave
1. [Projectbeschrijving](#1-projectbeschrijving)
2. [Technologiekeuzes](#2-technologiekeuzes)
3. [Backendarchitectuur](#3-backendarchitectuur)
4. [Authenticatie en autorisatie](#4-authenticatie-en-autorisatie)
5. [Beveiliging](#5-beveiliging)
6. [Datamodellering](#6-datamodellering)
7. [Frontendarchitectuur](#7-frontendarchitectuur)
8. [Reflectie](#8-reflectie)

---

## 1. Projectbeschrijving

StealMyPaintscheme is een webapplicatie voor miniaturenschilders. Gebruikers kunnen verfschema's aanmaken en delen: een stap-voor-stap beschrijving van hoe een miniatuur geschilderd wordt, inclusief welke verven, technieken en onderdelen aan bod komen. Andere gebruikers kunnen deze schema's "stelen" (kopiëren naar hun eigen collectie) en eventueel aanpassen.

Kernfunctionaliteit:
- Registreren en inloggen
- Verfschema's aanmaken, bewerken en verwijderen
- Schema's van andere gebruikers bekijken en stelen
- Eigen verfvoorraad (inventory) bijhouden
- Zien welke verven in een schema ontbreken in de eigen voorraad
- Afbeelding uploaden bij een schema
- Beheerdersrol voor het verwijderen van andermans schema's

---

## 2. Technologiekeuzes

### Backend: ASP.NET Core 10 (Minimal API)
Gekozen voor .NET 10 omdat dit de nieuwste LTS-versie is met actieve ondersteuning. De Minimal API aanpak is gebruikt in plaats van MVC-controllers omdat het voor een project van deze schaal minder boilerplate vereist: er zijn geen aparte controller-klassen nodig en routes worden direct geregistreerd. De nadelen (minder structuur out-of-the-box) zijn opgevangen door de code zelf op te splitsen in losse endpoint-bestanden en een service/repository-laag.

### Database: PostgreSQL met Entity Framework Core
PostgreSQL is een volwassen open-source relationele database met goede ondersteuning voor JSON-kolommen, wat nuttig is voor de `Tags`-lijst op verfschema's. Entity Framework Core is gekozen als ORM omdat het goed integreert met .NET, migrations ondersteunt en LINQ-queries schrijven intuïtiever maakt dan raw SQL. De Npgsql-provider biedt volledige PostgreSQL-compatibiliteit.

### Authenticatie: JWT (JSON Web Tokens)
JWT is gekozen omdat het stateless is: de server hoeft geen sessieopslag bij te houden. Elke request bevat het token in de `Authorization`-header, wat goed werkt voor een SPA (Single Page Application) als Angular. Het token bevat de `userId` en `isAdmin`-claim zodat de backend direct weet wie de request doet zonder een extra database-aanroep.

### Wachtwoordhashing: BCrypt
BCrypt is de industriestandaard voor wachtwoordopslag. Het voegt automatisch een salt toe (voorkomt rainbow table-aanvallen) en heeft een instelbare kostenfactor zodat het rekenintensief blijft naarmate hardware sneller wordt. MD5 of SHA-256 zijn bewust niet gebruikt omdat die niet geschikt zijn voor wachtwoordopslag.

### Frontend: Angular 21
Angular is gekozen vanwege de strikte TypeScript-ondersteuning, de ingebouwde dependency injection en de duidelijke scheiding tussen component, template en stijl. Versie 21 introduceert standalone components als standaard, waardoor NgModule niet meer nodig is en de projectstructuur eenvoudiger blijft.

### Styling: Tailwind CSS 4
Tailwind CSS zorgt voor snelle, consistente styling zonder aparte CSS-bestanden per component te schrijven. Versie 4 werkt als PostCSS-plugin zonder aparte configuratiebestanden.

---

## 3. Backendarchitectuur

De backend is opgebouwd in vier lagen:

```
HTTP Request
     │
     ▼
Endpoints/        ← HTTP afhandeling: parameters lezen, HTTP-codes teruggeven
     │
     ▼
Services/         ← Businesslogica: validatie, regels, orkestratie
     │
     ▼
Repositories/     ← Database-abstractie: enkel queries, geen logica
     │
     ▼
AppDbContext      ← Entity Framework Core
```

### Endpoints
Elke domeingroep heeft een eigen bestand met een `Map...Endpoints()` extensiemethode op `WebApplication`. Program.cs roept deze methodes aan en bevat zelf geen logica meer. Dit houdt Program.cs leesbaar (75 regels) en maakt het makkelijk om een specifiek endpoint te vinden.

### Services
De businesslogica zit volledig in de services. Voorbeelden:
- `UserService` controleert of een gebruikersnaam of e-mail al bestaat voordat een account aangemaakt wordt.
- `PaintSchemeService` valideert of een schema minimaal één stap heeft bij een update, zodat niet per ongeluk alle stappen gewist worden.
- `InventoryService` voegt een verf automatisch samen met een bestaand inventory-item als de gebruiker dezelfde verf al heeft (hoeveelheid wordt opgeteld).

De services kennen geen HTTP-concepten: ze geven domeinobjecten of exceptions terug. De vertaling naar HTTP-statuscodes gebeurt uitsluitend in de endpoints.

### Repositories
De repositories abstraheren de database-aanroepen. Elke repository heeft een bijbehorende interface (`IUserRepository`, `IPaintSchemeRepository`, etc.). Dit maakt het mogelijk om de database-implementatie later te vervangen (bijv. voor tests of een andere database) zonder de services aan te passen. De repositories bevatten alleen queries en geen businesslogica.

### ClaimsPrincipalExtensions
De JWT-claim extractie (`userId`, `isAdmin`) was eerder acht keer letterlijk herhaald in Program.cs. Dit is vervangen door twee extensiemethoden op `ClaimsPrincipal`:

```csharp
userPrincipal.GetUserId()    // geeft int? terug
userPrincipal.GetIsAdmin()   // geeft bool terug
```

Hierdoor is er één plek waar dit gedefinieerd staat, en hoeft een aanpassing (bijv. claimnaam wijzigen) maar op één plek te gebeuren.

---

## 4. Authenticatie en autorisatie

### Hoe weet de backend wie een gebruiker is?
Na een succesvolle login genereert `UserService.LoginAsync()` een JWT-token. Dit token bevat twee custom claims:
- `userId`: het database-ID van de gebruiker
- `isAdmin`: `"true"` of `"false"`

Bij elke beveiligde request stuurt de frontend dit token mee in de `Authorization: Bearer <token>`-header. Het JWT-middleware valideert automatisch de handtekening, de geldigheidsperiode en de issuer/audience. Als het token geldig is, is de `ClaimsPrincipal` beschikbaar in de endpoint.

### Hoe weet de backend wat een gebruiker mag?
Er zijn twee niveaus:

**Eigenaarschap**: bij het ophalen, bewerken en verwijderen van resources (verfschema's, inventory) filtert de backend altijd op `UserId == huidigGebruikerId`. Een gebruiker kan nooit bij de data van een ander komen, ook niet door het ID in de URL te manipuleren.

**Beheerdersrol**: de `isAdmin`-claim in het token geeft toegang tot de `"AdminOnly"` policy. Beheerders kunnen elk verfschema verwijderen, ongeacht eigenaarschap. De `isAdmin`-claim wordt bij registratie altijd op `false` gezet, ongeacht wat de client meestuurt.

### JWT-instellingen
- Geldigheidsduur: 3 uur
- Algoritme: HMAC-SHA256
- Validatie: issuer, audience, handtekening en verloopdatum worden allemaal gecontroleerd

---

## 5. Beveiliging

### Wachtwoorden
Wachtwoorden worden opgeslagen als BCrypt-hash. Bij registratie wordt het plaintext-wachtwoord direct gehasht voordat het in de database belandt. Bij login wordt `BCrypt.Verify()` gebruikt om het ingevoerde wachtwoord te vergelijken met de opgeslagen hash.

### Geen wachtwoordhash in API-responses
Bij registratie werd eerder het volledige `User`-object teruggestuurd, inclusief de gehashte password. Dit is opgelost met een `UserResponse` DTO (Data Transfer Object) dat alleen `Id`, `Username`, `Email` en `IsAdmin` bevat. De API stuurt nooit een wachtwoord (ook geen hash) terug naar de client.

### Bestandsupload
Bij het uploaden van een afbeelding worden twee checks uitgevoerd:
1. **Bestandsextensie**: alleen `.jpg`, `.jpeg`, `.png`, `.webp` en `.gif` zijn toegestaan.
2. **Magic bytes**: de eerste bytes van het bestand worden gelezen en vergeleken met de bekende signaturen van afbeeldingsformaten (JPEG: `FF D8 FF`, PNG: `89 50 4E 47`, etc.). Dit voorkomt dat iemand een uitvoerbaar bestand hernoemt naar `.jpg` om het te uploaden.

Geüploade bestanden krijgen een willekeurige UUID als bestandsnaam zodat ze niet geraden kunnen worden.

### Registratie: dubbele uniciteitscheck
Bij registratie wordt gecontroleerd of de gebruikersnaam én het e-mailadres al in gebruik zijn. Als een van beide al bestaat, krijgt de gebruiker een duidelijke foutmelding terug.

---

## 6. Datamodellering

### Entiteiten en relaties

```
User
 ├── PaintScheme[]   (1 gebruiker heeft meerdere verfschema's)
 └── InventoryItem[] (1 gebruiker heeft meerdere inventory-items)

PaintScheme
 └── Step[]          (1 schema heeft meerdere stappen)

Step
 └── Paint?          (optionele koppeling naar een verf)

InventoryItem
 └── Paint           (verwijzing naar een verf)

Paint                (gebruikersspecifieke verf)
GlobalPaint          (gedeelde verfcatalogus, beheerd door admin)
```

### GlobalPaint vs. Paint
Er zijn twee verfentiteiten. `GlobalPaint` bevat een centrale catalogus van Citadel/Warhammer-verven, geïmporteerd door een beheerder. `Paint` is een gebruikersspecifieke verf die gekoppeld kan worden aan stappen in een schema. Dit onderscheid maakt het mogelijk dat de frontend een autocomplete aanbiedt vanuit de globale catalogus, terwijl de gebruiker ook een eigen verflijst bijhoudt.

### IsStolen
Wanneer een gebruiker een schema "steelt", wordt een kopie aangemaakt met `IsStolen = true`. Dit veld maakt het mogelijk om in de "Mijn schema's"-weergave onderscheid te maken tussen eigen originelen en gestolen schema's. Gestolen schema's worden niet getoond in de algemene browse-weergave.

### Tags als JSON-array
De `Tags`-kolom op `PaintScheme` is een `List<string>` die door EF Core als JSON-array wordt opgeslagen in PostgreSQL. Dit is gekozen omdat tags geen eigen ID nodig hebben en niet vanuit andere entiteiten worden gerefereerd — een aparte Tags-tabel zou onnodige complexiteit toevoegen.

---

## 7. Frontendarchitectuur

### Standalone components
Angular 21 gebruikt standalone components als standaard. Elk component declareert zijn eigen imports (`FormsModule`, `RouterLink`, etc.) in plaats van via een gedeelde NgModule. Dit maakt de afhankelijkheden van een component direct zichtbaar in het bestand zelf.

### Signals
State management gebruikt de nieuwe Angular Signals API in plaats van RxJS BehaviorSubjects voor lokale componentstate. Signals zijn synchrone, reactieve waarden die Angular's change detection efficiënter maken. Computed signals (zoals `filteredSchemes` en `ownSchemes`) herberekenen automatisch wanneer hun afhankelijkheden veranderen.

### Services
De frontend heeft drie services die HTTP-communicatie afhandelen:
- `AuthService`: beheert het JWT-token in localStorage en biedt signals aan (`isLoggedIn`, `userId`, `isAdmin`) die door componenten gelezen worden.
- `PaintSchemeService`: alle CRUD-operaties voor verfschema's inclusief afbeelding-upload.
- `InventoryService`: inventory-beheer en ophalen van de globale verfcatalogus.

Het token uitlezen uit localStorage is bewust in de service geplaatst zodat componenten hier niet zelf mee hoeven om te gaan.

### Routing
De applicatie gebruikt Angular Router met client-side navigatie. Er is een wildcard-route (`**`) die terugvalt op de landingspagina. Alle routes zijn publiek bereikbaar in de URL, maar beveiligde pagina's tonen geen inhoud als de gebruiker niet is ingelogd.

---

## 8. Reflectie

### Wat ging goed
De keuze voor JWT met BCrypt-hashing leverde direct een werkend authenticatiesysteem op dat voldoet aan moderne beveiligingsstandaarden. De magic byte-validatie bij uploads is een bewuste extra beveiligingslaag die voorkomt dat kwaadaardige bestanden als afbeelding worden doorgegeven.

De Signals API in Angular maakte het eenvoudig om afgeleide state (gefilterde schema's, eigen vs. gestolen schema's) reactief te berekenen zonder handmatig subscriptions te beheren.

### Wat beter had gekund

**Program.cs is te groot geworden**
De grootste fout in de ontwikkeling was dat alle logica direct in Program.cs werd geschreven. Aan het begin leek dat snel en overzichtelijk, maar naarmate het project groeide werd het bestand steeds moeilijker te navigeren. Na het toevoegen van de repository-laag en services is Program.cs teruggebracht van 506 naar 75 regels. Dit had eerder moeten gebeuren: als de structuur er vanaf het begin was geweest, had iedere toevoeging automatisch op de juiste plek terechtgekomen.

**Herhaling van claim-extractie**
De code om de `userId` uit het JWT-token te halen was acht keer letterlijk herhaald in Program.cs. Dit was technische schuld die zich ophopte doordat elk nieuw endpoint gekopieerd werd van het vorige. De oplossing (`ClaimsPrincipalExtensions`) was eenvoudig maar had er direct bij het tweede endpoint al in gemoeten.

**Geen route guards in de frontend**
Beveiligde pagina's in Angular zijn wel zichtbaar in de URL maar tonen geen inhoud zonder login. Een `canActivate` guard zou de gebruiker direct doorsturen naar de loginpagina in plaats van een lege pagina te tonen — dat is zowel veiliger als gebruiksvriendelijker.

**Hardcoded baseUrl in services**
De backend-URL (`http://localhost:5166`) staat hardcoded in elke frontend-service. Bij deployment naar een andere omgeving moet dit op meerdere plekken handmatig worden aangepast. Angular's `environment.ts`-bestanden zijn de juiste oplossing hiervoor.

**Secrets in configuratiebestand**
De JWT-sleutel en het databasewachtwoord staan in `appsettings.json`. Voor een productieomgeving horen deze in omgevingsvariabelen of een secrets manager, nooit in een bestand dat in versiebeheer staat.
