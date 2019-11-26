# RiverFuzz

Core fuzzing code is located in `/Fuzz/Program.cs`.

The program has a bunch of hardcoded paths, so it probably won't work if you just install it. If you want to set it up let me know, and I can set up some sort of resources config.

## Dependencies:
 - PosgreSQL 11
 - Visual Studio Community 2019 (Or VS Code on Linux)
 - .NET Core 3.0

## Basic Concepts

### Goals

The fuzzer starts with a limited set of seed requests representing normal usage of the site, and uses those seed requests to discover new behavior in the site.

### Overview

In each generation, the fuzzer:
1. Generates a new set of viable sequences by mutating the existing population.
2. Executes each viable sequence and captures the results.
3. Buckets the results (e.g. deduplicate).
4. Selects the shortest sequences from each bucket to be added to the new population.
5. Repeats with the new population.

### Request Sequences

Here's an example user flow for a user logging in and adding an order to their cart. Notice how there are certain values that are copied from responses into later requests.

#### Login Request

This request requires two values -- an email and a password. Currently I'm seeding the request with these values (see `fuzzer.cs`)

```
POST /rest/user/login HTTP/1.1
Host: localhost
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0
Accept: application/json, text/plain, */*
Accept-Language: en-US,en;q=0.5
Accept-Encoding: gzip, deflate
Content-Type: application/json
Content-Length: 45
Connection: close
Referer: http://localhost/
Cookie: language=en; cookieconsent_status=dismiss; continueCode=E3OzQenePWoj4zk293aRX8KbBNYEAo9GL5qO1ZDwp6JyVxgQMmrlv7npKLVy; io=AFnBxvy_r2xmQgdKAAAB; welcomebanner_status=dismiss

{"email":"asdf@asdf.com","password":"123456"}
```

#### Login Response

```
HTTP/1.1 200 OK
X-Powered-By: Express
Access-Control-Allow-Origin: *
X-Content-Type-Options: nosniff
X-Frame-Options: SAMEORIGIN
Content-Type: application/json; charset=utf-8
Content-Length: 767
ETag: W/"2ff-zkxkrfng7DfzThSlbzGg5S8LzZ0"
Vary: Accept-Encoding
Date: Fri, 11 Oct 2019 05:27:39 GMT
Connection: close

{"authentication":{"token":"eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmZ0Bhc2RmZy5jb20iLCJwYXNzd29yZCI6IjA0MGI3Y2Y0YTU1MDE0ZTE4NTgxM2UwNjQ0NTAyZWE5Iiwicm9sZSI6ImN1c3RvbWVyIiwibGFzdExvZ2luSXAiOiIwLjAuMC4wIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMTAtMTEgMDU6MDI6MTAuMDAwICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMTAtMTEgMDU6MDI6MTAuMDAwICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU3MDc3MTY1OSwiZXhwIjoxNTcwNzg5NjU5fQ.CKsCQuG1EvzHVhxA2EhQnFefNlsmo5fEQwk-0ECg50LN1qJXpvtkhkyQjk4a11QWuRixC-NnfUEHpLoNtc049HS74kml0RoJyKlVIfNjDbmXCYC0AkJX4wvup9M1sSrvBVM_fMWhiGTaLEjq-NHUMWqjZhxIZNUI_lH0-UfXW84","bid":1069,"umail":"asdfg@asdfg.com"}}
```

#### Add To Cart Request

Here, the `token` parameter from the previous response will be copied into both the `Authorization` header and the `token` cookie, and the value `bid` parameter will be copied into the `BasketId` json value.

```
POST /api/BasketItems/ HTTP/1.1
Host: localhost
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0
Accept: application/json, text/plain, */*
Accept-Language: en-US,en;q=0.5
Accept-Encoding: gzip, deflate
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmZ0Bhc2RmZy5jb20iLCJwYXNzd29yZCI6IjA0MGI3Y2Y0YTU1MDE0ZTE4NTgxM2UwNjQ0NTAyZWE5Iiwicm9sZSI6ImN1c3RvbWVyIiwibGFzdExvZ2luSXAiOiIwLjAuMC4wIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMTAtMTEgMDU6MDI6MTAuMDAwICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMTAtMTEgMDU6MDI6MTAuMDAwICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU3MDc3MTY1OSwiZXhwIjoxNTcwNzg5NjU5fQ.CKsCQuG1EvzHVhxA2EhQnFefNlsmo5fEQwk-0ECg50LN1qJXpvtkhkyQjk4a11QWuRixC-NnfUEHpLoNtc049HS74kml0RoJyKlVIfNjDbmXCYC0AkJX4wvup9M1sSrvBVM_fMWhiGTaLEjq-NHUMWqjZhxIZNUI_lH0-UfXW84
Content-Type: application/json
Content-Length: 47
Connection: close
Referer: http://localhost/
Cookie: language=en; cookieconsent_status=dismiss; continueCode=yXjv6Z5jWJnzD6a3YvmwPRXK7roAy2TWpde2Og19yEN84plqxkMBbLVQrDeo; io=VlYFHzXxw3xIn8fEAAAC; welcomebanner_status=dismiss; token=eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmZ0Bhc2RmZy5jb20iLCJwYXNzd29yZCI6IjA0MGI3Y2Y0YTU1MDE0ZTE4NTgxM2UwNjQ0NTAyZWE5Iiwicm9sZSI6ImN1c3RvbWVyIiwibGFzdExvZ2luSXAiOiIwLjAuMC4wIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMTAtMTEgMDU6MDI6MTAuMDAwICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMTAtMTEgMDU6MDI6MTAuMDAwICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU3MDc3MTY1OSwiZXhwIjoxNTcwNzg5NjU5fQ.CKsCQuG1EvzHVhxA2EhQnFefNlsmo5fEQwk-0ECg50LN1qJXpvtkhkyQjk4a11QWuRixC-NnfUEHpLoNtc049HS74kml0RoJyKlVIfNjDbmXCYC0AkJX4wvup9M1sSrvBVM_fMWhiGTaLEjq-NHUMWqjZhxIZNUI_lH0-UfXW84

{"ProductId":24,"BasketId":"1069","quantity":1}
```

#### Add To Cart Response

```
HTTP/1.1 200 OK
X-Powered-By: Express
Access-Control-Allow-Origin: *
X-Content-Type-Options: nosniff
X-Frame-Options: SAMEORIGIN
Content-Type: application/json; charset=utf-8
Content-Length: 161
ETag: W/"a1-mVibLRyO0Y+0LzUYRrg74T8CNqU"
Vary: Accept-Encoding
Date: Fri, 11 Oct 2019 05:27:43 GMT
Connection: close

{"status":"success","data":{"id":37,"ProductId":24,"BasketId":"1069","quantity":1,"updatedAt":"2019-10-11T05:27:43.448Z","createdAt":"2019-10-11T05:27:43.448Z"}}
```

### Tokens

'Token' is the term used to represent any part of a request or response that has a value we might want to copy the value of, or copy a value into. The most obvious tokens in the login request above are the `username` and `password` json tokens. The most obvious tokens in the login response are the `token`, the `bid` and the `umail` tokens.

### Substitutions

A substitution is the act of replacing the old value of a token with a new value.