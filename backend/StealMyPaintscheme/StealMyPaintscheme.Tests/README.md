# Backend unit tests

Voer vanuit de backendmap uit:

```powershell
dotnet test .\StealMyPaintscheme.Tests\StealMyPaintscheme.Tests.csproj
```

De tests gebruiken xUnit en Moq om de services te testen zonder database of draaiende API. Ze controleren servicegedrag, validatie, toegangscontrole en belangrijke repository-aanroepen; databaseconfiguratie, HTTP-endpoints en volledige integratie zijn hiermee niet getest.
