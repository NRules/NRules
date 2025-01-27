using Domain;
using NRules;
using NRules.Fluent;
using Rules;

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