namespace GREhigh.Infrastructure.Interfaces {
    public interface IInfrastructureFactory<TEntity>  {
        public TEntity GetInfrastructure();
    }
}
