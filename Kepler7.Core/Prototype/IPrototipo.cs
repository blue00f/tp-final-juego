namespace Kepler7.Core.Prototype;

public interface IPrototipo<out T>
{
    T Clonar();
}
