using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(50)]
public class InputManager : Singleton<InputManager>
{
    private Action<Vector2> _MovementAction;
    void Start()
    {
        InputMaster inputMaster = new InputMaster();
        inputMaster.Player.Enable();
        inputMaster.Player.Movement.performed += MovementOnperformed;
    }

    public void MovementOnperformed(InputAction.CallbackContext _context)
    {
      //  Debug.Log(_context);
        _MovementAction?.Invoke(_context.ReadValue<Vector2>());
    }

    public IObservable<Vector2> OnInputMovement()
    {
        return Observable.FromEvent<Vector2>(_e => _MovementAction += _e,_e => _MovementAction -= _e);
    }
}
