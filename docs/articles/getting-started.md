# Getting Started

This guide shows step by step how to install and use `NRules` rules engine to create an auto insurance quote calculator that has its logic encoded as standalone rules, separated from the application logic.

You can also find this complete example under the `samples` folder.

This guide shows how to use `NRules` with the fluent DSL in C#. To see how to use `NRules` with externalized rules, written in R# rules language, see [NRules.Languange](https://github.com/NRules/NRules.Language).

## Creating Project Structure
Use .NET CLI in a terminal to create a new Visual Studio solution and project structure.

# [Windows](#tab/windows)
```console
dotnet new sln -o GettingStarted
cd GettingStarted

dotnet new console -o App
dotnet sln GettingStarted.sln add App\App.csproj

dotnet new classlib -o Domain
dotnet sln GettingStarted.sln add Domain\Domain.csproj

dotnet new classlib -o Rules
dotnet sln GettingStarted.sln add Rules\Rules.csproj

dotnet add App\App.csproj reference Rules\Rules.csproj
dotnet add App\App.csproj reference Domain\Domain.csproj
dotnet add Rules\Rules.csproj reference Domain\Domain.csproj
```
# [MacOS/Linux](#tab/nix)
```console
dotnet new sln -o GettingStarted
cd GettingStarted

dotnet new console -o App
dotnet sln GettingStarted.sln add App/App.csproj

dotnet new classlib -o Domain
dotnet sln GettingStarted.sln add Domain/Domain.csproj

dotnet new classlib -o Rules
dotnet sln GettingStarted.sln add Rules/Rules.csproj

dotnet add App/App.csproj reference Rules/Rules.csproj
dotnet add App/App.csproj reference Domain/Domain.csproj
dotnet add Rules/Rules.csproj reference Domain/Domain.csproj
```
---

## Installing NRules
Add NRules package references to the corresponding projects. In this sample application, we are keeping rules in a separate project, so this project only needs to reference the `NRules.Fluent` package. The domain model project will contain plain c# objects (POCO), so no additional references are necessary. Finally, the application project will need to use rules engine runtime, as well as load rules from the rules assembly, so it's best to reference the meta package `NRules` that brings all the necessary components.

In the terminal, run the following commands.

# [Windows](#tab/windows)
```console
dotnet add App\App.csproj package NRules
dotnet add Rules\Rules.csproj package NRules.Fluent
```
# [MacOS/Linux](#tab/nix)
```console
dotnet add App/App.csproj package NRules
dotnet add Rules/Rules.csproj package NRules.Fluent
```
---

Open the resulting solution in an IDE to continue creating the application.

## Creating Domain Model
NRules is designed to author rules in terms of a domain model, so we start by creating a simple one, which describes auto-insurance domain. Add the following domain model classes to the `Domain` project.

```c#
public class Driver(string name, int age, int yearsOfExperience)
{
    public string Name { get; } = name;
    public int Age { get; } = age;
    public int YearsOfExperience { get; } = yearsOfExperience;
}

public class InsuranceQuote(Driver driver, decimal basePremium)
{
    public Driver Driver { get; } = driver;
    public decimal BasePremium { get; } = basePremium;
    public decimal FinalPremium { get; private set; } = basePremium;

    public void ApplySurcharge(decimal amount) => FinalPremium += amount;
    public void ApplyDiscount(decimal amount) => FinalPremium -= amount;
}

public class TrafficViolation(Driver driver, DateTime date, string violationType)
{
    public Driver Driver { get; } = driver;
    public DateTime Date { get; } = date;
    public string ViolationType { get; } = violationType;
}
```

