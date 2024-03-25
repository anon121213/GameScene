using UnityEngine;

public interface IControleble
{
    void Move(Vector3 _direction);
    void Run(bool _run);
    void Jump();   
}
