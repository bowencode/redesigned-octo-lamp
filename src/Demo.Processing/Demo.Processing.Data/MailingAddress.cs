﻿using System.ComponentModel.DataAnnotations;

namespace Demo.Processing.Data;

public class MailingAddress
{
    public int Id { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    [StringLength(2)]
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}