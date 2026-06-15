---
name: csharp-mstest
description: 'Write concise MSTest 3.x/4.x tests with correct modern asserts. Use only MSTest in this repository (no xUnit/NUnit suggestions).'
---

# MSTest Assert-First Guide (MSTest 3.x/4.x)

||| 
|-|-|
| **Framework:** | MSTest only |
| **Goal:** | Correct, semantic asserts instead of generic patterns |
| **Do not use:** | xUnit, NUnit, FluentAssertions recommendations |

Use this skill to write short, readable MSTest tests with the **right assert for intent**.

## Core Rules
- Use only MSTest APIs and attributes in this repository.
- Prefer semantic asserts over generic ones.
- Prefer `Assert` APIs over `StringAssert`/`CollectionAssert` when equivalent exists.
- Prefer `Assert.Throws*` over `[ExpectedException]`.
- Keep tests small and behavior-focused (AAA, one behavior per test).

## Assert Selection (Important)

Prefer these patterns:

```csharp
// Count / size
Assert.HasCount(5, list);            // preferred
Assert.IsEmpty(list);                // preferred
Assert.IsNotEmpty(list);             // preferred

// Single item
var item = Assert.ContainsSingle(list);

// Containment
Assert.Contains(expected, list);
Assert.DoesNotContain(unexpected, list);

// Type
var typed = Assert.IsInstanceOfType<MyType>(obj);

// Exceptions
var ex = Assert.ThrowsExactly<InvalidOperationException>(() => Act());
var exAsync = await Assert.ThrowsAsync<ArgumentException>(() => ActAsync());
```

Avoid these when a semantic assert exists:

- `Assert.AreEqual(5, list.Count)` -> use `Assert.HasCount(5, list)`
- `Assert.AreEqual(1, list.Count)` -> use `Assert.ContainsSingle(list)`
- `Assert.IsTrue(list.Any())` -> use `Assert.IsNotEmpty(list)`
- `Assert.IsTrue(!list.Any())` -> use `Assert.IsEmpty(list)`
- Hard cast + null checks -> use `Assert.IsInstanceOfType<T>`

## Quick Assert Reference

### Collection Assertions (Assert class)

```csharp
Assert.Contains(expectedItem, collection);
Assert.DoesNotContain(unexpectedItem, collection);
Assert.ContainsSingle(collection);
Assert.HasCount(5, collection);
Assert.IsEmpty(collection);
Assert.IsNotEmpty(collection);
```

### String Assertions (Assert class)

```csharp
Assert.Contains("expected", actualString);
Assert.StartsWith("prefix", actualString);
Assert.EndsWith("suffix", actualString);
Assert.DoesNotStartWith("prefix", actualString);
Assert.DoesNotEndWith("suffix", actualString);
Assert.MatchesRegex(@"\d{3}-\d{4}", phoneNumber);
Assert.DoesNotMatchRegex(@"\d+", textOnly);
```

### Comparison Assertions

```csharp
Assert.IsGreaterThan(lowerBound, actual);
Assert.IsGreaterThanOrEqualTo(lowerBound, actual);
Assert.IsLessThan(upperBound, actual);
Assert.IsLessThanOrEqualTo(upperBound, actual);
Assert.IsInRange(actual, low, high);
Assert.IsPositive(number);
Assert.IsNegative(number);
```

### Type Assertions

```csharp
// MSTest 3.x
Assert.IsInstanceOfType<MyClass>(obj, out var typed3x);
typed3x.DoSomething();

// MSTest 4.x
var typed4x = Assert.IsInstanceOfType<MyClass>(obj);
typed4x.DoSomething();

Assert.IsNotInstanceOfType<WrongType>(obj);
```

## Minimal MSTest Structure

```csharp
[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var sut = new Calculator();

        // Act
        var result = sut.Add(2, 3);

        // Assert
        Assert.AreEqual(5, result);
    }
}
```

## Data-Driven Tests (Short)

- Use `[DataRow(...)]` for small inline sets.
- Use `[DynamicData]` with `IEnumerable<(...)>` (ValueTuple) for typed larger sets.
- Avoid new `IEnumerable<object[]>` data sources.

## TestContext (Only What Matters)

- Use `TestContext.CancellationToken` in async tests, especially with `[Timeout]`.

```csharp
[TestMethod]
[Timeout(5000)]
public async Task Request_CompletesWithinTimeout()
{
    await client.GetAsync(url, TestContext.CancellationToken);
}
```

## Common Mistakes to Avoid

```csharp
// ❌ Wrong argument order
Assert.AreEqual(actual, expected);
// ✅ Correct
Assert.AreEqual(expected, actual);

// ❌ Using ExpectedException (obsolete)
[ExpectedException(typeof(ArgumentException))]
// ✅ Use Assert.Throws
Assert.Throws<ArgumentException>(() => Method());

// ❌ Using LINQ Single() - unclear exception
var item = items.Single();
// ✅ Use ContainsSingle - better failure message
var item = Assert.ContainsSingle(items);

// ❌ Generic count assertion
Assert.AreEqual(5, items.Count);
// ✅ Semantic count assertion
Assert.HasCount(5, items);

// ❌ Hard cast - unclear exception
var handler = (MyHandler)result;
// ✅ Type assertion - shows actual type on failure
var handler = Assert.IsInstanceOfType<MyHandler>(result);

// ❌ Ignoring cancellation token
await client.GetAsync(url, CancellationToken.None);
// ✅ Flow test cancellation
await client.GetAsync(url, TestContext.CancellationToken);

// ❌ Making TestContext nullable - leads to unnecessary null checks
public TestContext? TestContext { get; set; }
// ❌ Using null! - MSTest already suppresses CS8618 for this property
public TestContext TestContext { get; set; } = null!;
// ✅ Declare without nullable or initializer - MSTest handles the warning
public TestContext TestContext { get; set; }
```
