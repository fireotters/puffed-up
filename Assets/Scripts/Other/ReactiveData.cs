using UnityEngine;
using System;

[Serializable]
public class ReactiveData<T>
{
    [SerializeField] T _data;
    public event Action<T> OnValueChanged;

    public ReactiveData(T data) => _data = data;

    public static implicit operator T(ReactiveData<T> exp) => exp.Value;

    public T Value
    {
        get { return _data; }
        set
        {
            if (!value.Equals(_data))
            {
                _data = value;
                OnValueChanged?.Invoke(_data);
            }
        }
    }
}
