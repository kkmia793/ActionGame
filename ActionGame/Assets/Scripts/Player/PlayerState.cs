using System;
using UnityEngine;
using Cysharp.Threading.Tasks; 

public class PlayerState
{
    public enum State
    {
        Alive,
        Dead
    }

    private State _currentState = State.Alive;
    public State CurrentState => _currentState;

    public event Action<State> OnStateChanged; // 状態変化イベント

    private PlayerCharacter _player;
    private float _fallThreshold;
    private float _speedThreshold;
    private float _totalDistance;
    private float _moveSpeed;
    private bool _isMonitoring; 

    public PlayerState(PlayerCharacter player, float fallThreshold, float speedThreshold)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _fallThreshold = fallThreshold;
        _speedThreshold = speedThreshold;
        _isMonitoring = false; 
    }

    public async UniTask StartMonitoringWithDelay(float delaySeconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds)); 
        _isMonitoring = true;
    }

    public void UpdateState()
    {
        if (!_isMonitoring) return; 
        MonitorState();
    }

    private void MonitorState()
    {
        if (_currentState == State.Dead) return;

        if (_player.CurrentSpeed.x < _speedThreshold || _player.transform.position.y < _fallThreshold)
        {
            ChangeState(State.Dead);
        }
    }

    private void ChangeState(State newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        OnStateChanged?.Invoke(newState);

        Debug.Log($"Player state changed to: {_currentState}");
    }

    public float CalculatePlayerTotalDistance()
    {
        _totalDistance += _player.CurrentSpeed.x * Time.deltaTime;
        return _totalDistance;
    }

    public float SetPlayerMoveSpeed(float newSpeed)
    {
        _moveSpeed = newSpeed;
        
        return _moveSpeed;
    }
    
}