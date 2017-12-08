# RT.CoreUserService
Service providing basic user profile functionality

The idea behind this API is to provide reusable user profile functionality to apps I develop in the future.  

Basic functions being:

1) Create User
2) Get User
3) Update User
4) Delete User
5) Login (Issue JWT)
6) Request Temporary Passcode (Email sent to User with code)
7) Redeem Temporary Passcode (Using passcode from email)

User sessions are managed by JSON Web Tokens, which allow for a stateless, scalable service.
