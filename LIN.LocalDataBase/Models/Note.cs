﻿namespace LIN.LocalDataBase.Models;

public class Note
{
    public int Id { get; set; } = 0;
    public string Tittle { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Color { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsDeleted { get; set; }
}