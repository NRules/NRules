namespace NRules.Fluent.Dsl
{
    public static class Context
    {
        public static void Insert(object fact)
        {
            //This is a marker method to use in expressions; it does not do anything
        }

        public static void Update(object fact)
        {
            //This is a marker method to use in expressions; it does not do anything
        }

        public static void Retract(object fact)
        {
            //This is a marker method to use in expressions; it does not do anything
        }
    }
}