using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ShopApp.Infrastructure.Data;

public class AdoDbContext
{
    private readonly NpgsqlConnection _connection;

    public AdoDbContext(IConfiguration configuration)
    {
        _connection = new NpgsqlConnection(configuration.GetConnectionString("PostgreSqlConnection"));
    }

    public async Task<List<T>> ExecuteQueryAsync<T>(string query, Func<NpgsqlDataReader, T> mapFunction)
    {
        var result = new List<T>();

        try
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();

            using (var command = new NpgsqlCommand(query, _connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        result.Add(mapFunction(reader)); // Map each row to an object of type T
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing query: {ex.Message}");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return result;
    }

    public async Task<T?> ExecuteSingleQueryAsync<T>(string query, Func<NpgsqlDataReader, T> mapFunction)
    {
        T? result = default;

        try
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();

            using (var command = new NpgsqlCommand(query, _connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync()) // Read only the first row
                        result = mapFunction(reader); // Map the row to an object of type T
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing query: {ex.Message}");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return result;
    }

    public async Task<T?> ExecuteSingleQueryAsync<T>(string query, Action<NpgsqlCommand> parameterize,
        Func<NpgsqlDataReader, T> mapFunction)
    {
        T? result = default;

        try
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, _connection);

            parameterize?.Invoke(command);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) result = mapFunction(reader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing query: {ex.Message}");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return result;
    }

    public async Task<int> ExecuteNonQueryAsync(string query, Action<NpgsqlCommand> parameterize = null)
    {
        var rowsAffected = 0;

        try
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();

            using (var command = new NpgsqlCommand(query, _connection))
            {
                parameterize?.Invoke(command);

                rowsAffected = await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing non-query: {ex.Message}");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return rowsAffected;
    }

    public async Task<int> ExecuteNonQueryReturningId(string query, Action<NpgsqlCommand> parameterize = null)
    {
        var id = 0;

        try
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();

            using (var command = new NpgsqlCommand(query, _connection))
            {
                parameterize?.Invoke(command);

                id = (int)await command.ExecuteScalarAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing non-query: {ex.Message}");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return id;
    }
}