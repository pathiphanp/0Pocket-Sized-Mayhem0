using UnityEngine;

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
        void CarStar();
    }
    public interface Invite
    {
        void InviteToCar(Car _car);
        void ChangeTargetToPortal();
    }

    public interface Dodge
    {
        void Dodge(GameObject _targetDodge);
    }
}

