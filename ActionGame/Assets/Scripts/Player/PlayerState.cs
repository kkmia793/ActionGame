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
    public event Action<float> OnDistanceUpdated; // 進行距離更新イベント

    private PlayerCharacter _player;
    private float _fallThreshold;
    private float _speedThreshold;
    private float _totalDistance;
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
        _isMonitoring = false; // モニタリングを無効化
        await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds));
        _isMonitoring = true; // 遅延後に有効化
    }

    public void UpdateState()
    {
        if (!_isMonitoring) return;
        MonitorState();
        UpdateDistance();
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

    private void UpdateDistance()
    {
        _totalDistance += _player.CurrentSpeed.x * Time.deltaTime;
        OnDistanceUpdated?.Invoke(_totalDistance); // 進行距離の更新を通知
    }

    public void SetPlayerMoveSpeed(float newSpeed)
    {
        _player.SetMoveSpeed(newSpeed);
    }

    public void ResetState()
    {
        _currentState = State.Alive;
        _totalDistance = 0;
        _isMonitoring = false;
    }
}