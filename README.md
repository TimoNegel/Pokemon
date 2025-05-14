## 📌 Pokémon-API-Projekt  
Eine App zur Anzeige und Sammlung von Pokémon  

## 📝 Beschreibung 
Dieses Projekt nutzt die **PokéAPI**, um Pokémon-Daten abzurufen und diese in einer übersichtlichen **Pokédex-ähnlichen Card/List View** darzustellen. Benutzer können sich detaillierte Informationen zu einzelnen Pokémon anzeigen lassen.  

Darüber hinaus bietet die App ein besonderes Feature für registrierte Nutzer: Jeden Tag gibt es die **Chance**, ein Pokémon einzusammeln. Allerdings bekommt man es nicht automatisch – man muss es aktiv einsammeln! Zudem kann es vorkommen, dass man das Pokémon bereits besitzt. In diesem Fall erhält man leider an diesem Tag kein neues Pokémon für die Sammlung.  

Um ein authentisches Erlebnis zu schaffen, werden die **Pokémon-Bilder dynamisch angepasst**, sodass ihre Größe je nach Evolutionsstufe variiert. So erscheinen größere Pokémon entsprechend prominenter als kleinere Pokémon, was die Darstellung realistischer macht.  

## 📜 Projektauftrag  
Dieses Projekt wurde **fiktiv** von einer **deutschen Pokémon-Community** als Proof of Concept mit einer Laufzeit von **3 Monaten** in Auftrag gegeben. Ziel ist es, die oben beschriebenen Funktionen zu implementieren und erste Erfahrungswerte zu sammeln.

Eine besondere Anforderung des Auftrags war die **dynamische Skalierung der Pokémon-Bilder je nach Evolutionsstufe**. Dadurch erscheinen größere Pokémon entsprechend prominenter als kleinere Pokémon, um eine möglichst realistische Darstellung zu gewährleisten. 
  
Da es sich um eine Machbarkeitsstudie handelt, sind einige Aspekte noch nicht vollständig enthalten oder funktionieren nicht optimal:  

❌ **Keine automatisierten Tests**  
❌ **Nicht alle Warnungen gefixt**  
❌ **Keine Pagination beim Laden der Daten im Pokédex**  
❌ **Nicht optimierte Ladezeiten in Pokédex, Pokémon-Sammlung und Detailansicht**  
❌ **Nicht alle Komponenten in eigene Klassen ausgelagert**  
❌ **CSS noch nicht vollständig ausgelagert**  
❌ **User-Management-Seiten nicht optimiert und designt**  
❌ **Fehlendes oder unvollständiges Error Handling**  
❌ **Theme noch nicht vollständig angepasst (insbesondere Dark Mode)**  
❌ **Desktop-Anwendung ohne mobile Optimierung**  

Dieses **fiktive** Proof of Concept dient als Grundlage für eine mögliche Weiterentwicklung und soll nach Ablauf der Testphase evaluiert werden.  

## 🎯 Zielgruppe  
Obwohl dieses Projekt als **fiktives Proof of Concept** entwickelt wurde, richtet es sich konzeptionell an folgende Gruppen:  

👾 **Pokémon-Fans & Sammler** – Die Anwendung ermöglicht es, Pokémon interaktiv zu entdecken und eine virtuelle Sammlung aufzubauen.  
💻 **Entwickler & Technik-Begeisterte** – Wer sich für API-Integration, Datenverarbeitung und UI-Design interessiert, kann von diesem Projekt lernen.  
🎮 **Community-Mitglieder & Tester** – Falls das Projekt über das Konzept hinaus erweitert wird, könnten Beta-Tester und Feedbackgeber eine wichtige Rolle spielen.  

Da das Projekt als Machbarkeitsstudie dient, ist es eine **potenzielle Grundlage für eine Weiterentwicklung**, bei der die Zielgruppen noch klarer definiert und spezifischer angesprochen werden können.  

## ⚙️ Technologien  
Dieses Projekt basiert auf folgenden Technologien:  

