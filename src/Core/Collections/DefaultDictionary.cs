#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections.Generic;

namespace Reko.Core.Collections;

/// <summary>
/// Helper class to create instances of <see cref="DefaultDictionary{TKey, TValue}"/>.
/// </summary>
public static class DefaultDictionary
{
    /// <summary>
    /// Creates an instance of the <see cref="DefaultDictionary{TKey, TValue}"/> class
    /// with the given <paramref name="valueFactory"/> to create default values.
    /// </summary>
    /// <param name="valueFactory">Factory method to create new instances.</param>
    /// <returns>A new dictionary instance.</returns>
    public static DefaultDictionary<TKey, TValue> Create<TKey, TValue>(Func<TValue> valueFactory)
        where TKey : notnull
    {
        return new DefaultDictionary<TKey, TValue>(valueFactory);
    }

    /// <summary>
    /// Creates an instance of the <see cref="DefaultDictionary{TKey, TValue}"/> class,
    /// automatically using the default constructor of <typeparamref name="TValue"/>,
    /// </summary>
    /// <returns>A new dictionary instance.</returns>
    public static DefaultDictionary<TKey, TValue> Create<TKey, TValue>()
        where TKey : notnull
        where TValue : new()
    {
        return new DefaultDictionary<TKey, TValue>(() => new());
    }

}

/// <summary>
/// A dictionary that creates default values for missing keys using a factory function.
/// </summary>
/// <typeparam name="TKey">Key type.</typeparam>
/// <typeparam name="TValue">Value type.</typeparam>
public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Func<TValue> valueFactory;

    /// <summary>
    /// Creates an instance of the <see cref="DefaultDictionary{TKey, TValue}"/> class
    /// using the specified <paramref name="valueFactory"/> to create default values.
    /// </summary>
    /// <param name="valueFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultDictionary(Func<TValue> valueFactory)
    {
        this.valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
    }

    /// <summary>
    /// Retrieves the value associated with the specified key. If the key does not exist,
    /// uses the value factory to create a default value, adds it to the dictionary, and returns it.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public new TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(key, out var value))
            {
                value = valueFactory();
                Add(key, value);
            }
            return value;
        }
        set
        {
            base[key] = value;
        }
    }
}
