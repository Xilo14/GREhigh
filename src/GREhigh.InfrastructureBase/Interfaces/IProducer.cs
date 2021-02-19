namespace GREhigh.InfrastructureBase.Interfaces {
    public interface IProducer<T> {
        bool TryProduce(T item);
    }
}
