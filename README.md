# RT.CoreUserService
Service providing basic user profile functionality

The idea behind this API is to provide reusable user profile functionality to apps I develop in the future.  

Basic functions being:

1) Create Profile
2) Edit Profile Information
3) Update Credentials
4) Delete Profile (Soft Delete)
5) Login
6) Fetch Profile
7) Forgot Credentials (Email sent to User with reset link)
8) Reset Credentials (Using guid in reset link)
9) Post Content
10) Comment on Content
11) Like/Dislike Content
12) Friend Request
13) Tag Friend In Content
14) Private Message Friend

User sessions are managed by JSON Web Tokens, which allow for a stateless, easily scalable service.
