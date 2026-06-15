---
name: concept-plan
description: 'Create structured concept and implementation plan documents. Use when designing a new feature, use case, or larger refactoring — NOT for bug fixes (use the bug-fix skill instead). Produces a standardized German-language document with a fixed set of sections that are always present.'
argument-hint: 'Short topic description'
---

# Concept & Implementation Plan

## Purpose
Generate structured concept and implementation plan documents for **features, use cases, and larger refactorings**. Not intended for bug fixes — use the `bug-fix` skill for those.

**Output language: German** — all generated content must be written in German, including section content, bullet points, diagram labels, and explanations.

## Usage
Do not ask the developer follow-up questions. Derive all missing document metadata from the provided description and context.

Concept and implementation plans must be saved as a file in the folder source/.documentation/.inbox.

Apply these rules before generating the document:
- Derive a **short, precise German title** from the topic description and file name
- Set **Erstelldatum** and **Letzte Änderung** to today's date
- Include **Issue**, **Task**, and **PBI** rows only for references explicitly provided by the user
- If no reference was provided at all, include exactly one empty fallback row: `| **Issue:** | |`
- Include **Ausarbeitung** only if the user explicitly provided it
- Clickable Markdown links are strongly encouraged—to source code, external documentation, and sections of this document, where appropriate

Then produce the complete document using the template below.

**All 7 sections must always be present.** If a section has no content for this specific document, write `— entfällt —` beneath the heading. Never skip or remove a section.
Use **Kapitel 7. Risiken & Offene Punkte** to capture assumptions, unknowns, missing references, and questions that would otherwise require follow-up.

If the input is primarily an architecture/trade-off discussion:
- Capture at least 2 options with pros/cons in **Kapitel 5. Implementierungsplan**
- Add a short recommendation and rationale in **Kapitel 3 oder 5**
- Capture unresolved assumptions and decision dependencies in **Kapitel 7**

---

## Template

# [Kurzer deutscher Titel]

|||
|-|-|
| **Erstelldatum:** | [heutiges Datum] |
| **Letzte Änderung:** | [heutiges Datum] |
| **Issue:** | #XX — Titel (Link) |
| **Task:** | XXXXX (Link) |
| **PBI:** | XXXXX (Link) |
| **Ausarbeitung:** | Titel (Link) *(optional)* |

Regeln zur Kopf-Tabelle:
- `Issue`, `Task`, `PBI` nur aufnehmen, wenn vom User explizit angegeben
- Wenn keine dieser Referenzen angegeben wurde: genau eine leere Fallback-Zeile verwenden: `| **Issue:** | |`
- `Ausarbeitung` nur aufnehmen, wenn vom User explizit angegeben

# 1. Kontext, Zielsetzung & Use Case
- Hintergrund und Auslöser (Bug, Feature, Anforderung)
- Ziel und gewünschtes Ergebnis
- Relevante Use Cases
- Begriffsdefinitionen *(nur wenn notwendig)*

# 2. Analyse / Ist-Zustand
- Aktueller Zustand in Code oder Architektur
- Betroffene Komponenten, Klassen, Schnittstellen
- Kernproblem bzw. identifizierte Lücke
- Code-Snippets oder Mermaid-Diagramme *(nur wenn zum Verständnis nötig)*

# 3. Lösungskonzept / Soll-Zustand
- Angestrebter Zustand nach der Umsetzung
- Wesentliche Designentscheidungen
- Explizite Nicht-Ziele (was bewusst nicht umgesetzt wird)

# 4. Contracts
- Relevante Kommunikations-Verträge (Redis, REST-APIs, Events, Datenformate, Protokolle)
- Pro Schnittstelle: Richtung, Datenstruktur, Protokoll

# 5. Implementierungsplan
- Phasen oder Meilensteine der Umsetzung (je Phase: Beschreibung + betroffene Komponenten)
- Mermaid-Diagramme *(falls dem Verständnis zuträglich)*
- Falls mehrere Ansätze existieren: kurzer Vergleich + begründete Empfehlung

# 6. Tests
- Geplante Teststufen: Unit / Integration / E2E
- Testfälle und Akzeptanzkriterien
- Relevante Randbedingungen oder Testdaten

# 7. Risiken
Nomenklatur: `R-##`

## 7.1 R-01: ...
### tl;dr
- Kurze Zusammenfassung

### Detail
- Detaillierte Ausführung mit Hintergrund
- Klickbare Links auf Quellcode, externe Dokumentation und Abschnitte dieses Dokuments sind ausdrücklich erwünscht, sofern zweckdienlich

### Empfehlung
- Begründete Empfehlung zur Umsetzung

## 7.2 R-02: ...
etc.

# 8. Offene Punkte
Nomenklatur: `OP-##`

## 8.1 OP-01: ...
### tl;dr
- Kurze Zusammenfassung

### Detail
- Detaillierte Ausführung mit Hintergrund
- Klickbare Links auf Quellcode, externe Dokumentation und Abschnitte dieses Dokuments sind ausdrücklich erwünscht, sofern zweckdienlich

### Empfehlung
- Begründete Empfehlung zur Umsetzung

## 8.2 OP-02: ...
etc.