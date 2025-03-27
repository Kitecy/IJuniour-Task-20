using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Raycaster _raycaster;
    [SerializeField] private FlagReplacer _flagReplacer;
    [SerializeField] private LayerMask _groundsMasks;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new();
    }

    private void OnEnable()
    {
        _playerInput.Enable();

        _playerInput.Player.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        _playerInput.Disable();

        _playerInput.Player.Click.performed -= OnClick;
    }

    private void OnClick(InputAction.CallbackContext obj)
    {
        if (_flagReplacer.IsBusy)
        {
            ReplaceFlag();
        }
        else
        {
            SelectBase();
        }
    }

    private void SelectBase()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (_raycaster.Raycast(ray, out Base @base) == false)
            return;

        _flagReplacer.SelectBase(@base);
    }

    private void ReplaceFlag()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (_raycaster.Raycast(ray, out Vector3 point, _groundsMasks) == false)
            return;

        _flagReplacer.ReplaceFlag(point);
    }
}
