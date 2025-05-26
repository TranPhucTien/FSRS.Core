# FSRS.Core

A .NET implementation of the Free Spaced Repetition Scheduler (FSRS) algorithm with full dependency injection support.

This is C# version of [py-fsrs](https://github.com/open-spaced-repetition/py-fsrs)

## What is FSRS?

FSRS (Free Spaced Repetition Scheduler) is a modern, open-source spaced repetition algorithm that helps optimize memory retention. It's designed to be more accurate and efficient than traditional algorithms like SM-2.

## Features

- ✅ Complete FSRS algorithm implementation
- ✅ Dependency injection support
- ✅ Configurable parameters and settings
- ✅ Comprehensive unit tests
- ✅ Easy integration with existing applications
- ✅ Target frameworks (.NET 8)

## Installation

```bash
dotnet add package FSRS.Core
```

## Quick Start

### Basic Usage

```csharp
using FSRS.Core.Models;
FSRS
using FSRS.Core.Services;

// Create a scheduler with default settings
var scheduler = new Scheduler();

// Create a new card
var card = new Card();

// Review the card
var (updatedCard, reviewLog) = scheduler.ReviewCard(card, Rating.Good);

Console.WriteLine($"Next review: {updatedCard.Due}");
Console.WriteLine($"Stability: {updatedCard.Stability}");
```

### With Dependency Injection (ASP.NET Core)

```csharp
using FSRS.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add FSRS services
builder.Services.AddFsrs(options =>
{
    options.DesiredRetention = 0.9;
    options.MaximumInterval = 365;
});

var app = builder.Build();
```

```csharp
[ApiController]
public class CardsController : ControllerBase
{
    private readonly IScheduler _scheduler;

    public CardsController(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    [HttpPost("review")]
    public ActionResult ReviewCard([FromBody] ReviewRequest request)
    {
        var card = new Card(request.CardId);
        var (updatedCard, reviewLog) = _scheduler.ReviewCard(card, request.Rating);
        
        return Ok(updatedCard.ToDictionary());
    }
}
```

## Configuration Options

```csharp
builder.Services.AddFsrs(options =>
{
    options.Parameters = customParameters; // Custom FSRS parameters
    options.DesiredRetention = 0.85;      // Target retention rate
    options.MaximumInterval = 36500;      // Maximum interval in days
    options.EnableFuzzing = true;         // Enable interval fuzzing
    options.LearningSteps = [             // Learning steps
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(10)
    ];
    options.RelearningSteps = [           // Relearning steps
        TimeSpan.FromMinutes(10)
    ];
});
```

## Documentation

For complete documentation, examples, and API reference, visit: [GitHub Repository](https://github.com/TranPhucTien/FSRS.Core)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.