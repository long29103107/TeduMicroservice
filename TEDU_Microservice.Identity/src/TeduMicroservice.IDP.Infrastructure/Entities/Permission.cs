﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeduMicroservice.IDP.Infrastructure.Common.Domains;

namespace TeduMicroservice.IDP.Infrastructure.Entities;

public class Permission : EntityBase<int>
{
    public Permission()
    {
            
    }
    public Permission(string function, string command, string roleId)
    {
        Function = function;
        Command = command;
        RoleId = roleId;
    }
    [Key]
    [MaxLength(50)]
    [Column(TypeName ="varchar(50)")]
    public string Function { get; set; }

    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Command { get; set; }

    [ForeignKey("RoleId")]
    public string RoleId { get; set; }
    public virtual IdentityRole Role { get; set; }  
}
