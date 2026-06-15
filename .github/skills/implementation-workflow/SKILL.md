---
name: implementation-workflow
description: 'Execute structured implementation work with minimal diffs. Use when implementing a feature/refactoring task without concept authoring.'
argument-hint: 'Task, scope, constraints, acceptance criteria'
---

# Implementation Workflow

## Purpose
Provide a compact, repeatable implementation flow for end-to-end coding tasks.

## When To Use
- Feature implementation without separate concept document
- Refactoring with clear target behavior
- Multi-step implementation tasks that need structure

## Procedure
1. Analyze context and identify impacted files/components.
2. Create a short task list with small verifiable steps.
3. Implement minimal, non-invasive changes.
4. Remove obsolete code in touched scope when safe.
5. Re-check assumptions if complexity increases.
6. Validate with the narrowest useful build/test checks.
7. Update relevant docs when behavior/contracts changed.

## Guardrails
- Keep scope tight; avoid unrelated refactorings.
- Prefer existing patterns over new abstractions.
- Clarify trade-offs only when behavior/UX/architecture is materially affected.
- Do not create concept documents here; use `concept-plan` for that.
