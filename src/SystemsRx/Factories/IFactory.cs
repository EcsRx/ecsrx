namespace SystemsRx.Factories
{
    public interface IFactory
    {
    }

    public interface IFactory<out Tout> : IFactory
    {
        Tout Create();
    }

    public interface IFactory<in Tin, out Tout> : IFactory
    {
        Tout Create(Tin arg);
    }
}