## Creating Rules
When using NRules fluent DSL, a rule is a class that inherits from [Rule](xref:NRules.Fluent.Dsl.Rule). A rule consists of a set of conditions (patterns that match facts in the rules engine's memory) and a set of actions executed by the engine should the rule fire.

Add the rule classes mentioned below to the `Rules` project.

Let's look at a couple of simple rules. We want to match an `InsuranceQuote`, and, depending on driver's age and years of experience, apply a discount or a surcharge.
Each pattern in the [When](xref:NRules.Fluent.Dsl.Rule.When) part of the rule is bound to a variable via an expression, and then can be used in the [Then](xref:NRules.Fluent.Dsl.Rule.Then) part of the rule.

```c#
public class YoungDriverSurchargeRule : Rule
{
    public override void Define()
    {
        InsuranceQuote quote = default!;

        When()
            .Match(() => quote, q => q.Driver.Age < 25);

        Then()
            .Do(ctx => quote.ApplySurcharge(100));
    }
}

public class ExperiencedDriverDiscountRule : Rule
{
    public override void Define()
    {
        InsuranceQuote quote = default!;

        When()
            .Match(() => quote, q => q.Driver.YearsOfExperience >= 5);

        Then()
            .Do(ctx => quote.ApplyDiscount(50));
    }
}
```

A more complicated rule below matches multiple different facts. Note that if there is more than one pattern in the rule, the patterns must be joined to avoid a Cartesian Product between the matching facts. In this example, the `TrafficViolation` facts are joined with the driver from the `InsuranceQuote` fact, so that the rule only considers vilations pertaining to the matched quote.

This rule adds a surcharge for any recent traffic violation, except those related to parking, provided there is more than one such violation. This rule also demonstrates a mechanism for calculating intermediate values, which can later be used in the downstream patterns and in the rule's actions.

```c#
public class TrafficViolationSurchargeRule : Rule
{
    public override void Define()
    {
        InsuranceQuote quote = default!;
        IEnumerable<TrafficViolation> violations = default!;
        int totalViolations = 0;

        When()
            .Match(() => quote)
            .Query(() => violations, q => q
                .Match<TrafficViolation>()
                .Where(v => v.Driver == quote.Driver,
                    v => v.ViolationType != "Parking",
                    v => v.Date >= DateTime.Now.AddYears(-2))
                .Collect())
            .Let(() => totalViolations, () => violations.Count())
            .Having(() => totalViolations > 1);
        
        Then()
            .Do(ctx => quote.ApplySurcharge(20 * totalViolations));
    }
}
```

## Running Rules
NRules is an inference engine. It means there is no predefined order in which rules are executed, and it runs a match/resolve/act cycle to figure it out. It first matches facts (instances of domain entities) with the rules and determines which rules can fire. The rules that matched facts are said to be activated. It then resolves the conflict by choosing a single rule that will actually fire. And, finally, it fires the chosen rule by executing its actions. The cycle is repeated until there are no more rules to fire.

We need to do several things for the engine to enter the match/resolve/act cycle.

First, we need to load the rules and translate them into an intermediate rules model. We do this by creating a [RuleRepository](xref:NRules.Fluent.RuleRepository) and letting it scan an assembly to find and instantitate the rule classes.

Then we need to compile the rules into an execution model (Rete network), so that the engine can efficiently match facts. This is achieved by compiling the rules into an [ISessionFactory](xref:NRules.ISessionFactory) - this should only be done once per the application lifetime.

Next, we need to create a working session with the engine ([ISession](xref:NRules.ISession)) and insert facts into it - this can be done multipel times, e.g. per request or per workflow.
Finally we tell the engine to start the match/resolve/act cycle and fire matching rules.

```c#
//Load rules
var repository = new RuleRepository();
repository.Load(x => x.From(typeof(YoungDriverSurchargeRule).Assembly));

//Compile rules
var factory = repository.Compile();

//Create rules session
var session = factory.CreateSession();

//Setup an event handler that prints the name of the fired rule
session.Events.RuleFiredEvent += (_, args) 
    => Console.WriteLine($"Fired rule: {args.Rule.Name}");

//Load domain model
var driver = new Driver(name: "John Doe", age: 24, yearsOfExperience: 1);
var quote = new InsuranceQuote(driver, basePremium: 1000);
TrafficViolation[] violations =
[
    new(driver, DateTime.Now.AddMonths(-1), "Speeding"),
    new(driver, DateTime.Now.AddMonths(-2), "Parking"),
    new(driver, DateTime.Now.AddYears(-1), "Red Light"),
    new(driver, DateTime.Now.AddYears(-3), "Speeding")
];

//Insert facts into rules engine's memory
session.Insert(quote);
session.InsertAll(violations);

//Start match/resolve/act cycle
session.Fire();

//Print results
Console.WriteLine($"Base premium for {driver.Name}: {quote.BasePremium}");
Console.WriteLine($"Final premium for {driver.Name}: {quote.FinalPremium}");
```

This prints
```console
Fired rule: Rules.YoungDriverSurchargeRule
Fired rule: Rules.TrafficViolationSurchargeRule
Base premium for John Doe: 1000
Final premium for John Doe: 1140
```

## Testing Rules
To unit test rules we can use `NRules.Testing` package. It provides the mechanism for bootstrapping a test instance of the rules engine, loading specified rules into it, and asserting rules firing with the provided facts. See [Unit Testing Rules](unit-testing-rules.md) for more details.

In the terminal, run the following commands to add the unit tests project to the solution.

# [Windows](#tab/windows)
```console
dotnet new classlib -o Rules.Tests
dotnet sln GettingStarted.sln add Rules.Tests\Rules.Tests.csproj
dotnet add Rules.Tests\Rules.Tests.csproj reference Rules\Rules.csproj
dotnet add Rules.Tests\Rules.Tests.csproj package Microsoft.NET.Test.Sdk
dotnet add Rules.Tests\Rules.Tests.csproj package NRules.Testing
dotnet add Rules.Tests\Rules.Tests.csproj package xunit
dotnet add Rules.Tests\Rules.Tests.csproj package xunit.runner.visualstudio
```
# [MacOS/Linux](#tab/nix)
```console
dotnet new classlib -o Rules.Tests
dotnet sln GettingStarted.sln add Rules.Tests/Rules.Tests.csproj
dotnet add Rules.Tests/Rules.Tests.csproj reference Rules/Rules.csproj
dotnet add Rules.Tests/Rules.Tests.csproj package Microsoft.NET.Test.Sdk
dotnet add Rules.Tests/Rules.Tests.csproj package NRules.Testing
dotnet add Rules.Tests/Rules.Tests.csproj package xunit
dotnet add Rules.Tests/Rules.Tests.csproj package xunit.runner.visualstudio
```
---

A test fixture for rules unit tests inherits from [RulesTestFixture](xref:NRules.Testing.RulesTestFixture), and uses the `Setup` object to add rules under test. Each test method in the fixture can follow the AAA pattern (Arrange, Act, Assert) of setting up facts under test, inserting them into session, calling `Fire` method, and asserting the rules firing expectations.

In the IDE, add the following test class to the `Rules.Tests` project.

```c#
public class YoungDriverSurchargeRuleTest : RulesTestFixture
{
    [Fact]
    public void Fire_QuoteWithDriverAt25_DoesNotFire()
    {
        // Arrange
        var driver = new Driver("John Do", 25, 6);
        var quote = new InsuranceQuote(driver, 1000);

        // Act
        Session.Insert(quote);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
    
    [Fact]
    public void Fire_QuoteWithDriverUnder25_Fires()
    {
        // Arrange
        var driver = new Driver("John Do", 24, 6);
        var quote = new InsuranceQuote(driver, 1000);

        // Act
        Session.Insert(quote);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact(quote)));
        Assert.Equal(1100, quote.FinalPremium);
    }
    
    public YoungDriverSurchargeRuleTest()
    {
        Setup.Rule<YoungDriverSurchargeRule>();
    }
}
```

Please read NRules documentation to learn more about various features and capabilities of the engine.