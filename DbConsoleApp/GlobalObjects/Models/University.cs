﻿using DbFramework.Attributes;
using System;

namespace DbConsoleApp.GlobalObjects.Models
{
    [TableName("University")]
    public class University
    {
        [Ignore]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string Address { get; set; }

        [Date]
        public DateTime? DestroyDate { get; set; }

        public override string ToString()
        {
            return $"Id = {Id}, Name = {Name}, Phone Number = {PhoneNumber}, Address {Address}, Closed Date {DestroyDate}";
        }
    }
}