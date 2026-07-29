# Date Adder

Adds/subtracts days from a dd/mm/yyyy date. No inbuilt or third-party date
library used.

Requires .NET 8 SDK or later.

```
dotnet build
dotnet run --project src/DateAdder -- 31/01/2016 1
New Date: 01/02/2016
```

Run with no args to get prompted instead. Negative days subtract. Supported
years: 0001-9999.

```
dotnet test
```