🔹 **Programmiersprachen:** C#  
🔹 **Framework:** Blazor Server – Für die Entwicklung der Anwendung  
   - **Begründung:** Blazor Server wurde gewählt, weil es eine leistungsstarke und moderne Lösung zur Entwicklung interaktiver Webanwendungen bietet. Es nutzt C# statt JavaScript, wodurch Entwickler in einer einzigen Sprache für Backend und Frontend arbeiten können. Ein weiterer Vorteil ist die **integrierte Authentifizierung** und das **User Management**, was die Sicherheit und Verwaltung der Benutzer erleichtert. Blazor Server ermöglicht eine **einfache Kommunikation mit der Datenbank**, da es direkt mit .NET und Entity Framework integriert werden kann. Außerdem reduziert es die **Client-seitige Rechenlast**, da die Logik auf dem Server läuft und nur UI-Updates an den Client gesendet werden. Dies führt zu einer **geringen Speicher- und CPU-Belastung** auf dem Client, was besonders für leistungsschwächere Geräte oder langsame Verbindungen von Vorteil ist. Zusätzlich unterstützt es **echte Echtzeit-Updates**, da es über SignalR arbeitet und schnelle Interaktionen ermöglicht. Ein weiterer Vorteil ist die **Wartungsfreundlichkeit**, da keine komplizierten JavaScript-Bibliotheken erforderlich sind. Durch **automatische State-Verwaltung** bleibt die UI konsistent, auch wenn sich Daten auf dem Server ändern. Nicht zuletzt bietet Blazor Server **eine enge Integration mit anderen .NET-Technologien**, was eine robuste Entwicklungsumgebung schafft. Da der Code auf dem Server läuft, sind sensible Daten und Geschäftslogik besser geschützt als bei Client-basierten Anwendungen.  

🔹 **Frontend-Komponenten:** MudBlazor – Ermöglicht eine moderne und anpassbare UI  

- **Begründung:** MudBlazor wurde gewählt, weil es eine moderne und anpassbare UI-Komponente für Blazor bietet. Es basiert auf Material Design und ermöglicht eine ansprechende und konsistente Gestaltung der Oberfläche. Die Bibliothek stellt zahlreiche **vorbereitete Komponenten** zur Verfügung, sodass Entwickler schneller eine ansprechende UI gestalten können. **Dark Mode-Unterstützung** ist standardmäßig enthalten, was eine bessere Benutzererfahrung für viele Nutzer ermöglicht. MudBlazor bietet eine einfache **Theme-Anpassung**, wodurch Farben, Schriften und Layouts flexibel geändert werden können. Die Integration mit Blazor ist **nahtlos**, sodass Entwickler keine zusätzlichen Abhängigkeiten oder komplexe Workarounds benötigen. Zudem ist die Bibliothek **gut dokumentiert**, was die Einarbeitung erleichtert. MudBlazor verbessert die **Performance** durch optimierte Komponenten und erleichtert die **Responsive Entwicklung** für mobile und Desktop-Anwendungen. Durch regelmäßige Updates bleibt die Bibliothek aktuell und sicher, was langfristige Wartbarkeit gewährleistet.  

