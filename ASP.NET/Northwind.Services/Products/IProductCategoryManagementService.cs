﻿// <copyright file="IProductCategoryManagementService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.Products
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a management service for product categories.
    /// </summary>
    public interface IProductCategoryManagementService
    {
        /// <summary>
        /// Shows a list of product categories using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="Category"/>.</returns>
        Task<IList<Category>> ShowCategoriesAsync(int offset, int limit);

        /// <summary>
        /// Try to show a product category with specified identifier.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="productCategory">A product category to return.</param>
        /// <returns>Returns true if a product category is returned; otherwise false.</returns>
        bool TryShowCategory(int categoryId, out Category productCategory);

        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="productCategory">A <see cref="Category"/> to create.</param>
        /// <returns>An identifier of a created product category.</returns>
        Task<int> CreateCategoryAsync(Category productCategory);

        /// <summary>
        /// Destroys an existed product category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>True if a product category is destroyed; otherwise false.</returns>
        Task<bool> DestroyCategoryAsync(int categoryId);

        /// <summary>
        /// Looks up for product categories with specified names.
        /// </summary>
        /// <param name="names">A list of product category names.</param>
        /// <returns>A list of product categories with specified names.</returns>
        Task<IList<Category>> LookupCategoriesByNameAsync(IList<string> names);

        /// <summary>
        /// Updates a product category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="productCategory">A <see cref="Category"/>.</param>
        /// <returns>True if a product category is updated; otherwise false.</returns>
        Task<bool> UpdateCategoriesAsync(int categoryId, Category productCategory);
    }
}
