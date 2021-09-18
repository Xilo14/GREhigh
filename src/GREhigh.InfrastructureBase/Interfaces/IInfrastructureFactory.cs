namespace GREhigh.Infrastructure.Interfaces {
    public interface IInfrastructureFactory<out TEntity> {
        public TEntity GetInfrastructure();
    }
}
