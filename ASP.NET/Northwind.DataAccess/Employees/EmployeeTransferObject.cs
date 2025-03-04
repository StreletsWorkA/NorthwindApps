﻿using Northwind.Services.Employees;
using System;

namespace Northwind.DataAccess.Employees
{
    /// <summary>
    /// Represents a TO for Northwind employees.
    /// </summary>
    public class EmployeeTransferObject
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Title { get; set; }

        public string TitleOfCourtesy { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? HireDate { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string HomePhone { get; set; }

        public string Extension { get; set; }

        public byte[] Photo { get; set; }

        public string Notes { get; set; }

        public int? ReportsTo { get; set; }

        public string PhotoPath { get; set; }

        public static explicit operator EmployeeTransferObject(Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            EmployeeTransferObject employeeTransferObject = new ()
            {
                Id = employee.EmployeeId,
                LastName = employee.LastName,
                FirstName = employee.FirstName,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy,
                BirthDate = employee.BirthDate,
                HireDate = employee.HireDate,
                Address = employee.Address,
                City = employee.City,
                Region = employee.Region,
                PostalCode = employee.PostalCode,
                Country = employee.Country,
                HomePhone = employee.HomePhone,
                Extension = employee.Extension,
                Photo = employee.Photo,
                Notes = employee.Notes,
                ReportsTo = employee.ReportsTo,
                PhotoPath = employee.PhotoPath,
            };

            return employeeTransferObject;
        }

        public static explicit operator Employee(EmployeeTransferObject employeeTransferObject)
        {
            if (employeeTransferObject is null)
            {
                throw new ArgumentNullException(nameof(employeeTransferObject));
            }

            Employee employee = new ()
            {
                EmployeeId = employeeTransferObject.Id,
                LastName = employeeTransferObject.LastName,
                FirstName = employeeTransferObject.FirstName,
                Title = employeeTransferObject.Title,
                TitleOfCourtesy = employeeTransferObject.TitleOfCourtesy,
                BirthDate = (DateTime)employeeTransferObject.BirthDate,
                HireDate = (DateTime)employeeTransferObject.HireDate,
                Address = employeeTransferObject.Address,
                City = employeeTransferObject.City,
                Region = employeeTransferObject.Region,
                PostalCode = employeeTransferObject.PostalCode,
                Country = employeeTransferObject.Country,
                HomePhone = employeeTransferObject.HomePhone,
                Extension = employeeTransferObject.Extension,
                Photo = employeeTransferObject.Photo,
                Notes = employeeTransferObject.Notes,
                ReportsTo = employeeTransferObject.ReportsTo,
                PhotoPath = employeeTransferObject.PhotoPath,
            };

            return employee;
        }
    }
}
