namespace NRules.Config
{
    public class Configure
    {
        public static Configure With()
        {
            return new Configure();
        }

        public Configuration Container(IContainer container)
        {
            container.Register<IContainer>(container);
            return new Configuration(container);
        }
    }
}