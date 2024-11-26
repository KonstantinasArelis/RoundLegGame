using System;

[Serializable]
public struct Directions
{
  public float front, right, back, left;

  public static Directions operator +(Directions a, Directions b) => new()
  {
    front = a.front + b.front,
    right = a.right + b.right,
    back = a.back + b.back,
    left = a.left + b.left
  };
}