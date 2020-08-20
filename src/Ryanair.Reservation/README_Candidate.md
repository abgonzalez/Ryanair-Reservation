## Ryanair Reservation Test - TravelLabs

- The API Ryanair.Reservation has been deployed on http://localhost:5005/, according to the launchSettings.json file.
- The solution is running on http://localhost:5005.
	- You can run from Visual Studio with IIS Express
	- Or you can deploy with Kerstel server. Go to .\Ryanair.Reservation folder and execute [dotnet run]. The API will be deployed on
	http://localhost:5005.
	
- You have a guide to know how to run the API on http://localhost:5005/swagger

- The database is in-memory and it's seed with the [InitialState.json] file

- There is two loggind systems:
	- Logging - Right now, the API has the tracking at maximum level - Information , and the log is on "C:\\logs\\Ryanair.Reservation-log-{Date}.txt",
	and it's tracking every action. 
	
- The API follow the below contracts in JSON/ XML format:
    * [GET /Flight](AppendixI.md)
    * [POST /Reservation](AppendixII.md - JSON Format) - I have created an example of creation of Reservtion in XML (AppendixII_CreationReservation.xml), to help with the test. This file is in the same folder as 
	Readme_Candidate.md
    * [GET /Reservation](AppendixIII.md): used to retrieve a reservation previously made.	

- The [appsettings.json] has several settings as :
    * "BagsLimitByFlight": 50,
    *  "BagsLimitByPassenger": 5,
    * "SeatsLimitByFlight": 50,
    * "TicketsLimitByFlight": 50

- It's using Automapper as Mapping framework

- There is a project with Test divide :
    * Database Test
    * Services Test
    * Controller Test
   
 If you need further questions you can check on http://localhost:5005/swagger or you can contact me
 
 Cheers