🔹 **API:** PokéAPI ([https://pokeapi.co](https://pokeapi.co)) – Abruf der Pokémon-Daten  

Da dieses Projekt als **Proof of Concept** entwickelt wurde, sind einige Optimierungen noch nicht vollständig umgesetzt (siehe Projektauftrag). Falls es weiterentwickelt wird, könnten weitere Technologien hinzukommen oder bestehende angepasst werden.  



## Technische Entscheidungen  

### Keine Verwendung einer PokéAPI-Wrapper-Bibliothek  
- Für dieses Projekt wurde bewusst auf die Nutzung einer **Wrapper-Bibliothek** für die PokéAPI verzichtet. Der direkte Zugriff auf die API ermöglicht eine **größere Flexibilität** in der Verarbeitung der Daten und verhindert unnötige Abhängigkeiten. Da dieses Projekt als **Übung** dient, soll die direkte Implementierung die praktische Erfahrung mit API-Aufrufen verbessern und ein besseres Verständnis der Datenstrukturen vermitteln.  

### Caching-Strategie  
- Das Projekt verwendet eine **mehrstufige Caching-Strategie** für Pokémon- und Moves-Daten, um API-Aufrufe zu minimieren und die Ladezeiten zu optimieren:  
1️⃣ Falls ein Pokémon oder ein Move bereits im **Cache** vorhanden ist, wird es direkt daraus geladen.  
2️⃣ Falls es nicht im Cache gespeichert wurde, wird es aus der **Datenbank** abgerufen.  
3️⃣ Falls es auch dort nicht vorhanden ist, wird es direkt über die **PokéAPI** geladen.  
4️⃣ Falls die Daten aus der **Datenbank** stammen, werden sie zusätzlich im **Cache gespeichert**, um zukünftige Abrufe zu beschleunigen.  
5️⃣ Falls die Daten direkt aus der **API** geladen werden, werden sie sowohl in der **Datenbank** als auch im **Cache gespeichert**, um die Verfügbarkeit zu verbessern und die API-Nutzung zu reduzieren.  

Diese Strategie sorgt für eine effiziente Verarbeitung der Daten und minimiert Ladezeiten für den Benutzer.  

### Retry-Logik bei API-Anfragen  
- Da externe APIs gelegentlich Verbindungsprobleme haben oder temporär ausfallen, wurde eine **Retry-Mechanik** integriert. Falls eine Anfrage an die PokéAPI fehlschlägt, wird sie **3 mal** in definierten Intervallen erneut gesendet, bevor ein Fehler geworfen wird. Dabei wird **exponentielles Backoff** verwendet, um unnötige Belastung der API zu vermeiden und die Chancen auf eine erfolgreiche Antwort zu erhöhen.  

### Error-Handling-Strategie  
- Da dieses Projekt eine **Machbarkeitsstudie** ist, wurde das **Error Handling noch nicht überall implementiert**. Die grundlegende Strategie fokussiert sich darauf, **möglichst wenige Daten zu verlieren**, während gleichzeitig sichergestellt wird, dass **Fehler nicht nachfolgende Prozesse blockieren**.  

Eine detaillierte Implementierung der Fehlerbehandlung wird in einer späteren Version ergänzt, sobald das Projekt über die Machbarkeitsstudie hinausgeht.  
## ⚙️ Installation & Setup  

Um dieses Projekt lokal auszuführen, folge den folgenden Schritten:  

### 1️⃣ **Repository klonen**  
Öffne eine Konsole und navigiere zu dem Verzeichnis, in dem du das Projekt speichern möchtest. Führe dann diesen Befehl aus, um das Repository zu klonen:  

```sh
git clone https://github.com/TimoNegel/Pokemon.git
cd Pokemon
```

### 2️⃣ **Abhängigkeiten installieren**  
Das Projekt verwendet **NuGet-Pakete**. Installiere die notwendigen Pakete mit:  

```sh
dotnet restore
```

### 3️⃣ **Datenbank einrichten**  
 Führe dazu folgenden Befehl aus:  

```sh
dotnet ef database update
```

### 4️⃣ **Projekt ausführen**  
Starte die Anwendung mit:  

```sh
dotnet run
```


Nach dem Start solltest du das Projekt unter **http://localhost:7111** oder einer ähnlichen lokalen Adresse erreichen können.  

## 🔮 Ausblicke in die Zukunft  

Dieses Projekt befindet sich aktuell in der **Proof of Concept-Phase**. Nach Abschluss der Testphase könnte es weiterentwickelt werden, um zusätzliche Funktionen und Verbesserungen zu integrieren.  

Mögliche zukünftige Erweiterungen könnten sein:  
✨ **Optimierte Ladezeiten** – Verbesserung der Performance in der Pokédex-Ansicht und Detailansicht  
✨ **Fehlermanagement ausbauen** – Implementierung eines umfangreichen Error-Handlings  
✨ **Pagination für große Datenmengen** – Effizientere Darstellung von Pokémon-Listen  
✨ **Dark Mode-Anpassung** – Optimierung der UI für dunkle Designs  
✨ **Mobile Unterstützung** – Anpassung des Designs für Smartphones und Tablets  
✨ **Mehr Features für die Pokémon-Sammlung** – Eventuelle Trading-Funktion oder erweiterte Statistiken  
✨ **Verbesserung des User-Managements** – Modernisierung der Benutzerverwaltung mit erweiterten Einstellungen  

Die nächsten Entwicklungsschritte hängen von der Auswertung der aktuellen Testphase ab. Falls das Projekt weitergeführt wird, könnten gezielt Community-Vorschläge integriert werden.  
