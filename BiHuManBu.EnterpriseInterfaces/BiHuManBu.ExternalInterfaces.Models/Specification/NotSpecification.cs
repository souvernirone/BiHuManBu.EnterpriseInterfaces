namespace BiHuManBu.ExternalInterfaces.Models.Specification
{
    public class NotSpecification<T>:CompositeSpecification<T>
    {
        private ISpecification<T> _innerSpecification;

        public NotSpecification(ISpecification<T> innerSpecification)
        {
            _innerSpecification = innerSpecification;
        }  
        public override bool IsSatisfiedBy(T candidate)
        {
            return !_innerSpecification.IsSatisfiedBy(candidate);
        }
    }
}
