<div align="center">

# 💬 ConnectO — Social Media & Chatting API

### Production-grade ASP.NET Core 10 Web API — Clean Architecture, CQRS + MediatR, SignalR Real-Time, JWT Auth, Redis, Cloudinary, Docker

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Neon-4169E1?logo=postgresql&logoColor=white)](https://neon.tech/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis&logoColor=white)](https://redis.io/)
[![SignalR](https://img.shields.io/badge/SignalR-Real--Time-0078D4?logo=microsoftazure&logoColor=white)](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
[![Cloudinary](https://img.shields.io/badge/Cloudinary-Media-3448C5?logo=cloudinary&logoColor=white)](https://cloudinary.com/)
[![Azure](https://img.shields.io/badge/Azure-Live%20on%20Production-0078D4?logo=microsoftazure&logoColor=white)](https://connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net/scalar/#description/introduction)
[![Docker Hub](https://img.shields.io/badge/Docker%20Hub-saif31%2Fconnecto--api-2496ED?logo=docker&logoColor=white)](https://hub.docker.com/)

> 🚀 **Live on Production** → [connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net](https://connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net/scalar/#description/introduction)

</div>

---

## 📑 Table of Contents

1. [Overview](#-overview)
2. [Live Production](#-live-production)
3. [Architecture](#-architecture--clean-architecture--cqrs)
4. [Design Patterns](#-design-patterns-used)
5. [Tech Stack](#-tech-stack)
6. [Features](#-features)
7. [Database Schema](#-database-schema)
8. [Project Structure](#-project-structure)
9. [Authentication & Authorization](#-authentication--authorization)
10. [Email Service](#-email-service)
11. [Real-Time (SignalR)](#-real-time-signalr)
12. [SignalR Testing Tool](#-signalr-testing-tool)
13. [Caching Strategy](#-caching-strategy)
14. [Media Uploads](#-media-uploads-cloudinary)
15. [Test Accounts](#-test-accounts)
16. [API Reference](#-api-reference)
17. [Running Locally](#-running-locally)
18. [Docker Deployment](#-docker-deployment)
19. [Roadmap](#-roadmap)
20. [What I Learned](#-what-i-learned)

---

## 📖 Overview

ConnectO is a full-featured social media & real-time chatting backend — built from scratch with production concerns in mind, not a tutorial clone. Every architectural decision was deliberate: from the CQRS command/query split, to user-scoped Redis caching, to OTP email verification, to SignalR presence tracking and real-time messaging.

- **7 independent projects** organized by Clean Architecture rings (Domain → Service → Persistence → Presentation → Web)
- **CQRS + MediatR** — every operation is an isolated Command or Query with its own handler and FluentValidation validator
- **JWT + refresh tokens** with rotation & revocation
- **Google OAuth** — seamless sign-in with Google accounts
- **Two-Factor Authentication (2FA)** — TOTP-based 2FA enable/disable
- **OTP email verification** — Redis-backed with 10-minute auto-expiry
- **SignalR** for real-time messaging, online presence, notifications, and read receipts
- **Push Notifications** — Web Push (VAPID) + Firebase Cloud Messaging (FCM) for mobile
- **Cloudinary** for all media uploads with automatic old-asset cleanup
- **User-scoped Redis caching** on search endpoints (30s TTL, per-user isolation)
- **Containerized** with a multi-stage Dockerfile + `docker-compose.yml`
- **Deployed on Azure App Service** (West Europe) backed by Neon PostgreSQL

---

## 🚀 Live Production

The API is **live and publicly accessible**:

| Resource | URL |
|---|---|
| **Scalar API Docs** | [connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net/scalar](https://connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net/scalar/#description/introduction) |
| **Base URL** | `https://connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net` |
| **Database** | Neon PostgreSQL (production branch) |
| **Region** | Azure West Europe |

### 🧪 Test Accounts

Two seeded accounts are available to explore the API immediately:

| Account | Email | Password |
|---|---|---|
| **ProfileA** | `profilea@connecto.test` | `Test@1234!` |
| **ProfileB** | `profileb@connecto.test` | `Test@1234!` |

Both accounts have `EmailConfirmed = true` — ready to log in with no extra steps.

---

## 🏗️ Architecture — Clean Architecture / CQRS

```
┌──────────────────────────────────────────────────────────┐
│            Social-Media-Chatting-APP-Web (Host)          │ ← Program.cs, middleware, DI wiring
│  CORS · JWT · Scalar · Global exception handler · SignalR│
├──────────────────────────────────────────────────────────┤
│         Social-Media-Chatting-APP-Presentation           │ ← HTTP boundary, SignalR Hubs
│  Auth · UserProfile · Posts · Chat · Notifications · …   │
├──────────────────────────────────────────────────────────┤
│            Social-Media-Chatting-APP-Service             │ ← Application layer (CQRS handlers)
│  Commands · Queries · Validators · MappingProfiles        │
├──────────────────────────────────────────────────────────┤
│        Social-Media-Chatting-APP-ServiceAbstraction      │ ← Service interfaces (DI contracts)
│  IAuthService · IUploadService · IOtpService · …         │
├──────────────────────────────────────────────────────────┤
│          Social-Media-Chatting-APP-Persistence           │ ← EF Core, Migrations, Seeding
│  AppDbContext · IdentityDbContext · Repository pattern   │
├──────────────────────────────────────────────────────────┤
│            Social-Media-Chatting-APP-Domain              │ ← Pure C#, no dependencies
│  Entities · Enums                                        │
├──────────────────────────────────────────────────────────┤
│          Social-Media-Chatting-APP-SharedLibrary         │ ← Cross-cutting
│  DTOs · Result<T> · SharedResponse                       │
└──────────────────────────────────────────────────────────┘
```

**Dependency rule:** every arrow points inward. `Domain` knows nothing about EF Core, ASP.NET, Redis, or Cloudinary. Swap any infrastructure piece — only the outer layer changes.

---

## 🎨 Design Patterns Used

| Pattern | Where | Why |
|---|---|---|
| **CQRS** | `Service/Features/*/Commands` & `Queries` | Read and write paths are completely separate — queries never mutate state |
| **MediatR** | All commands/queries dispatched via `ISender` | Decouples controllers from handlers; pipeline behaviors plug in transparently |
| **Pipeline Behavior** | `ValidationBehavior<TRequest, TResponse>` | FluentValidation runs automatically for every command/query — zero boilerplate in handlers |
| **Result Pattern** | `SharedLibrary.SharedResponse.Result<T>` | Explicit success/failure instead of exceptions for control flow |
| **Repository** | `Persistence/Repositories` | Abstracts EF Core away from the service layer |
| **DTO / AutoMapper** | `Service/Common/MappingProfiles` | Never leak domain entities to the API surface |
| **Options Pattern** | `JwtOptions`, `CloudinarySettings` | Strongly-typed configuration binding |
| **Action Filter** | `[RedisCache]` attribute | User-scoped response caching without polluting controller logic |
| **Dependency Injection** | Extension methods in `Web/Extensions` | Clean DI composition — `Program.cs` stays readable across 7 projects |

---

## 🧰 Tech Stack

<div align="center">

| Layer | Technology |
|---|---|
| **Runtime** | ASP.NET Core 10 · C# 13 |
| **Data** | Entity Framework Core 10 · PostgreSQL (Neon) |
| **Identity** | ASP.NET Core Identity |
| **Cache** | Redis 7 (StackExchange.Redis) |
| **Real-Time** | SignalR |
| **Push** | Web Push (VAPID) · Firebase Cloud Messaging (FCM) |
| **Media** | Cloudinary SDK |
| **Email** | SMTP (background queue · MailKit) |
| **Messaging** | MediatR (CQRS) |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **Docs** | Scalar (OpenAPI 3.1) |
| **Container** | Docker (multi-stage) + Docker Compose |
| **Hosting** | Azure App Service (West Europe) |
| **Database Host** | Neon PostgreSQL (serverless) |

</div>

---

## ✨ Features

### 🔐 Identity & Auth
- Register with username, display name, email, password
- Email OTP verification — Redis-backed, 10-minute auto-expiry, zero manual cleanup
- Resend OTP support
- Login with JWT access token + refresh token (rotation on every refresh)
- Logout / token revocation — logs user out on all devices instantly
- **Google OAuth** — sign in with Google (`/api/Auth/google-login`)
- **Two-Factor Authentication (2FA)** — enable/disable TOTP-based 2FA
- Forgot Password + Reset Password via email link
- `[Authorize]` gates on all user-only endpoints

### 👤 User Profile
- View own private profile (full details)
- Update profile — display name, bio, privacy settings
- Upload / replace profile picture via Cloudinary (old image auto-deleted)
- View public profile of any user by ID
- Privacy controls — `ShowOnlineStatus`, `ShowLastSeen`, `AllowMessageFromStrangers`
- Delete profile picture

### 🔍 User Search
- Search users by username or display name (`/api/UserSearch?q=`)
- **User-scoped Redis caching** — `user-search:{userId}:{query}` key, 30s TTL

### 🤝 Friendship System
- Send / respond to friend requests (accept / decline)
- Unfriend users
- Block / unblock users
- View friends list, blocked users, incoming & outgoing friend requests

### 📝 Posts & Feed
- Create posts (text, media, repost, quote-repost)
- Edit and delete posts
- Get post by ID
- Paginated personal feed (cursor-based)
- Get posts by author (cursor-based)
- Repost support

### 💬 Comments
- Add comments on posts (with optional media attachment)
- Edit / delete comments
- Nested replies on comments (cursor-paginated)
- Get all comments on a post (cursor-paginated)

### ❤️ Likes
- Like / unlike posts
- Like / unlike comments

### 💬 Conversations & Messaging
- Create DM conversations
- Create group conversations
- List all conversations (cursor-paginated)
- Get conversation details
- Send messages (text, media, replies)
- Get message history (cursor-paginated)
- Mark messages as read
- Group management — add/remove participants, update group info, change roles, leave, delete

### 🔔 Notifications
- Get paginated notifications feed (cursor-based)
- Mark notifications as read (single or bulk)

### 📲 Push Notifications
- Web Push (VAPID) — subscribe/unsubscribe
- Firebase Cloud Messaging (FCM) — register/unregister device tokens
- VAPID public key endpoint

### 📁 Media Upload
- Upload any file (image, video, document) to Cloudinary
- Purpose-aware upload (avatar, post media, message attachment)
- Conversation-scoped uploads

### 🟢 Online Presence (SignalR)
- Real-time online/offline status via `PresenceHub`
- `LastSeen` timestamp updated on disconnect
- Respects `ShowOnlineStatus` privacy setting

---

## 🗄️ Database Schema

The full entity-relationship diagram for ConnectO's production database:

![Database Schema](Final%20DB%20Schema.png)

> The schema covers all domains: Users, Friendships, Posts, Comments, Likes, Conversations, Messages, Notifications, PushSubscriptions, DeviceTokens, MediaAssets, and ASP.NET Core Identity tables.

---

## 📂 Project Structure

```
Social-Media-Chatting-APP/
├── Social-Media-Chatting-APP-Domain/           # Pure entities + enums
│   └── Entities/
│       ├── AppUser.cs
│       ├── Post.cs · Comment.cs · Like.cs
│       ├── Friendship.cs
│       ├── Conversation.cs · Message.cs
│       ├── Notification.cs
│       ├── PushSubscription.cs · DeviceToken.cs
│       └── MediaAsset.cs
│
├── Social-Media-Chatting-APP-Persistence/      # EF Core + Identity + Migrations
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   ├── Repositories/
│   └── Seeding/DataSeeder.cs                  # ProfileA + ProfileB test accounts
│
├── Social-Media-Chatting-APP-Service/          # Application layer (CQRS)
│   └── Features/
│       ├── Authentication/
│       ├── UserProfile/
│       ├── Posts/ · Comments/ · Likes/
│       ├── Friendship/
│       ├── Conversation/ · Message/
│       ├── Notifications/
│       ├── Push/ · DeviceTokens/
│       ├── Upload/
│       └── UserSearch/
│
├── Social-Media-Chatting-APP-Presentation/     # Thin controllers + SignalR Hubs
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── UserProfileController.cs · UserSearchController.cs
│   │   ├── PostController.cs · CommentsController.cs · LikesController.cs
│   │   ├── FriendshipController.cs
│   │   ├── ConversationController.cs · GroupConversationController.cs
│   │   ├── MessageController.cs
│   │   ├── NotificationsController.cs
│   │   ├── PushController.cs · DeviceTokenController.cs
│   │   └── UploadController.cs
│   └── Hubs/
│       ├── PresenceHub.cs
│       └── ChatHub.cs
│
├── Social-Media-Chatting-APP-Web/              # Host project
│   ├── Program.cs
│   ├── Extensions/
│   └── CustomMiddlewares/ExceptionHandlerMiddleware.cs
│
├── Dockerfile
├── docker-compose.yml
└── .env.example
```

---

## 🔒 Authentication & Authorization

**Access + refresh token flow**

```
┌──────────┐                         ┌─────────────┐
│  Client  │  POST /api/Auth/login    │    API      │
│          │ ───────────────────▶    │             │
│          │   access token (15m)    │             │
│          │   refresh token (7d)    │             │
│          │ ◀───────────────────    │             │
│          │                         │             │
│          │  requests with Bearer   │             │
│          │ ───────────────────▶    │             │
│          │                         │             │
│          │  when access expires:   │             │
│          │  POST /api/Auth/refresh  │             │
│          │ ───────────────────▶    │             │
│          │   NEW access + NEW refresh            │
│          │   (old refresh revoked) │             │
│          │ ◀───────────────────    │             │
└──────────┘                         └─────────────┘
```

**OTP Email Verification flow**

```
Register → OTP generated → stored in Redis (TTL: 10min) → sent via SMTP background queue
                                      ↓
                         POST /api/Auth/verify-otp { email, otp }
                                      ↓
                          Redis validates → user confirmed
```

---

## ✉️ Email Service

- Auth flows call `OtpService.GenerateAndSendAsync(...)` to create a 6-digit code and send it to the user.
- OTP stored in Redis with 10-minute TTL and 3-attempt limit.
- The actual email send is asynchronous: enqueued into `BackgroundEmailQueue` — HTTP response returns immediately.
- `EmailSenderBackgroundService` dequeues and sends via MailKit with STARTTLS (`smtp.gmail.com:587`).
- Password reset uses `AuthService.ForgotPasswordAsync(...)` — generates a short-lived reset token sent by email.

---

## ⚡ Real-Time (SignalR)

**Presence Hub** — tracks online/offline status in real time

```
Client connects with JWT       →  PresenceHub.OnConnectedAsync()
                                    → sets IsOnline = true
                                    → broadcasts to followers

Client disconnects             →  PresenceHub.OnDisconnectedAsync()
                                    → sets IsOnline = false
                                    → updates LastSeen = DateTime.UtcNow
```

**Chat Hub** — real-time messaging

```
Client sends message           →  ChatHub
                                    → persists to DB
                                    → broadcasts to conversation participants
                                    → triggers push notification if recipient offline
                                    → supports typing indicators & read receipts
```

> ⚠️ SignalR doesn't use `Authorization` headers — the JWT must be passed as a query string parameter: `?access_token=YOUR_TOKEN`

---

## 🧪 SignalR Testing Tool

A dedicated web-based tool is available to test all SignalR hubs interactively without writing any client code:

**Repo:** [github.com/sefffo/ConnectO-SignalR-Testing-Tool](https://github.com/sefffo/ConnectO-SignalR-Testing-Tool)

![SignalR Testing Tool](https://raw.githubusercontent.com/sefffo/ConnectO/main/Test%20Tool.png)

### Features
- Connect to **PresenceHub** and **ChatHub** with your JWT token
- Fire any hub method and see server responses in real time
- Test message send, typing indicators, read receipts, and presence events
- Pre-configured to point to the production Azure endpoint

### Quickstart
```bash
git clone https://github.com/sefffo/ConnectO-SignalR-Testing-Tool.git
cd ConnectO-SignalR-Testing-Tool
# open index.html in your browser — no build step needed
```

Set the **Server URL** to:
```
https://connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net
```
Paste your JWT from `/api/Auth/login` and connect.

---

## 🚀 Caching Strategy

**User-scoped search cache**

```
         ┌─────────────────────────────┐
         │  GET /api/UserSearch?q=saif │
         │  Authorization: Bearer ...  │
         └──────────────┬──────────────┘
                        │
              Extract userId from JWT
                        │
         ┌──────────────▼──────────────┐
         │  Cache key:                 │
         │  user-search:{userId}:?q=saif│
         └────┬──────────────┬─────────┘
            hit              miss
             │                │
     ┌───────▼──┐   ┌─────────▼─────────┐
     │ Return   │   │  Query PostgreSQL  │
     │ cached   │   │  Map → DTOs        │
     │ JSON     │   │  SET Redis TTL 30s │
     └──────────┘   └────────────────────┘
```

- **TTL: 30 seconds** — fresh enough for a social app, long enough to absorb repeated searches
- **Per-user isolation** — `user-search:userA:?q=saif` ≠ `user-search:userB:?q=saif`

---

## 🖼️ Media Uploads (Cloudinary)

Profile picture upload flow:

```
POST /api/UserProfile/my-profile/upload-picture  (multipart/form-data)
         │
         ▼
  Validator: size ≤ 5 MB · extension: .jpg / .jpeg / .png / .webp
         │
         ▼
  UploadService.UploadAsync()  →  Cloudinary folder: profile-pictures/
         │
         ▼
  If user had previous picture:
  → UploadService.DeleteAsync(oldPublicId)
         │
         ▼
  AppUser.ProfilePicture = newUrl · SaveChanges()
```

General file upload via `POST /api/Upload` supports images, videos, and documents — purpose-aware (avatar, post, message attachment).

---

## 📖 API Reference

Full interactive docs: **[Scalar UI on Production](https://connecto-fvfuauetc7buamaz.westeurope-01.azurewebsites.net/scalar/#description/introduction)**

### 🔐 Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Auth/register` | ❌ | Create account |
| POST | `/api/Auth/login` | ❌ | Returns access + refresh tokens |
| POST | `/api/Auth/refresh` | ❌ | Rotate tokens |
| POST | `/api/Auth/logout` | ✅ | Revoke refresh token |
| POST | `/api/Auth/verify-otp` | ❌ | Verify email OTP |
| POST | `/api/Auth/resend-otp` | ❌ | Resend OTP |
| POST | `/api/Auth/forgot-password` | ❌ | Send reset link |
| POST | `/api/Auth/reset-password` | ❌ | Reset password |
| GET  | `/api/Auth/google-login` | ❌ | Google OAuth redirect |
| GET  | `/api/Auth/google-callback` | ❌ | Google OAuth callback |
| POST | `/api/Auth/2fa/enable` | ✅ | Enable 2FA |
| POST | `/api/Auth/2fa/disable` | ✅ | Disable 2FA |

### 👤 User Profile
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET    | `/api/UserProfile/my-profile` | ✅ | My full profile |
| PUT    | `/api/UserProfile/my-profile` | ✅ | Update profile |
| GET    | `/api/UserProfile/{userId}` | ✅ | Public profile |
| POST   | `/api/UserProfile/my-profile/upload-picture` | ✅ | Upload profile picture |
| DELETE | `/api/UserProfile/my-profile/picture` | ✅ | Delete profile picture |

### 🔍 Search
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/UserSearch?q={term}` | ✅ | Search users (Redis-cached) |

### 🤝 Friendship
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST   | `/api/Friendship/send/{targetUserId}` | ✅ | Send friend request |
| PUT    | `/api/Friendship/respond/{friendshipId}` | ✅ | Accept / decline request |
| DELETE | `/api/Friendship/unfriend/{targetUserId}` | ✅ | Unfriend |
| POST   | `/api/Friendship/block/{targetUserId}` | ✅ | Block user |
| DELETE | `/api/Friendship/unblock/{targetUserId}` | ✅ | Unblock user |
| GET    | `/api/Friendship/friends` | ✅ | Friends list |
| GET    | `/api/Friendship/blocked` | ✅ | Blocked users |
| GET    | `/api/Friendship/requests/incoming` | ✅ | Incoming requests |
| GET    | `/api/Friendship/requests/outgoing` | ✅ | Outgoing requests |

### 📝 Posts
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST   | `/api/Post` | ✅ | Create post |
| GET    | `/api/Post/{postId}` | ✅ | Get post |
| PUT    | `/api/Post/{postId}` | ✅ | Edit post |
| DELETE | `/api/Post/{postId}` | ✅ | Delete post |
| GET    | `/api/Post/feed` | ✅ | Paginated feed |
| GET    | `/api/Post/posts/{authorId}` | ✅ | Posts by author |
| POST   | `/api/Post/repost` | ✅ | Repost / quote-repost |

### 💬 Comments
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST   | `/api/Comments/{postId}/comments` | ✅ | Add comment |
| GET    | `/api/Comments/{postId}/comments` | ✅ | Get comments |
| PUT    | `/api/Comments/{postId}/comments/{commentId}` | ✅ | Edit comment |
| DELETE | `/api/Comments/{postId}/comments/{commentId}` | ✅ | Delete comment |
| GET    | `/api/Comments/{commentId}/replies` | ✅ | Get replies |

### ❤️ Likes
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Likes/posts/{postId}` | ✅ | Like / unlike post |
| POST | `/api/Likes/comments/{commentId}` | ✅ | Like / unlike comment |

### 💬 Conversations
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET  | `/api/Conversation` | ✅ | List conversations |
| GET  | `/api/Conversation/{conversationId}` | ✅ | Get conversation |
| POST | `/api/Conversation/dm` | ✅ | Start DM |
| POST | `/api/Conversation/group` | ✅ | Create group |

### 👥 Group Conversations
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST   | `/api/GroupConversation/{conversationId}/participants` | ✅ | Add participants |
| DELETE | `/api/GroupConversation/{conversationId}/participants/{participantId}` | ✅ | Remove participant |
| PATCH  | `/api/GroupConversation/{conversationId}/info` | ✅ | Update group info |
| PATCH  | `/api/GroupConversation/{conversationId}/role` | ✅ | Change role |
| POST   | `/api/GroupConversation/{conversationId}/leave` | ✅ | Leave group |
| DELETE | `/api/GroupConversation/{conversationId}` | ✅ | Delete group |

### 📨 Messages
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Message` | ✅ | Send message |
| GET  | `/api/Message/{conversationId}` | ✅ | Message history |
| POST | `/api/Message/read` | ✅ | Mark as read |

### 🔔 Notifications
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/Notifications` | ✅ | Get notifications |
| PUT | `/api/Notifications/read` | ✅ | Mark as read |

### 📲 Push
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET    | `/api/push/vapid-key` | ❌ | Get VAPID public key |
| POST   | `/api/push/subscribe` | ✅ | Web Push subscribe |
| DELETE | `/api/push/unsubscribe` | ✅ | Web Push unsubscribe |

### 📱 Device Tokens (FCM)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST   | `/api/device-tokens/register` | ✅ | Register FCM token |
| DELETE | `/api/device-tokens/unregister` | ✅ | Unregister FCM token |

### 📁 Upload
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Upload` | ✅ | Upload file to Cloudinary |

---

## 🏃 Running Locally

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker Desktop (for PostgreSQL + Redis)

### Option A — Docker Compose (easiest)

```bash
git clone https://github.com/sefffo/Social-Media-Chatting-APP.git
cd Social-Media-Chatting-APP
cp .env.example .env   # fill in your secrets
docker-compose up --build
```

→ Scalar docs open at **http://localhost:5000/scalar**

### Option B — Native dotnet

```bash
dotnet restore
dotnet ef database update --project Social-Media-Chatting-APP-Persistence --startup-project Social-Media-Chatting-APP-Web
dotnet run --project Social-Media-Chatting-APP-Web
```

### Required Environment Variables

```env
ConnectionStrings__DefaultConnection=Host=...;Database=ConnectO;Username=...;Password=...;SSL Mode=Require
ConnectionStrings__Redis=localhost:6379
JwtOptions__SecretKey=your-secret-key
JwtOptions__Issuer=ConnectO
JwtOptions__Audience=ConnectO
Cloudinary__CloudName=your-cloud-name
Cloudinary__ApiKey=your-api-key
Cloudinary__ApiSecret=your-api-secret
EmailSettings__Host=smtp.gmail.com
EmailSettings__Port=587
EmailSettings__Email=your-email
EmailSettings__Password=your-app-password
```

---

## 🐳 Docker Deployment

**Multi-stage Dockerfile**

```dockerfile
# Stage 1: SDK image (~800MB) — compiles the code
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.slnx .
COPY */*.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish Social-Media-Chatting-APP-Web/... -c Release -o /app/publish --no-restore

# Stage 2: Runtime image (~220MB) — only what's needed to run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Social-Media-Chatting-APP-Web.dll"]
```

**Final image ≈ 220 MB** — no SDK, no source code shipped to production.

---

## 🗺️ Roadmap

- [x] Authentication (Register, Login, OTP, JWT + Refresh Tokens, Google OAuth, 2FA, Forgot/Reset Password)
- [x] User Profile (view, update, profile picture via Cloudinary)
- [x] User Search with user-scoped Redis caching
- [x] SignalR Presence Hub (online/offline tracking)
- [x] Friendship system (send, accept, block, unfriend)
- [x] Posts, Comments, Likes (full CRUD + feeds)
- [x] Repost / quote-repost
- [x] Direct & group conversations
- [x] Real-time messaging via SignalR ChatHub
- [x] Push Notifications (Web Push VAPID + FCM)
- [x] Notifications feed
- [x] Media upload (Cloudinary — images, video, docs)
- [x] Deployed on Azure App Service + Neon PostgreSQL
- [ ] Follow system
- [ ] Stories
- [ ] Automated testing (xUnit + k6 load testing)
- [ ] CI/CD pipeline (GitHub Actions → Azure)

---

## 🎓 What I Learned

| Area | Takeaway |
|---|---|
| **CQRS + MediatR** | Splitting reads and writes makes every feature self-contained — adding a new feature doesn't touch existing handlers |
| **Pipeline Behaviors** | Validation as a cross-cutting concern — handlers stay clean, validators stay focused |
| **Result Pattern** | Business failures (404, 403, validation) are values, not exceptions — the caller always knows what to expect |
| **Redis OTP** | TTL-based expiry removes the need for cleanup jobs entirely — let the infrastructure handle it |
| **User-scoped caching** | Public caches leak data between users — the key must always include the requesting user's identity |
| **SignalR + JWT** | SignalR doesn't use Authorization headers — the token must be passed via query string and validated manually |
| **Cloudinary cleanup** | Uploading a new image without deleting the old one leaks storage — always store `PublicId` alongside the URL |
| **FluentValidation** | Declarative validation rules are far more readable and testable than imperative `if` chains |
| **Push Notifications** | Web Push and FCM require separate pipelines — VAPID for browsers, FCM device tokens for mobile |
| **Neon PostgreSQL** | Serverless Postgres branches mirror Git workflow — branch per environment, zero ops overhead |

---

<div align="center">

### 🔗 Related

[**SignalR Testing Tool**](https://github.com/sefffo/ConnectO-SignalR-Testing-Tool) · [**E-Commerce REST API**](https://github.com/sefffo/E-Commerce-dotnet-API) · [**E-Commerce Dashboard**](https://github.com/sefffo/ecommerce-dashboard)

---

**Built by [Saif Lotfy](https://www.linkedin.com/in/saif-lotfy-769451310/)** — backend engineer, Cairo 🇪🇬

*If this project helped you, a ⭐ on the repo would mean the world.*

</div>
