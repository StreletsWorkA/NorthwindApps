﻿// <copyright file="EmployeeSqlServerDataAccessObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ReSharper disable CheckNamespace

namespace Northwind.DataAccess.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class EmployeeSqlServerDataAccessObject : IEmployeeDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public EmployeeSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            const string commandText = @"DELETE FROM dbo.Employees WHERE EmployeeID = @employeeID SELECT @@ROWCOUNT";

            await using var command = new SqlCommand(commandText, this.connection);
            const string employeeBaseId = "@employeeID";
            command.Parameters.Add(employeeBaseId, SqlDbType.Int);
            command.Parameters[employeeBaseId].Value = employeeId;
            var result = await command.ExecuteScalarAsync().ConfigureAwait(true);
            return ((int)result) > 0;
        }

        /// <inheritdoc/>
        public EmployeeTransferObject FindEmployee(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            const string commandText =
                @"SELECT e.EmployeeId, e.LastName, e.FirstName, e.Title, e.TitleOfCourtesy, e.BirthDate, e.HireDate, e.Address, e.City, e.Region, e.PostalCode, e.Country, e.HomePhone, e.Extension, e.Photo, e.Notes, e.ReportsTo, e.PhotoPath FROM dbo.Employees as e
                  WHERE e.EmployeeID = @employeeId";

            using var command = new SqlCommand(commandText, this.connection);
            const string employeeBaseId = "@employeeId";
            command.Parameters.Add(employeeBaseId, SqlDbType.Int);
            command.Parameters[employeeBaseId].Value = employeeId;

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new ArgumentException($"Employee with id {employeeId} not found.", nameof(employeeId));
            }

            return CreateEmployee(reader);
        }

        /// <inheritdoc/>
        public async Task<int> InsertEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            const string commandText =
                @"INSERT INTO dbo.Employees (LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo, PhotoPath) OUTPUT Inserted.EmployeeID
                VALUES (@lastName, @firstName, @title, @titleOfCourtesy, @birthDate, @hireDate, @address, @city, @region, @postalCode, @country, @homePhone, @extension, @photo, @notes, @reportsTo, @photoPath)";

            await using var command = new SqlCommand(commandText, this.connection);
            AddSqlParameters(employee, command);
            var id = await command.ExecuteScalarAsync().ConfigureAwait(true);
            return (int)id;
        }

        /// <inheritdoc/>
        public async Task<IList<EmployeeTransferObject>> SelectEmployeesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            const string commandTemplate =
                @"SELECT e.EmployeeID, e.LastName, e.FirstName, e.Title, e.TitleOfCourtesy, e.BirthDate, e.HireDate, e.Address, e.City, e.Region, e.PostalCode, e.Country, e.HomePhone, e.Extension, e.Photo, e.Notes, e.ReportsTo, e.PhotoPath FROM dbo.Employees as e
                  ORDER BY e.EmployeeID
                  OFFSET {0} ROWS
                  FETCH FIRST {1} ROWS ONLY";

            string commandText = string.Format(CultureInfo.CurrentCulture, commandTemplate, offset, limit);
            return await this.ExecuteReaderAsync(commandText).ConfigureAwait(true);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            const string commandText =
                @"UPDATE dbo.Employees SET LastName = @lastName, FirstName = @firstName, Title = @title, TitleOfCourtesy = @titleOfCourtesy, BirthDate = @birthDate, HireDate = @hireDate, Address = @address, City = @city, Region = @region, PostalCode = @postalCode, Country = @country, HomePhone = @homePhone, Extension = @extension, Photo = @photo, Notes = @notes, ReportsTo = @reportsTo, PhotoPath = @photoPath
                  WHERE EmployeeID = @employeeId
                  SELECT @@ROWCOUNT";

            await using var command = new SqlCommand(commandText, this.connection);
            AddSqlParameters(employee, command);
            const string employeeId = "@employeeId";
            command.Parameters.Add(employeeId, SqlDbType.Int);
            command.Parameters[employeeId].Value = employee.Id;
            var result = await command.ExecuteScalarAsync().ConfigureAwait(true);
            return ((int)result) > 0;
        }

        private static EmployeeTransferObject CreateEmployee(SqlDataReader reader)
        {
            var id = (int)reader["EmployeeID"];
            var lastName = (string)reader["LastName"];
            var firstName = (string)reader["FirstName"];
            var title = (string)reader["Title"];
            var titleOfCourtesy = (string)reader["TitleOfCourtesy"];
            var birthDate = (DateTime)reader["BirthDate"];
            var hireDate = (DateTime)reader["HireDate"];
            var address = (string)reader["Address"];
            var city = (string)reader["City"];
            var region = (string)reader["Region"];
            var postalCode = (string)reader["PostalCode"];
            var country = (string)reader["Country"];
            var homePhone = (string)reader["HomePhone"];
            var extension = (string)reader["Extension"];

            const string PhotoColumnName = "Photo";
            byte[] photo = null;
            if (reader[PhotoColumnName] != DBNull.Value)
            {
                photo = (byte[])reader[PhotoColumnName];
            }

            var notes = (string)reader["Notes"];
            var reportsTo = (int)reader["ReportsTo"];
            var photoPath = (string)reader["PhotoPath"];

            return new EmployeeTransferObject
            {
                Id = id,
                LastName = lastName,
                FirstName = firstName,
                Title = title,
                TitleOfCourtesy = titleOfCourtesy,
                BirthDate = birthDate,
                HireDate = hireDate,
                Address = address,
                City = city,
                Region = region,
                PostalCode = postalCode,
                Country = country,
                HomePhone = homePhone,
                Extension = extension,
                Photo = photo,
                Notes = notes,
                ReportsTo = reportsTo,
                PhotoPath = photoPath,
            };
        }

        private static void AddSqlParameters(EmployeeTransferObject employee, SqlCommand command)
        {
            const string employeeLastNameParameter = "@lastName";
            command.Parameters.Add(employeeLastNameParameter, SqlDbType.NVarChar, 20);
            command.Parameters[employeeLastNameParameter].IsNullable = true;

            if (employee.LastName != null)
            {
                command.Parameters[employeeLastNameParameter].Value = employee.LastName;
            }
            else
            {
                command.Parameters[employeeLastNameParameter].Value = DBNull.Value;
            }

            const string employeeFirstNameParameter = "@firstName";
            command.Parameters.Add(employeeFirstNameParameter, SqlDbType.NVarChar, 10);
            command.Parameters[employeeFirstNameParameter].IsNullable = true;

            if (employee.FirstName != null)
            {
                command.Parameters[employeeFirstNameParameter].Value = employee.FirstName;
            }
            else
            {
                command.Parameters[employeeFirstNameParameter].Value = DBNull.Value;
            }

            const string employeeTitleParameter = "@title";
            command.Parameters.Add(employeeTitleParameter, SqlDbType.NVarChar, 30);
            command.Parameters[employeeTitleParameter].IsNullable = true;

            if (employee.Title != null)
            {
                command.Parameters[employeeTitleParameter].Value = employee.Title;
            }
            else
            {
                command.Parameters[employeeTitleParameter].Value = DBNull.Value;
            }

            const string employeeTitleOfCourtesyParameter = "@titleOfCourtesy";
            command.Parameters.Add(employeeTitleOfCourtesyParameter, SqlDbType.NVarChar, 25);
            command.Parameters[employeeTitleOfCourtesyParameter].IsNullable = true;

            if (employee.TitleOfCourtesy != null)
            {
                command.Parameters[employeeTitleOfCourtesyParameter].Value = employee.TitleOfCourtesy;
            }
            else
            {
                command.Parameters[employeeTitleOfCourtesyParameter].Value = DBNull.Value;
            }

            const string employeeBirthDateParameter = "@birthDate";
            command.Parameters.Add(employeeBirthDateParameter, SqlDbType.Date);
            command.Parameters[employeeBirthDateParameter].Value = employee.BirthDate;

            const string employeeHireDateParameter = "@hireDate";
            command.Parameters.Add(employeeHireDateParameter, SqlDbType.Date);
            command.Parameters[employeeHireDateParameter].Value = employee.HireDate;

            const string employeeAddressParameter = "@address";
            command.Parameters.Add(employeeAddressParameter, SqlDbType.NVarChar, 60);
            command.Parameters[employeeAddressParameter].Value = employee.Address;

            const string employeeCityParameter = "@city";
            command.Parameters.Add(employeeCityParameter, SqlDbType.NVarChar, 15);
            command.Parameters[employeeCityParameter].Value = employee.City;

            const string employeeRegionParameter = "@region";
            command.Parameters.Add(employeeRegionParameter, SqlDbType.NVarChar, 15);
            command.Parameters[employeeRegionParameter].Value = employee.Region;

            const string employeePostalCodeParameter = "@postalCode";
            command.Parameters.Add(employeePostalCodeParameter, SqlDbType.NVarChar, 10);
            command.Parameters[employeePostalCodeParameter].Value = employee.PostalCode;

            const string employeeCountryParameter = "@country";
            command.Parameters.Add(employeeCountryParameter, SqlDbType.NVarChar, 15);
            command.Parameters[employeeCountryParameter].Value = employee.Country;

            const string employeeHomePhoneParameter = "@homePhone";
            command.Parameters.Add(employeeHomePhoneParameter, SqlDbType.NVarChar, 24);
            command.Parameters[employeeHomePhoneParameter].Value = employee.HomePhone;

            const string employeeExtensionParameter = "@extension";
            command.Parameters.Add(employeeExtensionParameter, SqlDbType.NVarChar, 4);
            command.Parameters[employeeExtensionParameter].Value = employee.Extension;

            const string employeePhotoParameter = "@photo";
            command.Parameters.Add(employeePhotoParameter, SqlDbType.Image);
            command.Parameters[employeePhotoParameter].IsNullable = true;

            if (employee.Photo != null)
            {
                command.Parameters[employeePhotoParameter].Value = employee.Photo;
            }
            else
            {
                command.Parameters[employeePhotoParameter].Value = DBNull.Value;
            }

            const string employeeNotesParameter = "@notes";
            command.Parameters.Add(employeeNotesParameter, SqlDbType.Text);
            command.Parameters[employeeNotesParameter].Value = employee.Notes;

            const string employeeReportsToParameter = "@reportsTo";
            command.Parameters.Add(employeeReportsToParameter, SqlDbType.Int);
            command.Parameters[employeeReportsToParameter].Value = employee.ReportsTo;

            const string employeePhotoPathParameter = "@photoPath";
            command.Parameters.Add(employeePhotoPathParameter, SqlDbType.NVarChar, 255);
            command.Parameters[employeePhotoPathParameter].Value = employee.PhotoPath;
        }

        private async Task<IList<EmployeeTransferObject>> ExecuteReaderAsync(string commandText)
        {
            var productCategories = new List<EmployeeTransferObject>();

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            await using var command = new SqlCommand(commandText, this.connection);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(true);
            while (reader.Read())
            {
                productCategories.Add(CreateEmployee(reader));
            }

            return productCategories;
        }
    }
}
