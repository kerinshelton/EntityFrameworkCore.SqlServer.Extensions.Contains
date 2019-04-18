# EntityFrameworkCore.SqlServer.Extensions.Contains

## Installing via NuGet

```powershell
Install-Package EntityFrameworkCore.SqlServer.Extensions.Contains
```

## Example

Create a search model with 2 params

```csharp
public class CarSearch
{
   public int Id { get; set; }
   public string Model { get; set; }
}

var carSearchObjects = new List<CarSearch>();
```

Your join would resemble something like this, which cant be done as the join would be between an in memory object list and a sql server table:

```csharp
from c in context.Cars
join o in carSearchObjects 
     on new { Id = c.Id, Model = c.Model } equals new { Id = o.Id, Model = o.Model }
select m;
```

## Usage


```csharp
using EntityFrameworkCore.SqlServer.Extensions.Contains;

using (var context = new DbContext())
{
    var carSearch = context.Cars.Take(800).Select(x => new { x.Model, x.Id }).ToList();

    var filtered = context.Cars.Contains(carSearch, "Cars", false, c => c.Model,  c => c.Id );

    Console.WriteLine(filtered.Count());
}

```
