# Backendtests

Voer vanuit de backendmap alle unit- en integratietests uit met:

```powershell
dotnet test .\StealMyPaintscheme.Tests\StealMyPaintscheme.Tests.csproj
```

## Unit tests

De unit tests gebruiken xUnit en Moq om services afzonderlijk te testen. Afhankelijkheden zoals repositories worden nagebootst; er wordt geen database of API gebruikt. De tests controleren servicegedrag, validatie, toegangscontrole en belangrijke repository-aanroepen.

## Integratietests

De integratietests in `Integration/ApiIntegrationTests.cs` sturen HTTP-aanvragen naar een tijdelijke API in het testproces. Ze testen de samenwerking tussen controllers, authenticatie, services, repositories en Entity Framework Core zonder mocks. Elke test gebruikt een eigen SQLite-database in het geheugen, die verdwijnt wanneer de test klaar is. De gebruikelijke PostgreSQL-database wordt niet gebruikt.

De tests controleren:

- Registratie: privacytoestemming, dubbele gebruikersnaam of e-mailadres, wachtwoordhash en het voorkomen van beheerderrechten via de aanvraag.
- Inloggen: gebruikersnaam of e-mailadres, verkeerde gegevens en toegang tot het eigen profiel met een geldig JWT.
- Beveiliging: profiel en voorraad zijn niet toegankelijk zonder geldig token.
- Voorraad: toevoegen en verhogen van een hoeveelheid, afscherming tussen gebruikers en wijzigen en verwijderen door de eigenaar.

Voer alleen de integratietests uit met:

```powershell
dotnet test .\StealMyPaintscheme.Tests\StealMyPaintscheme.Tests.csproj --filter "FullyQualifiedName~StealMyPaintscheme.Tests.Integration"
```

De API gebruikt hiervoor de omgeving `Testing`: de normale PostgreSQL-migraties en het automatisch starten van de frontend worden daarin niet uitgevoerd. De tests gebruiken SQLite en `EnsureCreated`, dus ze bewijzen niet dat PostgreSQL-specifieke queries, migraties, externe netwerkverbindingen of de frontend correct werken. Daarvoor zijn aanvullende tests met PostgreSQL en eventueel een volledige end-to-endomgeving nodig.

## Beveiligingstests

De tests in `Security/ApiSecurityTests.cs` gebruiken dezelfde tijdelijke API en SQLite-testdatabase. Ze controleren:

- JWT's: een geldige login en een geldig testtoken krijgen toegang; een onjuiste handtekening, afzender of doelgroep, een verlopen of nog niet geldig token en een ontbrekende gebruikers-ID worden afgewezen. De verlopen en nog niet geldige tokens liggen meer dan vijf minuten buiten de geldigheidsperiode; de standaard toegestane klokafwijking wordt niet getest.
- Beheerderrechten: een anonieme gebruiker, gewone gebruiker en gebruiker met een aangepast token kunnen geen globale verf importeren; een daadwerkelijk ingelogde beheerder kan dat wel. Ook wordt gecontroleerd dat een registratie geen beheerderrechten kan toekennen.
- Eigenaarschap: een andere gebruiker kan een paint scheme niet wijzigen of verwijderen, ook niet door een gebruikers-ID mee te sturen. De eigenaar kan beide acties wel uitvoeren.
- Inloggen: één invoer met een SQL-injectiepatroon wordt afgewezen en levert geen token op.
- Uploaden: een aanvraag zonder token, een bestand met verboden extensie en een bestand met een onjuiste afbeeldingsheader worden geweigerd.

Voer alleen deze beveiligingstests uit met:

```powershell
dotnet test .\StealMyPaintscheme.Tests\StealMyPaintscheme.Tests.csproj --filter "FullyQualifiedName~StealMyPaintscheme.Tests.Security"
```

Dit zijn gerichte regressietests, geen volledige securityaudit of penetratietest. Ze bewijzen bijvoorbeeld niet dat alle SQL-injecties, XSS, misbruik van uploads of beveiligingsproblemen in PostgreSQL en de productieomgeving zijn uitgesloten. De uploadtests controleren alleen de genoemde extensie en header, niet of een bestand veilig is. Ook afhankelijkheden, rate limiting, transportbeveiliging en de frontend vallen buiten deze tests.
