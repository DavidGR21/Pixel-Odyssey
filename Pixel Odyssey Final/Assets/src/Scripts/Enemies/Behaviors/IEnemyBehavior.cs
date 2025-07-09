
///<summary>
/// Esta interfaz define el comportamiento de un enemigo en el juego.
/// Un enemigo puede tener diferentes comportamientos como
/// atacar, perseguir, patrullar, etc. Cada comportamiento
///  se implementa en una clase que hereda de esta interfaz.
/// </summary>
public interface IEnemyBehavior
{
    void Execute(Enemy enemy);
}
