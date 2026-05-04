# Orc.CsvTextEditor

Orc.CsvTextEditor is a CSV text editor component for WPF, based on AvalonEdit. It provides column-aware editing, syntax highlighting, and programmatic control of CSV content in Line of Business applications.

---

## Critical Rules (Read First)

These rules are **non-negotiable**. Violating them causes broken builds, crashes, or downstream breakage.

### 1. Never Edit Generated Files

Files matching `*.generated.cs` are auto-generated.

- **NEVER** manually edit these files

### 2. ABI / API Stability

This project maintains a stable public API. Breaking changes break downstream apps.

| Allowed | Never |
|---------|-------|
| Add new overloads | Modify existing signatures |
| Add new methods | Remove public APIs |
| Add new classes | Change return types |

### 3. Tests Are Mandatory

**Building alone is NOT sufficient.** Run tests before claiming completion (see [Commands](#commands)).

### 4. Branch Protection (COMPLIANCE REQUIRED)

**Direct commits to protected branches are a policy violation.**

| Repository | Protected Branches |
|------------|-------------------|
| Orc.CsvTextEditor | `master` |
| Orc.CsvTextEditor | `develop` |

**Required workflow:**

1. **Create a feature branch FIRST** — Use naming convention: `feature/issue-NNNN-description`
2. **Make all commits on the feature branch** — Never commit directly to protected branches
3. **Submit a Pull Request** — Changes must be reviewed by a human before merging

```bash
# CORRECT — Always create a feature branch first
git checkout -b feature/issue-1234-fix-description

# NEVER DO THIS — Policy violation
git checkout develop && git commit  # FORBIDDEN

# NEVER DO THIS — Policy violation
git checkout master && git commit  # FORBIDDEN
```

The repository has protected branches that must be respected.

---

## Commands

Single source of truth for all commands:

| Task | Command |
|------|---------|
| **Build** | `dotnet cake --target=build` |
| **Test** | `dotnet cake --target=test` |
| **Build and test** | `dotnet cake --target=buildandtest` |

---

## Architecture & Directories

### Solution Overview

```
Orc.CsvTextEditor         => Core WPF CSV editor library (targets net8.0-windows, net9.0-windows, net10.0-windows)
Orc.CsvTextEditor.Example => Example WPF application demonstrating the editor
Orc.CsvTextEditor.Tests   => Automated tests (targets net10.0-windows)
```

### Directory Guide

| Directory | Editable? | Notes |
|-----------|-----------|-------|
| `*.generated.cs` | No | Leave as-is |
| `deployment` | No | Deployment / build scripts |
| `src/Orc.CsvTextEditor/Services` | Yes | Editor services and interfaces |
| `src/Orc.CsvTextEditor/Controls` | Yes | WPF controls |
| `src/Orc.CsvTextEditor/Operations` | Yes | Editor operations |
| `src/Orc.CsvTextEditor/Transformers` | Yes | AvalonEdit document transformers |
| `src/Orc.CsvTextEditor/Models` | Yes | Data models |
| `src/Orc.CsvTextEditor.Tests` | Yes | Test project |

### Key Dependencies

- **AvalonEdit** — Underlying text editor component
- **Catel** — MVVM / IoC framework (via `Catel.Fody`, `Catel.SourceGenerators`, and `Orc.Controls`)
- **Fody** — IL weaving (method timing, obsolete handling, null checks)

---

## Writing Code

### Code Style

- Use `nullable` annotations throughout (enabled per-project via `Directory.Build.nullable.props`)
- Use C# explicit types where configured (see `Directory.Build.shared.explicit.props`)
- Do not reformat unrelated code in your commits

### Anti-Patterns (Never Do This)

| Anti-Pattern | Why |
|-------------|-----|
| Modifying method signatures | ABI breaking |
| Manual edits to `*.generated.cs` | Overwritten on regenerate |
| Using default parameters in public APIs | ABI breaking |
| **Skipping failing tests** | **Unacceptable — tests must pass** |
| Reformatting unrelated code | Makes diffs hard to review |

---

## Testing & Debugging

### Running Tests

```bash
dotnet cake --target=test
```

### Tests MUST Pass

> **NON-NEGOTIABLE:** Tests must PASS before claiming completion.
>
> - Do NOT skip failing tests
> - Do NOT claim completion if tests fail
> - Do NOT use `SkipException` to work around failures

### Public API Tests

The test project includes a public API snapshot test (`PublicApiFacts`) that detects breaking API changes. If you intentionally change the public API, update the approved snapshot file:

```
src/Orc.CsvTextEditor.Tests/PublicApiFacts.Orc_CsvTextEditor_HasNoBreakingChanges_Async.verified.txt
```

### Writing Tests

1. Use NUnit to write tests
2. Create a Facts class for a feature
3. Combine Pascal / Snake case for test methods (e.g. `Feature_Does_Work`)

```csharp
[Test]
public void Feature_Does_Work()
{
    var result = 47 - 5;

    Assert.That(result, Is.EqualTo(42));
}
```

**Philosophy:** Tests FAIL when wrong, never skip (except missing hardware).

### Debugging Methodology

1. **Establish baseline** — What's the known-good state?
2. **One change at a time** — Verify each change before proceeding
3. **Track changes in a table** — Log what you changed and the result
4. **Platform differences are signals** — If X works and Y fails, the difference IS the answer
5. **Revert if worse** — Don't pile fixes on top of failures

---

## Further Reading

| Topic | Document |
|-------|----------|
| Contributing guidelines | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Project documentation | http://opensource.wildgums.com |
| AvalonEdit | https://github.com/icsharpcode/AvalonEdit |
| Catel framework | https://github.com/catel/catel |
