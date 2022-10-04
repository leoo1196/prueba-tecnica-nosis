using System.Diagnostics.CodeAnalysis;
using Core.Errors;

namespace Core.Results;
/// <summary>
/// Clase util para retornar información sobre errores producidos en la ejecución de un método.
/// Los objetos derivados de <see cref="Errors.Error"/> pueden convertirse implicitamente a esta clase.
/// </summary>
public class Result
{
    /// <summary>
    /// Obtiene la instancia del error.
    /// </summary>
    public Error? Error { get; }

    protected Result(Error? error) => Error = error;

    /// <summary>
    /// Propiedad que indica si el resultado contiene un error.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public virtual bool HasError => Error is not null;

    /// <summary>
    /// Genera una instancia de <see cref="Result"/> que no contiene ningún error. Util para devolver al final
    /// de un método que completo su ejecución sin errores.
    /// </summary>
    /// <returns>Una instancia de <see cref="Result"/>.</returns>
    public static Result Empty() => new(null);

    /// <summary>
    /// Genera una instancia de <see cref="Result"/> que contiene un error.
    /// </summary>
    /// <param name="error">El error producido.</param>
    /// <returns>Una instancia de <see cref="Result"/>.</returns>
    public static Result FromError(Error? error) => new(error);

    /// <summary>
    /// Define la conversión implicita de un objeto derivado de <see cref="Errors.Error"/> a una instancia
    /// de <see cref="Result"/> conteniendo ese error.
    /// </summary>
    /// <param name="error">El error producido.</param>
    public static implicit operator Result(Error? error) => new(error);
}

/// <summary>
/// Clase util para retornar información sobre errores producidos en la ejecución de un método. Opcionalmente
/// permite retornar un modelo especificado como parametro generico. Tanto los objetos de tipo <typeparamref name="TModel"/>
/// como los objetos derivados de <see cref="Error"/> pueden convertirse implicitamente a esta clase.
/// </summary>
/// <typeparam name="TModel">Tipo del modelo.</typeparam>
public class Result<TModel> : Result
{
    /// <summary>
    /// Obtiene la instancia del modelo.
    /// </summary>
    public TModel? Model { get; }

    private Result(Error? error) : base(error) => Model = default;

    private Result(TModel model) : base(null) => Model = model;

    [MemberNotNullWhen(false, nameof(Model))]
    public override bool HasError => base.HasError;

    /// <summary>
    /// Genera una instancia de <see cref="Result{TModel}"/> que contiene un error.
    /// </summary>
    /// <param name="error">El error producido.</param>
    /// <returns>Una instancia de <see cref="Result{TModel}"/>.</returns>
    public static new Result<TModel> FromError(Error? error) => new(error);

    /// <summary>
    /// Genera una instancia de <see cref="Result{TModel}"/> que contiene un modelo.
    /// </summary>
    /// <param name="model">Instancia del modelo a retornar.</param>
    /// <returns>Una instancia de <see cref="Result{TModel}"/>.</returns>
    public static Result<TModel> FromModel(TModel model) => new(model);

    /// <summary>
    /// Define la conversión implicita de un objeto derivado de <see cref="Error"/> a una instancia
    /// de <see cref="Result{TModel}"/> conteniendo ese error.
    /// </summary>
    /// <param name="error">El error producido.</param>
    public static implicit operator Result<TModel>(Error? error) => new(error);

    /// <summary>
    /// Define la conversión implicita de un objeto de tipo <see cref="TModel"/> a una instancia
    /// de <see cref="Result{TModel}"/> conteniendo ese modelo.
    /// </summary>
    /// <param name="model">El modelo a retornar.</param>
    public static implicit operator Result<TModel>(TModel model) => new(model);

    /// <summary>
    /// Permite el encadenamiento de métodos que trasformen el resultado de la ejecución anterior. La ejecución
    /// se interrumpe cuando un método devuelve un error.
    /// </summary>
    /// <typeparam name="TTransform">Nuevo tipo de dato a retornar.</typeparam>
    /// <param name="next">Función transformadora.</param>
    /// <returns>Un nuevo <see cref="Result{TModel}"/> para continuar encadenando llamadas.</returns>
    public Result<TTransform> Then<TTransform>(Func<TModel, Result<TTransform>> next)
    {
        return HasError ? Error : next(Model);
    }

    /// <summary>
    /// Permite el encadenamiento de métodos asincronicos que trasformen el resultado de la ejecución anterior. La ejecución
    /// se interrumpe cuando un método devuelve un error.
    /// </summary>
    /// <typeparam name="TTransform">Nuevo tipo de dato a retornar.</typeparam>
    /// <param name="next">Función transformadora.</param>
    /// <returns>Una tarea que retorna un nuevo <see cref="Result{TModel}"/>.</returns>
    public async Task<Result<TTransform>> Then<TTransform>(Func<TModel, Task<Result<TTransform>>> next)
    {
        return HasError ? Error : await next(Model);
    }
}