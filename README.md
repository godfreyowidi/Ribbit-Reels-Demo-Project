## RIBBIT REELS

Social Media based learning experience like nothing before 😂


### important commands
dotnet ef migrations add InitialCreate --project ../RibbitReels.Data --startup-project .

dotnet ef database update --project ../RibbitReels.Data --startup-project .

 dotnet test --logger "console;verbosity=detailed"

 dotnet test ./RibbitReels.IntegrationTests/RibbitReels.IntegrationTests.csproj

 sqlcmd -S localhost,1433 -U sa -P 'Password123!' -d RibbitReelsDb 