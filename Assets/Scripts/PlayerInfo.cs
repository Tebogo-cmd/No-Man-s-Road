using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public enum PowerUpType { Invisible, DoubleJump, Multiplier}
    public int Score { get; set; } = 0; 
    public float Lives { get; set; }  = 100;   //max 100%

    public HashSet<PowerUpType> PowerUps = new();

    public bool HasPowerUp(PowerUpType powerUp) => PowerUps.Contains(powerUp);

    public void SetPowerUp(PowerUpType powerUp) => PowerUps.Add(powerUp);




}
