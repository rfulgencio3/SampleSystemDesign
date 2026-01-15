# Scenario: UrlShortenerService (Scaling Reads)

## Business Goal
Develop a high-performance URL shortening service for the Global Event and Technology Platform. The primary challenge is handling a massive volume of read requests (redirections) with minimal latency, as the write-to-read ratio is approximately 1:1000.

## System Design Pattern: Scaling Reads
The solution must implement the **Scaling Reads** pattern using:
1.  **In-Memory Caching (Redis):** For storing the mapping between the short code and the original URL.
2.  **Read-Through/Cache-Aside Strategy:** The service should first check the cache. Only on a cache miss should it query the database.
3.  **Database:** A relational database (simulated via an interface) will be the source of truth.

## Implementation Tasks (AI Agent Focus)
1.  **Domain:** Define the `ShortUrl` Entity (Id, OriginalUrl, ShortCode, ExpirationDate) and the `IShortUrlRepository` interface.
2.  **Application:** Create the `GetOriginalUrlQuery` and `GetOriginalUrlQueryHandler` to handle the redirection logic.
3.  **Infrastructure:** Implement a `RedisShortUrlCache` class that implements the `IShortUrlRepository` interface for read operations, prioritizing the cache.
4.  **Presentation:** Create a minimal API endpoint to handle the redirection request (`/r/{shortCode}`).
