# GenericRest – Structured REST API Integration in VB.NET

**GenericRest** is a lightweight, extensible VB.NET library for consuming REST APIs with minimal boilerplate. It enables developers to interact with any RESTful service using strongly typed objects for requests and responses.

## 🚀 Features

- 🔄 Unified handling for GET and POST requests
- 🧱 Typed request/response objects using generics
- 🍚 Native support for cookies and custom headers
- 🔧 Switch between `HttpWebRequest` and `HttpClient` backends
- 🔄 Sync and Async support
- 🔍 Simple URL parameter building with automatic escaping

## 🛠️ Quick Start

### Add a REST GET Call

```vbnet
Dim request As New RestGet("https://api.example.com", "users", New List(Of String) From {"123"})
Dim user = RestUtil.GetResponse(Of User)(request)
```

### Add a REST POST Call with Body

```vbnet
Dim payload As New CreateOrderRequest With {.ProductId = 123, .Quantity = 2}
Dim request As New RestPost(Of CreateOrderRequest, OrderResponse)("https://api.example.com", "orders", Nothing, payload)
Dim response = RestUtil.GetResponse(Of CreateOrderRequest, OrderResponse)(request)
```

### Async Version

```vbnet
Dim response = Await RestUtil.GetResponseAsync(Of CreateOrderRequest, OrderResponse)(request)
```

## 📦 How It Works

- `RestGet` and `RestPost(Of T)` are concrete classes that inherit from `RestRequest`, which builds the final request URL and handles headers/cookies.
- `RestUtil.GetResponse` takes in a `RestRequest` and returns a deserialized object, using JSON.NET under the hood.
- Request body and response are fully typed with generics (`Of T`, `Of T1, T2`), so your code is strongly typed throughout.
- All low-level networking is abstracted behind `IWebClient` (which `GenericHttpWebRequest` implements).

## ✨ Why Use GenericRest?

- No need to manually build URLs or parse JSON responses.
- Works seamlessly with any REST API that returns JSON.
- Simple debugging – all requests/responses go through centralized, extendable logic.
- Great for internal APIs, Shopify, or third-party services.

## 🧩 Project Structure

| File                       | Purpose                                          |
| -------------------------- | ------------------------------------------------ |
| `RestGet.vb`               | Represents a GET request                         |
| `RestPost.vb`              | Represents a POST request (optionally generic)   |
| `RestRequest.vb`           | Base class to build and format REST URLs         |
| `RestUtil.vb`              | Utility class to send requests and parse results |
| `GenericHttpWebRequest.vb` | Default implementation of `IWebClient`           |
| `IWebClient.vb`            | Interface abstraction for HTTP client logic      |

## 📄 Example Use Case

Here's an example of a Shopify-like order system sending structured customer data to a drop shipper:

```vbnet
Dim customer = New CustomerAddress With {.Name = "John Doe", .City = "Seattle"}
Dim postReq = New RestPost(Of CustomerAddress, DropShipResponse)("https://drop.api.com", "create", Nothing, customer)
Dim result = RestUtil.GetResponse(Of CustomerAddress, DropShipResponse)(postReq)
```

No need to manually handle serialization, content types, or HTTP status codes—just pass structured objects and get results.

## 🗪 Requirements

- .NET Framework 4.6+ or compatible
- Newtonsoft.Json

## 📝 License

MIT License

