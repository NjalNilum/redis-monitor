---
name: bug-fix
description: 'Find and fix bugs in C# or C++ codebases. Use when investigating crashes, incorrect behavior, memory issues, regressions, or unexpected exceptions. Guides through structured reproduction, root-cause analysis, fix, and verification. Not for new features or refactorings (use concept-plan instead).'
argument-hint: 'Short bug description or error message'
---

# Bug Fix — C# / C++

## Purpose
Guide a structured bug investigation and fix in C# or C++ codebases. All output and summaries in **German**.

## Procedure

### 1. Reproduce
- Clarify exact steps to reproduce the bug (input, state, environment)
- Confirm whether the bug is deterministic or intermittent
- Identify the failing assertion, exception, crash, or wrong output

### 2. Isolate
- Narrow down to the smallest reproducing case
- Identify the affected component, class, function, or module
- Check recent changes (git log/blame) that could have introduced the regression

### 3. Root-Cause Analysis
- Trace execution path to the failure point
- Identify the actual defect (logic error, null reference, race condition, memory corruption, etc.)
- Document the root cause in 1–3 bullet points before touching any code

### 4. Fix
- Make the minimal targeted change — do not refactor unrelated code
- Ensure the fix addresses the root cause, not just the symptom
- Apply language-specific checks (see below)

### 5. Verify
- Confirm the original reproduction case no longer fails
- Run existing unit/integration tests; add a regression test if none covers this case
- Check for side effects on related functionality

---

## Language-Specific Notes

### C#
- Check for `NullReferenceException`: use `?.`, null checks, or `ArgumentNullException.ThrowIfNull()`
- Async bugs: verify `await` usage, avoid `async void`, check `ConfigureAwait`
- Threading: check for shared mutable state without locks, use `Interlocked` or `lock` correctly
- Resource leaks: ensure `IDisposable` objects are in `using` blocks
- Use the debugger (breakpoints, watch, call stack) and structured logging to trace state

### C++
- Memory: check for use-after-free, double-free, buffer overflows — use AddressSanitizer (`-fsanitize=address`) or Valgrind
- Undefined behavior: uninitialized variables, signed integer overflow, strict aliasing violations
- Threading: data races → use sanitizers (`-fsanitize=thread`), verify mutex scope
- Check compiler warnings at max level (`/W4` MSVC, `-Wall -Wextra` GCC/Clang) — treat new warnings as errors
- Prefer RAII and smart pointers (`unique_ptr`, `shared_ptr`) over raw ownership

---

## Output Summary (in German)
After completing the investigation, produce a short summary with:
- **Symptom:** Was war das beobachtete Fehlverhalten?
- **Ursache:** Was war die eigentliche Ursache?
- **Fix:** Welche Änderung wurde vorgenommen und warum?
- **Verifikation:** Wie wurde die Korrektur bestätigt?
- **Regressionstest:** Welcher Test wurde ergänzt (oder warum keiner nötig war)?
