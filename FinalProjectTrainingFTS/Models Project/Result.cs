namespace FinalProjectTrainingFTS.ModelsProject;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }
    public int Code { get; }

    private Result(T value , int code) => (IsSuccess, Value, Error , Code) = (true, value, null , code);
    private Result(string error , int code) => (IsSuccess, Value, Error , Code) = (false, default, error ,code);

    public static Result<T> Success(T value) => new Result<T>(value , 200);
    public static Result<T> Failure(string error ,int code) => new Result<T>(error , code);

   
}