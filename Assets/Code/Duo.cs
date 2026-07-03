using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Spaceship
{
    public struct Duo : IEquatable<Duo>
    {
        public float Left;
        public float Right;

        public Duo(float left, float right)
        {
            Left = left;
            Right = right;
        }

        public void Deconstruct(out float left, out float right)
        {
            left = Left;
            right = Right;
        }

        public void Swap() => (Left, Right) = (Right, Left);

#region Conversion Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(Duo duo) => new(duo.Left, duo.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Duo(Vector2 vector) => new(vector.x, vector.y);
#endregion

#region Arithmetic Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator +(Duo lhs, Duo rhs) => new(lhs.Left + rhs.Left, lhs.Right + rhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator +(Duo lhs, float rhs) => new(lhs.Left + rhs, lhs.Right + rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator +(float lhs, Duo rhs) => new(lhs + rhs.Left, lhs + rhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator -(Duo lhs) => new(-lhs.Left, -lhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator -(Duo lhs, Duo rhs) => new(lhs.Left - rhs.Left, lhs.Right - rhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator -(Duo lhs, float rhs) => new(lhs.Left - rhs, lhs.Right - rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator -(float lhs, Duo rhs) => new(lhs - rhs.Left, lhs - rhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator *(Duo lhs, Duo rhs) => new(lhs.Left * rhs.Left, lhs.Right * rhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator *(Duo lhs, float rhs) => new(lhs.Left * rhs, lhs.Right * rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator *(float lhs, Duo rhs) => new(lhs * rhs.Left, lhs * rhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator /(Duo lhs, Duo rhs) => new(lhs.Left / rhs.Left, lhs.Right / rhs.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator /(Duo lhs, float rhs) => new(lhs.Left / rhs, lhs.Right / rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Duo operator /(float lhs, Duo rhs) => new(lhs / rhs.Left, lhs / rhs.Right);
#endregion

#region Equality Methods
        public override int GetHashCode() => HashCode.Combine(Left, Right);
        public override bool Equals(object obj) => obj is Duo other && Equals(other);

        public bool Equals(Duo other) => Left.Equals(other.Left) && Right.Equals(other.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Duo lhs, Duo rhs) => lhs.Equals(rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Duo left, Duo rhs) => !left.Equals(rhs);
#endregion
    }
}
