---
name: review-duplication
description: 'Analyze duplication in code reviews (exact, structural, behavioral, cross-layer, pattern drift) and provide consolidation recommendations with severity and confidence.'
argument-hint: 'Review scope: files/modules and suspected duplicate areas'
---

# Review Duplication

## Purpose
Detect harmful duplication and redundancy in reviews and provide practical consolidation guidance.

## When To Use
- Similar logic appears in multiple places
- New helpers overlap with existing abstractions
- Pattern drift is suspected across modules/layers

## Procedure
1. Identify duplicate type:
   - exact
   - structural
   - behavioral
   - cross-layer
   - pattern drift
2. Verify whether semantics are truly shared.
3. Assess whether consolidation reduces real risk/cost or only adds indirection.
4. Return per finding:
   - new location
   - existing location
   - duplication type
   - risk/impact
   - concrete consolidation direction

## Severity Guide
- Critical: immediate correctness/security/divergence risk
- Major: clear maintenance burden or architectural drift
- Minor: low-risk drift/inconsistency
- Suggestion: optional simplification

## Guardrails
- Do not force consolidation when boundaries justify local duplication.
- State assumptions and confidence when context is incomplete.
- Prefer existing shared abstractions over new ad-hoc utilities.
