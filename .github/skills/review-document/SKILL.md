---
name: review-document
description: 'Create structured German review documents with fixed sections and severity-based findings. Use when a standalone review artifact is requested.'
argument-hint: 'Review scope (PR/files/module) and optional references'
---

# Review Document

## Purpose
Generate a concise, structured review document in German.

**Output language: German** — all section content, findings, and recommendations must be in German.

## Usage
Do not ask follow-up questions. Derive missing metadata from prompt and context.

Review documents must be saved as a file in the folder source/.documentation/.reviews.

Apply these rules before generating the document:
- Derive a short German title from scope/topic
- Set `Erstelldatum` and `Letzte Änderung` to today's date
- Include `Issue`, `Task`, `PBI` only if explicitly provided
- If no reference is provided, include one empty fallback row: `| **Issue:** | |`
- Clickable Markdown links are strongly encouraged—to source code, external documentation, and sections of this document, where appropriate

All sections must be present. If no content exists for a section, write `— entfällt —`.

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

Regeln zur Kopf-Tabelle:
- `Issue`, `Task`, `PBI` nur bei expliziter User-Angabe
- Sonst genau eine leere Fallback-Zeile: `| **Issue:** | |`

# 1. Scope & Kontext
- Review-Umfang (PR, Dateien, Modul)
- Ziel und Randbedingungen

# 2. Zusammenfassung
- Gesamtbewertung: Approve / Request Changes / Needs Discussion
- Anzahl Findings je Severity

# 3. Findings (Critical)
- ID, Ort, Problem, Risiko, Empfehlung

# 4. Findings (Major)
- ID, Ort, Problem, Risiko, Empfehlung

# 5. Findings (Minor/Suggestion)
- ID, Ort, Problem, Empfehlung

# 6. Test- & Regressionssicht
- Abdeckungslücken, notwendige Tests, Regression-Risiken

# 7. Risiken
> Nur falls erforderlich, Reviews enthalten planmäßig nur harte Empfehlungen, keine offenen Fragen.

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
> Nur falls erforderlich, Reviews enthalten planmäßig nur harte Empfehlungen, keine offenen Fragen.  

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