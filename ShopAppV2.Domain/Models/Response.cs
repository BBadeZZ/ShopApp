namespace ShopApp.Domain.Models;

public class Response
{
    public Response(string status, string message)
    {
        Status = status;
        Message = message;
    }

    public string Status { get; set; }
    public string Message { get; set; }

    public static Response Success(string message)
    {
        return new Response("success", message);
    }

    public static Response Fail(string message)
    {
        return new Response("fail", message);
    }
}

public class Response<T> : Response
{
    public Response(string status, string message, T? data = default)
        : base(status, message)
    {
        Data = data;
    }

    public T? Data { get; set; }

    public new static Response<T> Success(string message, T data)
    {
        return new Response<T>("success", message, data);
    }

    public new static Response<T> Success(string message)
    {
        return new Response<T>("success", message);
    }

    public new static Response<T> Fail(string message)
    {
        return new Response<T>("fail", message);
    }
}