using Xunit;


namespace Ryanair.Reservation.Services
{
    public class ReservationNumberService_Test
    {

        [Fact]
        public void ShouldMoveUpOneLetter_Test()
        {
            ////Arrange
            var reservationService = new ReservationNumberService();

            ///Act
            for (int i = 0; i < 999; ++i)
                reservationService.Next();
            var result = reservationService.Next();

            ///Assert
            Assert.Equal("AAB001", result);

        }
        [Fact]
        public void ShouldMoveUpRandomLetter_Test()
        {
            ////Arrange
            var reservationService = new ReservationNumberService();

            ///Act
            for (int i = 0; i < 25974; ++i)
                reservationService.Next();
            var result = reservationService.Next();

            ///Assert
            Assert.Equal("ABZ001", result);

        }
    }
}
