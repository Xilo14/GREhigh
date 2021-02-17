namespace GREhigh.Infrastructure.Interfaces {
    public interface IInfrastructureFactory<TEntity> where TEntity : class {
        public TEntity GetInfrastructure();
    }
}
