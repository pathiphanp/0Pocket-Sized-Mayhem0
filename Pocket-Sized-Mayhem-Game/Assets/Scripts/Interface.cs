namespace Interface
{
    public interface TakeDamage
    {
        TargetType TakeDamage();
    }
    public interface Fear
    {
        void AddFear();
    }
    public interface AddInCar<T>
    {
        void AddInCar(T _waitPosition);
        void invisible();
    }
}

