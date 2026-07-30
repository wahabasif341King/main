# Flowline — PMS Frontend

React (Vite) + Context API frontend for your Project Management backend.

## Setup

```bash
npm install
npm run dev
```

App khulega `http://localhost:5173` par.

## Backend URL

`src/api/axios.js` mein base URL set hai:
```
http://localhost:8005/api
```
Agar tumhara backend kisi aur port pe chalta hai, ye line update kar do.

## ⚠️ Zaroori: Backend mein CORS enable karo

React (`localhost:5173`) se .NET API (`localhost:8005`) ko call karne ke liye,
apne `Program.cs` mein ye add karo:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

Aur `app.UseAuthentication();` se **pehle**:
```csharp
app.UseCors("AllowReactApp");
```

Bina isके browser requests block ho jayengi (CORS error console mein dikhega).

## Folder structure

```
src/
  api/
    axios.js       -> backend calls ke liye axios instance (auto Bearer token attach karta hai)
    jwt.js          -> JWT token decode karne ka helper
  context/
    AuthContext.jsx -> login/register/logout state, poori app mein useAuth() se access hota hai
  components/
    Layout.jsx       -> sidebar + page wrapper
    ProtectedRoute.jsx -> login ke bina protected pages block karta hai
  pages/
    Login.jsx
    Register.jsx
    Dashboard.jsx
    Projects.jsx
    ProjectDetail.jsx
    Notifications.jsx
```

## Flow

1. `/register` → account banao
2. `/login` → token milta hai, `localStorage` mein save hota hai, `AuthContext` decode karke user info nikalta hai
3. Har protected page (`Dashboard`, `Projects`, etc.) automatically Bearer token bhejta hai (axios interceptor se)
4. Token expire/invalid ho to automatically `/login` pe redirect ho jata hai
