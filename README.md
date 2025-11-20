🔗URL Shortener with QR Generation, Analytics & Expiry

A fast, reliable, production-ready URL Shortener built with C# (.NET 8), Supabase PostgreSQL, Render, and Vercel.
Users can shorten URLs, generate QR codes, track clicks, and set expiry times — all through a clean frontend UI.

Demo: https://url-shortener-azure-xi.vercel.app/

🚀 Features:

✅ Core Features

Shorten any long URL
Automatically generate a unique shortcode
Optional QR code generation
Custom expiry date for each URL
Simple and clean UI

📊 Analytics Features

Click count tracking
Store user IP and User-Agent
Track timestamp of each click
Rate limiting per user (IP-based)

🔐 Security Features

Filters invalid URLs
Rate limit abuse protection
All DB operations happen through secure backend
No direct DB access from frontend

🗄 Database

Uses Supabase PostgreSQL with tables:
Urls — stores original URL, shortcode, QR data, expiry
Clicks — logs every click
RateLimits — tracks API usage per IP

🧱 Tech Stack:

🖥 Backend

C# / .NET 8 Web API (Minimal APIs)
Entity Framework Core
Npgsql PostgreSQL Provider
QRCoder library for QR generation
Hosted on Render

🌐 Frontend

HTML, CSS, JavaScript (Vanilla)
Hosted on Vercel

🗄 Database

Supabase PostgreSQL
EF Core migrations are automatically applied

🧩 How It Works (Architecture)

Below is the simple flow of the application:

User → Frontend → Backend → PostgreSQL DB
                           ↓
                         Redirect

1️⃣ User enters a long URL

The frontend sends:

{
  "url": "https://example.com",
  "generateQr": false,
  "expiryDays": 7,
  "customAlias": null,
  "baseUrl": null
}


to:

POST /api/shorten

2️⃣ Backend validates URL

Checks if URL is valid
Checks rate limit (IP-based)
Generates a shortcode (random or custom)

3️⃣ URL stored in database

Saved in the Urls table with timestamp and expiry rules.

4️⃣ QR Code generation (optional)

If generateQr = true, backend generates: data:image/png;base64,xxxxxxx

5️⃣ Backend returns short URL

Example:
https://short-it-rvc9.onrender.com/b7EvuE

6️⃣ User clicks on short URL

Browser hits:

GET /{code}

The backend:

✔ Fetches the entry
✔ Checks if expired
✔ Logs the click in Clicks table
✔ Redirects to the original URL

📁 Folder Structure
url-shortener/
│
├── backend/
│   └── UrlShortener.Api/
│       ├── Program.cs
│       ├── Data/
│       ├── Services/
│       ├── Models/
│       ├── Migrations/
│       ├── UrlShortener.Api.csproj
│       └── Dockerfile
│
└── frontend/
    ├── index.html
    ├── script.js
    ├── styles.css
    └── assets/

🛠 API Endpoints
1️⃣ Shorten URL
POST /api/shorten

Request Body:

{
  "url": "https://example.com",
  "generateQr": true,
  "expiryDays": 7,
  "customAlias": null,
  "baseUrl": null
}


Response:

{
  "shortUrl": "https://short-it-rvc9.onrender.com/b7EvuE",
  "qrBase64": "data:image/png;base64,..."
}

2️⃣ Redirect Short URL
GET /{shortcode}
Redirects user to original URL and logs click.

🌟 Future Improvements

🧑‍💼 User authentication + personal dashboard
📊 Analytics UI page (graphs, device stats)
🔐 Password-protected URLs
🧪 Custom domain support
📦 Bulk URL creation
🤖 AI link categorizer
🗑 Auto-delete expired links
📈 Live analytics (WebSockets or Supabase Realtime)