namespace Domain;

public class Driver(string name, int age, int yearsOfExperience)
{
    public string Name { get; } = name;
    public int Age { get; } = age;
    public int YearsOfExperience { get; } = yearsOfExperience;
}