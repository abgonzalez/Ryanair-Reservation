namespace Ryanair.Reservation.Services
{
    public interface IReservationNumberService
    {
        string Value { get; }

        string IncreaseLetters();
        string Next();
    }
}