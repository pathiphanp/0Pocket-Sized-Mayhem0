using UnityEngine;

namespace Interface
{
    public interface TakeDamage
    {
        bool TakeDamage();
        TargetType ThisType();
    }
    public interface Fear
    {
        void AddFear();
    }
    public interface AddInCar<T>
    {
        void AddInCar(T _waitPosition);
        void DoActionOnCarStar();
    }
    public interface OutCar
    {
        void OutCar(Vector3 _diraction);
    }
    public interface Invite
    {
        void InviteToCar(Car _car);
        void ChangeTargetToPortal();
        Car myCarTarget();
    }
    public interface Dodge
    {
        void Dodge(GameObject _targetDodge);
        void RemoveDodge();

    }

    public interface SetObjectPool<T>
    {
        void AddPool(T _objectPool);
        void ReturnToPool();
        void ResetStatus();
        void SetUpStartGame();
    }

    public interface SetGuardEffectProtect
    {
        void AddGuardEffect();
        void RemoveGuardEffect();
    }
    public interface GuardProtectAction
    {
        void GuardProtect(Vector3 _attackArea);
    }
}

