# Backend op Linux met Docker

Deze configuratie draait alleen de backend. Een bestaande PostgreSQL-database en een apart geconfigureerde frontend en HTTPS-reverse-proxy zijn nodig.

1. Stel op de server de omgevingsvariabelen `DATABASE_CONNECTION` en `JWT_KEY` in. Gebruik een sterke, willekeurige JWT-sleutel van minstens 32 bytes en bewaar beide waarden buiten de repository. Bijvoorbeeld:

   ```sh
   export DATABASE_CONNECTION='Host=database-host;Database=steal_my_paint_scheme;Username=...;Password=...'
   export JWT_KEY='vervang-dit-door-een-lange-willekeurige-geheime-sleutel'
   ```

   `localhost` in de databaseverbinding verwijst naar de backendcontainer, niet naar de Linux-server. Gebruik een vanuit de container bereikbaar databaseadres. De databasegebruiker moet migraties kunnen uitvoeren.

2. Voer vanuit deze map uit:

   ```sh
   docker compose up -d --build
   ```

De backend luistert op `127.0.0.1:8080` op de server. Configureer een reverse proxy of Cloudflare Tunnel voor HTTPS en stuur API-aanvragen én `/uploads/` naar deze backend. Configureer de frontend apart. Dit bestand configureert geen Cloudflare, database of frontend.

Afbeeldingen staan in het Docker-volume `uploads` en blijven behouden bij het opnieuw bouwen of vervangen van de container. Een volume is geen back-up: maak regelmatig afzonderlijke back-ups van zowel het volume als de database, ook buiten de server. Gebruik geen `docker compose down -v`, want daarmee wordt het volume verwijderd.

Bestaande afbeeldingen worden niet automatisch overgezet. Kopieer de inhoud van de bestaande `wwwroot/uploads` naar het volume vóór de overstap en behoud de bestandsnamen. Controleer daarna of bestaande afbeeldings-URL's werken. Zorg bij het kopiëren dat de containergebruiker de bestanden kan lezen en de map kan beschrijven.

De voorbeeldwaarden in `appsettings.json` zijn niet geschikt voor productie. De variabelen hierboven overschrijven de databaseverbinding en JWT-sleutel; zorg ook dat de overige productie-instellingen bij de uiteindelijke hosting passen.