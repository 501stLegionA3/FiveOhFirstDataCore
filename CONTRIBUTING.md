# Contributing to 501st Data Core
Contributions are always welcome, but we do ask you follow some basic guidelines when contributing code.

# Proper Pull Requests
Please make sure all PRs are made off the proper base so there are no merge conflicts.
All PRs must build successfuly with our CI, and any non-building PRs will not be approved.

# Commit Messages

Commit messages need to be descriptive of what the commit changes. Commits should only be focused around changing a single thing (working on a single issiue).
In the event that the change is purely documentation and not modify the way the code works, append `[skip ci]` to the start of the commit message.

# Breaking Changes

Any changes that prevent all curent users for continuing to use Partner Bot at the time of the update is considered a breaking change.
PRs with these types of changes need to include why the change is needed, how a user can continue to use Partner Bot, and, if needed,
a method of updating current user data to whaterver new implementation is there.

For example, moving for standard message sending to webhook sending is a breaking change (we moved from V5.X.X to V6.X.X)

# Code Style

We follow the [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
with a few small differences.

We prefer `this`. It is not required, but do not remove any exsisting uses of it.

Code comments that span multiple lines of code use `// ... more comment text ...` to indicate that it is continuing between lines of code.
Ex:
```csharp
// We get the random number ...
int rand = GetRandomNumber();
// ... then send it to the user.
await ctx.RespondAsync(rand.ToString());
```

It is recommended to run the code cleanup profile 1 before subitting a PR or other code change. This profile by default only removes and sorts using statements.
