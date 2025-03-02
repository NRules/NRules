namespace Domain;

public class TrafficViolation(Driver driver, DateTime date, string violationType)
{
    public Driver Driver { get; } = driver;
    public DateTime Date { get; } = date;
    public string ViolationType { get; } = violationType;
}